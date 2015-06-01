using System;

#if PORTABLE
#else
using System.Runtime.Serialization;
#endif

namespace System.Serialisation.Core
{
    /// <summary>
    ///   Occurs if no instance of a type can be created. Maybe the type lacks on a public standard (parameterless) constructor?
    /// </summary>
#if PORTABLE
#elif SILVERLIGHT
#else
    [Serializable]
#endif
    public class CreatingInstanceException : Exception
    {
        ///<summary>
        ///</summary>
        public CreatingInstanceException()
        {
        }

        ///<summary>
        ///</summary>
        ///<param name = "message"></param>
        public CreatingInstanceException(string message) : base(message)
        {
        }

        ///<summary>
        ///</summary>
        ///<param name = "message"></param>
        ///<param name = "innerException"></param>
        public CreatingInstanceException(string message, Exception innerException) : base(message, innerException)
        {
        }


#if PORTABLE
#elif SILVERLIGHT
#else
        /// <summary>
        /// </summary>
        /// <param name = "info"></param>
        /// <param name = "context"></param>
        protected CreatingInstanceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif

    }
}