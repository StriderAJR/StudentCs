using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionBuilder.Logic
{
    public class Function
    {
        ReversePolishNotation rpn = new ReversePolishNotation();
        private object[] reversePolishNotation;

        private bool hasArgument;
        private double rangeStart, rangeEnd, delta;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="rangeStart">Ќачальное значение аргумента дл€ вычислени€ функции</param>
        /// <param name="rangeEnd"> онечное значение аргумента дл€ вычислени€ функции</param>
        /// <param name="delta">Ўаг изменени€ аргумента</param>
        public Function(string expression, double rangeStart = double.NaN, double rangeEnd = double.NaN, double delta = double.NaN)
        {
            this.rangeStart = rangeStart;
            this.rangeEnd = rangeEnd;
            this.delta = delta;

            reversePolishNotation = rpn.Parse(expression);

            hasArgument = reversePolishNotation.Any(x => x is Argument);
        }

        public Dictionary<double, double> CalculateFunctionValues()
        {
            if (hasArgument && (double.IsNaN(rangeStart) || double.IsNaN(rangeEnd) || double.IsNaN(delta)))
            {
                throw new Exception("¬ выражении задан аргумент, но не заданы значени€ дл€ его подстановки.");
            }

            Dictionary<double, double> result = new Dictionary<double, double>();

            double argumentValue = rangeStart;
            do
            {
                result.Add(argumentValue, Calculate(argumentValue));
                argumentValue += delta;
            }
            while (hasArgument && (rangeEnd - argumentValue) > 0);

            return result;
        }

        private double Calculate(double argumentValue = double.NaN)
        {
            if (reversePolishNotation == null || reversePolishNotation.Length == 0)
            {
                throw new Exception("¬ыражение еще не было обработано.");
            }

            Stack<double> numbers = new Stack<double>(); // можно и без стека, а бегать назад по массиву, но так мне нравитс€ больше
                                                         // пробегаем по всем токенам в ќѕ«
            foreach (var token in reversePolishNotation)
            {
                if (token is double)
                {
                    // если число - сохран€ем в стек дл€ дальнейшего использовани€
                    numbers.Push((double)token);
                }
                else if (token is Argument)
                {
                    numbers.Push(argumentValue);
                }
                else
                {
                    // если операци€...
                    var operation = (Operation)token;
                    var args = new double[operation.OperandCount];
                    // извлекаем из стека чисел столько раз, сколько аргументов у данной операции
                    // причем помним, что числа в стеке лежат в обратном пор€дке - их нужно развернуть
                    for (int i = operation.OperandCount; i > 0; i--)
                    {
                        args[i - 1] = numbers.Pop();
                    }
                    var subResult = operation.Evaluate(args);
                    // кладем результат обратно в стек чисел
                    numbers.Push(subResult);
                }
            }

            // т.к. мы уверены, что наша ќѕ« корректна, то в принципе, когда дошли до конца записи,
            // последнее число в стеке - наш итоговый результат

            return numbers.Pop();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (object token in reversePolishNotation)
            {
                sb.Append(token.ToString());
                sb.Append(" ");
            }

            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}