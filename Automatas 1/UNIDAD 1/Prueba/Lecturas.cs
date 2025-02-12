using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Prueba
{
    public class Lecturas : IDisposable
    {
        StreamReader archivo;
        StreamWriter log;
        public Lecturas()
        {
            archivo = new StreamReader("prueba.cpp");
            log = new StreamWriter("prueba.log");
        }
        public Lecturas(string nombre)
        {
            archivo = new StreamReader(nombre);
            log = new StreamWriter("prueba.log");
        }

        public void Dispose()
        {

            archivo.Close();
            log.Close();
        }
        public void Copy()
        {
            while (!archivo.EndOfStream)
            {
                log.Write((char)archivo.Read());
            }
        }
        public void Encrypt()
        {
            char c;
            while (!archivo.EndOfStream)
            {
                c = (char)archivo.Read();
                if (char.IsLetter(c))
                {
                    log.Write((char)(c));
                }
                else
                {
                    log.Write(c);
                }

            }
        }

        public int contarLetras()
        {
            char c;
            int Cont = 0;
            while (!archivo.EndOfStream)
            {
                c = (char)archivo.Read();
                if (char.IsLetter(c))
                {
                    Cont++;
                }

            }
            return Cont;
        }

        public int contarDigitos()
        {
            char c;
            int contD = 0;
            while (!archivo.EndOfStream)
            {
                c = (char)archivo.Read();
                if (char.IsDigit(c))
                {
                    contD++;
                }

            }
            return contD;
        }

        public int contarEspacios()
        {
            char c;
            int contE = 0;
            while (!archivo.EndOfStream)
            {
                c = (char)archivo.Read();
                if (char.IsWhiteSpace(c))
                {
                    contE++;
                }

            }
            return contE;
        }

        public dynamic palabra()
        {
            dynamic d = "Empty";
            string buffer = "";
            char c;
            while (char.IsWhiteSpace(c = (char)archivo.Read()) && !archivo.EndOfStream)
            {

            }

            if (char.IsLetter(c))
            {
                buffer += c;
                while (char.IsLetter(c = (char)archivo.Read()) && !archivo.EndOfStream)
                {
                    buffer += c;
                }
            }
            else if (char.IsDigit(c))
            {
                buffer += c;
                while (char.IsDigit(c = (char)archivo.Read()) && !archivo.EndOfStream)
                {
                    buffer += c;
                }
            }
            log.WriteLine(buffer + " es");
            return buffer.ToString();
        }

        public bool finArchivo()
        {
            return archivo.EndOfStream;
        }


    }
}