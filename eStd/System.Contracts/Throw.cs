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
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;

namespace System.Contracts
{
    /// <summary>
    /// All throw logic is factored out of the public extension methods and put in this helper class. This 
    /// allows more methods to be a candidate for inlining by the JIT compiler.
    /// </summary>
    internal static class Throw
    {
        internal static void ValueShouldNotBeNull<T>(ContractValidator<T> validator, string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldNotBeNull,
                conditionDescription, validator.ArgumentName);
                
            validator.ThrowException(condition);
        }

        internal static void ValueShouldBeBetween<T>(ContractValidator<T> validator, T minValue, T maxValue,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldBeBetweenXAndY,
                conditionDescription, validator.ArgumentName, minValue.Stringify(), maxValue.Stringify());

            string additionalMessage = GetActualValueMessage(validator);
            ConstraintViolationType violationType = 
                GetEnumViolationOrDefault<T>(ConstraintViolationType.OutOfRangeViolation);

            validator.ThrowException(condition, additionalMessage, violationType);
        }

        internal static void ValueShouldBeEqualTo<T>(ContractValidator<T> validator, T value, 
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldBeEqualToX,
                conditionDescription, validator.ArgumentName, value.Stringify());

            string additionalMessage = GetActualValueMessage(validator);
            ConstraintViolationType violationType = GetEnumViolationOrDefault<T>();

            validator.ThrowException(condition, additionalMessage, violationType);
        }

        internal static void ValueShouldBeNull<T>(ContractValidator<T> validator, string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldBeNull,
                conditionDescription, validator.ArgumentName);

            validator.ThrowException(condition);
        }

        internal static void ValueShouldBeGreaterThan<T>(ContractValidator<T> validator, T minValue,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldBeGreaterThanX,
                conditionDescription, validator.ArgumentName, minValue.Stringify());

            string additionalMessage = GetActualValueMessage(validator);
            ConstraintViolationType violationType =
                GetEnumViolationOrDefault<T>(ConstraintViolationType.OutOfRangeViolation);

            validator.ThrowException(condition, additionalMessage, violationType);
        }

        internal static void ValueShouldNotBeGreaterThan<T>(ContractValidator<T> validator, T minValue,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldNotBeGreaterThanX,
                conditionDescription, validator.ArgumentName, minValue.Stringify());

            string additionalMessage = GetActualValueMessage(validator);
            ConstraintViolationType violationType =
                GetEnumViolationOrDefault<T>(ConstraintViolationType.OutOfRangeViolation);

            validator.ThrowException(condition, additionalMessage, violationType);
        }

        internal static void ValueShouldBeGreaterThanOrEqualTo<T>(ContractValidator<T> validator, T minValue,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldBeGreaterThanOrEqualToX,
               conditionDescription, validator.ArgumentName, minValue.Stringify());

            string additionalMessage = GetActualValueMessage(validator);
            ConstraintViolationType type =
                GetEnumViolationOrDefault<T>(ConstraintViolationType.OutOfRangeViolation);

            validator.ThrowException(condition, additionalMessage, type);
        }

        internal static void ValueShouldNotBeGreaterThanOrEqualTo<T>(ContractValidator<T> validator, T maxValue,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldNotBeGreaterThanOrEqualToX,
               conditionDescription, validator.ArgumentName, maxValue.Stringify());

            string additionalMessage = GetActualValueMessage(validator);
            ConstraintViolationType type =
                GetEnumViolationOrDefault<T>(ConstraintViolationType.OutOfRangeViolation);

            validator.ThrowException(condition, additionalMessage, type);
        }

        internal static void ValueShouldBeSmallerThan<T>(ContractValidator<T> validator, T maxValue,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldBeSmallerThanX,
               conditionDescription, validator.ArgumentName, maxValue.Stringify());

            string additionalMessage = GetActualValueMessage(validator);
            ConstraintViolationType violationType =
                GetEnumViolationOrDefault<T>(ConstraintViolationType.OutOfRangeViolation);

            validator.ThrowException(condition, additionalMessage, violationType);
        }

        internal static void ValueShouldNotBeSmallerThan<T>(ContractValidator<T> validator, T minValue,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldNotBeSmallerThanX,
               conditionDescription, validator.ArgumentName, minValue.Stringify());

            string additionalMessage = GetActualValueMessage(validator);
            ConstraintViolationType violationType =
                GetEnumViolationOrDefault<T>(ConstraintViolationType.OutOfRangeViolation);

            validator.ThrowException(condition, additionalMessage, violationType);
        }

        internal static void ValueShouldBeSmallerThanOrEqualTo<T>(ContractValidator<T> validator, T maxValue,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldBeSmallerThanOrEqualToX,
               conditionDescription, validator.ArgumentName, maxValue.Stringify());

            string additionalMessage = GetActualValueMessage(validator);
            ConstraintViolationType violationType =
                GetEnumViolationOrDefault<T>(ConstraintViolationType.OutOfRangeViolation);

            validator.ThrowException(condition, additionalMessage, violationType);
        }

        internal static void ValueShouldNotBeSmallerThanOrEqualTo<T>(ContractValidator<T> validator, T minValue,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldNotBeSmallerThanOrEqualToX,
               conditionDescription, validator.ArgumentName, minValue.Stringify());

            string additionalMessage = GetActualValueMessage(validator);
            ConstraintViolationType violationType =
                GetEnumViolationOrDefault<T>(ConstraintViolationType.OutOfRangeViolation);

            validator.ThrowException(condition, additionalMessage, violationType);
        }

        internal static void ExpressionEvaluatedFalse<T>(ContractValidator<T> validator, string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldBeValid,
                conditionDescription, validator.ArgumentName);

            string additionalMessage = GetActualValueMessage(validator);
            ConstraintViolationType violationType = GetEnumViolationOrDefault<T>();

            validator.ThrowException(condition, additionalMessage, violationType);
        }

        internal static void LambdaXShouldHoldForValue<T>(ContractValidator<T> validator, LambdaExpression lambda,
            string conditionDescription)
        {
            string lambdaDefinition = GetLambdaDefinition(lambda);

            string condition = GetFormattedConditionMessage(validator, SR.LambdaXShouldHoldForValue,
                conditionDescription, validator.ArgumentName, lambdaDefinition);

            string additionalMessage = GetActualValueMessage(validator);
            ConstraintViolationType violationType = GetEnumViolationOrDefault<T>();

            validator.ThrowException(condition, additionalMessage, violationType);
        }
        
        internal static void ValueShouldBeNullOrAnEmptyString(ContractValidator<string> validator,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.StringShouldBeNullOrEmpty,
                conditionDescription, validator.ArgumentName);

            validator.ThrowException(condition);
        }

        internal static void ValueShouldBeAnEmptyString(ContractValidator<string> validator, 
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.StringShouldBeEmpty,
                conditionDescription, validator.ArgumentName);

            validator.ThrowException(condition);
        }

        internal static void ValueShouldNotBeAnEmptyString(ContractValidator<string> validator,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.StringShouldNotBeEmpty,
                conditionDescription, validator.ArgumentName);

            validator.ThrowException(condition);
        }

        internal static void ValueShouldNotBeNullOrAnEmptyString(ContractValidator<string> validator,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.StringShouldNotBeNullOrEmpty,
                conditionDescription, validator.ArgumentName);

            validator.ThrowException(condition);
        }

        internal static void ValueShouldBeUnequalTo<T>(ContractValidator<T> validator, T value,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldBeUnequalToX,
                conditionDescription, validator.ArgumentName, value.Stringify());

            ConstraintViolationType violationType = GetEnumViolationOrDefault<T>();

            validator.ThrowException(condition, violationType);
        }

        internal static void ValueShouldNotBeBetween<T>(ContractValidator<T> validator, T minValue, T maxValue,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldNotBeBetweenXAndY,
                conditionDescription, validator.ArgumentName, minValue.Stringify(), maxValue.Stringify());

            string additionalMessage = GetActualValueMessage(validator);
            ConstraintViolationType violationType = GetEnumViolationOrDefault<T>();

            validator.ThrowException(condition, additionalMessage, violationType);
        }

        internal static void StringShouldBeNullOrWhiteSpace(ContractValidator<string> validator,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.StringShouldBeNullOrWhiteSpace,
                conditionDescription, validator.ArgumentName);

            validator.ThrowException(condition);
        }

        internal static void StringShouldNotBeNullOrWhiteSpace(ContractValidator<string> validator,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.StringShouldNotBeNullOrWhiteSpace,
                conditionDescription, validator.ArgumentName);

            validator.ThrowException(condition);
        }

        internal static void StringShouldHaveLength(ContractValidator<string> validator, int length,
            string conditionDescription)
        {
            string condition;

            if (length == 1)
            {
                condition = GetFormattedConditionMessage(validator, SR.StringShouldBe1CharacterLong,
                    conditionDescription, validator.ArgumentName);
            }
            else
            {
                condition = GetFormattedConditionMessage(validator, SR.StringShouldBeXCharactersLong,
                    conditionDescription, validator.ArgumentName, length);
            }

            string additionalMessage = GetActualStringLengthMessage(validator);

            validator.ThrowException(condition, additionalMessage);
        }

        internal static void StringShouldNotHaveLength(ContractValidator<string> validator, int length,
            string conditionDescription)
        {
            string condition;

            if (length == 1)
            {
                condition = GetFormattedConditionMessage(validator, SR.StringShouldNotBe1CharacterLong,
                    conditionDescription, validator.ArgumentName);
            }
            else
            {
                condition = GetFormattedConditionMessage(validator, SR.StringShouldNotBeXCharactersLong,
                    conditionDescription, validator.ArgumentName, length);
            }

            validator.ThrowException(condition);
        }

        internal static void StringShouldBeLongerThan(ContractValidator<string> validator, int minLength,
            string conditionDescription)
        {
            string condition;

            if (minLength == 1)
            {
                condition = GetFormattedConditionMessage(validator, SR.StringShouldBeLongerThan1Character,
                    conditionDescription, validator.ArgumentName);
            }
            else
            {
                condition = GetFormattedConditionMessage(validator, SR.StringShouldBeLongerThanXCharacters,
                    conditionDescription, validator.ArgumentName, minLength);
            }

            string additionalMessage = GetActualStringLengthMessage(validator);

            validator.ThrowException(condition, additionalMessage);
        }

        internal static void StringShouldBeShorterThan(ContractValidator<string> validator, int maxLength,
            string conditionDescription)
        {
            string condition;

            if (maxLength == 1)
            {
                condition = GetFormattedConditionMessage(validator, SR.StringShouldBeShorterThan1Character,
                    conditionDescription, validator.ArgumentName);
            }
            else
            {
                condition = GetFormattedConditionMessage(validator, SR.StringShouldBeShorterThanXCharacters,
                    conditionDescription, validator.ArgumentName, maxLength);
            }

            string additionalMessage = GetActualStringLengthMessage(validator);

            validator.ThrowException(condition, additionalMessage);
        }

        internal static void StringShouldBeShorterOrEqualTo(ContractValidator<string> validator, int maxLength,
            string conditionDescription)
        {
            string condition;

            if (maxLength == 1)
            {
                condition = 
                    GetFormattedConditionMessage(validator, SR.StringShouldBeShorterOrEqualTo1Character,
                    conditionDescription, validator.ArgumentName);
            }
            else
            {
                condition = 
                    GetFormattedConditionMessage(validator, SR.StringShouldBeShorterOrEqualToXCharacters,
                    conditionDescription, validator.ArgumentName, maxLength);
            }

            string additionalMessage = GetActualStringLengthMessage(validator);

            validator.ThrowException(condition, additionalMessage);
        }

        internal static void StringShouldBeLongerOrEqualTo(ContractValidator<string> validator, int minLength,
            string conditionDescription)
        {
            string condition;

            if (minLength == 1)
            {
                condition =
                    GetFormattedConditionMessage(validator, SR.StringShouldBeLongerOrEqualTo1Character,
                    conditionDescription, validator.ArgumentName);
            }
            else
            {
                condition =
                    GetFormattedConditionMessage(validator, SR.StringShouldBeLongerOrEqualToXCharacters,
                    conditionDescription, validator.ArgumentName, minLength);
            }

            string additionalMessage = GetActualStringLengthMessage(validator);

            validator.ThrowException(condition, additionalMessage);
        }

        internal static void StringShouldContain(ContractValidator<string> validator, string value,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.StringShouldContainX,
                conditionDescription, validator.ArgumentName, value.Stringify());

            validator.ThrowException(condition);
        }

        internal static void StringShouldNotContain(ContractValidator<string> validator, string value,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.StringShouldNotContainX,
                conditionDescription, validator.ArgumentName, value.Stringify());

            validator.ThrowException(condition);
        }

        internal static void StringShouldNotEndWith(ContractValidator<string> validator, string value,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.StringShouldNotEndWithX,
                conditionDescription, validator.ArgumentName, value.Stringify());

            validator.ThrowException(condition);
        }

        internal static void StringShouldNotStartWith(ContractValidator<string> validator, string value,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.StringShouldNotStartWithX,
                conditionDescription, validator.ArgumentName, value.Stringify());

            validator.ThrowException(condition);
        }

        internal static void StringShouldEndWith(ContractValidator<string> validator, string value,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.StringShouldEndWithX,
                conditionDescription, validator.ArgumentName, value.Stringify());

            validator.ThrowException(condition);
        }

        internal static void StringShouldStartWith(ContractValidator<string> validator, string value,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.StringShouldStartWithX,
                conditionDescription, validator.ArgumentName, value.Stringify());

            validator.ThrowException(condition);
        }

        internal static void ValueShouldBeOfType<T>(ContractValidator<T> validator, Type type,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldBeOfTypeX,
                conditionDescription, validator.ArgumentName, type.Name);

            validator.ThrowException(condition);
        }

        internal static void ValueShouldNotBeOfType<T>(ContractValidator<T> validator, Type type,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldNotBeOfTypeX,
                conditionDescription, validator.ArgumentName, type.Name);

            validator.ThrowException(condition);
        }

        internal static void ValueShouldBeTrue<T>(ContractValidator<T> validator, string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldBeTrue,
                conditionDescription, validator.ArgumentName);

            validator.ThrowException(condition);
        }

        internal static void ValueShouldBeFalse<T>(ContractValidator<T> validator, string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldBeFalse,
                conditionDescription, validator.ArgumentName);
            
            validator.ThrowException(condition);
        }

        internal static void ValueShouldBeANumber<T>(ContractValidator<T> validator, string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldBeANumber,
                conditionDescription, validator.ArgumentName);

            validator.ThrowException(condition);
        }

        internal static void ValueShouldNotBeANumber<T>(ContractValidator<T> validator, string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldNotBeANumber,
                conditionDescription, validator.ArgumentName);

            validator.ThrowException(condition);
        }

        internal static void ValueShouldBeInfinity<T>(ContractValidator<T> validator, string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldBeInfinity,
                conditionDescription, validator.ArgumentName);

            validator.ThrowException(condition);
        }

        internal static void ValueShouldNotBeInfinity<T>(ContractValidator<T> validator, string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldNotBeInfinity,
                conditionDescription, validator.ArgumentName);

            validator.ThrowException(condition);
        }

        internal static void ValueShouldBeNegativeInfinity<T>(ContractValidator<T> validator, string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldBeNegativeInfinity,
                conditionDescription, validator.ArgumentName);

            validator.ThrowException(condition);
        }

        internal static void ValueShouldNotBeNegativeInfinity<T>(ContractValidator<T> validator, string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldNotBeNegativeInfinity,
                conditionDescription, validator.ArgumentName);

            validator.ThrowException(condition);
        }

        internal static void ValueShouldBePositiveInfinity<T>(ContractValidator<T> validator, string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldBePositiveInfinity,
                conditionDescription, validator.ArgumentName);

            validator.ThrowException(condition);
        }

        internal static void ValueShouldNotBePositiveInfinity<T>(ContractValidator<T> validator, string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.ValueShouldNotBePositiveInfinity,
                conditionDescription, validator.ArgumentName);

            validator.ThrowException(condition);
        }

        internal static void CollectionShouldBeEmpty<T>(ContractValidator<T> validator, string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.CollectionShouldBeEmpty,
                conditionDescription, validator.ArgumentName);

            validator.ThrowException(condition);
        }

        internal static void CollectionShouldNotBeEmpty<T>(ContractValidator<T> validator,
            string conditionDescription) where T : IEnumerable
        {
            string condition = GetFormattedConditionMessage(validator, SR.CollectionShouldNotBeEmpty,
                conditionDescription, validator.ArgumentName);

            validator.ThrowException(condition);
        }

        internal static void CollectionShouldContain<T>(ContractValidator<T> validator, object value,
            string conditionDescription) where T : IEnumerable
        {
            string condition = GetFormattedConditionMessage(validator, SR.CollectionShouldContainX,
                conditionDescription, validator.ArgumentName, value.Stringify());

            validator.ThrowException(condition);
        }

        internal static void CollectionShouldNotContain<T>(ContractValidator<T> validator, object value,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.CollectionShouldNotContainX,
                conditionDescription, validator.ArgumentName, value.Stringify());

            validator.ThrowException(condition);
        }

        internal static void CollectionShouldContainAtLeastOneOf<T>(ContractValidator<T> validator,
            IEnumerable values, string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.CollectionShouldContainAtLeastOneOfX,
                conditionDescription, validator.ArgumentName, values.Stringify());

            validator.ThrowException(condition);
        }

        internal static void CollectionShouldNotContainAnyOf<T>(ContractValidator<T> validator, IEnumerable values,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.CollectionShouldNotContainAnyOfX,
               conditionDescription, validator.ArgumentName, values.Stringify());

            validator.ThrowException(condition);
        }

        internal static void CollectionShouldContainAllOf<T>(ContractValidator<T> validator, IEnumerable values,
            string conditionDescription) where T : IEnumerable
        {
            string condition = GetFormattedConditionMessage(validator, SR.CollectionShouldContainAllOfX,
               conditionDescription, validator.ArgumentName, values.Stringify());

            validator.ThrowException(condition);
        }

        internal static void CollectionShouldNotContainAllOf<T>(ContractValidator<T> validator, IEnumerable values,
            string conditionDescription)
        {
            string condition = GetFormattedConditionMessage(validator, SR.CollectionShouldNotContainAllOfX,
               conditionDescription, validator.ArgumentName, values.Stringify());

            validator.ThrowException(condition);
        }

        internal static void CollectionShouldContainNumberOfElements<T>(ContractValidator<T> validator, 
            int numberOfElements, string conditionDescription) where T : IEnumerable
        {
            string condition;

            if (numberOfElements == 1)
            {
                condition = GetFormattedConditionMessage(validator, SR.CollectionShouldContain1Element,
                    conditionDescription, validator.ArgumentName);
            }
            else
            {
                condition = GetFormattedConditionMessage(validator, SR.CollectionShouldContainXElements,
                    conditionDescription, validator.ArgumentName, numberOfElements);
            }
            
            validator.ThrowException(condition, GetCollectionContainsElementsMessage(validator));
        }

        internal static void CollectionShouldNotContainNumberOfElements<T>(ContractValidator<T> validator, 
            int numberOfElements, string conditionDescription)
        {
            string condition;

            if (numberOfElements == 1)
            {
                condition = GetFormattedConditionMessage(validator, SR.CollectionShouldNotContain1Element,
                   conditionDescription, validator.ArgumentName);
            }
            else
            {
                condition = GetFormattedConditionMessage(validator, SR.CollectionShouldNotContainXElements,
                   conditionDescription, validator.ArgumentName, numberOfElements);
            }

            validator.ThrowException(condition);
        }

        internal static void CollectionShouldContainLessThan<T>(ContractValidator<T> validator, int numberOfElements,
            string conditionDescription) where T : IEnumerable
        {
            string condition;

            if (numberOfElements == 1)
            {
                condition = GetFormattedConditionMessage(validator, SR.CollectionShouldContainLessThan1Element,
                    conditionDescription, validator.ArgumentName);
            }
            else
            {
                condition = GetFormattedConditionMessage(validator, SR.CollectionShouldContainLessThanXElements,
                    conditionDescription, validator.ArgumentName, numberOfElements);
            }

            string additionalMessage = GetCollectionContainsElementsMessage(validator);

            validator.ThrowException(condition, additionalMessage);
        }

        internal static void CollectionShouldNotContainLessThan<T>(ContractValidator<T> validator,
            int numberOfElements, string conditionDescription) where T : IEnumerable
        {
            string condition;

            if (numberOfElements == 1)
            {
                condition = 
                    GetFormattedConditionMessage(validator, SR.CollectionShouldNotContainLessThan1Element,
                        conditionDescription, validator.ArgumentName);
            }
            else
            {
                condition = 
                    GetFormattedConditionMessage(validator, SR.CollectionShouldNotContainLessThanXElements,
                        conditionDescription, validator.ArgumentName, numberOfElements);
            }

            string additionalMessage = GetCollectionContainsElementsMessage(validator);

            validator.ThrowException(condition, additionalMessage);
        }

        internal static void CollectionShouldContainLessOrEqual<T>(ContractValidator<T> validator,
            int numberOfElements, string conditionDescription) where T : IEnumerable
        {
            string condition =
                GetFormattedConditionMessage(validator, SR.CollectionShouldContainXOrLessElements, 
                conditionDescription, validator.ArgumentName, numberOfElements);

            string additionalMessage = GetCollectionContainsElementsMessage(validator);

            validator.ThrowException(condition, additionalMessage);
        }

        internal static void CollectionShouldNotContainLessOrEqual<T>(ContractValidator<T> validator,
            int numberOfElements, string conditionDescription) where T : IEnumerable
        {
            string condition =
               GetFormattedConditionMessage(validator, SR.CollectionShouldNotContainXOrLessElements,
               conditionDescription, validator.ArgumentName, numberOfElements);

            string additionalMessage = GetCollectionContainsElementsMessage(validator);

            validator.ThrowException(condition, additionalMessage);
        }

        internal static void CollectionShouldContainMoreThan<T>(ContractValidator<T> validator, int numberOfElements,
            string conditionDescription) where T : IEnumerable
        {
            string condition;

            if (numberOfElements == 1)
            {
                condition = GetFormattedConditionMessage(validator, SR.CollectionShouldContainMoreThan1Element,
                    conditionDescription, validator.ArgumentName);
            }
            else
            {
                condition = GetFormattedConditionMessage(validator, SR.CollectionShouldContainMoreThanXElements,
                    conditionDescription, validator.ArgumentName, numberOfElements);
            }

            string additionalMessage = GetCollectionContainsElementsMessage(validator);

            validator.ThrowException(condition, additionalMessage);
        }

        internal static void CollectionShouldNotContainMoreThan<T>(ContractValidator<T> validator,
            int numberOfElements, string conditionDescription) where T : IEnumerable
        {
            string condition;

            if (numberOfElements == 1)
            {
                condition = 
                    GetFormattedConditionMessage(validator, SR.CollectionShouldNotContainMoreThan1Element,
                    conditionDescription, validator.ArgumentName);
            }
            else
            {
                condition =
                    GetFormattedConditionMessage(validator, SR.CollectionShouldNotContainMoreThanXElements,
                    conditionDescription, validator.ArgumentName, numberOfElements);
            }

            string additionalMessage = GetCollectionContainsElementsMessage(validator);

            validator.ThrowException(condition, additionalMessage);
        }

        internal static void CollectionShouldContainMoreOrEqual<T>(ContractValidator<T> validator,
            int numberOfElements, string conditionDescription) where T : IEnumerable
        {
            string condition = 
                GetFormattedConditionMessage(validator, SR.CollectionShouldContainXOrMoreElements,
                    conditionDescription, validator.ArgumentName, numberOfElements);
            
            string additionalMessage = GetCollectionContainsElementsMessage(validator);

            validator.ThrowException(condition, additionalMessage);
        }

        internal static void CollectionShouldNotContainMoreOrEqual<T>(ContractValidator<T> validator,
            int numberOfElements, string conditionDescription) where T : IEnumerable
        {
            string condition =
                GetFormattedConditionMessage(validator, SR.CollectionShouldNotContainXOrMoreElements,
                    conditionDescription, validator.ArgumentName, numberOfElements);

            string additionalMessage = GetCollectionContainsElementsMessage(validator);

            validator.ThrowException(condition, additionalMessage);
        }

        // This method returns extra information about the value of the validator.
        private static string GetActualValueMessage<T>(ContractValidator<T> validator)
        {
            object value = validator.Value;

            // When the ToString method of the given type isn't overloaded, it returns the Type.FullName.
            // This information isn't very useful to the user, so in that case, we'll simply return null,
            // meaning: no extra information.
            if (value == null || value.GetType().FullName != value.ToString())
            {
                return SR.GetString(SR.TheActualValueIsX, validator.ArgumentName, validator.Value.Stringify());
            }

            return null;
        }

        private static string GetActualStringLengthMessage(ContractValidator<string> validator)
        {
            int length = validator.Value != null ? validator.Value.Length : 0;

            if (length == 1)
            {
                return SR.GetString(SR.TheActualValueIs1CharacterLong, validator.ArgumentName);
            }
            else
            {
                return SR.GetString(SR.TheActualValueIsXCharactersLong, validator.ArgumentName, length);
            }
        }

        private static string GetCollectionContainsElementsMessage<T>(ContractValidator<T> validator)
            where T : IEnumerable
        {
            IEnumerable collection = validator.Value;

            if (collection == null)
            {
                return SR.GetString(SR.CollectionIsCurrentlyANullReference, validator.ArgumentName);
            }
            else
            {
                int numberOfElements = CollectionHelpers.GetLength(collection);

                if (numberOfElements == 1)
                {
                    return SR.GetString(SR.CollectionContainsCurrently1Element, validator.ArgumentName);
                }
                else
                {
                    return SR.GetString(SR.CollectionContainsCurrentlyXElements, validator.ArgumentName,
                       numberOfElements);
                }
            }
        }

        // Returns the 'InvalidEnumViolation' when the T is an Enum and otherwise 'Default'.
        private static ConstraintViolationType GetEnumViolationOrDefault<T>()
        {
            return GetEnumViolationOrDefault<T>(ConstraintViolationType.Default);
        }

        // Returns the 'InvalidEnumViolation' when the T is an Enum and otherwise the specified defaultValue.
        private static ConstraintViolationType GetEnumViolationOrDefault<T>(
            ConstraintViolationType defaultValue)
        {
            if (typeof(T).IsEnum)
            {
                return ConstraintViolationType.InvalidEnumViolation;
            }
            else
            {
                return defaultValue;
            }
        }

        private static string GetFormattedConditionMessage<T>(ContractValidator<T> validator, string resourceKey,
            string conditionDescription, params object[] resourceFormatArguments)
        {
            if (conditionDescription != null)
            {
                return FormatConditionDescription(validator, conditionDescription);
            }
            else
            {
                return SR.GetString(resourceKey, resourceFormatArguments);
            }
        }

        private static string FormatConditionDescription<T>(ContractValidator<T> validator, 
            string conditionDescription)
        {
            try
            {
                return String.Format(CultureInfo.CurrentCulture, conditionDescription ?? String.Empty,
                    validator.ArgumentName);
            }
            catch (FormatException)
            {
                // We catch a FormatException. This code should only throw exceptions generated by the
                // validator.BuildException method. Throwing another exception would confuse the user and
                // would make debugging harder. When the user supplied an unformattable description, we simply
                // use the unformatted description as condition.
                return conditionDescription;
            }
        }

        private static string GetLambdaDefinition(LambdaExpression lambda)
        {
            if (lambda == null)
            {
                return "null";
            }

            Debug.Assert(lambda.Body != null, "It should be impossible to create a lambda without a body.");

            return lambda.Body.ToString();
        }
    }
}
