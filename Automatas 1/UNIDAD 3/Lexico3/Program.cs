using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexico3
{
    class Program
    {
        static void Main(string[] args)
        {

            using (Lexico l = new Lexico("prueba.cpp"))
            {

                try
                {
                    bool usarMatrizInterna = false; // Cambiar a false para utilizar la matriz externa
                    while (!l.finArchivo())
                    {
                        l.nexToken(usarMatrizInterna);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }
        }
    }
}

