using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Lexico_1
{
    public class Error : Exception
    {
        public Error(string message, StreamWriter log ) : base(message)
        {
            log.WriteLine("Error: " + message);
        }
    }
}