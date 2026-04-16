using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ExampleWpf;

public partial class MainWindow : Window
{
    private const int CellSize = 40;
    private const int Rows = 10;
    private const int Cols = 10;

    private int _heroX = 1;
    private int _heroY = 1;
    private Rectangle _hero;

    private int[,] _map;

    public MainWindow()
    {
        InitializeComponent();

        this.KeyDown += OnKeyDown;

        InitMap();
    }

    private void NewGame_Click(object sender, RoutedEventArgs e)
    {
        ShowGame();
    }

    private void LoadGame_Click(object sender, RoutedEventArgs e)
    {
        ShowGame();
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void ShowGame()
    {
        MenuPanel.Visibility = Visibility.Collapsed;
        GamePanel.Visibility = Visibility.Visible;

        DrawMap();
        DrawHero();
    }

    private void InitMap()
    {
        _map = new int[Rows, Cols];

        for (var y = 0; y < Rows; y++)
        {
            for (var x = 0; x < Cols; x++)
            {
                if (x == 0 || y == 0 || x == Cols - 1 || y == Rows - 1)
                {
                    _map[y, x] = 1; // blocked
                }
                else
                {
                    _map[y, x] = 0; // walkable
                }
            }
        }
    }

    private void DrawMap()
    {
        GameCanvas.Children.Clear();

        for (var y = 0; y < Rows; y++)
        {
            for (var x = 0; x < Cols; x++)
            {
                var rect = new Rectangle
                {
                    Width = CellSize,
                    Height = CellSize,
                    Fill = _map[y, x] == 0 ? Brushes.Green : Brushes.Black
                };

                Canvas.SetLeft(rect, x * CellSize);
                Canvas.SetTop(rect, y * CellSize);

                GameCanvas.Children.Add(rect);
            }
        }
    }

    private void DrawHero()
    {
        _hero = new Rectangle
        {
            Width = CellSize,
            Height = CellSize,
            Fill = Brushes.Red
        };

        GameCanvas.Children.Add(_hero);

        UpdateHeroPosition();
    }

    private void UpdateHeroPosition()
    {
        Canvas.SetLeft(_hero, _heroX * CellSize);
        Canvas.SetTop(_hero, _heroY * CellSize);
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        var newX = _heroX;
        var newY = _heroY;

        if (e.Key == Key.Left)
        {
            newX--;
        }
        if (e.Key == Key.Right)
        {
            newX++;
        }
        if (e.Key == Key.Up)
        {
            newY--;
        }
        if (e.Key == Key.Down)
        {
            newY++;
        }

        if (_map[newY, newX] == 0)
        {
            _heroX = newX;
            _heroY = newY;
            UpdateHeroPosition();
        }
    }
}