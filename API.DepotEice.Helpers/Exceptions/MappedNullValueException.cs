using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace API.DepotEice.Helpers.Exceptions
{
    public class MappedNullValueException : Exception
    {
        public MappedNullValueException()
        {
        }

        public MappedNullValueException(string message) : base(message)
        {
        }

        public MappedNullValueException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MappedNullValueException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
