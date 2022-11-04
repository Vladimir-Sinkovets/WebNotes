using System;
using System.Runtime.Serialization;

namespace Notes.BLL.Exceptions
{
    [Serializable]
    public class ExistedTagNameException : Exception
    {
        public ExistedTagNameException()
        {
        }

        public ExistedTagNameException(string message) : base(message)
        {
        }

        public ExistedTagNameException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ExistedTagNameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}