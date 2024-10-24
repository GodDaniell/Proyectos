using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prueba
{
    class Program
    {
        static void Main(string[] args) 
        {
            using (Lecturas L = new Lecturas("prueba.cpp"))
            {
                Console.WriteLine("Los numeros de letras es: " + L.contarLetras());
                Console.WriteLine("Los numeros de digitos es: " + L.contarDigitos());
                Console.WriteLine("Los espacios en el archivo son: " + L.contarEspacios());
                
            }
        }
    }
}
