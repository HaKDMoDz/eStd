﻿// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// Microsoft Public License (MS-PL)
// 
// This license governs use of the accompanying software. If you use the
// software, you accept this license. If you do not accept the license, do not
// use the software.
// 
// 1. Definitions
// 
//   The terms "reproduce," "reproduction," "derivative works," and
//   "distribution" have the same meaning here as under U.S. copyright law. A
//   "contribution" is the original software, or any additions or changes to
//   the software. A "contributor" is any person that distributes its
//   contribution under this license. "Licensed patents" are a contributor's
//   patent claims that read directly on its contribution.
// 
// 2. Grant of Rights
// 
//   (A) Copyright Grant- Subject to the terms of this license, including the
//       license conditions and limitations in section 3, each contributor
//       grants you a non-exclusive, worldwide, royalty-free copyright license
//       to reproduce its contribution, prepare derivative works of its
//       contribution, and distribute its contribution or any derivative works
//       that you create.
// 
//   (B) Patent Grant- Subject to the terms of this license, including the
//       license conditions and limitations in section 3, each contributor
//       grants you a non-exclusive, worldwide, royalty-free license under its
//       licensed patents to make, have made, use, sell, offer for sale,
//       import, and/or otherwise dispose of its contribution in the software
//       or derivative works of the contribution in the software.
// 
// 3. Conditions and Limitations
// 
//   (A) No Trademark License- This license does not grant you rights to use
//       any contributors' name, logo, or trademarks.
// 
//   (B) If you bring a patent claim against any contributor over patents that
//       you claim are infringed by the software, your patent license from such
//       contributor to the software ends automatically.
// 
//   (C) If you distribute any portion of the software, you must retain all
//       copyright, patent, trademark, and attribution notices that are present
//       in the software.
// 
//   (D) If you distribute any portion of the software in source code form, you
//       may do so only under this license by including a complete copy of this
//       license with your distribution. If you distribute any portion of the
//       software in compiled or object code form, you may only do so under a
//       license that complies with this license.
// 
//   (E) The software is licensed "as-is." You bear the risk of using it. The
//       contributors give no express warranties, guarantees or conditions. You
//       may have additional consumer rights under your local laws which this
//       license cannot change. To the extent permitted under your local laws,
//       the contributors exclude the implied warranties of merchantability,
//       fitness for a particular purpose and non-infringement.
//       

using System;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.ClearScript.Util;

namespace Microsoft.ClearScript
{
    /// <summary>
    /// Provides optional script-callable utility functions.
    /// </summary>
    /// <remarks>
    /// Use <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see> to expose a
    /// <c>HostFunctions</c> instance to script code. Each instance can only be exposed in one
    /// script engine.
    /// </remarks>
    public class HostFunctions : IScriptableObject
    {
        private ScriptEngine engine;

        // ReSharper disable EmptyConstructor

        /// <summary>
        /// Initializes a new <see cref="HostFunctions"/> instance.
        /// </summary>
        public HostFunctions()
        {
            // the help file builder (SHFB) insists on an empty constructor here
        }

        // ReSharper restore EmptyConstructor

        #region script-callable interface

        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Creates an empty host object.
        /// </summary>
        /// <returns>A new empty host object.</returns>
        /// <remarks>
        /// This function is provided for script languages that do not support external
        /// instantiation. It creates an object that supports dynamic property addition and
        /// removal. The host can manipulate it via the <see cref="IPropertyBag"/> interface.
        /// </remarks>
        /// <example>
        /// The following code creates an empty host object and adds several properties to it.
        /// It assumes that an instance of <see cref="HostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// var item = host.newObj();
        /// item.label = "Widget";
        /// item.weight = 123.45;
        /// </code>
        /// </example>
        public PropertyBag newObj()
        {
            return new PropertyBag();
        }

        /// <summary>
        /// Creates a host object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="args">Optional constructor arguments.</param>
        /// <returns>A new host object of the specified type.</returns>
        /// <remarks>
        /// This function is provided for script languages that do not support external
        /// instantiation. For information about the mapping between host members and script-
        /// callable properties and methods, see
        /// <see cref="ScriptEngine.AddHostObject(string, HostItemFlags, object)">AddHostObject</see>.
        /// </remarks>
        /// <example>
        /// The following code imports the <see cref="System.Random"/> class, creates an
        /// instance using the
        /// <see href="http://msdn.microsoft.com/en-us/library/ctssatww.aspx">Random(Int32)</see>
        /// constructor, and calls the <see cref="System.Random.NextDouble"/> method.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// var RandomT = host.type("System.Random");
        /// var random = host.newObj(RandomT, 100);
        /// var value = random.NextDouble();
        /// </code>
        /// </example>
        /// <seealso cref="ExtendedHostFunctions.type(string, object[])"/>
        public T newObj<T>(params object[] args)
        {
            return (T)typeof(T).CreateInstance(args);
        }

        /// <summary>
        /// Performs dynamic instantiation.
        /// </summary>
        /// <param name="target">The dynamic host object that provides the instantiation operation to perform.</param>
        /// <param name="args">Optional instantiation arguments.</param>
        /// <returns>The result of the operation, which is usually a new dynamic host object.</returns>
        /// <remarks>
        /// This function is provided for script languages that do not support external
        /// instantiation.
        /// </remarks>
        public object newObj(IDynamicMetaObjectProvider target, params object[] args)
        {
            MiscHelpers.VerifyNonNullArgument(target, "target");

            object result;
            if (target.GetMetaObject(Expression.Constant(target)).TryCreateInstance(args, out result))
            {
                return result;
            }

            throw new InvalidOperationException("Invalid dynamic instantiation");
        }

        /// <summary>
        /// Creates a host array.
        /// </summary>
        /// <typeparam name="T">The element type of the array to create.</typeparam>
        /// <param name="lengths">One or more integers representing the array dimension lengths.</param>
        /// <returns>A new host array.</returns>
        /// <remarks>
        /// For information about the mapping between host members and script-callable properties
        /// and methods, see
        /// <see cref="ScriptEngine.AddHostObject(string, HostItemFlags, object)">AddHostObject</see>.
        /// </remarks>
        /// <example>
        /// The following code creates a 5x3 host array of strings.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// var StringT = host.type("System.String");
        /// var array = host.newArr(StringT, 5, 3);
        /// </code>
        /// </example>
        /// <seealso cref="ExtendedHostFunctions.type(string, object[])"/>
        public object newArr<T>(params int[] lengths)
        {
            return Array.CreateInstance(typeof(T), lengths);
        }

