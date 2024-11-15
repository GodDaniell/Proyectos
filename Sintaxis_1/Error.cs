using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Sintaxis_1
{
    public class Error : Exception
    {
        public Error(string message, int linea) : base("Error " + message) { }
        public Error(string message, StreamWriter logger) : base("Error " + message)
        {
            logger.WriteLine("Error " + message);
        }

        public Error(string message, StreamWriter logger, int linea) : base("Error " + message + " en la linea " + linea)
        {
            logger.WriteLine("Error " + message + " en la linea " + linea);
        }
    }
}