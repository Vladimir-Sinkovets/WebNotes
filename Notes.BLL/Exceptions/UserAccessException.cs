using System;
using System.Runtime.Serialization;

namespace Notes.BLL.Exceptions
{
    [Serializable]
    public class UserAccessException : Exception
    {
        public UserAccessException()
        {
        }

        public UserAccessException(string message) : base(message)
        {
        }

        public UserAccessException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}