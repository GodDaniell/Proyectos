using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Sintaxis_1
{
    public class Error : Exception
    {
        public Error(string message) : base("Error " + message) { }
        public Error(string message, StreamWriter logger) : base("Error " + message)
        {
            logger.WriteLine("Error " + message);
        }

        public Error(string message, StreamWriter logger, int line) : base("Error: " + message + " on line " + line)
        {
            logger.WriteLine("Error: " + message + " on line " + line);
        }
    }
}