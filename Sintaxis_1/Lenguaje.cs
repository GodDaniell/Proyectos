using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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

/*
REQUERIMIENTOS 2:
    1. Concatenaciones 
    2. Inicializar una variable desde la declaración (LISTO)
    3. Evaluar las expresiones matemáticas (LISTO)
    4. Levantar si en el Console.ReadLine() no ingresan números (LISTO)
    5. Modificar la variable con el resto de operadores (Incremento de factor y termino) (LISTO)
    6. Hacer que funcione el else

*/

namespace Sintaxis_1
{
    public class Lenguaje : Sintaxis
    {
        Stack<float> s;
        List<Variable> l;
        public Lenguaje() : base()
        {
            s = new Stack<float>();
            l = new List<Variable>();
        }

        public Lenguaje(string name) : base(name)
        {
            s = new Stack<float>();
            l = new List<Variable>();
        }

        private void displayStack()
        {
            Console.WriteLine("Contenido del stack: ");
            foreach (float elemento in s)
            {
                Console.WriteLine(elemento);
            }
        }

        private void displayList()
        {
            log.WriteLine("Lista de variables : ");
            foreach (Variable elemento in l)
            {
                log.WriteLine($"{elemento.getNombre()} {elemento.GetTipoDato()} {elemento.getValor()}");
            }
        }
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
            displayList();
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
            Variable.TipoDato t = Variable.TipoDato.Char;

            switch (getContenido())
            {
                case "int":
                    t = Variable.TipoDato.Int;
                    break;
                case "float":
                    t = Variable.TipoDato.Float;
                    break;
            }

            match(Tipos.TipoDato);
            ListaIdentificadores(t);
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
        private void ListaIdentificadores(Variable.TipoDato t)
        {
            if (l.Find(variable => variable.getNombre() == getContenido()) != null)
            {
                throw new Error("La variable " + getContenido() + " ya existe", log, linea, columna);
            }
            l.Add(new Variable(t, getContenido()));
            match(Tipos.Identificador);

            if (getContenido() == "=")
            {
                match("=");
                Expresion();
                float r = s.Pop();
            }

            if (getContenido() == ",")
            {
                match(",");
                ListaIdentificadores(t);
            }
        }

        // BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones(bool ejecuta)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(ejecuta);
            }
            else
            {
                match("}");
            }
        }

        // ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool ejecuta)
        {
            Instruccion(ejecuta);
            if (getContenido() != "}")
            {
                ListaInstrucciones(ejecuta);
            }
            else
            {
                match("}");
            }
        }

        // Instruccion -> console | If | While | do | For | Variables | Asignación
        private void Instruccion(bool ejecuta)
        {
            if (getContenido() == "Console")
            {
                console(ejecuta);
                match(";");
            }
            else if (getContenido() == "if")
            {
                If(ejecuta);
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
            Variable? v = l.Find(variable => variable.getNombre() == getContenido());
            if (v == null)
            {
                throw new Error("Sintaxis: la variable " + getContenido() + " ya existe", log, linea, columna);
            }
            //Console.Write(getContenido() + "=");
            match(Tipos.Identificador);

            if (getContenido() == "=")
            {
                match("=");
                if (getContenido() == "Console")
                {
                    match("Console");
                    match(".");

                    if (getContenido() == "Read"){
                        match("Read");
                        Console.Read();
                    }else{
                        match("ReadLine");
                        Console.ReadLine();
                    }
                    match("(");
                    match(")");
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

            float r = s.Pop();
            v.setValor(r);
            //displayStack();

        }

        // If -> if (Condicion) bloqueInstrucciones | instruccion
        // (else bloqueInstrucciones | instruccion)?
        private void If(bool ejecuta2)
        {
            match("if");
            match("(");
            bool ejecuta = Condicion() && ejecuta2;
            Console.WriteLine(ejecuta);
            match(")");

            if (getContenido() == "{")
            {
                BloqueInstrucciones(ejecuta);
            }
            else
            {
                Instruccion(ejecuta);
            }

            if (getContenido() == "else")
            {
                match("else");

                if (getContenido() == "{")
                {
                    BloqueInstrucciones(false);
                }
                else
                {
                    Instruccion(false);
                }
            }
        }

        // Condicion -> Expresion operadorRelacional Expresion
        private bool Condicion()
        {
            Expresion();
            float valor1 = s.Pop();
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion();
            float valor2 = s.Pop();

            switch (operador)
            {
                case "<":
                    return valor1 < valor2;
                case ">":
                    return valor1 > valor2;
                case "<=":
                    return valor1 <= valor2;
                case ">=":
                    return valor1 >= valor2;
                case "==":
                    return valor1 == valor2;
                default:
                    return valor1!= valor2;
                
            }
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
                BloqueInstrucciones(true);
            }
            else
            {
                Instruccion(true);
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
                BloqueInstrucciones(true);
            }
            else
            {
                Instruccion(true);
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
                BloqueInstrucciones(true);
            }
            else
            {
                Instruccion(true);
            }
        }
 
        // Console -> Console.(WriteLine|Write) (cadena concatenaciones?);
        private void console(bool ejecuta)
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
                    if (getContenido() == "+")
                    {
                        Concatenaciones();
                    }
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
            BloqueInstrucciones(true);
        }

        // Concatenaciones -> (Cadena | Identificador) (+concatenaciones)?
        private void Concatenaciones()
        {
            if (getClasificacion() == Tipos.Cadena){
                match(Tipos.Cadena);
            }else{
                match(Tipos.Identificador);
            }

            if (getContenido() == "+")
            {
                match("+");
                Concatenaciones();
            }
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
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino();
                //Console.Write(operador + " ");

                float n1 = s.Pop();
                float n2 = s.Pop();

                switch (operador)
                {

                    case "+":
                        s.Push(n2 + n1);
                        break;

                    case "-":
                        s.Push(n2 - n1);
                        break;
                }
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
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor();
                //Console.Write(operador + " ");
                float n1 = s.Pop();
                float n2 = s.Pop();

                switch (operador)
                {

                    case "*":
                        s.Push(n2 * n1);
                        break;

                    case "/":
                        s.Push(n2 / n1);
                        break;

                    case "%":
                        s.Push(n2 % n1);
                        break;
                }
            }
        }

        // Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                s.Push(float.Parse(getContenido()));
                //Console.Write(getContenido() + " ");
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                Variable? v = l.Find(variable => variable.getNombre() == getContenido());
                if (v == null)
            {
                throw new Error("Sintaxis: la variable " + getContenido() + " no esta definida en ", log, linea, columna);
            }
                
                s.Push(v.getValor());
                //Console.Write(getClasificacion() + " ");
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
