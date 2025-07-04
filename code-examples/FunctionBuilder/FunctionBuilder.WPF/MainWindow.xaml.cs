using System.Collections.Generic;
using FunctionBuilder.Logic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FunctionBuilder.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            string expression = tbExpression.Text;
            double rangeStart = string.IsNullOrEmpty(tbRangeStart.Text) ? double.NaN : double.Parse(tbRangeStart.Text);
            double rangeEnd = string.IsNullOrEmpty(tbRangeEnd.Text) ? double.NaN : double.Parse(tbRangeEnd.Text);
            double delta = string.IsNullOrEmpty(tbDelta.Text) ? double.NaN : double.Parse(tbDelta.Text);

            Function function = new Function(expression, rangeStart, rangeEnd, delta);
            Dictionary<double, double> functionValues = function.CalculateFunctionValues();

            spRPN.Visibility = Visibility.Visible;
            tbRPN.Text = function.ToString();

            if (functionValues.Count == 1)
            {
                spResult.Visibility = Visibility.Visible;
                tbResult.Text = function.CalculateFunctionValues().First().Value.ToString();
            }
            else
            {
                gFunctionValues.Visibility = Visibility.Visible;
                gFunctionValues.ItemsSource = functionValues.Select(x => new FunctionValue { X = x.Key, Y = x.Value }).ToList();
            }
        }
    }
}
