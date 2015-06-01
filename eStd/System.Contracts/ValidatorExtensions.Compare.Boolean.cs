using System;

namespace System.Contracts
{
    // Comparable checks for Boolean
    public static partial class ValidatorExtensions
    {
        /// <summary>
        /// Checks whether the given value is <b>true</b>. An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is <b>false</b>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is <b>false</b>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<bool> IsTrue(this ContractValidator<bool> validator)
        {
            if (!validator.Value)
            {
                Throw.ValueShouldBeTrue(validator, null);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is <b>true</b>. An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is <b>false</b>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is <b>false</b>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<bool> IsTrue(this ContractValidator<bool> validator, string conditionDescription)
        {
            if (!validator.Value)
            {
                Throw.ValueShouldBeTrue(validator, conditionDescription);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is <b>true</b>. An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is <b>false</b> or null, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is <b>false</b> or null, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<bool?> IsTrue(this ContractValidator<bool?> validator)
        {
            if (!(validator.Value == true))
            {
                Throw.ValueShouldBeTrue(validator, null);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is <b>true</b>. An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is <b>false</b> or null, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is <b>false</b> or null, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<bool?> IsTrue(this ContractValidator<bool?> validator, string conditionDescription)
        {
            if (!(validator.Value == true))
            {
                Throw.ValueShouldBeTrue(validator, conditionDescription);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is <b>false</b>. An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is <b>true</b>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is <b>true</b>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<bool> IsFalse(this ContractValidator<bool> validator)
        {
            if (validator.Value)
            {
                Throw.ValueShouldBeFalse(validator, null);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is <b>false</b>. An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is <b>true</b>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is <b>true</b>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<bool> IsFalse(this ContractValidator<bool> validator, string conditionDescription)
        {
            if (validator.Value)
            {
                Throw.ValueShouldBeFalse(validator, conditionDescription);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is <b>false</b>. An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is <b>true</b> or null, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is <b>true</b> or null, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<bool?> IsFalse(this ContractValidator<bool?> validator)
        {
            if (!(validator.Value == false))
            {
                Throw.ValueShouldBeFalse(validator, null);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is <b>false</b>. An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is <b>true</b> or null, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is <b>true</b> or null, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<bool?> IsFalse(this ContractValidator<bool?> validator, string conditionDescription)
        {
            if (!(validator.Value == false))
            {
                Throw.ValueShouldBeFalse(validator, conditionDescription);
            }

            return validator;
        }
    }
}