        /// <summary>
        /// Creates a host variable of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of variable to create.</typeparam>
        /// <param name="initValue">An optional initial value for the variable.</param>
        /// <returns>A new host variable of the specified type.</returns>
        /// <remarks>
        /// A host variable is a strongly typed object that holds a value of the specified type.
        /// Host variables are useful for passing method arguments by reference. In addition to
        /// being generally interchangeable with their stored values, host variables support the
        /// following properties:
        /// <para>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Property</term>
        ///         <term>Access</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><c>value</c></term>
        ///         <term>read-write</term>
        ///         <description>The current value of the host variable.</description>
        ///     </item>
        ///     <item>
        ///         <term><c>out</c></term>
        ///         <term>read-only</term>
        ///         <description>A reference to the host variable that can be passed as an <c><see href="http://msdn.microsoft.com/en-us/library/t3c3bfhx(VS.80).aspx">out</see></c> argument.</description>
        ///     </item>
        ///     <item>
        ///         <term><c>ref</c></term>
        ///         <term>read-only</term>
        ///         <description>A reference to the host variable that can be passed as a <c><see href="http://msdn.microsoft.com/en-us/library/14akc2c7(VS.80).aspx">ref</see></c> argument.</description>
        ///     </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <example>
        /// The following code demonstrates using a host variable to invoke a method with an
        /// <c>out</c> parameter.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// // import a dictionary type
        /// var StringT = host.type("System.String");
        /// var StringDictT = host.type("System.Collections.Generic.Dictionary", StringT, StringT);
        /// // create and populate a dictionary
        /// var dict = host.newObj(StringDictT);
        /// dict.Add("foo", "bar");
        /// dict.Add("baz", "qux");
        /// // look up a dictionary entry
        /// var result = host.newVar(StringT);
        /// var found = dict.TryGetValue("baz", result.out);
        /// </code>
        /// </example>
        /// <seealso cref="ExtendedHostFunctions.type(string, object[])"/>
        public object newVar<T>(T initValue = default(T))
        {
            return new HostVariable<T>(initValue);
        }

        /// <summary>
        /// Creates a delegate that invokes a script function.
        /// </summary>
        /// <typeparam name="T">The type of delegate to create.</typeparam>
        /// <param name="scriptFunc">The script function for which to create a delegate.</param>
        /// <returns>A new delegate that invokes the specified script function.</returns>
        /// <remarks>
        /// If the delegate signature includes parameters passed by reference, the corresponding
        /// arguments to the script function will be <see cref="newVar{T}">host variables</see>.
        /// The script function can set the value of an output argument by assigning the
        /// corresponding host variable's <c>value</c> property.
        /// </remarks>
        /// <example>
        /// The following code demonstrates delegating a callback to a script function.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// // create and populate an array of integers
        /// var EnumerableT = host.type("System.Linq.Enumerable", "System.Core");
        /// var array = EnumerableT.Range(1, 5).ToArray();
        /// // import the callback type required to call Array.ForEach
        /// var Int32T = host.type("System.Int32");
        /// var CallbackT = host.type("System.Action", Int32T);
        /// // use Array.ForEach to calculate a sum
        /// var sum = 0;
        /// var ArrayT = host.type("System.Array");
        /// ArrayT.ForEach(array, host.del(CallbackT, function (value) { sum += value; }));
        /// </code>
        /// </example>
        /// <seealso cref="ExtendedHostFunctions.type(string, string, object[])"/>
        /// <seealso cref="ExtendedHostFunctions.type(string, object[])"/>
        public T del<T>(object scriptFunc)
        {
            return DelegateFactory.CreateDelegate<T>(GetEngine(), scriptFunc);
        }

        /// <summary>
        /// Creates a delegate that invokes a script function and returns no value.
        /// </summary>
        /// <param name="argCount">The number of arguments to pass to the script function.</param>
        /// <param name="scriptFunc">The script function for which to create a delegate.</param>
        /// <returns>A new delegate that invokes the specified script function and returns no value.</returns>
        /// <remarks>
        /// This function creates a delegate that accepts <paramref name="argCount"/> arguments and
        /// returns no value. The type of all parameters is <see cref="System.Object"/>. Such a
        /// delegate is often useful in strongly typed contexts because of
        /// <see href="http://msdn.microsoft.com/en-us/library/ms173174(VS.80).aspx">contravariance</see>.
        /// </remarks>
        /// <example>
        /// The following code demonstrates delegating a callback to a script function.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// // create and populate an array of strings
        /// var StringT = host.type("System.String");
        /// var array = host.newArr(StringT, 3);
        /// array.SetValue("first", 0);
        /// array.SetValue("second", 1);
        /// array.SetValue("third", 2);
        /// // use Array.ForEach to generate console output
        /// var ArrayT = host.type("System.Array");
        /// var ConsoleT = host.type("System.Console");
        /// ArrayT.ForEach(array, host.proc(1, function (value) { ConsoleT.WriteLine(value); }));
        /// </code>
        /// </example>
        /// <seealso cref="ExtendedHostFunctions.type(string, object[])"/>
        /// <seealso cref="newArr{T}"/>
        public object proc(int argCount, object scriptFunc)
        {
            return DelegateFactory.CreateProc(GetEngine(), scriptFunc, argCount);
        }

        /// <summary>
        /// Creates a delegate that invokes a script function and returns a value of the specified type.
        /// </summary>
        /// <typeparam name="T">The return value type.</typeparam>
        /// <param name="argCount">The number of arguments to pass to the script function.</param>
        /// <param name="scriptFunc">The script function for which to create a delegate.</param>
        /// <returns>A new delegate that invokes the specified script function and returns a value of the specified type.</returns>
        /// <remarks>
        /// This function creates a delegate that accepts <paramref name="argCount"/> arguments and
        /// returns a value of the specified type. The type of all parameters is
        /// <see cref="System.Object"/>. Such a delegate is often useful in strongly typed contexts
        /// because of
        /// <see href="http://msdn.microsoft.com/en-us/library/ms173174(VS.80).aspx">contravariance</see>.
        /// </remarks>
        /// <example>
        /// The following code demonstrates delegating a callback to a script function.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// // create and populate an array of strings
        /// var StringT = host.type("System.String");
        /// var array = host.newArr(StringT, 3);
        /// array.SetValue("first", 0);
        /// array.SetValue("second", 1);
        /// array.SetValue("third", 2);
        /// // import LINQ extensions
        /// var EnumerableT = host.type("System.Linq.Enumerable", "System.Core");
        /// // use LINQ to create an array of modified strings
        /// var selector = host.func(StringT, 1, function (value) { return value.toUpperCase(); });
        /// array = array.Select(selector).ToArray();
        /// </code>
        /// </example>
        /// <seealso cref="ExtendedHostFunctions.type(string, object[])"/>
        /// <seealso cref="ExtendedHostFunctions.type(string, string, object[])"/>
        public object func<T>(int argCount, object scriptFunc)
        {
            return DelegateFactory.CreateFunc<T>(GetEngine(), scriptFunc, argCount);
        }

