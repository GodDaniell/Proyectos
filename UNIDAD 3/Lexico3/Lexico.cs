using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections;

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
