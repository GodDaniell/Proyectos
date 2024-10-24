using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexico2
{
    class Program
    {
        static void Main(string[] args)
        {

            using (Lexico l = new Lexico("prueba.cpp"))
            {

                try
                {
                    while (!l.finArchivo())
                    {
                        l.NextToken();
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