        /// <summary>
        /// Gets the <see cref="System.Type"/> for the specified host type. This version is invoked
        /// if the specified object can be used as a type argument.
        /// </summary>
        /// <typeparam name="T">The host type for which to get the <see cref="System.Type"/>.</typeparam>
        /// <returns>The <see cref="System.Type"/> for the specified host type.</returns>
        /// <remarks>
        /// This function is similar to the C#
        /// <c><see href="http://msdn.microsoft.com/en-us/library/58918ffs(VS.71).aspx">typeof</see></c>
        /// operator. It is overloaded with <see cref="typeOf(object)"/> and selected at runtime if
        /// <typeparamref name="T"/> can be used as a type argument.
        /// <para>
        /// This function throws an exception if the script engine's
        /// <see cref="ScriptEngine.AllowReflection"/> property is set to <c>false</c>.
        /// </para>
        /// </remarks>
        /// <example>
        /// The following code retrieves the assembly-qualified name of a host type.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// var StringT = host.type("System.String");
        /// var name = host.typeOf(StringT).AssemblyQualifiedName;
        /// </code>
        /// </example>
        /// <seealso cref="ExtendedHostFunctions.type(string, object[])"/>
        public Type typeOf<T>()
        {
            GetEngine().CheckReflection();
            return typeof(T);
        }

        /// <summary>
        /// Gets the <see cref="System.Type"/> for the specified host type. This version is invoked
        /// if the specified object cannot be used as a type argument.
        /// </summary>
        /// <param name="value">The host type for which to get the <see cref="System.Type"/>.</param>
        /// <returns>The <see cref="System.Type"/> for the specified host type.</returns>
        /// <remarks>
        /// This function is similar to the C#
        /// <c><see href="http://msdn.microsoft.com/en-us/library/58918ffs(VS.71).aspx">typeof</see></c>
        /// operator. It is overloaded with <see cref="typeOf{T}"/> and selected at runtime if
        /// <paramref name="value"/> cannot be used as a type argument. Note that this applies to
        /// some host types; examples are static types and overloaded generic type groups.
        /// <para>
        /// This function throws an exception if the script engine's
        /// <see cref="ScriptEngine.AllowReflection"/> property is set to <c>false</c>.
        /// </para>
        /// </remarks>
        /// <example>
        /// The following code retrieves the assembly-qualified name of a host type.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// var ConsoleT = host.type("System.Console");
        /// var name = host.typeOf(ConsoleT).AssemblyQualifiedName;
        /// </code>
        /// </example>
        /// <seealso cref="ExtendedHostFunctions.type(string, object[])"/>
        public Type typeOf(object value)
        {
            GetEngine().CheckReflection();

            var hostType = value as HostType;
            if (hostType == null)
            {
                throw new ArgumentException("Invalid host type", "value");
            }

            if (hostType.Types.Length > 1)
            {
                throw new ArgumentException(MiscHelpers.FormatInvariant("'{0}' does not identify a unique host type", hostType.Types[0].GetLocator()), "value");
            }

            return hostType.Types[0];
        }

        /// <summary>
        /// Determines whether an object is compatible with the specified host type.
        /// </summary>
        /// <typeparam name="T">The host type with which to test <paramref name="value"/> for compatibility.</typeparam>
        /// <param name="value">The object to test for compatibility with the specified host type.</param>
        /// <returns><c>True</c> if <paramref name="value"/> is compatible with the specified type, <c>false</c> otherwise.</returns>
        /// <remarks>
        /// This function is similar to the C#
        /// <c><see href="http://msdn.microsoft.com/en-us/library/scekt9xw(VS.71).aspx">is</see></c>
        /// operator.
        /// </remarks>
        /// <example>
        /// The following code defines a function that determines whether an object implements
        /// <see cref="System.IComparable"/>.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// function isComparable(value)
        /// {
        ///     var IComparableT = host.type("System.IComparable");
        ///     return host.isType(IComparableT, value);
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="ExtendedHostFunctions.type(string, object[])"/>
        public bool isType<T>(object value)
        {
            return value is T;
        }

        /// <summary>
        /// Casts an object to the specified host type, returning <c>null</c> if the cast fails.
        /// </summary>
        /// <typeparam name="T">The host type to which to cast <paramref name="value"/>.</typeparam>
        /// <param name="value">The object to cast to the specified host type.</param>
        /// <returns>The result of the cast if successful, <c>null</c> otherwise.</returns>
        /// <remarks>
        /// This function is similar to the C#
        /// <c><see href="http://msdn.microsoft.com/en-us/library/cscsdfbt(VS.71).aspx">as</see></c>
        /// operator.
        /// </remarks>
        /// <example>
        /// The following code defines a function that disposes an object if it implements
        /// <see cref="System.IDisposable"/>.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// function dispose(value)
        /// {
        ///     var IDisposableT = host.type("System.IDisposable");
        ///     var disposable = host.asType(IDisposableT, value);
        ///     if (disposable) {
        ///         disposable.Dispose();
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="ExtendedHostFunctions.type(string, object[])"/>
        public object asType<T>(object value) where T : class
        {
            return HostItem.Wrap(GetEngine(), value as T, typeof(T));
        }

        /// <summary>
        /// Casts an object to the specified host type.
        /// </summary>
        /// <typeparam name="T">The host type to which to cast <paramref name="value"/>.</typeparam>
        /// <param name="value">The object to cast to the specified host type.</param>
        /// <returns>The result of the cast.</returns>
        /// <remarks>
        /// If the cast fails, this function throws an exception.
        /// </remarks>
        /// <example>
        /// The following code casts a floating-point value to a 32-bit integer.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// var Int32T = host.type("System.Int32");
        /// var intValue = host.cast(Int32T, 12.5);
        /// </code>
        /// </example>
        /// <seealso cref="ExtendedHostFunctions.type(string, object[])"/>
        public object cast<T>(object value)
        {
            return HostItem.Wrap(GetEngine(), value.DynamicCast<T>(), typeof(T));
        }

