﻿using System;
using System.Reflection;

namespace System.Contracts
{
    /// <summary>
    /// Internal helper class to cache a <see cref="AlternativeExceptionCondition"/> and 
    /// <see cref="ConstructorInfo"/> instance per exception type.
    /// </summary>
    /// <typeparam name="TException">The type of the exception.</typeparam>
    internal static class AlternativeExceptionHelper<TException> where TException : Exception
    {
        internal static readonly ConstructorInfo Constructor = FindConstructor();

        internal static readonly AlternativeExceptionContract Condition = BuildCondition();

        private static ConstructorInfo FindConstructor()
        {
            if (typeof(TException).IsAbstract)
            {
                return null;
            }

            return typeof(TException).GetConstructor(new[] { typeof(string) });
        }

        private static AlternativeExceptionContract BuildCondition()
        {
            bool isValidType = FindConstructor() != null;

            return isValidType ? new AlternativeExceptionConditionInternal() : null;
        }

        /// <summary>Allows creating validators for a specific exception type.</summary>
        private sealed class AlternativeExceptionConditionInternal : AlternativeExceptionContract
        {
            /// <summary>
            /// Returns a new <see cref="ContractValidator{T}">ConditionValidator</see> that allows you to
            /// validate the preconditions of the given argument, given it a default ArgumentName of 'value'.
            /// </summary>
            /// <typeparam name="T">The type of the argument to validate.</typeparam>
            /// <param name="value">The value of the argument to validate.</param>
            /// <returns>A new <see cref="ContractValidator{T}">ConditionValidator</see> containing the 
            /// <paramref name="value"/> and "value" as argument name.</returns>
            public override ContractValidator<T> Requires<T>(T value)
            {
                return new RequiresWithCustomExceptionValidator<T, TException>("value", value);
            }

            /// <summary>
            /// Returns a new <see cref="ContractValidator{T}">ConditionValidator</see> that allows you to
            /// validate the preconditions of the given argument.
            /// </summary>
            /// <typeparam name="T">The type of the argument to validate.</typeparam>
            /// <param name="value">The value of the argument to validate.</param>
            /// <param name="argumentName">The name of the argument to validate</param>
            /// <returns>A new <see cref="ContractValidator{T}">ConditionValidator</see> containing the 
            /// <paramref name="value"/> and "value" as argument name.</returns>
            public override ContractValidator<T> Requires<T>(T value, string argumentName)
            {
                return new RequiresWithCustomExceptionValidator<T, TException>(argumentName, value);
            }
        }
    }
}