using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FunctionBuilder.Logic
{
    public class ReversePolishNotation
    {
        Stack<Parenthessis> parenthessises = new Stack<Parenthessis>();

        public object[] Parse(string expressionString)
        {
            List<object> expression = ParseExpression(expressionString, 0);
            List<object> rpn = new List<object>();
            ToReversePolishNotation(expression, 0, ref rpn);

            return rpn.ToArray();
        }

        private List<object> ParseExpression(string expressionString, int i)
        {
            List<object> expression = new List<object>();

            while (i < expressionString.Length)
            {
                if (char.IsDigit(expressionString[i]))
                {
                    i = ReadNumber(expressionString, i, out double number);
                    expression.Add(number);
                }
                else if (char.IsWhiteSpace(expressionString[i]))
                {
                    i++;
                }
                else if (expressionString[i] == '(' || expressionString[i] == ')')
                {
                    expression.Add(new Parenthessis(expressionString[i]));
                    i++;
                }
                else
                {
                    i = ReadOperationOrArgument(expressionString, i, out object token);
                    expression.Add(token);
                }
            }

            return expression;
        }

        private int ReadNumber(string expression, int i, out double number)
        {
            StringBuilder sb = new StringBuilder();
            while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
            {
                sb.Append(expression[i++]);
            }

            string numberString = sb.ToString();
            number = string.IsNullOrEmpty(numberString)
                ? double.NaN
                : double.Parse(numberString, CultureInfo.InvariantCulture);

            return i;
        }

        private int ReadOperationOrArgument(string expression, int i, out object token)
        {
            bool isAlphabetic = char.IsLetter(expression[i]);

            StringBuilder sb = new StringBuilder();
            while (i < expression.Length                                    // не дойдем до конца строки,...
                && !char.IsDigit(expression[i])                             // не встретим цифру...
                && expression[i] != '(' && expression[i] != ')'             // или скобку...
                && ((isAlphabetic && char.IsLetter(expression[i]))          // или буквенная операция не прервется символом...
                    || (!isAlphabetic && !char.IsLetter(expression[i]))))   // или наоборот
            {
                sb.Append(expression[i++]);
            }

            string opString = sb.ToString();
            switch (opString)
            {
                case "+": token = new Plus(); break;
                case "-": token = new Minus(); break;
                case "*": token = new Multiply(); break;
                case "/": token = new Devide(); break;
                case "log": token = new Log(); break;
                case "x": token = new Argument(); break;
                default: throw new Exception("Неизвестная операция");
            }

            return i;
        }

        private int ToReversePolishNotation(List<object> expression, int i, ref List<object> rpn)
        {
            Stack<object> operands = new Stack<object>();
            Stack<Operation> operations = new Stack<Operation>();

            for (; i < expression.Count; i++)
            {
                object token = expression[i];
                if (token is double number)
                {
                    operands.Push(number);
                }
                else if (token is Operation op)
                {
                    if (operations.Count != 0 && op.Priority < operations.Peek().Priority)
                    {
                        operands.Push(ExtractLastOperation(operands, operations));
                    }

                    operations.Push(op);
                }
                else if (token is Argument arg)
                {
                    operands.Push(arg);
                }
                else if (token is Parenthessis parenthessis)
                {
                    if (parenthessis.IsOpening)
                    {
                        parenthessises.Push(parenthessis);
                        // TODO пошли рекурсивно парсить выражение внутри
                        List<object> subRpn = new List<object>();
                        i = ToReversePolishNotation(expression, i + 1, ref subRpn);
                        operands.Push(subRpn);
                    }
                    else
                    {
                        if (parenthessises.Count != 0 && parenthessises.Peek().IsOpening)
                        {
                            parenthessises.Pop();
                            break;
                        }

                        throw new Exception("Неправильная вложенность скобок");
                    }
                }
            }

            // Теперь нужно разложить "список в списке" в плоский вид
            rpn = StackToRpn(operands, operations);
            return i;
        }

        private List<object> StackToRpn(Stack<object> operands, Stack<Operation> operations)
        {
            while (operations.Count != 0)
            {
                operands.Push(ExtractLastOperation(operands, operations));
            }

            return ConvertToList(operands.ToList());
        }

        private static List<object> ExtractLastOperation(Stack<object> operands, Stack<Operation> operations)
        {
            Operation oldOperation = operations.Pop();
            object[] newOperand = new object[oldOperation.OperandCount + 1];
            for (int i = oldOperation.OperandCount - 1; i >= 0; i--)
            {
                newOperand[i] = operands.Pop();
            }

            newOperand[oldOperation.OperandCount] = oldOperation;
            return newOperand.ToList();
        }

        private List<object> ConvertToList(List<object> list)
        {
            List<object> result = new List<object>();
            foreach (object operand in list)
            {
                if (operand is List<object>)
                {
                    List<object> subList = (List<object>)operand;
                    result.AddRange(ConvertToList(subList));
                }
                else
                {
                    result.Add(operand);
                }
            }

            return result;
        }
    }
}