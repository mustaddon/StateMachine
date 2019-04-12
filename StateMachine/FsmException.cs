using System;
using System.Collections.Generic;
using System.Text;

namespace RandomSolutions
{
    public class FsmException : Exception
    {
        public FsmException() : this(null, null) { }

        public FsmException(string message) : this(message, null) { }

        public FsmException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
