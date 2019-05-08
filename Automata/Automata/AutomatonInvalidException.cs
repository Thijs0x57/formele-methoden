using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class AutomatonInvalidException : Exception
    {
        public AutomatonInvalidException()
        {
        }

        public AutomatonInvalidException(string message) : base(message)
        {
        }

        public AutomatonInvalidException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AutomatonInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
