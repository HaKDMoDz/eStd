using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Creek.Contracts
{
    public static class ContractsExtensions
    {
        [DebuggerStepThrough]
        public static ContractValidator<T> ExistsIn<T>(
            this ContractValidator<T> validator, IEnumerable<T> collection)
        {
            // Value, ArgumentName and ThrowException won't show up in the 
            // IntelliSense dropdown list, but you can use them.
            if (collection == null || !collection.Contains(validator.Value))
            {
                validator.ThrowException(validator.ArgumentName +
                                         " should be in the supplied collection");
            }

            return validator;
        }
    }
}