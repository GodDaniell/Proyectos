using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections;
using ClosedXML.Excel;

namespace Lexico3
{
    public class Lexico : Token, IDisposable
    {
        StreamReader archivo;
        StreamWriter log;
        StreamWriter asm;
        int linea;
        const int F = -1;
        const int E = -2;

        int[,] TRAND =
        {
            {  0,  1,  2, 33,  1, 12, 14,  8,  9, 10, 11, 23, 16, 16, 18, 20, 21, 26, 25, 27, 29, 32, 34,  0,  F, 33 },
            {  F,  1,  1,  F,  1,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F },
            {  F,  F,  2,  3,  5,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F },
            {  E,  E,  4,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E },
            {  F,  F,  4,  F,  5,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F },
            {  E,  E,  7,  E,  E,  6,  6,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E },
            {  E,  E,  7,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E },
            {  F,  F,  7,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F },
            {  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F },
            {  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F },
            {  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F },
            {  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F },
            {  F,  F,  F,  F,  F, 13,  F,  F,  F,  F,  F, 13,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F },
            {  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F },
            {  F,  F,  F,  F,  F,  F, 13,  F,  F,  F,  F, 13,  F,  F,  F,  F,  F,  F, 15,  F,  F,  F,  F,  F,  F,  F },
            {  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F },
            {  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, 17,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F },
            {  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F },
            {  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, 19,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F },
            {  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F },
            {  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, 19,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F },
            {  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, 22,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F },
            {  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F },
            {  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, 24,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F },
            {  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F },
            {  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, 24,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F },
            {  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, 24,  F,  F,  F,  F,  F,  F, 24,  F,  F,  F,  F,  F,  F,  F },
            { 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 28, 27, 27, 27, 27,  E, 27 },
            {  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F },
            { 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 },
            {  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E, 31,  E,  E,  E,  E,  E },
            {  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F },
            {  F,  F, 32,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F },
            {  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F },
            {  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, 17, 36,  F,  F,  F,  F,  F,  F,  F,  F,  F, 35,  F,  F,  F },
            { 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35,  0, 35, 35 },
            { 36, 36, 36, 36, 36, 36, 36, 36, 36, 36, 36, 36, 37, 36, 36, 36, 36, 36, 36, 36, 36, 36, 36, 36, 36, 36 },
            { 36, 36, 36, 36, 36, 36, 36, 36, 36, 36, 36, 36, 37, 36, 36, 36, 36, 36, 36, 36, 36, 36,  0, 36, 36, 36 },
        };

         private int[,] matrizExcel = new int[37, 26];
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

        private void CargarMatrizDesdeExcel()
{
    matrizExcel = new int[37, 26]; // Ajusta el tamaño según las dimensiones reales.

    using (var workbook = new XLWorkbook(@"C:\Users\danie\OneDrive\Escritorio\Lenguajes y Autómatas I\Proyectos\UNIDAD 3\TRAND2.xlsx"))
    {
        var worksheet = workbook.Worksheet(2);
        for (int i = 0; i < 37; i++)
        {
            for (int j = 0; j < 26; j++)
            {
                var cellValue = worksheet.Cell(i + 1, j + 1).GetValue<string>();

                // Manejar celdas vacías
                if (string.IsNullOrWhiteSpace(cellValue))
                {
                    matrizExcel[i, j] = -1; // Valor predeterminado para celdas vacías
                }
                else if (cellValue == "F")
                {
                    matrizExcel[i, j] = -1; // "F" se convierte a -1
                }
                else if (cellValue == "E")
                {
                    matrizExcel[i, j] = -2; // "E" se convierte a -2
                }
                else if (int.TryParse(cellValue, out int result))
                {
                    matrizExcel[i, j] = result; // Si es un número, lo convierte a entero
                }
                else
                {
                    throw new Exception($"Error: La celda ({i + 1}, {j + 1}) contiene un valor no entero: '{cellValue}'");
                }
            }
        }
    }
}


        private int Columna(char c)
        {
            if (c == '\n')
            {
                return 23;
            }
            else if (finArchivo())
            {
                return 24;
            }
            else if (char.IsWhiteSpace(c))
            {
                return 0;
            }
            else if (char.ToLower(c) == 'e')
            {
                return 4;
            }
            else if (char.IsLetter(c))
            {
                return 1;
            }
            else if (char.IsDigit(c))
            {
                return 2;
            }
            else if (c == '.')
            {
                return 3;
            }
            else if (c == '+')
            {
                return 5;
            }
            else if (c == '-')
            {
                return 6;
            }
            else if (c == ';')
            {
                return 7;
            }
            else if (c == '{')
            {
                return 8;
            }
            else if (c == '}')
            {
                return 9;
            }
            else if (c == '?')
            {
                return 10;
            }
            else if (c == '=')
            {
                return 11;
            }
            else if (c == '*')
            {
                return 12;
            }
            else if (c == '%')
            {
                return 13;
            }
            else if (c == '&')
            {
                return 14;
            }
            else if (c == '|')
            {
                return 15;
            }
            else if (c == '!')
            {
                return 16;
            }
            else if (c == '<')
            {
                return 17;
            }
            else if (c == '>')
            {
                return 18;
            }
            else if (c == '"')
            {
                return 19;
            }
            else if (c == '\'')
            {
                return 20;
            }
            else if (c == '#')
            {
                return 21;
            }
            else if (c == '/')
            {
                return 22;
            }
            else if (c == '.')
            {
                return 3;
            }
            return 25;
        }

