#region Copyright (c) 2009 S. van Deursen
/* The CuttingEdge.Conditions library enables developers to validate pre- and postconditions in a fluent 
 * manner.
 * 
 * To contact me, please visit my blog at http://www.cuttingedge.it/blogs/steven/ 
 *
 * Copyright (c) 2009 S. van Deursen
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
 * associated documentation files (the "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT
 * LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO
 * EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;

namespace System.Contracts
{
    // String checks
    public static partial class ValidatorExtensions
    {
        /// <summary>
        /// Checks whether the given value is <b>null</b> (Nothing in Visual Basic), empty, or consists only 
        /// of white-space characters.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is not <b>null</b>, not empty and does not consists only of white-space characters, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is not <b>null</b>, not empty and does not consists only of white-space characters, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> IsNullOrWhiteSpace(this ContractValidator<string> validator)
        {
            if (!StringIsNullOrWhiteSpace(validator.Value))
            {
                Throw.StringShouldBeNullOrWhiteSpace(validator, null);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is <b>null</b> (Nothing in Visual Basic), empty, or consists only 
        /// of white-space characters.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is not <b>null</b>, not empty and does not consists only of white-space characters, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is not <b>null</b>, not empty and does not consists only of white-space characters, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> IsNullOrWhiteSpace(this ContractValidator<string> validator,
            string conditionDescription)
        {
            if (!StringIsNullOrWhiteSpace(validator.Value))
            {
                Throw.StringShouldBeNullOrWhiteSpace(validator, conditionDescription);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is not <b>null</b> (Nothing in Visual Basic), not empty, and does 
        /// not consists only of white-space characters.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is empty or consists only of white-space characters, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the length of the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is <b>null</b>, empty or or consists only of white-space characters, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> IsNotNullOrWhiteSpace(this ContractValidator<string> validator)
        {
            if (StringIsNullOrWhiteSpace(validator.Value))
            {
                Throw.StringShouldNotBeNullOrWhiteSpace(validator, null);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is not <b>null</b> (Nothing in Visual Basic), not empty, and does 
        /// not consists only of white-space characters.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is empty or consists only of white-space characters, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the length of the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is <b>null</b>, empty or or consists only of white-space characters, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> IsNotNullOrWhiteSpace(this ContractValidator<string> validator,
            string conditionDescription)
        {
            if (StringIsNullOrWhiteSpace(validator.Value))
            {
                Throw.StringShouldNotBeNullOrWhiteSpace(validator, conditionDescription);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is shorter in length than <paramref name="maxLength"/>.
        /// An exception is thrown otherwise. A null reference is considered to have a length of 0.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="maxLength">The smallest invalid length.</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the length of the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is greater or equal to <paramref name="maxLength"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="maxLength"/> is smaller or equal to 0, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the length of the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is greater or equal <paramref name="maxLength"/> to, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> IsShorterThan(this ContractValidator<string> validator, int maxLength)
        {
            string value = validator.Value;

            int valueLength = 0;
            if (value != null)
            {
                valueLength = value.Length;
            }

            if (!(valueLength < maxLength))
            {
                Throw.StringShouldBeShorterThan(validator, maxLength, null);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is shorter in length than <paramref name="maxLength"/>.
        /// An exception is thrown otherwise. A null reference is considered to have a length of 0.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="maxLength">The smallest invalid length.</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the length of the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is greater or equal to <paramref name="maxLength"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="maxLength"/> is smaller or equal to 0, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the length of the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is greater or equal <paramref name="maxLength"/> to, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> IsShorterThan(this ContractValidator<string> validator, int maxLength,
            string conditionDescription)
        {
            string value = validator.Value;

            int valueLength = 0;
            if (value != null)
            {
                valueLength = value.Length;
            }

            if (!(valueLength < maxLength))
            {
                Throw.StringShouldBeShorterThan(validator, maxLength, conditionDescription);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is shorter or equal in length than <paramref name="maxLength"/>. 
        /// An exception is thrown otherwise. A null reference is considered to have a length of 0.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="maxLength">The biggest valid length.</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the length of the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is smaller than <paramref name="maxLength"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="maxLength"/> is smaller than 0, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the length of the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is smaller than <paramref name="maxLength"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> IsShorterOrEqual(this ContractValidator<string> validator, int maxLength)
        {
            string value = validator.Value;

            int valueLength = 0;
            if (value != null)
            {
                valueLength = value.Length;
            }

            if (!(valueLength <= maxLength))
            {
                Throw.StringShouldBeShorterOrEqualTo(validator, maxLength, null);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is shorter or equal in length than <paramref name="maxLength"/>. 
        /// An exception is thrown otherwise. A null reference is considered to have a length of 0.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="maxLength">The biggest valid length.</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the length of the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is smaller than <paramref name="maxLength"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="maxLength"/> is smaller than 0, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the length of the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is smaller than <paramref name="maxLength"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> IsShorterOrEqual(this ContractValidator<string> validator, int maxLength,
            string conditionDescription)
        {
            string value = validator.Value;

            int valueLength = 0;
            if (value != null)
            {
                valueLength = value.Length;
            }

            if (!(valueLength <= maxLength))
            {
                Throw.StringShouldBeShorterOrEqualTo(validator, maxLength, conditionDescription);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is longer in length than <paramref name="minLength"/>.
        /// An exception is thrown otherwise. A null reference is considered to have a length of 0.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="minLength">The biggest invalid length.</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the length of the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is smaller or equal to <paramref name="minLength"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="minLength"/> is greater or equal to 0, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the length of the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is smaller or equal to <paramref name="minLength"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> IsLongerThan(this ContractValidator<string> validator, int minLength)
        {
            string value = validator.Value;

            int valueLength = 0;
            if (value != null)
            {
                valueLength = value.Length;
            }

            if (!(valueLength > minLength))
            {
                Throw.StringShouldBeLongerThan(validator, minLength, null);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is longer in length than <paramref name="minLength"/>.
        /// An exception is thrown otherwise. A null reference is considered to have a length of 0.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="minLength">The biggest invalid length.</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the length of the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is smaller or equal to <paramref name="minLength"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="minLength"/> is greater or equal to 0, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the length of the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is smaller or equal to <paramref name="minLength"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> IsLongerThan(this ContractValidator<string> validator, int minLength,
            string conditionDescription)
        {
            string value = validator.Value;

            int valueLength = 0;
            if (value != null)
            {
                valueLength = value.Length;
            }

            if (!(valueLength > minLength))
            {
                Throw.StringShouldBeLongerThan(validator, minLength, conditionDescription);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is longer or equal in length than <paramref name="minLength"/>.
        /// An exception is thrown otherwise. A null reference is considered to have a length of 0.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="minLength">The smallest valid length.</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the length of the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is smaller than <paramref name="minLength"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="minLength"/> is greater than 0, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the length of the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is smaller than <paramref name="minLength"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> IsLongerOrEqual(this ContractValidator<string> validator, int minLength)
        {
            string value = validator.Value;

            int valueLength = 0;
            if (value != null)
            {
                valueLength = value.Length;
            }

            if (!(valueLength >= minLength))
            {
                Throw.StringShouldBeLongerOrEqualTo(validator, minLength, null);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is longer or equal in length than <paramref name="minLength"/>.
        /// An exception is thrown otherwise. A null reference is considered to have a length of 0.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="minLength">The smallest valid length.</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the length of the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is smaller than <paramref name="minLength"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="minLength"/> is greater than 0, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the length of the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is smaller than <paramref name="minLength"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> IsLongerOrEqual(this ContractValidator<string> validator, int minLength,
            string conditionDescription)
        {
            string value = validator.Value;

            int valueLength = 0;
            if (value != null)
            {
                valueLength = value.Length;
            }

            if (!(valueLength >= minLength))
            {
                Throw.StringShouldBeLongerOrEqualTo(validator, minLength, conditionDescription);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is equal in length to <paramref name="length"/>.
        /// An exception is thrown otherwise. A null reference is considered to have a length of 0.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="length">The valid length.</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the length of <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> un equals <paramref name="length"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="length"/> un equals 0, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the length of <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> un equals <paramref name="length"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> HasLength(this ContractValidator<string> validator, int length)
        {
            string value = validator.Value;

            int valueLength = 0;
            if (value != null)
            {
                valueLength = value.Length;
            }

            if (!(valueLength == length))
            {
                Throw.StringShouldHaveLength(validator, length, null);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is equal in length to <paramref name="length"/>.
        /// An exception is thrown otherwise. A null reference is considered to have a length of 0.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="length">The valid length.</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the length of <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> un equals <paramref name="length"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="length"/> un equals 0, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the length of <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> un equals <paramref name="length"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> HasLength(this ContractValidator<string> validator, int length,
            string conditionDescription)
        {
            string value = validator.Value;

            int valueLength = 0;
            if (value != null)
            {
                valueLength = value.Length;
            }

            if (!(valueLength == length))
            {
                Throw.StringShouldHaveLength(validator, length, conditionDescription);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is unequal in length to <paramref name="length"/>.
        /// An exception is thrown otherwise. A null reference is considered to have a length of 0.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="length">The invalid length.</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the length of <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> equals <paramref name="length"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="length"/> un equals 0, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the length of <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> equals <paramref name="length"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> DoesNotHaveLength(this ContractValidator<string> validator, int length)
        {
            string value = validator.Value;

            int valueLength = 0;
            if (value != null)
            {
                valueLength = value.Length;
            }

            if (!(valueLength != length))
            {
                Throw.StringShouldNotHaveLength(validator, length, null);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is unequal in length to <paramref name="length"/>.
        /// An exception is thrown otherwise. A null reference is considered to have a length of 0.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="length">The invalid length.</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the length of <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> equals <paramref name="length"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="length"/> un equals 0, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the length of <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> equals <paramref name="length"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> DoesNotHaveLength(this ContractValidator<string> validator, int length,
            string conditionDescription)
        {
            string value = validator.Value;

            int valueLength = 0;
            if (value != null)
            {
                valueLength = value.Length;
            }

            if (!(valueLength != length))
            {
                Throw.StringShouldNotHaveLength(validator, length, conditionDescription);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is null or an <see cref="String.Empty"/> string.
        /// An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is not null or empty, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is not null or empty, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> IsNullOrEmpty(this ContractValidator<string> validator)
        {
            if (!String.IsNullOrEmpty(validator.Value))
            {
                Throw.ValueShouldBeNullOrAnEmptyString(validator, null);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is null or an <see cref="String.Empty"/> string.
        /// An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is not null or empty, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is not null or empty, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> IsNullOrEmpty(this ContractValidator<string> validator, 
            string conditionDescription)
        {
            if (!String.IsNullOrEmpty(validator.Value))
            {
                Throw.ValueShouldBeNullOrAnEmptyString(validator, conditionDescription);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is not null and not an <see cref="String.Empty"/> string.
        /// An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is <see cref="String.Empty"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is null or empty, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is null or empty, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> IsNotNullOrEmpty(this ContractValidator<string> validator)
        {
            bool valueIsInvalid = String.IsNullOrEmpty(validator.Value);

            if (valueIsInvalid)
            {
                Throw.ValueShouldNotBeNullOrAnEmptyString(validator, null);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is not null and not an <see cref="String.Empty"/> string.
        /// An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is <see cref="String.Empty"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is null or empty, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is null or empty, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> IsNotNullOrEmpty(this ContractValidator<string> validator,
            string conditionDescription)
        {
            bool valueIsInvalid = String.IsNullOrEmpty(validator.Value);

            if (valueIsInvalid)
            {
                Throw.ValueShouldNotBeNullOrAnEmptyString(validator, conditionDescription);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is an <see cref="String.Empty"/> string. An exception is thrown 
        /// otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is not empty, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is not empty, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> IsEmpty(this ContractValidator<string> validator)
        {
            if (!(validator.Value != null && validator.Value.Length == 0))
            {
                Throw.ValueShouldBeAnEmptyString(validator, null);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is an <see cref="String.Empty"/> string. An exception is thrown 
        /// otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is not empty, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is not empty, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> IsEmpty(this ContractValidator<string> validator, string conditionDescription)
        {
            if (!(validator.Value != null && validator.Value.Length == 0))
            {
                Throw.ValueShouldBeAnEmptyString(validator, conditionDescription);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is not an <see cref="String.Empty"/> string. An exception is thrown
        /// otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is empty, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is empty, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> IsNotEmpty(this ContractValidator<string> validator)
        {
            if (validator.Value != null && validator.Value.Length == 0)
            {
                Throw.ValueShouldNotBeAnEmptyString(validator, null);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value is not an <see cref="String.Empty"/> string. An exception is thrown
        /// otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is empty, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is empty, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> IsNotEmpty(this ContractValidator<string> validator, 
            string conditionDescription)
        {
            if (validator.Value != null && validator.Value.Length == 0)
            {
                Throw.ValueShouldNotBeAnEmptyString(validator, conditionDescription);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value starts with the specified <paramref name="value"/>.
        /// An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="value">The value to compare.</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> does not start with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="value"/> is not a null reference, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> does not start with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> StartsWith(this ContractValidator<string> validator, string value)
        {
            return StartsWith(validator, value, StringComparison.CurrentCulture, null);
        }

        /// <summary>
        /// Checks whether the given value starts with the specified <paramref name="value"/>.
        /// An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="value">The value to compare.</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> does not start with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="value"/> is not a null reference, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> does not start with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> StartsWith(this ContractValidator<string> validator, string value,
            string conditionDescription)
        {
            return StartsWith(validator, value, StringComparison.CurrentCulture, conditionDescription);
        }

        /// <summary>
        /// Checks whether the given value starts with the specified <paramref name="value"/> using the
        /// specified <paramref name="comparisonType"/>.
        /// An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="value">The value to compare.</param>
        /// <param name="comparisonType">One of the <see cref="StringComparison"/> values that determines how 
        /// this string and value are compared</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> does not start with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="value"/> is not a null reference, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> does not start with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> StartsWith(this ContractValidator<string> validator, string value,
            StringComparison comparisonType)
        {
            return StartsWith(validator, value, comparisonType, null);
        }

        /// <summary>
        /// Checks whether the given value starts with the specified <paramref name="value"/> using the
        /// specified <paramref name="comparisonType"/>.
        /// An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="value">The value to compare.</param>
        /// <param name="comparisonType">One of the <see cref="StringComparison"/> values that determines how 
        /// this string and value are compared</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> does not start with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="value"/> is not a null reference, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> does not start with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> StartsWith(this ContractValidator<string> validator, string value,
            StringComparison comparisonType, string conditionDescription)
        {
            string validatorValue = validator.Value;

            bool valueIsValid =
                (value == null && validatorValue == null) ||
                (value != null && validatorValue != null && validatorValue.StartsWith(value, comparisonType));

            if (!valueIsValid)
            {
                Throw.StringShouldStartWith(validator, value, conditionDescription);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value does not start with the specified <paramref name="value"/>.
        /// An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="value">The value to compare.</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> start with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="value"/> is not a null reference, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> start with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> DoesNotStartWith(this ContractValidator<string> validator, string value)
        {
            return DoesNotStartWith(validator, value, StringComparison.CurrentCulture, null);
        }

        /// <summary>
        /// Checks whether the given value does not start with the specified <paramref name="value"/>.
        /// An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="value">The value to compare.</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> start with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="value"/> is not a null reference, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> start with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> DoesNotStartWith(this ContractValidator<string> validator, string value,
            string conditionDescription)
        {
            return DoesNotStartWith(validator, value, StringComparison.CurrentCulture, conditionDescription);
        }

        /// <summary>
        /// Checks whether the given value does not start with the specified <paramref name="value"/> using the
        /// specified comparison option.
        /// An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="value">The value to compare.</param>
        /// <param name="comparisonType">One of the <see cref="StringComparison"/> values that determines how 
        /// this string and value are compared</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> start with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="value"/> is not a null reference, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> start with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> DoesNotStartWith(this ContractValidator<string> validator, string value, 
            StringComparison comparisonType)
        {
            return DoesNotStartWith(validator, value, comparisonType, null);
        }

        /// <summary>
        /// Checks whether the given value does not start with the specified <paramref name="value"/> using the
        /// specified comparison option.
        /// An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="value">The value to compare.</param>
        /// <param name="comparisonType">One of the <see cref="StringComparison"/> values that determines how 
        /// this string and value are compared</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> start with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="value"/> is not a null reference, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> start with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> DoesNotStartWith(this ContractValidator<string> validator, string value,
            StringComparison comparisonType, string conditionDescription)
        {
            string validatorValue = validator.Value;

            bool valueIsInvalid =
                (value == null && validatorValue == null) ||
                (value != null && validatorValue != null && validatorValue.StartsWith(value, comparisonType));

            if (valueIsInvalid)
            {
                Throw.StringShouldNotStartWith(validator, value, conditionDescription);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value contains the specified <paramref name="value"/>.
        /// An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="value">The value to compare.</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> does not contain <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="value"/> contains no null reference, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> does not contain <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> Contains(this ContractValidator<string> validator, string value)
        {
            return Contains(validator, value, null);
        }

        /// <summary>
        /// Checks whether the given value contains the specified <paramref name="value"/>.
        /// An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="value">The value to compare.</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> does not contain <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="value"/> contains no null reference, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> does not contain <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> Contains(this ContractValidator<string> validator, string value,
            string conditionDescription)
        {
            string validatorValue = validator.Value;

            bool valueIsValid =
                (value == null && validatorValue == null) ||
                (value != null && validatorValue != null && validatorValue.Contains(value));

            if (!valueIsValid)
            {
                Throw.StringShouldContain(validator, value, conditionDescription);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the given value does not contain the specified <paramref name="value"/>.
        /// An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="value">The value to compare.</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> contains <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> and <paramref name="value"/> are both null references, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> contains <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> DoesNotContain(this ContractValidator<string> validator, string value)
        {
            return DoesNotContain(validator, value, null);
        }

        /// <summary>
        /// Checks whether the given value does not contain the specified <paramref name="value"/>.
        /// An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="value">The value to compare.</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> contains <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> and <paramref name="value"/> are both null references, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> contains <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> DoesNotContain(this ContractValidator<string> validator, string value,
            string conditionDescription)
        {
            string validatorValue = validator.Value;

            bool valueIsInvalid =
                (value == null && validatorValue == null) ||
                (value != null && validatorValue != null && validatorValue.Contains(value));

            if (valueIsInvalid)
            {
                Throw.StringShouldNotContain(validator, value, conditionDescription);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the end of the given value matches the specified <paramref name="value"/>.
        /// An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="value">The value to compare.</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> does not end with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="value"/> is not a null reference, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> does not end with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> EndsWith(this ContractValidator<string> validator, string value)
        {
            return EndsWith(validator, value, StringComparison.CurrentCulture, null);
        }

        /// <summary>
        /// Checks whether the end of the given value matches the specified <paramref name="value"/>.
        /// An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="value">The value to compare.</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> does not end with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="value"/> is not a null reference, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> does not end with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> EndsWith(this ContractValidator<string> validator, string value,
            string conditionDescription)
        {
            return EndsWith(validator, value, StringComparison.CurrentCulture, conditionDescription);
        }

        /// <summary>
        /// Checks whether the end of the given value matches the specified <paramref name="value"/> using the
        /// specified comparison option.
        /// An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="value">The value to compare.</param>
        /// <param name="comparisonType">One of the <see cref="StringComparison"/> values that determines how 
        /// this string and value are compared</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> does not end with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="value"/> is not a null reference, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> does not end with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> EndsWith(this ContractValidator<string> validator, string value,
            StringComparison comparisonType)
        {
            return EndsWith(validator, value, comparisonType, null);
        }

        /// <summary>
        /// Checks whether the end of the given value matches the specified <paramref name="value"/> using the
        /// specified comparison option.
        /// An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="value">The value to compare.</param>
        /// <param name="comparisonType">One of the <see cref="StringComparison"/> values that determines how 
        /// this string and value are compared</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> does not end with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="value"/> is not a null reference, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> does not end with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> EndsWith(this ContractValidator<string> validator, string value,
            StringComparison comparisonType, string conditionDescription)
        {
            string validatorValue = validator.Value;

            bool valueIsValid =
                (value == null && validatorValue == null) ||
                (value != null && validatorValue != null && validatorValue.EndsWith(value, comparisonType));

            if (!valueIsValid)
            {
                Throw.StringShouldEndWith(validator, value, conditionDescription);
            }

            return validator;
        }

        /// <summary>
        /// Checks whether the end of the given value does not match the specified <paramref name="value"/>.
        /// An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="value">The value to compare.</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> ends with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="value"/> is not a null reference, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> ends with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> DoesNotEndWith(this ContractValidator<string> validator, string value)
        {
            return DoesNotEndWith(validator, value, StringComparison.CurrentCulture, null);
        }

        /// <summary>
        /// Checks whether the end of the given value does not match the specified <paramref name="value"/>.
        /// An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="value">The value to compare.</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> ends with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="value"/> is not a null reference, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> ends with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> DoesNotEndWith(this ContractValidator<string> validator, string value,
            string conditionDescription)
        {
            return DoesNotEndWith(validator, value, StringComparison.CurrentCulture, conditionDescription);
        }

        /// <summary>
        /// Checks whether the end of the given value does not match the specified <paramref name="value"/> 
        /// using the specified comparison option.
        /// An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="value">The value to compare.</param>
        /// <param name="comparisonType">One of the <see cref="StringComparison"/> values that determines how 
        /// this string and value are compared</param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> ends with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="value"/> is not a null reference, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> ends with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> DoesNotEndWith(this ContractValidator<string> validator, string value,
            StringComparison comparisonType)
        {
            return DoesNotEndWith(validator, value, comparisonType, null);
        }

        /// <summary>
        /// Checks whether the end of the given value does not match the specified <paramref name="value"/> 
        /// using the specified comparison option.
        /// An exception is thrown otherwise.
        /// </summary>
        /// <param name="validator">The <see cref="ContractValidator{T}"/> that holds the value that has to be checked.</param>
        /// <param name="value">The value to compare.</param>
        /// <param name="comparisonType">One of the <see cref="StringComparison"/> values that determines how 
        /// this string and value are compared</param>
        /// <param name="conditionDescription">
        /// The description of the condition that should hold. The string may hold the placeholder '{0}' for 
        /// the <see cref="ContractValidator{T}.ArgumentName">ArgumentName</see>.
        /// </param>
        /// <returns>The specified <paramref name="validator"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> ends with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> is a null reference and <paramref name="value"/> is not a null reference, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Requires{T}(T,string)">Requires</see> extension method.</exception>
        /// <exception cref="PostconditionException">Thrown when the <see cref="ContractValidator{T}.Value">Value</see> of the specified <paramref name="validator"/> ends with <paramref name="value"/>, while the specified <paramref name="validator"/> is created using the <see cref="Condition.Ensures{T}(T,string)">Ensures</see> extension method.</exception>
        public static ContractValidator<string> DoesNotEndWith(this ContractValidator<string> validator, string value,
            StringComparison comparisonType, string conditionDescription)
        {
            string validatorValue = validator.Value;

            bool valueIsInvalid =
                (value == null && validatorValue == null) ||
                (value != null && validatorValue != null && validatorValue.EndsWith(value, comparisonType));

            if (valueIsInvalid)
            {
                Throw.StringShouldNotEndWith(validator, value, conditionDescription);
            }

            return validator;
        }

        private static bool StringIsNullOrWhiteSpace(string value)
        {
            if (value != null)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (!char.IsWhiteSpace(value[i]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}