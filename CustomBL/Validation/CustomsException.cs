using System;
using System.Runtime.Serialization;

namespace Custom.BL.Validation
{
    [Serializable]
    public class CustomsException : Exception
    {
        public CustomsException() { }

        public CustomsException(string message) : base(message) { }

        public CustomsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}