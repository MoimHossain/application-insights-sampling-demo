

using System;
using System.Collections.Generic;
using System.Text;

namespace AI.Sampling.Example
{
    public class Cmd
    {
        public static Cmd Ln = new Cmd();

        public Cmd Gray(object message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(message);
            return this;
        }

        public Cmd White(object message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(message);
            return this;
        }

        public Cmd Green(object message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(message);
            return this;
        }
        public Cmd Red(object message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(message);
            return this;
        }

        public Cmd Yellow(object message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(message);
            return this;
        }

        public Cmd Cyan(object message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(message);
            return this;
        }

        public Cmd EOL()
        {
            Console.WriteLine("");
            return this;
        }
    }
}

