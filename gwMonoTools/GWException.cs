using System;
using System.Collections.Generic;
using System.Text;

namespace org.geekwisdom
{
    class GWException: Exception
    {
        private long errorCode;
        public GWException()
        {
            errorCode =-1;
        }

        public GWException(string message,int ErrorCode)
            : base(message)
        {
            errorCode =ErrorCode;
        }

        public long getErrorCode()
        {
            return errorCode;
        }
  

        
    }
}
