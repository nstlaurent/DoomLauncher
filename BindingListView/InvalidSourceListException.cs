using System;
using System.Collections.Generic;
using System.Text;

namespace Equin.ApplicationFramework
{
    [Serializable]
    public class InvalidSourceListException : Exception
    {
        public InvalidSourceListException()
            : base(Properties.Resources.InvalidSourceList)
        {
            
        }

        public InvalidSourceListException(string message)
            : base(message)
        {

        }

        public InvalidSourceListException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {

        }
    }
}