        /// <summary>
        /// Determines whether an object is a host type. This version is invoked if the specified
        /// object cannot be used as a type argument.
        /// </summary>
        /// <param name="value">The object to test.</param>
        /// <returns><c>True</c> if <paramref name="value"/> is a host type, <c>false</c> otherwise.</returns>
        /// <remarks>
        /// This function is overloaded with <see cref="isTypeObj{T}"/> and selected at runtime if
        /// <paramref name="value"/> cannot be used as a type argument. Note that this applies to
        /// some host types; examples are static types and overloaded generic type groups.
        /// </remarks>
        public bool isTypeObj(object value)
        {
            return value is HostType;
        }

        // ReSharper disable UnusedTypeParameter

        /// <summary>
        /// Determines whether an object is a host type. This version is invoked if the specified
        /// object can be used as a type argument.
        /// </summary>
        /// <typeparam name="T">The host type (ignored).</typeparam>
        /// <returns><c>True</c>.</returns>
        /// <remarks>
        /// This function is overloaded with <see cref="isTypeObj(object)"/> and selected at
        /// runtime if <typeparamref name="T"/> can be used as a type argument. Because type
        /// arguments are always host types, this method ignores its type argument and always
        /// returns <c>true</c>.
        /// </remarks>
        public bool isTypeObj<T>()
        {
            return true;
        }

        // ReSharper restore UnusedTypeParameter

        /// <summary>
        /// Creates a strongly typed flag set.
        /// </summary>
        /// <typeparam name="T">The type of flag set to create.</typeparam>
        /// <param name="args">The flags to include in the flag set.</param>
        /// <returns>A strongly typed flag set containing the specified flags.</returns>
        /// <remarks>
        /// This function throws an exception if <typeparamref name="T"/> is not a flag set type.
        /// </remarks>
        /// <example>
        /// The following code demonstrates using a strongly typed flag set.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// // import URI types
        /// var UriT = host.type("System.Uri", "System");
        /// var UriFormatT = host.type("System.UriFormat", "System");
        /// var UriComponentsT = host.type("System.UriComponents", "System");
        /// // create a URI
        /// var uri = host.newObj(UriT, "http://www.example.com:8080/path/to/file/sample.htm?x=1&amp;y=2");
        /// // extract URI components
        /// var components = host.flags(UriComponentsT.Scheme, UriComponentsT.Host, UriComponentsT.Path);
        /// var result = uri.GetComponents(components, UriFormatT.Unescaped);
        /// </code>
        /// </example>
        /// <seealso cref="ExtendedHostFunctions.type(string, string, object[])"/>
        public T flags<T>(params T[] args)
        {
            var type = typeof(T);
            if (!type.IsFlagsEnum())
            {
                throw new InvalidOperationException(MiscHelpers.FormatInvariant("{0} is not a flag set type", type.GetFullFriendlyName()));
            }

            try
            {
                return args.Aggregate(0UL, (flags, arg) => flags | Convert.ToUInt64(arg, CultureInfo.InvariantCulture)).DynamicCast<T>();
            }
            catch (OverflowException)
            {
                return args.Aggregate(0L, (flags, arg) => flags | Convert.ToInt64(arg, CultureInfo.InvariantCulture)).DynamicCast<T>();
            }
        }

        /// <summary>
        /// Converts the specified value to a strongly typed <see cref="System.SByte"/> instance.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>An object that can be passed to a parameter of type <see cref="System.SByte"/>.</returns>
        /// <remarks>
        /// This function converts <paramref name="value"/> to <see cref="System.SByte"/> and
        /// packages the result to retain its numeric type across the host-script boundary. It may
        /// be useful for passing arguments to <see cref="System.SByte"/> parameters if the script
        /// engine does not support that type natively.
        /// </remarks>
        /// <example>
        /// The following code adds an element of type <see cref="System.SByte"/> to a strongly
        /// typed list.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// // import types
        /// ElementT = host.type("System.SByte");
        /// ListT = host.type("System.Collections.Generic.List", ElementT);
        /// // create a list
        /// list = host.newObj(ListT);
        /// // add a list element
        /// list.Add(host.toSByte(42));
        /// </code>
        /// </example>
        /// <seealso cref="ExtendedHostFunctions.type(string, object[])"/>
        public object toSByte(IConvertible value)
        {
            return HostObject.Wrap(Convert.ToSByte(value));
        }

        /// <summary>
        /// Converts the specified value to a strongly typed <see cref="System.Byte"/> instance.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>An object that can be passed to a parameter of type <see cref="System.Byte"/>.</returns>
        /// <remarks>
        /// This function converts <paramref name="value"/> to <see cref="System.Byte"/> and
        /// packages the result to retain its numeric type across the host-script boundary. It may
        /// be useful for passing arguments to <see cref="System.Byte"/> parameters if the script
        /// engine does not support that type natively.
        /// </remarks>
        /// <example>
        /// The following code adds an element of type <see cref="System.Byte"/> to a strongly
        /// typed list.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// // import types
        /// ElementT = host.type("System.Byte");
        /// ListT = host.type("System.Collections.Generic.List", ElementT);
        /// // create a list
        /// list = host.newObj(ListT);
        /// // add a list element
        /// list.Add(host.toByte(42));
        /// </code>
        /// </example>
        /// <seealso cref="ExtendedHostFunctions.type(string, object[])"/>
        public object toByte(IConvertible value)
        {
            return HostObject.Wrap(Convert.ToByte(value));
        }

        /// <summary>
        /// Converts the specified value to a strongly typed <see cref="System.Int16"/> instance.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>An object that can be passed to a parameter of type <see cref="System.Int16"/>.</returns>
        /// <remarks>
        /// This function converts <paramref name="value"/> to <see cref="System.Int16"/> and
        /// packages the result to retain its numeric type across the host-script boundary. It may
        /// be useful for passing arguments to <see cref="System.Int16"/> parameters if the script
        /// engine does not support that type natively.
        /// </remarks>
        /// <example>
        /// The following code adds an element of type <see cref="System.Int16"/> to a strongly
        /// typed list.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// // import types
        /// ElementT = host.type("System.Int16");
        /// ListT = host.type("System.Collections.Generic.List", ElementT);
        /// // create a list
        /// list = host.newObj(ListT);
        /// // add a list element
        /// list.Add(host.toInt16(42));
        /// </code>
        /// </example>
        /// <seealso cref="ExtendedHostFunctions.type(string, object[])"/>
        public object toInt16(IConvertible value)
        {
            return HostObject.Wrap(Convert.ToInt16(value));
        }

