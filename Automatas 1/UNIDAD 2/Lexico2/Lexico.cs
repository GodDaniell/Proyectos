using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections;

namespace Lexico2
{
    public class Lexico : Token, IDisposable
    {
        StreamReader archivo;
        StreamWriter log;
        StreamWriter asm;
        int linea;
        const int F = -1;
        const int E = -2;


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

        private int automata(char c, int estado)
        {
            int nuevoEstado = estado;

            switch (estado)
            {
                // case 0: Estado inicial
                case 0:
                    if (char.IsWhiteSpace(c))
                    {
                        nuevoEstado = 0; // Ignora espacios en blanco
                    }
                    else if (char.IsLetter(c))
                    {
                        nuevoEstado = 1; // Letra inicial -> Identificador
                    }
                    else if (char.IsDigit(c))
                    {
                        nuevoEstado = 2; // Dígito inicial -> Número
                    }
                    else if (c == ';')
                    {
                        nuevoEstado = 8; // Fin de sentencia
                    }
                    else if (c == '{')
                    {
                        nuevoEstado = 9; // Inicio de bloque
                    }
                    else if (c == '}')
                    {
                        nuevoEstado = 10; // Fin de bloque
                    }
                    else if (c == '?')
                    {
                        nuevoEstado = 11; // Operador ternario
                    }
                    else if (c == '+')
                    {
                        nuevoEstado = 12;
                    }
                    else if (c == '-')
                    {
                        nuevoEstado = 14;
                    }
                    else if (c == '*' || c == '%')
                    {
                        nuevoEstado = 16;
                    }
                    else if (c == '&')
                    {
                        nuevoEstado = 18;
                    }
                    else if (c == '|')
                    {
                        nuevoEstado = 20;
                    }
                    else if (c == '!')
                    {
                        nuevoEstado = 21;
                    }
                    else if (c == '=')
                    {
                        nuevoEstado = 23;
                    }
                    else if (c == '>')
                    {
                        nuevoEstado = 25;
                    }
                    else if (c == '<')
                    {
                        nuevoEstado = 26;
                    }
                    else if (c == '"')
                    {
                        nuevoEstado = 27;
                    }
                    else if (c == '\'')
                    {
                        nuevoEstado = 29;
                    }
                    else if (c == '#')
                    {
                        nuevoEstado = 32;
                    }
                    else if (c == '/')
                    {
                        nuevoEstado = 34;
                    }
                    else
                    {
                        nuevoEstado = 33;
                    }
                    break;

                case 1:
                    setClasificacion(Tipos.Identificador);
                    if (char.IsLetterOrDigit(c))
                    {
                        nuevoEstado = 1; // Continua si es letra o dígito
                    }
                    else
                    {
                        nuevoEstado = F; // Otro carácter, finaliza identificador
                    }
                    break;

                case 2:
                    setClasificacion(Tipos.Numero);
                    if (char.IsDigit(c))
                    {
                        nuevoEstado = 2; // Continúa con más dígitos
                    }
                    else if (c == '.')
                    {
                        nuevoEstado = 3; // Punto decimal -> Número decimal
                    }
                    else if (char.ToLower(c) == 'e')
                    {
                        nuevoEstado = 5; // Notación científica
                    }
                    else
                    {
                        nuevoEstado = F; // Fin de número
                    }
                    break;

                case 3:
                    if (char.IsDigit(c))
                    {
                        nuevoEstado = 4; // Continua en parte decimal
                    }
                    else
                    {
                        nuevoEstado = E; // Error si no es dígito
                    }
                    break;

                case 4:
                    if (char.IsDigit(c))
                    {
                        nuevoEstado = 4; // Continúa con más dígitos decimales
                    }
                    else if (char.ToLower(c) == 'e')
                    {
                        nuevoEstado = 5; // Notación científica
                    }
                    else
                    {
                        nuevoEstado = F; // Fin de número decimal
                    }
                    break;

                case 5:
                    if (c == '+' || c == '-')
                    {
                        nuevoEstado = 6; // Verificar signo en notación científica
                    }
                    else if (char.IsDigit(c))
                    {
                        nuevoEstado = 7; // Dígito en notación científica
                    }
                    else
                    {
                        nuevoEstado = E; // Error
                    }
                    break;

                case 6:
                    if (char.IsDigit(c))
                    {
                        nuevoEstado = 7; // Continúa con dígitos en notación científica
                    }
                    else
                    {
                        nuevoEstado = E; // Error si no es dígito
                    }
                    break;

                case 7:
                    if (char.IsDigit(c))
                    {
                        nuevoEstado = 7; // Continúa con más dígitos en notación científica
                    }
                    else
                    {
                        nuevoEstado = F; // Fin de número en notación científica
                    }
                    break;

                case 8:
                    setClasificacion(Tipos.FinSentencia);
                    nuevoEstado = F; // Fin de análisis para el carácter especial ';'
                    break;

                case 9:
                    setClasificacion(Tipos.InicioBloque);
                    nuevoEstado = F; // Termina la clasificación en el estado final
                    break;

                case 10:
                    {
                        setClasificacion(Tipos.FinBloque);
                        nuevoEstado = F; // Termina la clasificación en el estado final
                    }
                    break;

                case 11:
                    {
                        setClasificacion(Tipos.OperadorTernario);
                        nuevoEstado = F; // Termina la clasificación en el estado final
                    }
                    break;

                case 12:
                    if (c == '+' || c == '=')
                    {
                        setClasificacion(Tipos.OperadorTermino);
                        nuevoEstado = 13;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;

                case 13:
                    setClasificacion(Tipos.IncrementoTermino);
                    nuevoEstado = F; // Fin de análisis
                    break;

                case 14:
                    setClasificacion(Tipos.OperadorTermino);
                    if (c == '-' || c == '=')
                    {
                        nuevoEstado = 13;
                    }
                    else if (c == '>')
                    {
                        nuevoEstado = 15; // Operador de puntero '->'
                    }
                    else
                    {
                        nuevoEstado = F; // Fin de análisis
                    }
                    break;

                case 15:
                    setClasificacion(Tipos.Puntero);
                    nuevoEstado = F; // Fin de análisis
                    break;

                case 16:
                    if (c == '=')
                    {
                        setClasificacion(Tipos.OperadorFactor);
                        nuevoEstado = 17;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;

                case 17:
                    setClasificacion(Tipos.IncrementoFactor);
                    nuevoEstado = F;
                    break;

                case 18:
                    setClasificacion(Tipos.Caracter);
                    if (c == '&')
                    {
                        nuevoEstado = 19;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;

                case 19:
                    setClasificacion(Tipos.OperadorLogico);
                    nuevoEstado = F;
                    break;

                case 20:
                    setClasificacion(Tipos.Caracter);
                    if (c == '|')
                    {
                        nuevoEstado = 19;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;

                case 21:
                    setClasificacion(Tipos.OperadorLogico);
                    if (c == '=')
                    {
                        nuevoEstado = 22;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;

                case 22:
                    setClasificacion(Tipos.OperadorRelacional);
                    nuevoEstado = F;
                    break;

                case 23:
                    setClasificacion(Tipos.Asignacion);
                    if (c == '=')
                    {
                        nuevoEstado = 24;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;

                case 24:
                    setClasificacion(Tipos.OperadorRelacional);
                    nuevoEstado = F;
                    break;

                case 25:
                    setClasificacion(Tipos.OperadorRelacional);
                    if (c == '=')
                    {
                        nuevoEstado = 24;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;

                case 26:
                    setClasificacion(Tipos.OperadorRelacional);
                    if (c == '>' | c == '=')
                    {
                        nuevoEstado = 24;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;

                case 27:
                    setClasificacion(Tipos.Cadena);
                    if (c == '"')
                    {
                        nuevoEstado = 28;
                    }
                    else if (finArchivo())
                    {
                        nuevoEstado = E;
                    }
                    else
                    {
                        nuevoEstado = 27;
                    }
                    break;

                case 28:
                    nuevoEstado = F;
                    break;

                case 29:
                    setClasificacion(Tipos.Caracter);
                    nuevoEstado = 30;
                    break;

                case 30:
                    if (c == '\'')
                    {
                        nuevoEstado = 31;
                    }
                    else
                    {
                        nuevoEstado = E;
                    }
                    break;

                case 31:
                    nuevoEstado = F;
                    break;

                case 32:
                    setClasificacion(Tipos.Caracter);
                    if (char.IsDigit(c))
                    {
                        nuevoEstado = 32;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;

                case 33:
                    setClasificacion(Tipos.Caracter);
                    nuevoEstado = F;
                    break;

                case 34:
                    setClasificacion(Tipos.OperadorFactor);
                    if (c == '=')
                    {
                        nuevoEstado = 17;
                    }
                    else if (c == '*')
                    {
                        nuevoEstado = 36;
                    }
                    else if (c == '/')
                    {
                        nuevoEstado = 35;
                    }
                    else
                    {
                        nuevoEstado = F;
                    }
                    break;

                case 35:
                    if (c == '\n')
                    {
                        nuevoEstado = 0;
                    }
                    else
                    {
                        nuevoEstado = 35;
                    }
                    break;

                case 36:
                    nuevoEstado = 36;
                    if (c == '*')
                    {
                        nuevoEstado = 37;
                    }
                    else if (finArchivo())
                    {
                        nuevoEstado = E;
                    }
                    else
                    {
                        nuevoEstado = 36;
                    }
                    break;

                case 37:
                    if (c == '/')
                    {
                        nuevoEstado = 0;
                    }
                    else if (c == '*')
                    {
                        nuevoEstado = 37;
                    }
                    else if (finArchivo())
                    {
                        nuevoEstado = E;
                    }
                    else
                    {
                        nuevoEstado = 36;
                    }
                    break;

            }

            return nuevoEstado;
        }

        public void NextToken()
        {
            char transicion;
            string buffer = "";
            int estado = 0;

            while (estado >= 0)
            {
                if (estado == 0)
                {
                    buffer = "";
                }
                transicion = (char)archivo.Peek();
                // Console.Write(transicion);
                estado = automata(transicion, estado);



                if (estado >= 0)
                {
                    archivo.Read();
                    if (transicion == '\n')
                    {
                        linea++;
                    }
                    if (estado > 0)
                    {
                        buffer += transicion;
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
bien definidos, un estado inicial, un alfabeto de entrada y una funcion de transicion 
*/
