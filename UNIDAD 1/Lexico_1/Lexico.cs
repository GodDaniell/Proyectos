using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Lexico_1
{
    public class Lexico : Token, IDisposable
    {
        StreamReader archivo;
        StreamWriter log;
        StreamWriter asm;
        int linea;

        public Lexico()
        {
            linea = 0;
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
                Console.Write("Se esta ejecutando en: " + nombreArchivo + "\n");
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
            log.WriteLine($"Hay {linea} líneas en el archivo prueba.cpp");

            archivo.Close();
            log.Close();
            asm.Close();
        }

        public void NextToken()
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

            else if (c == '$')
            {
                setClasificacion(Tipos.Caracter);
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
                else if ((c = (char)archivo.Peek()) == '>')
                {
                    setClasificacion(Tipos.Puntero);
                    buffer += c;
                    archivo.Read();
                }
            }

            else if (c == '<')
            {
                setClasificacion(Tipos.OperadorRelacional);
                if ((c = (char)archivo.Peek()) == '=' || c == '>')
                {
                    buffer += c;
                    archivo.Read();
                }
            }

            else if (c == '>')
            {
                setClasificacion(Tipos.OperadorRelacional);
                if ((c = (char)archivo.Peek()) == '=')
                {
                    buffer += c;
                    archivo.Read();
                }
            }

            else if (c == '!')
            {
                setClasificacion(Tipos.OperadorLogico);
                if ((c = (char)archivo.Peek()) == '=')
                {
                    setClasificacion(Tipos.OperadorRelacional);
                    buffer += c;
                    archivo.Read();
                }
            }

            else if (c == '&')
            {
                setClasificacion(Tipos.Caracter);
                if ((c = (char)archivo.Peek()) == '&')
                {
                    setClasificacion(Tipos.OperadorLogico);
                    buffer += c;
                    archivo.Read();
                }
            }

            else if (c == '|')
            {
                setClasificacion(Tipos.Caracter);
                if ((c = (char)archivo.Peek()) == '|')
                {
                    setClasificacion(Tipos.OperadorLogico);
                    buffer += c;
                    archivo.Read();
                }
            }

            else if (c == '*' || c == '/' || c == '%')
            {
                setClasificacion(Tipos.OperadorFactor);
                if ((c = (char)archivo.Peek()) == '=')
                {
                    setClasificacion(Tipos.IncrementoFactor);
                    buffer += c;
                    archivo.Read();
                }
            }

            else if (c == '"')
            {
                setClasificacion(Tipos.Cadena);

                while ((c = (char)archivo.Read()) != '"')
                {
                    buffer += c;
                    if (finArchivo())
                    {
                        throw new Error("en Lexico: La cadena no se cerró con comillas ", log, linea);
                    }
                }
                buffer += c;
            }

            else if (c == '#')
            {
                setClasificacion(Tipos.Caracter);

                while (char.IsDigit((char)archivo.Peek()))
                {
                    c = (char)archivo.Read();
                    buffer += c;
                }
            }

            else if (c == '@')
            {
                setClasificacion(Tipos.Caracter);

            }
            else if (c == '\'')
            {
                setClasificacion(Tipos.Caracter);


                c = (char)archivo.Read();

                if (c == '\'')
                {
                    if ((char)archivo.Peek() == '\'')  // Si hay una tercera comilla, es un carácter comilla
                    {
                        c = (char)archivo.Read();  // Leer la tercera comilla
                        buffer += c;
                    }
                    else
                    {
                        throw new Error("en Léxico: Las comillas simples están vacías", log, linea);
                    }
                    buffer += c;
                }
                else
                {
                    buffer += c;

                    if ((char)archivo.Peek() == '\'')
                    {
                        c = (char)archivo.Read();
                        buffer += c;
                    }
                    else
                    {
                        throw new Error("en Léxico: No se cerró con comillas simples", log, linea);
                    }
                }
            }



            else if (char.IsDigit(c))
            {
                setClasificacion(Tipos.Numero);

                while (char.IsDigit((char)archivo.Peek()))
                {
                    c = (char)archivo.Read();
                    buffer += c;
                }

                // Verificamos si el siguiente carácter es un punto decimal
                if ((char)archivo.Peek() == '.')
                {
                    c = (char)archivo.Read();
                    buffer += c;

                    if (!char.IsDigit((char)archivo.Peek()))
                    {
                        throw new Error("en Léxico: Se esperaba un dígito después del punto decimal", log, linea);
                    }

                    while (char.IsDigit((char)archivo.Peek()))
                    {
                        c = (char)archivo.Read();
                        buffer += c;
                    }
                }

                // Verificamos si el siguiente carácter es 'e' o 'E' para notación científica
                if (char.ToLower((char)archivo.Peek()) == 'e')
                {
                    c = (char)archivo.Read();
                    buffer += c;

                    c = (char)archivo.Peek();
                    if (c == '+' || c == '-')
                    {
                        c = (char)archivo.Read();
                        buffer += c;
                    }

                    if (!char.IsDigit((char)archivo.Peek()))
                    {
                        throw new Error("en Léxico: Se esperaba un dígito después de 'e' ", log, linea);
                    }

                    while (char.IsDigit((char)archivo.Peek()))
                    {
                        c = (char)archivo.Read();
                        buffer += c;
                    }
                }
            }

            else
            {
                setClasificacion(Tipos.Caracter);
            }
            setContenido(buffer);
            log.WriteLine(getContenido() + " ---> " + getClasificacion());
        }

        public bool finArchivo()
        {
            return archivo.EndOfStream;
        }
    }
}


