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
        /// <param name="rangeStart">��������� �������� ��������� ��� ���������� �������</param>
        /// <param name="rangeEnd">�������� �������� ��������� ��� ���������� �������</param>
        /// <param name="delta">��� ��������� ���������</param>
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
                throw new Exception("� ��������� ����� ��������, �� �� ������ �������� ��� ��� �����������.");
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
                throw new Exception("��������� ��� �� ���� ����������.");
            }

            Stack<double> numbers = new Stack<double>(); // ����� � ��� �����, � ������ ����� �� �������, �� ��� ��� �������� ������
                                                         // ��������� �� ���� ������� � ���
            foreach (var token in reversePolishNotation)
            {
                if (token is double)
                {
                    // ���� ����� - ��������� � ���� ��� ����������� �������������
                    numbers.Push((double)token);
                }
                else if (token is Argument)
                {
                    numbers.Push(argumentValue);
                }
                else
                {
                    // ���� ��������...
                    var operation = (Operation)token;
                    var args = new double[operation.OperandCount];
                    // ��������� �� ����� ����� ������� ���, ������� ���������� � ������ ��������
                    // ������ ������, ��� ����� � ����� ����� � �������� ������� - �� ����� ����������
                    for (int i = operation.OperandCount; i > 0; i--)
                    {
                        args[i - 1] = numbers.Pop();
                    }
                    var subResult = operation.Evaluate(args);
                    // ������ ��������� ������� � ���� �����
                    numbers.Push(subResult);
                }
            }

            // �.�. �� �������, ��� ���� ��� ���������, �� � ��������, ����� ����� �� ����� ������,
            // ��������� ����� � ����� - ��� �������� ���������

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