        /// <summary>
        /// Converts the specified value to a strongly typed <see cref="System.UInt16"/> instance.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>An object that can be passed to a parameter of type <see cref="System.UInt16"/>.</returns>
        /// <remarks>
        /// This function converts <paramref name="value"/> to <see cref="System.UInt16"/> and
        /// packages the result to retain its numeric type across the host-script boundary. It may
        /// be useful for passing arguments to <see cref="System.UInt16"/> parameters if the script
        /// engine does not support that type natively.
        /// </remarks>
        /// <example>
        /// The following code adds an element of type <see cref="System.UInt16"/> to a strongly
        /// typed list.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// // import types
        /// ElementT = host.type("System.UInt16");
        /// ListT = host.type("System.Collections.Generic.List", ElementT);
        /// // create a list
        /// list = host.newObj(ListT);
        /// // add a list element
        /// list.Add(host.toUInt16(42));
        /// </code>
        /// </example>
        /// <seealso cref="ExtendedHostFunctions.type(string, object[])"/>
        public object toUInt16(IConvertible value)
        {
            return HostObject.Wrap(Convert.ToUInt16(value));
        }

        /// <summary>
        /// Converts the specified value to a strongly typed <see cref="System.Char"/> instance.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>An object that can be passed to a parameter of type <see cref="System.Char"/>.</returns>
        /// <remarks>
        /// This function converts <paramref name="value"/> to <see cref="System.Char"/> and
        /// packages the result to retain its numeric type across the host-script boundary. It may
        /// be useful for passing arguments to <see cref="System.Char"/> parameters if the script
        /// engine does not support that type natively.
        /// </remarks>
        /// <example>
        /// The following code adds an element of type <see cref="System.Char"/> to a strongly
        /// typed list.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// // import types
        /// ElementT = host.type("System.Char");
        /// ListT = host.type("System.Collections.Generic.List", ElementT);
        /// // create a list
        /// list = host.newObj(ListT);
        /// // add a list element
        /// list.Add(host.toChar(42));
        /// </code>
        /// </example>
        /// <seealso cref="ExtendedHostFunctions.type(string, object[])"/>
        public object toChar(IConvertible value)
        {
            return HostObject.Wrap(Convert.ToChar(value));
        }

        /// <summary>
        /// Converts the specified value to a strongly typed <see cref="System.Int32"/> instance.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>An object that can be passed to a parameter of type <see cref="System.Int32"/>.</returns>
        /// <remarks>
        /// This function converts <paramref name="value"/> to <see cref="System.Int32"/> and
        /// packages the result to retain its numeric type across the host-script boundary. It may
        /// be useful for passing arguments to <see cref="System.Int32"/> parameters if the script
        /// engine does not support that type natively.
        /// </remarks>
        /// <example>
        /// The following code adds an element of type <see cref="System.Int32"/> to a strongly
        /// typed list.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// // import types
        /// ElementT = host.type("System.Int32");
        /// ListT = host.type("System.Collections.Generic.List", ElementT);
        /// // create a list
        /// list = host.newObj(ListT);
        /// // add a list element
        /// list.Add(host.toInt32(42));
        /// </code>
        /// </example>
        /// <seealso cref="ExtendedHostFunctions.type(string, object[])"/>
        public object toInt32(IConvertible value)
        {
            return HostObject.Wrap(Convert.ToInt32(value));
        }

        /// <summary>
        /// Converts the specified value to a strongly typed <see cref="System.UInt32"/> instance.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>An object that can be passed to a parameter of type <see cref="System.UInt32"/>.</returns>
        /// <remarks>
        /// This function converts <paramref name="value"/> to <see cref="System.UInt32"/> and
        /// packages the result to retain its numeric type across the host-script boundary. It may
        /// be useful for passing arguments to <see cref="System.UInt32"/> parameters if the script
        /// engine does not support that type natively.
        /// </remarks>
        /// <example>
        /// The following code adds an element of type <see cref="System.UInt32"/> to a strongly
        /// typed list.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// // import types
        /// ElementT = host.type("System.UInt32");
        /// ListT = host.type("System.Collections.Generic.List", ElementT);
        /// // create a list
        /// list = host.newObj(ListT);
        /// // add a list element
        /// list.Add(host.toUInt32(42));
        /// </code>
        /// </example>
        /// <seealso cref="ExtendedHostFunctions.type(string, object[])"/>
        public object toUInt32(IConvertible value)
        {
            return HostObject.Wrap(Convert.ToUInt32(value));
        }

        /// <summary>
        /// Converts the specified value to a strongly typed <see cref="System.Int64"/> instance.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>An object that can be passed to a parameter of type <see cref="System.Int64"/>.</returns>
        /// <remarks>
        /// This function converts <paramref name="value"/> to <see cref="System.Int64"/> and
        /// packages the result to retain its numeric type across the host-script boundary. It may
        /// be useful for passing arguments to <see cref="System.Int64"/> parameters if the script
        /// engine does not support that type natively.
        /// </remarks>
        /// <example>
        /// The following code adds an element of type <see cref="System.Int64"/> to a strongly
        /// typed list.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// // import types
        /// ElementT = host.type("System.Int64");
        /// ListT = host.type("System.Collections.Generic.List", ElementT);
        /// // create a list
        /// list = host.newObj(ListT);
        /// // add a list element
        /// list.Add(host.toInt64(42));
        /// </code>
        /// </example>
        /// <seealso cref="ExtendedHostFunctions.type(string, object[])"/>
        public object toInt64(IConvertible value)
        {
            return HostObject.Wrap(Convert.ToInt64(value));
        }

        /// <summary>
        /// Converts the specified value to a strongly typed <see cref="System.UInt64"/> instance.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>An object that can be passed to a parameter of type <see cref="System.UInt64"/>.</returns>
        /// <remarks>
        /// This function converts <paramref name="value"/> to <see cref="System.UInt64"/> and
        /// packages the result to retain its numeric type across the host-script boundary. It may
        /// be useful for passing arguments to <see cref="System.UInt64"/> parameters if the script
        /// engine does not support that type natively.
        /// </remarks>
        /// <example>
        /// The following code adds an element of type <see cref="System.UInt64"/> to a strongly
        /// typed list.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// // import types
        /// ElementT = host.type("System.UInt64");
        /// ListT = host.type("System.Collections.Generic.List", ElementT);
        /// // create a list
        /// list = host.newObj(ListT);
        /// // add a list element
        /// list.Add(host.toUInt64(42));
        /// </code>
        /// </example>
        /// <seealso cref="ExtendedHostFunctions.type(string, object[])"/>
        public object toUInt64(IConvertible value)
        {
            return HostObject.Wrap(Convert.ToUInt64(value));
        }

