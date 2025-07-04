using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FunctionBuilder.WPF
{
    /// <summary>
    /// Interaction logic for PlotControl.xaml
    /// </summary>
    public partial class PlotControl : UserControl
    {
        public PlotControl()
        {
            InitializeComponent();
        }

        private void CanvasPlot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Draw();
        }

        private void Draw()
        {
            canvasPlot.Children.Clear();

            Line xAxis = new Line();
            double height = canvasPlot.ActualHeight;
            double width = canvasPlot.ActualWidth;

            xAxis.X1 = 0;
            xAxis.Y1 = height / 2;
            xAxis.X2 = width;
            xAxis.Y2 = height / 2;
            xAxis.Stroke = Brushes.Red;
            xAxis.StrokeThickness = 2;

            Polygon arrow1 = new Polygon();
            arrow1.Points.Add(new Point(height / 2, width));
            arrow1.Points.Add(new Point(height / 2 - 10, width - 15));
            arrow1.Points.Add(new Point(height / 2 + 10, width - 15));
            arrow1.Fill = Brushes.Black;

            canvasPlot.Children.Add(xAxis);
            canvasPlot.Children.Add(arrow1);
        }
    }
}
