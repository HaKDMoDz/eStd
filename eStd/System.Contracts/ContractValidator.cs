﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Contracts
{
    /// <summary>
    /// Enables validation of pre- and postconditions. This class isn't used directly by developers. Instead 
    /// the class should be created by the <see cref="Contract.Requires{T}(T)">Requires</see> and
    /// <see cref="Contract.Ensures{T}(T)">Ensures</see> extension methods.
    /// </summary>
    /// <typeparam name="T">The type of the argument to be validated</typeparam>
    /// <example>
    /// The following example shows how to use <b>CuttingEdge.Conditions</b>.
    /// <code><![CDATA[
    /// using System.Collections;
    /// 
    /// using CuttingEdge.Conditions;
    /// 
    /// public class ExampleClass
    /// {
    ///     private enum StateType { Uninitialized = 0, Initialized };
    ///     
    ///     private StateType currentState;
    /// 
    ///     public ICollection GetData(int? id, string xml, IEnumerable col)
    ///     {
    ///         // Check all preconditions:
    ///         Condition.Requires(id, "id")
    ///             .IsNotNull()          // throws ArgumentNullException on failure
    ///             .IsInRange(1, 999)    // ArgumentOutOfRangeException on failure
    ///             .IsNotEqualTo(128);   // throws ArgumentException on failure
    /// 
    ///         Condition.Requires(xml, "xml")
    ///             .StartsWith("<data>") // throws ArgumentException on failure
    ///             .EndsWith("</data>"); // throws ArgumentException on failure
    /// 
    ///         Condition.Requires(col, "col")
    ///             .IsNotNull()          // throws ArgumentNullException on failure
    ///             .IsEmpty();           // throws ArgumentException on failure
    /// 
    ///         // Do some work
    /// 
    ///         // Example: Call a method that should return a not null ICollection
    ///         object result = BuildResults(xml, col);
    /// 
    ///         // Check all postconditions:
    ///         // A PostconditionException will be thrown at failure.
    ///         Condition.Ensures(result, "result")
    ///             .IsNotNull()
    ///             .IsOfType(typeof(ICollection));
    /// 
    ///         return result as ICollection;
    ///     }
    /// }
    /// ]]></code>
    /// The following code examples shows how to extend the library with your own 'Invariant' entry point
    /// method. The first example shows a class with an Add method that validates the class state (the
    /// class invariants) before adding the <b>Person</b> object to the internal array and that code should
    /// throw an <see cref="InvalidOperationException"/>.
    /// <code><![CDATA[
    /// using CuttingEdge.Conditions;
    /// 
    /// public class Person { }
    /// 
    /// public class PersonCollection 
    /// {
    ///     public PersonCollection(int capicity)
    ///     {
    ///         this.Capacity = capicity;
    ///     }
    /// 
    ///     public void Add(Person person)
    ///     {
    ///         // Throws a ArgumentNullException when person == null
    ///         Condition.Requires(person, "person").IsNotNull();
    ///         
    ///         // Throws an InvalidOperationException on failure
    ///         Invariants.Invariant(this.Count, "Count").IsLessOrEqual(this.Capacity);
    ///         
    ///         this.AddInternal(person);
    ///     }
    ///
    ///     public int Count { get; private set; }
    ///     public int Capacity { get; private set; }
    ///     
    ///     private void AddInternal(Person person)
    ///     {
    ///         // some logic here
    ///     }
    ///     
    ///     public bool Contains(Person person)
    ///     {
    ///         // some logic here
    ///         return false;
    ///     }
    /// }
    /// ]]></code>
    /// The following code example will show the implementation of the <b>Invariants</b> class.
    /// <code><![CDATA[
    /// using System;
    /// using CuttingEdge.Conditions;
    /// 
    /// namespace MyCompanyRootNamespace
    /// {
    ///     public static class Invariants
    ///     {
    ///         public static ConditionValidator<T> Invariant<T>(T value)
    ///         {
    ///             return new InvariantValidator<T>("value", value);
    ///         }
    /// 
    ///         public static ConditionValidator<T> Invariant<T>(T value, string argumentName)
    ///         {
    ///             return new InvariantValidator<T>(argumentName, value);
    ///         }
    /// 
    ///         // Internal class that inherits from ConditionValidator<T>
    ///         sealed class InvariantValidator<T> : ConditionValidator<T>
    ///         {
    ///             public InvariantValidator(string argumentName, T value)
    ///                 : base(argumentName, value)
    ///             {
    ///             }
    /// 
    ///             protected override void ThrowExceptionCore(string condition,
    ///                 string additionalMessage, ConstraintViolationType type)
    ///             {
    ///                 string exceptionMessage = string.Format("Invariant '{0}' failed.", condition);
    /// 
    ///                 if (!String.IsNullOrEmpty(additionalMessage))
    ///                 {
    ///                     exceptionMessage += " " + additionalMessage;
    ///                 }
    /// 
    ///                 // Optionally, the 'type' parameter can be used, but never throw an exception
    ///                 // when the value of 'type' is unknown or unvalid.
    ///                 throw new InvalidOperationException(exceptionMessage);
    ///             }
    ///         }
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    [DebuggerDisplay("{GetType().Name} ( ArgumentName: {ArgumentName}, Value: {Value} )")]
    public abstract class ContractValidator<T>
    {
        /// <summary>Gets the value of the argument.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)] // see top of page for note on this attribute.
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification =
            "We chose to make the Value a public field, so the Extension methods can use it, without we " +
            "have to worry about extra method calls.")]
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "The rule is correct. We tried to make ConditionValidator<T> immutable, but " +
            "the validated type T can indeed be mutable (a collection for example). However, for us it's " +
            "enough to be sure that the value or the reference to the value won't change.")]
        public readonly T Value;

        private readonly string argumentName;

        /// <summary>Initializes a new instance of the <see cref="ContractValidator{T}"/> class.</summary>
        /// <param name="argumentName">The name of the argument to be validated</param>
        /// <param name="value">The value of the argument to be validated</param>
        protected ContractValidator(string argumentName, T value)
        {
            // This constructor is internal. It is not useful for a user to inherit from this class.
            // When this ctor is made protected, so should be the BuildException method.
            this.Value = value;
            this.argumentName = argumentName;
        }

        /// <summary>Gets the name of the argument.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)] // see top of page for note on this attribute.
        public string ArgumentName
        {
            get { return this.argumentName; }
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current System.Object.
        /// </summary>
        /// <param name="obj">The System.Object to compare with the current System.Object.</param>
        /// <returns>
        /// true if the specified System.Object is equal to the current System.Object; otherwise, false.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)] // see top of page for note on this attribute.
        [Obsolete("This method is not part of the conditions framework. Please use the IsEqualTo method.", true)]
#pragma warning disable 809 // Remove the Obsolete attribute from the overriding member, or add it to the ...
        public override bool Equals(object obj)
#pragma warning restore 809
        {
            return base.Equals(obj);
        }

        /// <summary>Returns the hash code of the current instance.</summary>
        /// <returns>The hash code of the current instance.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)] // see top of page for note on this attribute.
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the <see cref="ContractValidator{T}"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents the <see cref="ContractValidator{T}"/>.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)] // see top of page for note on this attribute.
        public override string ToString()
        {
            return base.ToString();
        }

        /// <summary>Gets the <see cref="System.Type"/> of the current instance.</summary>
        /// <returns>The <see cref="System.Type"/> instance that represents the exact runtime 
        /// type of the current instance.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)] // see top of page for note on this attribute.
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = 
            "This FxCop warning is valid, but this method is used to be able to attach an " + 
            "EditorBrowsableAttrubute to the GetType method, which will hide the method when the user " +
            "browses the methods of the ConditionValidator class with IntelliSense. The GetType method has " +
            "no value for the user who will only use this class for validation.")]
        public new Type GetType()
        {
            return base.GetType();
        }

        /// <summary>Throws an exception.</summary>
        /// <param name="condition">Describes the condition that doesn't hold, e.g., "Value should not be 
        /// null".</param>
        /// <param name="additionalMessage">An additional message that will be appended to the exception
        /// message, e.g. "The actual value is 3.". This value may be null or empty.</param>
        /// <param name="type">Gives extra information on the exception type that must be build. The actual
        /// implementation of the validator may ignore some or all values.</param>
        [EditorBrowsable(EditorBrowsableState.Never)] // see top of page for note on this attribute.
        [DebuggerStepThrough]
        public void ThrowException(string condition, string additionalMessage, ConstraintViolationType type)
        {
            this.ThrowExceptionCore(condition, additionalMessage, type);
        }

        /// <summary>Throws an exception.</summary>
        /// <param name="condition">Describes the condition that doesn't hold, e.g., "Value should not be 
        /// null".</param>
        [EditorBrowsable(EditorBrowsableState.Never)] // see top of page for note on this attribute.
        public void ThrowException(string condition)
        {
            this.ThrowExceptionCore(condition, null, ConstraintViolationType.Default);
        }

        internal void ThrowException(string condition, string additionalMessage)
        {
            this.ThrowExceptionCore(condition, additionalMessage, ConstraintViolationType.Default);
        }

        internal void ThrowException(string condition, ConstraintViolationType type)
        {
            this.ThrowExceptionCore(condition, null, type);
        }

        /// <summary>Throws an exception.</summary>
        /// <param name="condition">Describes the condition that doesn't hold, e.g., "Value should not be 
        /// null".</param>
        /// <param name="additionalMessage">An additional message that will be appended to the exception
        /// message, e.g. "The actual value is 3.". This value may be null or empty.</param>
        /// <param name="type">Gives extra information on the exception type that must be build. The actual
        /// implementation of the validator may ignore some or all values.</param>
        /// <remarks>
        /// Implement this method when deriving from <see cref="ContractValidator{T}"/>.
        /// The implementation should at least build the exception message from the 
        /// <paramref name="condition"/> and optional <paramref name="additionalMessage"/>. Usage of the
        /// <paramref name="type"/> is completely optional, but the implementation should at least be flexible
        /// and be able to handle unknown <see cref="ConstraintViolationType"/> values. Values may be added
        /// in future releases.
        /// </remarks>
        /// <example>
        /// For an example see the documentation for <see cref="ContractValidator{T}"/>.
        /// </example>
        protected abstract void ThrowExceptionCore(string condition, string additionalMessage,
            ConstraintViolationType type);
    }
}