        private void Clasificar(int estado)
        {
            switch (estado)
            {
                case 1:
                    setClasificacion(Tipos.Identificador);
                    break;

                case 2:
                    setClasificacion(Tipos.Numero);
                    break;

                case 8:
                    setClasificacion(Tipos.FinBloque);
                    break;

                case 9:
                    setClasificacion(Tipos.InicioBloque);
                    break;

                case 10:
                    setClasificacion(Tipos.FinBloque);
                    break;

                case 11:
                    setClasificacion(Tipos.OperadorTernario);
                    break;

                case 12:
                case 14:
                    setClasificacion(Tipos.OperadorTermino);
                    break;

                case 13:
                    setClasificacion(Tipos.IncrementoTermino);
                    break;

                case 15:
                    setClasificacion(Tipos.Puntero);
                    break;

                case 16:
                    setClasificacion(Tipos.OperadorFactor);
                    break;

                case 17:
                    setClasificacion(Tipos.IncrementoFactor);
                    break;

                case 18:
                case 20:
                case 29:
                case 32:
                case 33:
                    setClasificacion(Tipos.Caracter);
                    break;

                case 19:
                case 21:
                    setClasificacion(Tipos.OperadorLogico);
                    break;

                case 22:
                case 24:
                case 25:
                case 26:
                    setClasificacion(Tipos.OperadorRelacional);
                    break;

                case 23:
                    setClasificacion(Tipos.Asignacion);
                    break;

                case 27:
                    setClasificacion(Tipos.Cadena);
                    break;
            }
        }

        public void NextToken(bool usarMatrizInterna)
        {
             if (!usarMatrizInterna)
            {
                //Console.WriteLine("Leyendo desde excel");
                CargarMatrizDesdeExcel(); // *Código nuevo*: Llamada a la carga desde Excel si es necesario
            }

            char c;
            string buffer = "";
            int estado = 0;

            while (estado >= 0)
            {
                if (estado == 0)
                {
                    buffer = "";
                }
                c = (char)archivo.Peek();
                // Console.Write(c);
                estado = TRAND[estado, Columna(c)];
                Clasificar(estado);



                if (estado >= 0)
                {
                    archivo.Read();
                    if (c == '\n')
                    {
                        linea++;
                    }
                    if (estado > 0)
                    {
                        buffer += c;
                    }
                }
            }

            if (estado == E)
            {
                if (getClasificacion() == Tipos.Numero)
                {
                    throw new Error("Lexico, se espera un digito ", log, linea);
                }
                else if (getClasificacion() == Tipos.Caracter)
                {
                    throw new Error("Léxico, por caracter inválido  ", log, linea);
                }
                else if (getClasificacion() == Tipos.Cadena)
                {
                    throw new Error("Léxico, se esperaba que cerrara la cadena", log, linea);
                }
                else
                {
                    throw new Error("Comentario no cerrado", log, linea);
                }
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

/*
Expresion Regular; Metodo formal que a traves de una secuencia de 
carcateres que define un PATRON de busqueda

a) Reglas BNF
b) Reglas BNF extendidas
c) Operaciones aplicadas al lenguaje

OAL

2. Concatenacion exponensial (exponente)
1. Concatenacion simple (·)
3. Cerradura Kleene (*)
4. Cerradura Positiva (+)
5. Cerradura Epsilon (?)
6. Operador OR (|)
7. Parentesis ()

L = {A, B, C, D, E, ...Z, a, b, c, d, ...z}
D = {1, 2, 3, 4, 5, 6, 7, 8, 9}

1. L.D
   LD
   >=


2. L^3   =  LLL
   L3D2  =  LLLDD
   D^5   =  DDDDD
   =2    =  ==
   

3. L* = Cero o mas letras
   D* = Cero o mas digitos
   

4.  L+ = Una o mas letras
    D+ = Uno o mas digitos


5.  L? = Cero o una letra (la letra es optativa-opcional)

6.  L | D  = Una letra o un digito
    + | -  = + o menos

7.  (L D) L? (letra seguido de digito y al final letra opcional) 


Produccion Gramatical

Clasificacion del Token -> Expresion Regular
 Identificador -> L (L | D)*
Numero ------------->
FinSentencia ------->
InicioBloque -------> 
FinBloque ---------->
OperadorTernario --->
OperadorTermino ---->
OperadorFactor ----->
OperadorRelacional -> 
OperadorLogico ----->
IncrementoTermino -->
IncrementoFactor ---> 
Moneda ------------->
Asignacion ---------> 
Puntero ------------>
Cadena ------------->
Caracter ----------->


Automata: Modelo matematico que representa una expresion regular a traves de un 
GRAFO, para una maquina de estado finito que consiste en un conjunto de estados 
bien definidos, un estado inicial, un alfabeto de entrada y una funcion de c 
*/
