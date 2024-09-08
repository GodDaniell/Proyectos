using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

/*
    Requerimiento 1: Sobrecargar el constructor Lexico para que reciba como
                     argumento el nombre del archivo a compilar
    Requerimiento 2: Tener un contador de líneas 
*/
namespace Lexico_1
{
    public class Lexico : Token, IDisposable
    {
        StreamReader archivo;
        StreamWriter log;
        StreamWriter asm;
        int linea;

        // Constructor por defecto
        public Lexico()
        {
            linea = 1;
            log = new StreamWriter("prueba.log");
            asm = new StreamWriter("prueba.asm");
            log.AutoFlush = true;
            asm.AutoFlush = true;
            if (File.Exists("prueba.cpp"))
            {
                archivo = new StreamReader("prueba.cpp");
            }
            else
            {
                throw new Error("El archivo prueba.cpp no existe", log);
            }
        }

        public Lexico(string nombreArchivo)
        {
            linea = 1;
            string logFileName = Path.ChangeExtension(nombreArchivo, ".log");
            string asmFileName = Path.ChangeExtension(nombreArchivo, ".asm");

            log = new StreamWriter(logFileName);
            asm = new StreamWriter(asmFileName);
            log.AutoFlush = true;
            asm.AutoFlush = true;

            if (Path.GetExtension(nombreArchivo) == ".cpp")
            {
                if (File.Exists(nombreArchivo))
                {
                    archivo = new StreamReader(nombreArchivo);
                }
                else
                {
                    throw new Error($"El archivo {nombreArchivo} no existe", log);
                }
            }
            else
            {
                throw new Error($"El archivo {nombreArchivo} no tiene la extensión .cpp", log);
            }
        }

        public void Dispose()
        {
            archivo.Close();
            log.Close();
            asm.Close();
        }

        public void nextToken()
        {
            char c;
            string buffer = "";

            while (char.IsWhiteSpace(c = (char)archivo.Read()))
            {
                if (c == '\n')
                {
                    linea++; 
                }
            }

            buffer += c;
            if (char.IsLetter(c))
            {
                setClasificacion(Tipos.Identificador);
                while (char.IsLetterOrDigit(c = (char)archivo.Peek()))
                {
                    buffer += c;
                    archivo.Read();
                }
            }
            else if (char.IsDigit(c))
            {
                setClasificacion(Tipos.Numero);
                while (char.IsDigit(c = (char)archivo.Peek()))
                {
                    buffer += c;
                    archivo.Read();
                }
                return;
            }
            else if (c == '$') 
            {
                c = (char)archivo.Peek(); 
                if (char.IsDigit(c))
                {
                    setClasificacion(Tipos.Moneda);
                    buffer += '$';
                    while (char.IsDigit(c))
                    {
                        buffer += c;
                        c = (char)archivo.Read(); 
                    }
                }
            }

            else if (c == ';')
            {
                setClasificacion(Tipos.FinSentencia);
            }
            else if (c == '{')
            {
                setClasificacion(Tipos.InicioBloque);
            }
            else if (c == '}')
            {
                setClasificacion(Tipos.FinBloque);
            }
            else if (c == '?')
            {
                setClasificacion(Tipos.OperadorTernario);
            }
            else if (c == '+')
            {
                setClasificacion(Tipos.OperadorTermino);
                if ((c = (char)archivo.Peek()) == '+' || c == '=')
                {
                    setClasificacion(Tipos.IncrementoTermino);
                    buffer += c;
                    archivo.Read();
                }
            }
            else if (c == '-')
            {
                setClasificacion(Tipos.OperadorTermino);
                if ((c = (char)archivo.Peek()) == '-' || c == '=')
                {
                    setClasificacion(Tipos.IncrementoTermino);
                    buffer += c;
                    archivo.Read();
                }
            }

            // Operadores relacionales: ==
            else if (c == '=')
            {
                setClasificacion(Tipos.Caracter);
                if ((c = (char)archivo.Peek()) == '=')
                {
                    setClasificacion(Tipos.OperadorRelacional);
                    buffer += c;
                    archivo.Read();
                }
            }

            // Operadores relacionales: <= y <>
            else if (c == '<')
            {
                setClasificacion(Tipos.Caracter);
                if ((c = (char)archivo.Peek()) == '=' || c == '>')
                {
                    setClasificacion(Tipos.OperadorRelacional);
                    buffer += c;
                    archivo.Read();
                }
            }

            // Operadores relacionales: >=
            else if (c == '>')
            {
                setClasificacion(Tipos.Caracter);
                if ((c = (char)archivo.Peek()) == '=')
                {
                    setClasificacion(Tipos.OperadorRelacional);
                    buffer += c;
                    archivo.Read();
                }
            }

            // Operador relacional: !=
            else if (c == '!')
            {
                setClasificacion(Tipos.Caracter); // Si solo es "!"
                if ((c = (char)archivo.Peek()) == '=')
                {
                    setClasificacion(Tipos.OperadorRelacional); // Si es "!="
                    buffer += c;
                    archivo.Read();
                }
            }

            // Nuevo token: Operadores lógicos "&&" y "||"
            else if (c == '&')
            {
                setClasificacion(Tipos.Caracter); // Si solo es "&"
                if ((c = (char)archivo.Peek()) == '&')
                {
                    setClasificacion(Tipos.OperadorLogico); // Si es "&&"
                    buffer += c;
                    archivo.Read();
                }
            }
            else if (c == '|')
            {
                setClasificacion(Tipos.Caracter); // Si solo es "|"
                if ((c = (char)archivo.Peek()) == '|')
                {
                    setClasificacion(Tipos.OperadorLogico); // Si es "||"
                    buffer += c;
                    archivo.Read();
                }
            }

            else if (c == '*' || c == '/' || c == '%')
            {
                setClasificacion(Tipos.OperadorFactor);
            }
            else
            {
                setClasificacion(Tipos.Caracter);
            }

            if (!finArchivo())
            {
                setContenido(buffer);
                log.WriteLine(getContenido() + " = " + getClasificacion());
            }
        }

        public bool finArchivo()
        {
            return archivo.EndOfStream;
        }
    }
}