        /// <summary>
        /// Converts the specified value to a strongly typed <see cref="System.Single"/> instance.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>An object that can be passed to a parameter of type <see cref="System.Single"/>.</returns>
        /// <remarks>
        /// This function converts <paramref name="value"/> to <see cref="System.Single"/> and
        /// packages the result to retain its numeric type across the host-script boundary. It may
        /// be useful for passing arguments to <see cref="System.Single"/> parameters if the script
        /// engine does not support that type natively.
        /// </remarks>
        /// <example>
        /// The following code adds an element of type <see cref="System.Single"/> to a strongly
        /// typed list.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// // import types
        /// ElementT = host.type("System.Single");
        /// ListT = host.type("System.Collections.Generic.List", ElementT);
        /// // create a list
        /// list = host.newObj(ListT);
        /// // add a list element
        /// list.Add(host.toSingle(42));
        /// </code>
        /// </example>
        /// <seealso cref="ExtendedHostFunctions.type(string, object[])"/>
        public object toSingle(IConvertible value)
        {
            return HostObject.Wrap(Convert.ToSingle(value));
        }

        /// <summary>
        /// Converts the specified value to a strongly typed <see cref="System.Double"/> instance.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>An object that can be passed to a parameter of type <see cref="System.Double"/>.</returns>
        /// <remarks>
        /// This function converts <paramref name="value"/> to <see cref="System.Double"/> and
        /// packages the result to retain its numeric type across the host-script boundary. It may
        /// be useful for passing arguments to <see cref="System.Double"/> parameters if the script
        /// engine does not support that type natively.
        /// </remarks>
        /// <example>
        /// The following code adds an element of type <see cref="System.Double"/> to a strongly
        /// typed list.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// // import types
        /// ElementT = host.type("System.Double");
        /// ListT = host.type("System.Collections.Generic.List", ElementT);
        /// // create a list
        /// list = host.newObj(ListT);
        /// // add a list element
        /// list.Add(host.toDouble(42));
        /// </code>
        /// </example>
        /// <seealso cref="ExtendedHostFunctions.type(string, object[])"/>
        public object toDouble(IConvertible value)
        {
            return HostObject.Wrap(Convert.ToDouble(value));
        }

        /// <summary>
        /// Converts the specified value to a strongly typed <see cref="System.Decimal"/> instance.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>An object that can be passed to a parameter of type <see cref="System.Decimal"/>.</returns>
        /// <remarks>
        /// This function converts <paramref name="value"/> to <see cref="System.Decimal"/> and
        /// packages the result to retain its numeric type across the host-script boundary. It may
        /// be useful for passing arguments to <see cref="System.Decimal"/> parameters if the script
        /// engine does not support that type natively.
        /// </remarks>
        /// <example>
        /// The following code adds an element of type <see cref="System.Decimal"/> to a strongly
        /// typed list.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// // import types
        /// ElementT = host.type("System.Decimal");
        /// ListT = host.type("System.Collections.Generic.List", ElementT);
        /// // create a list
        /// list = host.newObj(ListT);
        /// // add a list element
        /// list.Add(host.toDecimal(42));
        /// </code>
        /// </example>
        /// <seealso cref="ExtendedHostFunctions.type(string, object[])"/>
        public object toDecimal(IConvertible value)
        {
            return HostObject.Wrap(Convert.ToDecimal(value));
        }

        /// <summary>
        /// Gets the value of a property in a dynamic host object that implements <see cref="IPropertyBag"/>.
        /// </summary>
        /// <param name="target">The dynamic host object that contains the property to get.</param>
        /// <param name="name">The name of the property to get.</param>
        /// <returns>The value of the specified property.</returns>
        /// <remarks>
        /// This function is provided for script languages that do not support dynamic properties.
        /// </remarks>
        public object getProperty(IPropertyBag target, string name)
        {
            MiscHelpers.VerifyNonNullArgument(target, "target");

            object result;
            if (target.TryGetValue(name, out result))
            {
                return result;
            }

            return Nonexistent.Value;
        }

        /// <summary>
        /// Sets a property value in a dynamic host object that implements <see cref="IPropertyBag"/>.
        /// </summary>
        /// <param name="target">The dynamic host object that contains the property to set.</param>
        /// <param name="name">The name of the property to set.</param>
        /// <param name="value">The new value of the specified property.</param>
        /// <returns>The result of the operation, which is usually the value assigned to the specified property.</returns>
        /// <remarks>
        /// This function is provided for script languages that do not support dynamic properties.
        /// </remarks>
        public object setProperty(IPropertyBag target, string name, object value)
        {
            MiscHelpers.VerifyNonNullArgument(target, "target");
            return target[name] = value;
        }

        /// <summary>
        /// Removes a property from a dynamic host object that implements <see cref="IPropertyBag"/>.
        /// </summary>
        /// <param name="target">The dynamic host object that contains the property to remove.</param>
        /// <param name="name">The name of the property to remove.</param>
        /// <returns><c>True</c> if the property was found and removed, <c>false</c> otherwise.</returns>
        /// <remarks>
        /// This function is provided for script languages that do not support dynamic properties.
        /// </remarks>
        public bool removeProperty(IPropertyBag target, string name)
        {
            MiscHelpers.VerifyNonNullArgument(target, "target");
            return target.Remove(name);
        }

        /// <summary>
        /// Gets the value of a property in a dynamic host object that implements <see cref="IDynamicMetaObjectProvider"/>.
        /// </summary>
        /// <param name="target">The dynamic host object that contains the property to get.</param>
        /// <param name="name">The name of the property to get.</param>
        /// <returns>The value of the specified property.</returns>
        /// <remarks>
        /// This function is provided for script languages that do not support dynamic properties.
        /// </remarks>
        public object getProperty(IDynamicMetaObjectProvider target, string name)
        {
            MiscHelpers.VerifyNonNullArgument(target, "target");

            object result;
            if (target.GetMetaObject(Expression.Constant(target)).TryGetMember(name, out result))
            {
                return result;
            }

            return Nonexistent.Value;
        }

        /// <summary>
        /// Sets a property value in a dynamic host object that implements <see cref="IDynamicMetaObjectProvider"/>.
        /// </summary>
        /// <param name="target">The dynamic host object that contains the property to set.</param>
        /// <param name="name">The name of the property to set.</param>
        /// <param name="value">The new value of the specified property.</param>
        /// <returns>The result of the operation, which is usually the value assigned to the specified property.</returns>
        /// <remarks>
        /// This function is provided for script languages that do not support dynamic properties.
        /// </remarks>
        public object setProperty(IDynamicMetaObjectProvider target, string name, object value)
        {
            MiscHelpers.VerifyNonNullArgument(target, "target");

