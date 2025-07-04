using FunctionBuilder.Logic;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FunctionBuilder.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("1+2", ExpectedResult = "1 2 +")]
        [TestCase("1+2/1", ExpectedResult = "1 2 1 / +")]
        public string Function_WithoutArgument(string expression)
        {
            return new Function(expression).ToString();
        }

        [TestCaseSource(nameof(TestCasesWithoutArgument))]
        public void RPN_WithoutArgument(string expression, int result, object[] rpnTokens)
        {
            Assert.That(rpnTokens, Is.EquivalentTo(new ReversePolishNotation().Parse(expression)));
            Assert.AreEqual(result, new Function(expression).CalculateFunctionValues().First().Value);
        }

        [TestCaseSource(nameof(TestCasesWithArgument))]
        public void Function_WithArgument(string expression, double rangeStart, double rangeEnd, double delta, Dictionary<double, double> expectedResult)
        {
            var func = new Function(expression, rangeStart, rangeEnd, delta);
            var result = func.CalculateFunctionValues();

            Assert.AreEqual(expectedResult.First().Value, result.First().Value);
        }

        public static IEnumerable<TestCaseData> TestCasesWithoutArgument
        {
            get
            {
                yield return new TestCaseData("1+2", 3, new object[] { 1.0, 2.0, new Plus() });
                yield return new TestCaseData("1+2/1", 3, new object[] { 1.0, 2.0, 1.0, new Devide(), new Plus() });
            }
        }

        public static IEnumerable<TestCaseData> TestCasesWithArgument
        {
            get
            {
                yield return new TestCaseData("1+2*x", 0, 4, 1, new Dictionary<double, double> 
                { 
                    { 0.0, 1.0 },
                    { 1.0, 3.0 },
                    { 2.0, 5.0 },
                    { 3.0, 7.0 },
                    { 4.0, 9.0 }
                });
            }
        }
    }
}