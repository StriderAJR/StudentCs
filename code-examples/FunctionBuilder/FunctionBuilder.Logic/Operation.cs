using System;

namespace FunctionBuilder.Logic
{
    public class Parenthessis
    {
        public bool IsOpening { get; }

        public Parenthessis(char parenthesis)
        {
            IsOpening = parenthesis == '(';
        }

        public override string ToString()
        {
            return IsOpening ? "(" : ")";
        }
    }

    public abstract class Operation
    {
        public abstract string Name { get; }
        public abstract byte OperandCount { get; }
        public abstract bool IsPrefix { get; }
        public abstract bool IsInfix { get; }
        public abstract bool IsPostfix { get; }
        public abstract int Priority { get; }

        public abstract double Evaluate(double[] @params);

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (obj is string)
            {
                return ((string)obj) == Name;
            }

            return obj.GetType() == this.GetType();
        }

        public static bool operator ==(Operation p1, object p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Operation p1, object p2)
        {
            return !p1.Equals(p2);
        }
    }

    public class Plus : Operation
    {
        public override string Name => "+";
        public override byte OperandCount => 2;
        public override bool IsPrefix => false;
        public override bool IsInfix => true;
        public override bool IsPostfix => false;
        public override int Priority => 1;

        public override double Evaluate(double[] @params)
        {
            if (@params.Length != 2)
                throw new ArgumentException("Неверное количество аргументов.");

            return @params[0] + @params[1];
        }
    }

    public class Minus : Operation
    {
        public override string Name => "-";
        public override byte OperandCount => 2;
        public override bool IsPrefix => false;
        public override bool IsInfix => true;
        public override bool IsPostfix => false;
        public override int Priority => 1;

        public override double Evaluate(double[] @params)
        {
            if (@params.Length != 2)
                throw new ArgumentException("Неверное количество аргументов.");

            return @params[0] - @params[1];
        }
    }

    public class Multiply : Operation
    {
        public override string Name => "*";
        public override byte OperandCount => 2;
        public override bool IsPrefix => false;
        public override bool IsInfix => true;
        public override bool IsPostfix => false;
        public override int Priority => 2;

        public override double Evaluate(double[] @params)
        {
            if (@params.Length != 2)
                throw new ArgumentException("Неверное количество аргументов.");

            return @params[0] * @params[1];
        }
    }

    public class Devide : Operation
    {
        public override string Name => "/";
        public override byte OperandCount => 2;
        public override bool IsPrefix => false;
        public override bool IsInfix => true;
        public override bool IsPostfix => false;
        public override int Priority => 2;

        public override double Evaluate(double[] @params)
        {
            if (@params.Length != 2)
                throw new ArgumentException("Неверное количество аргументов.");

            return @params[0] / @params[1];
        }

        public override bool Equals(object obj)
        {
            return obj is Devide;
        }
    }

    public class Log : Operation
    {
        public override string Name => "log";
        public override byte OperandCount => 2;
        public override bool IsPrefix => true;
        public override bool IsInfix => false;
        public override bool IsPostfix => false;
        public override int Priority => 3;

        public override double Evaluate(double[] @params)
        {
            if (@params.Length != 2)
                throw new ArgumentException("Неверное количество аргументов.");

            return Math.Log(@params[0], @params[1]);
        }
    }
}