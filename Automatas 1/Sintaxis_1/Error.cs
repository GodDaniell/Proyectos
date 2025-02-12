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
        public Error(string message, StreamWriter log) : base("Error " + message)
        {
            log.WriteLine("Error " + message );
        }

        public Error(string message, StreamWriter log, int linea, int columna) : base("Error " + message + " en la linea " + linea + " y columna " + columna)
        {
            log.WriteLine("Error " + message + " en la linea " + linea + " y columna " + columna);
        }
    }
}