            object result;
            if (target.GetMetaObject(Expression.Constant(target)).TrySetMember(name, value, out result))
            {
                return result;
            }

            throw new InvalidOperationException("Invalid dynamic property assignment");
        }

        /// <summary>
        /// Removes a property from a dynamic host object that implements <see cref="IDynamicMetaObjectProvider"/>.
        /// </summary>
        /// <param name="target">The dynamic host object that contains the property to remove.</param>
        /// <param name="name">The name of the property to remove.</param>
        /// <returns><c>True</c> if the property was found and removed, <c>false</c> otherwise.</returns>
        /// <remarks>
        /// This function is provided for script languages that do not support dynamic properties.
        /// </remarks>
        public bool removeProperty(IDynamicMetaObjectProvider target, string name)
        {
            MiscHelpers.VerifyNonNullArgument(target, "target");

            bool result;
            if (target.GetMetaObject(Expression.Constant(target)).TryDeleteMember(name, out result))
            {
                return result;
            }

            throw new InvalidOperationException("Invalid dynamic property deletion");
        }

        /// <summary>
        /// Gets the value of an element in a dynamic host object that implements <see cref="IDynamicMetaObjectProvider"/>.
        /// </summary>
        /// <param name="target">The dynamic host object that contains the element to get.</param>
        /// <param name="indices">One or more indices that identify the element to get.</param>
        /// <returns>The value of the specified element.</returns>
        /// <remarks>
        /// This function is provided for script languages that do not support general indexing.
        /// </remarks>
        public object getElement(IDynamicMetaObjectProvider target, params object[] indices)
        {
            MiscHelpers.VerifyNonNullArgument(target, "target");

            object result;
            if (target.GetMetaObject(Expression.Constant(target)).TryGetIndex(indices, out result))
            {
                return result;
            }

            return Nonexistent.Value;

        }

        /// <summary>
        /// Sets an element value in a dynamic host object that implements <see cref="IDynamicMetaObjectProvider"/>.
        /// </summary>
        /// <param name="target">The dynamic host object that contains the element to set.</param>
        /// <param name="value">The new value of the element.</param>
        /// <param name="indices">One or more indices that identify the element to set.</param>
        /// <returns>The result of the operation, which is usually the value assigned to the specified element.</returns>
        /// <remarks>
        /// This function is provided for script languages that do not support general indexing.
        /// </remarks>
        public object setElement(IDynamicMetaObjectProvider target, object value, params object[] indices)
        {
            MiscHelpers.VerifyNonNullArgument(target, "target");

            object result;
            if (target.GetMetaObject(Expression.Constant(target)).TrySetIndex(indices, value, out result))
            {
                return result;
            }

            throw new InvalidOperationException("Invalid dynamic element assignment");
        }

        /// <summary>
        /// Removes an element from a dynamic host object that implements <see cref="IDynamicMetaObjectProvider"/>.
        /// </summary>
        /// <param name="target">The dynamic host object that contains the element to remove.</param>
        /// <param name="indices">One or more indices that identify the element to remove.</param>
        /// <returns><c>True</c> if the element was found and removed, <c>false</c> otherwise.</returns>
        /// <remarks>
        /// This function is provided for script languages that do not support general indexing.
        /// </remarks>
        public bool removeElement(IDynamicMetaObjectProvider target, params object[] indices)
        {
            MiscHelpers.VerifyNonNullArgument(target, "target");

            bool result;
            if (target.GetMetaObject(Expression.Constant(target)).TryDeleteIndex(indices, out result))
            {
                return result;
            }

            throw new InvalidOperationException("Invalid dynamic element deletion");
        }

        /// <summary>
        /// Casts a dynamic host object to its static type.
        /// </summary>
        /// <param name="value">The object to cast to its static type.</param>
        /// <returns>The specified object in its static type form, stripped of its dynamic members.</returns>
        /// <remarks>
        /// A dynamic host object that implements <see cref="IDynamicMetaObjectProvider"/> may have
        /// dynamic members that override members of its static type. This function can be used to
        /// gain access to type members overridden in this manner.
        /// </remarks>
        public object toStaticType(IDynamicMetaObjectProvider value)
        {
            return HostItem.Wrap(GetEngine(), value, HostItemFlags.HideDynamicMembers);
        }

        // ReSharper restore InconsistentNaming

        #endregion

        internal ScriptEngine GetEngine()
        {
            if (engine == null)
            {
                throw new InvalidOperationException("Operation requires a script engine");
            }

            return engine;
        }

        #region IScriptableObject implementation

        // ReSharper disable ParameterHidesMember

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This member is not expected to be re-implemented in derived classes.")]
        void IScriptableObject.OnExposedToScriptCode(ScriptEngine engine)
        {
            MiscHelpers.VerifyNonNullArgument(engine, "engine");

            if (this.engine == null)
            {
               this.engine = engine;
                return;
            }

            if (this.engine !=engine)
            {
                throw new ArgumentException("Invalid script engine", "engine");
            }
        }

        // ReSharper restore ParameterHidesMember

        #endregion
    }

    /// <summary>
    /// Provides optional script-callable utility functions. This extended version allows script
    /// code to import host types.
    /// </summary>
    public class ExtendedHostFunctions : HostFunctions
    {
        // ReSharper disable EmptyConstructor

        /// <summary>
        /// Initializes a new <see cref="ExtendedHostFunctions"/> instance.
        /// </summary>
        public ExtendedHostFunctions()
        {
            // the help file builder (SHFB) insists on an empty constructor here
        }

        // ReSharper restore EmptyConstructor

        #region script-callable interface

        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Imports a host type by name.
        /// </summary>
        /// <param name="name">The fully qualified name of the host type to import.</param>
        /// <param name="hostTypeArgs">Optional generic type arguments.</param>
        /// <returns>The imported host type.</returns>
        /// <remarks>
        /// Host types are imported in the form of objects whose properties and methods are bound
        /// to the host type's static members and nested types. If <paramref name="name"/> refers
        /// to a generic type, the corresponding object will be invocable with type arguments to
        /// yield a specific type.
        /// <para>
        /// For more information about the mapping between host members and script-callable
        /// properties and methods, see
        /// <see cref="ScriptEngine.AddHostObject(string, HostItemFlags, object)">AddHostObject</see>.
        /// </para>
        /// </remarks>
        /// <example>
        /// The following code imports the
        /// <see href="http://msdn.microsoft.com/en-us/library/xfhwa508.aspx">Dictionary</see>
        /// generic type and uses it to create a string dictionary.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// var DictT = host.type("System.Collections.Generic.Dictionary");
        /// var StringT = host.type("System.String");
        /// var dict = host.newObj(DictT(StringT, StringT));
        /// </code>
        /// Another way to create a string dictionary is to import the specific type directly.
        /// <code lang="JavaScript">
        /// var StringT = host.type("System.String");
        /// var StringDictT = host.type("System.Collections.Generic.Dictionary", StringT, StringT);
        /// var dict = host.newObj(StringDictT);
        /// </code>
        /// </example>
        public object type(string name, params object[] hostTypeArgs)
        {
            return TypeHelpers.ImportType(name, null, false, hostTypeArgs);
        }

