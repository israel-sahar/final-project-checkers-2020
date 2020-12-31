using System;
using System.Runtime.Serialization;
using System.Windows;

namespace Client
{
    [Serializable]
    internal class SquareAlreadyTakenException : Exception
    {
        private Point to;

        public SquareAlreadyTakenException()
        {
        }

        public SquareAlreadyTakenException(string message) : base(message)
        {
        }

        public SquareAlreadyTakenException(Point to)
        {
            this.to = to;
        }

        public SquareAlreadyTakenException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SquareAlreadyTakenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}