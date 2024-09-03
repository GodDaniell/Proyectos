using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Lexico_1
{
    public class Lexico : Token, IDisposable
    {
        StreamReader archivo;
        StreamWriter log;
        StreamWriter asm;
        public Lexico() 
        {
            log     = new StreamWriter("prueba.log");
            asm     = new StreamWriter("prueba.asm");
            if (File.Exists("prueba.cpp"))
            {
                archivo = new StreamReader("prueba.cpp");
            }
            else
            {
                throw new Error("El archivo prueba.cpp no existe",log);
            }
        }
        public void Dispose()
        {
            archivo.Close();
            log.Close();
            asm.Close();
        }
    }
}