        /// <summary>
        /// Imports a host type by name from the specified assembly.
        /// </summary>
        /// <param name="name">The fully qualified name of the host type to import.</param>
        /// <param name="assemblyName">The name of the assembly that contains the host type to import.</param>
        /// <param name="hostTypeArgs">Optional generic type arguments.</param>
        /// <returns>The imported host type.</returns>
        /// <remarks>
        /// Host types are imported in the form of objects whose properties and methods are bound
        /// to the host type's static members and nested types. If <paramref name="name"/> refers
        /// to a generic type, the corresponding object will be invocable with type arguments to
        /// yield a specific type.
        /// <para>
        /// For more information about the mapping between host members and script-callable
        /// properties and methods, see
        /// <see cref="ScriptEngine.AddHostObject(string, HostItemFlags, object)">AddHostObject</see>.
        /// </para>
        /// </remarks>
        /// <example>
        /// The following code imports <see cref="System.Linq.Enumerable"/> and uses it to create
        /// an array of strings.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// var EnumerableT = host.type("System.Linq.Enumerable", "System.Core");
        /// var Int32T = host.type("System.Int32");
        /// var StringT = host.type("System.String");
        /// var SelectorT = host.type("System.Func", Int32T, StringT);
        /// var selector = host.del(SelectorT, function (num) { return StringT.Format("The number is {0}.", num); });
        /// var array = EnumerableT.Range(0, 5).Select(selector).ToArray();
        /// </code>
        /// </example>
        /// <seealso cref="type(string, object[])"/>
        public object type(string name, string assemblyName, params object[] hostTypeArgs)
        {
            return TypeHelpers.ImportType(name, assemblyName, true, hostTypeArgs);
        }

        /// <summary>
        /// Imports the host type for the specified <see cref="System.Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="System.Type"/> that specifies the host type to import.</param>
        /// <returns>The imported host type.</returns>
        /// <remarks>
        /// Host types are imported in the form of objects whose properties and methods are bound
        /// to the host type's static members and nested types. If <paramref name="type"/> refers
        /// to a generic type, the corresponding object will be invocable with type arguments to
        /// yield a specific type.
        /// <para>
        /// For more information about the mapping between host members and script-callable
        /// properties and methods, see
        /// <see cref="ScriptEngine.AddHostObject(string, HostItemFlags, object)">AddHostObject</see>.
        /// </para>
        /// </remarks>
        public object type(Type type)
        {
            return HostType.Wrap(type);
        }

        /// <summary>
        /// Imports the host array type for the specified element type.
        /// </summary>
        /// <typeparam name="T">The element type for the host array type to import.</typeparam>
        /// <param name="rank">The number of dimensions for the host array type to import.</param>
        /// <returns>The imported host array type.</returns>
        public object arrType<T>(int rank = 1)
        {
            return HostType.Wrap(typeof(T).MakeArrayType(rank));
        }

        /// <summary>
        /// Imports types from one or more host assemblies.
        /// </summary>
        /// <param name="assemblyNames">The names of the assemblies that contain the types to import.</param>
        /// <returns>The imported host type collection.</returns>
        /// <remarks>
        /// Host type collections provide convenient scriptable access to all the types defined in one
        /// or more host assemblies. They are hierarchical collections where leaf nodes represent types
        /// and parent nodes represent namespaces. For example, if an assembly contains a type named
        /// "Acme.Gadgets.Button", the corresponding collection will have a property named "Acme" whose
        /// value is an object with a property named "Gadgets" whose value is an object with a property
        /// named "Button" whose value represents the <c>Acme.Gadgets.Button</c> host type.
        /// </remarks>
        /// <example>
        /// The following code imports types from several core assemblies and uses
        /// <see cref="System.Linq.Enumerable"/> to create an array of integers.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// var clr = host.lib("mscorlib", "System", "System.Core");
        /// var array = clr.System.Linq.Enumerable.Range(0, 5).ToArray();
        /// </code>
        /// </example>
        public HostTypeCollection lib(params string[] assemblyNames)
        {
            return lib(null, assemblyNames);
        }

        /// <summary>
        /// Imports types from one or more host assemblies and merges them with an existing host type collection.
        /// </summary>
        /// <param name="collection">The host type collection with which to merge types from the specified assemblies.</param>
        /// <param name="assemblyNames">The names of the assemblies that contain the types to import.</param>
        /// <returns>A host type collection: <paramref name="collection"/> if it is not <c>null</c>, a new host type collection otherwise.</returns>
        /// <remarks>
        /// Host type collections provide convenient scriptable access to all the types defined in one
        /// or more host assemblies. They are hierarchical collections where leaf nodes represent types
        /// and parent nodes represent namespaces. For example, if an assembly contains a type named
        /// "Acme.Gadgets.Button", the corresponding collection will have a property named "Acme" whose
        /// value is an object with a property named "Gadgets" whose value is an object with a property
        /// named "Button" whose value represents the <c>Acme.Gadgets.Button</c> host type.
        /// </remarks>
        /// <example>
        /// The following code imports types from several core assemblies and uses
        /// <see cref="System.Linq.Enumerable"/> to create an array of integers.
        /// It assumes that an instance of <see cref="ExtendedHostFunctions"/> is exposed under
        /// the name "host"
        /// (see <see cref="ScriptEngine.AddHostObject(string, object)">AddHostObject</see>).
        /// <code lang="JavaScript">
        /// var clr = host.lib("mscorlib");
        /// host.lib(clr, "System");
        /// host.lib(clr, "System.Core");
        /// var array = clr.System.Linq.Enumerable.Range(0, 5).ToArray();
        /// </code>
        /// </example>
        public HostTypeCollection lib(HostTypeCollection collection, params string[] assemblyNames)
        {
            var target = collection ?? new HostTypeCollection();
            Array.ForEach(assemblyNames, target.AddAssembly);
            return target;
        }

        // ReSharper restore InconsistentNaming

        #endregion
    }
}
