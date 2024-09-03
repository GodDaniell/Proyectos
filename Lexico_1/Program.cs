using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexico_1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (Lexico T = new Lexico())
                {
                    T.SetContenido("HOLA");
                    T.SetClasificacion(Token.Tipos.Identificador);

                    Console.WriteLine(T.GetContenido() + " = " + T.GetClasificacion());

                    T.SetContenido("123");
                    T.SetClasificacion(Token.Tipos.Numero);

                    Console.WriteLine(T.GetContenido() + " = " + T.GetClasificacion());
                }

            }
                catch (Exception e)
                {
                Console.WriteLine("Error: " + e.Message);
                }
            

        }
    }
}


