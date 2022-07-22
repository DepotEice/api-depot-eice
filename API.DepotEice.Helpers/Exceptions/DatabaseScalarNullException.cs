using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace API.DepotEice.Helpers.Exceptions
{
    /// <summary>
    /// Throw when a execute scalar function returned a null object
    /// </summary>
    public class DatabaseScalarNullException : Exception
    {
        public DatabaseScalarNullException()
        {
        }

        public DatabaseScalarNullException(string message)
            : base(message)
        {
        }

        public DatabaseScalarNullException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected DatabaseScalarNullException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
