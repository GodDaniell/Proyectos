using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
REQUERIMIENTOS
1.- Indicar en el error lexico o sintatico el numero de linea y caracter
listo: 2.- En el log, colocar el nombre del archivo a compilar, la fecha y la hora (funcion que lea la hora de la compu y grabarlo en el log)
3.- Agregar el resto de asignaciones:
    ID = Expresion
    ID++
    ID--
    ID IncrementoTermino Expresion
    ID IncrementoFactor Expresion
    ID = Console.Read ()
    ID = Console.ReadLine ()

4.- Emular el console.Write() y el console.WriteLine()
5.- Emular el console.Read() y el console.ReadLine
*/

namespace Sintaxis_1
{
    public class Lenguaje : Sintaxis
    {
        public Lenguaje()
            : base()
        {
            log.WriteLine("Constructor lenguaje");
        }

        public Lenguaje(string name)
            : base(name)
        {
            log.WriteLine("Constructor lenguaje");
        }

        // ? Cerradura epsilon
        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            if (getContenido() == "using")
            {
                Librerias();
            }

            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }

            Main();
        }

        //Librerias -> using ListaLibrerias; Librerias?
        private void Librerias()
        {
            match("using");
            ListaLibrerias();
            match(";");

            if (getContenido() == "using")
            {
                Librerias();
            }
        }

        //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            match(Tipos.TipoDato);
            ListaIdentificadores();
            match(";");

            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
        }

        //ListaLibrerias -> identificador (.ListaLibrerias)?
        private void ListaLibrerias()
        {
            match(Tipos.Identificador);

            if (getContenido() == ".")
            {
                match(".");
                ListaLibrerias();
            }
        }

        // ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void ListaIdentificadores()
        {
            match(Tipos.Identificador);

            if (getContenido() == ",")
            {
                match(",");
                ListaIdentificadores();
            }
        }

        // BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones()
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones();
            }
            else
            {
                match("}");
            }
        }

        // ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones()
        {
            Instruccion();
            if (getContenido() != "}")
            {
                ListaInstrucciones();
            }
            else
            {
                match("}");
            }
        }

        // Instruccion -> console | If | While | do | For | Variables | Asignación
        private void Instruccion()
        {
            if (getContenido() == "Console")
            {
                console();
                match(";");
            }
            else if (getContenido() == "if")
            {
                If();
            }
            else if (getContenido() == "while")
            {
                While();
            }
            else if (getContenido() == "do")
            {
                Do();
            }
            else if (getContenido() == "for")
            {
                For();
            }
            else if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
            else
            {
                Asignacion();
                match(";");
            }
        }

        // Asignacion -> Identificador = Expresion;
        /*
            3.- Agregar el resto de asignaciones:
            ID = Expresion
            ID++
            ID--
            ID = Console.ReadLine ()
            ID IncrementoTermino Expresion
            ID IncrementoFactor Expresion
            ID = Console.Read ()
        */
        private void Asignacion()
        {
            match(Tipos.Identificador);

            if (getContenido() == "=")
            {
                match("=");
                if (getContenido() == "Console")
                {
                    console();
                }
                else
                {
                    Expresion();
                }
            }
            else if (getContenido() == "++" || getContenido() == "--")
            {
                match(Tipos.IncrementoTermino);
            }
            else
            {
                if (getClasificacion() == Tipos.IncrementoTermino)
                {
                    match(Tipos.IncrementoTermino);
                }
                else
                {
                    match(Tipos.IncrementoFactor);
                }
                Expresion();
            }

        }

        // If -> if (Condicion) bloqueInstrucciones | instruccion
        // (else bloqueInstrucciones | instruccion)?
        private void If()
        {
            match("if");
            match("(");
            Condicion();
            match(")");

            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }

            if (getContenido() == "else")
            {
                match("else");

                if (getContenido() == "{")
                {
                    BloqueInstrucciones();
                }
                else
                {
                    Instruccion();
                }
            }
        }

        // Condicion -> Expresion operadorRelacional Expresion
        private void Condicion()
        {
            Expresion();
            match(Tipos.OperadorRelacional);
            Expresion();
        }

        // While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While()
        {
            match("while");
            match("(");
            Condicion();
            match(")");

            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
        }

        // Do -> do
        // bloqueInstrucciones | intruccion
        // while(Condicion);
        private void Do()
        {
            match("do");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            match("while");
            match("(");
            Condicion();
            match(")");
            match(";");
        }

        // For -> for(Asignacion; Condicion; Asignacion)
        // BloqueInstrucciones | Intruccion
        private void For()
        {
            match("for");
            match("(");
            Asignacion();
            match(";");
            Condicion();
            match(";");
            Asignacion();
            match(")");

            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
        }

        // Console -> Console.(WriteLine|Write) (cadena concatenaciones?);
        private void console()
        {
            string contenido = "";

            match("Console");
            match(".");

            string operacion = getContenido();

            if (operacion == "Write")
            {
                match("Write");
            }
            else if (operacion == "WriteLine")
            {
                match("WriteLine");
            }
            /*else if (operacion == "Read")
            {
                match("Read");
            }
            else
            {
                match("ReadLine");
            }
            */
            match("(");

            if (operacion == "Write" || operacion == "WriteLine")
            {
                if (getClasificacion() == Tipos.Cadena || getClasificacion() == Tipos.Identificador)
                {
                    contenido = getContenido().Trim('"');
                    if (getClasificacion() == Tipos.Cadena)
                    {
                        match(Tipos.Cadena);
                    }
                    else
                    {
                        match(Tipos.Identificador);
                    }

                    /*
                    if (getContenido() == "+")
                    {
                        // Concatenaciones(ref contenido);
                    }
                    */
                }
            }

            match(")");

            if (operacion == "Write")
            {
                Console.Write(contenido);
                log.Write(contenido);
            }
            else
            {
                Console.WriteLine(contenido);
                log.WriteLine(contenido);
            }
        }




        // Main -> static void Main(string[] args) BloqueInstrucciones
        private void Main()
        {
            match("static");
            match("void");
            match("Main");
            match("(");
            match("string");
            match("[");
            match("]");
            match("args");
            match(")");
            BloqueInstrucciones();
        }

        // Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }

        // MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                match(Tipos.OperadorTermino);
                Termino();
            }
        }

        // Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }

        // PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                match(Tipos.OperadorFactor);
                Factor();
            }
        }

        // Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                Expresion();
                match(")");
            }
        }
    }
}
