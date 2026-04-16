using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using StrategyGame.Wpf.Game;
using StrategyGame.Wpf.Models;

namespace StrategyGame.Wpf;

public partial class MainWindow : Window
{
    private const double CellSize = 42;
    private const double CellMargin = 2;
    private static readonly string DefaultMapPath =
        System.IO.Path.Combine(AppContext.BaseDirectory, "maps", "simple.txt");

    private readonly StrategyGameState gameState;
    private bool isGameStarted;
    private string statusMessage = "Start a new game to begin.";

    public MainWindow()
    {
        InitializeComponent();

        gameState = new StrategyGameState(width: 20, height: 12);
        PlayerTypeComboBox.ItemsSource = Enum.GetValues<PlayerType>();
        PlayerTypeComboBox.SelectedItem = PlayerType.Knight;

        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        PlayerNameTextBox.Focus();
        PlayerNameTextBox.SelectAll();
    }

    private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (!isGameStarted)
        {
            return;
        }

        Coordinate? shift = e.Key switch
        {
            Key.W => new Coordinate(-1, 0),
            Key.Up => new Coordinate(-1, 0),
            Key.S => new Coordinate(1, 0),
            Key.Down => new Coordinate(1, 0),
            Key.A => new Coordinate(0, -1),
            Key.Left => new Coordinate(0, -1),
            Key.D => new Coordinate(0, 1),
            Key.Right => new Coordinate(0, 1),
            _ => null
        };

        if (shift is null)
        {
            return;
        }

        bool moved = gameState.MovePlayer(shift.Value);
        statusMessage = moved
            ? "Move completed."
            : "You cannot move there.";

        DrawScene();
        e.Handled = true;
    }

    private void StartGameButton_OnClick(object sender, RoutedEventArgs e)
    {
        string playerName = PlayerNameTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(playerName))
        {
            StartScreenErrorTextBlock.Text = "Enter player name first.";
            PlayerNameTextBox.Focus();
            return;
        }

        if (PlayerTypeComboBox.SelectedItem is not PlayerType playerType)
        {
            StartScreenErrorTextBlock.Text = "Select player class.";
            PlayerTypeComboBox.Focus();
            return;
        }

        StartScreenErrorTextBlock.Text = string.Empty;
        string mapSource;

        try
        {
            if (File.Exists(DefaultMapPath))
            {
                gameState.StartNewGame(playerName, playerType, DefaultMapPath);
                mapSource = System.IO.Path.GetFileName(DefaultMapPath);
                statusMessage = "Game started. Map loaded from file.";
            }
            else
            {
                gameState.StartNewGame(playerName, playerType);
                mapSource = "generated map";
                statusMessage = "Game started. File map not found, generated fallback map.";
            }
        }
        catch (Exception ex)
        {
            gameState.StartNewGame(playerName, playerType);
            mapSource = "generated map";
            statusMessage = $"Map loading failed, generated fallback map. {ex.Message}";
        }

        MapSourceTextBlock.Text = mapSource;
        ShowGameScreen();
        DrawScene();
        Focus();
    }

    private void ShowGameScreen()
    {
        isGameStarted = true;
        StartScreenBorder.Visibility = Visibility.Collapsed;
        GamePanel.Visibility = Visibility.Visible;
    }

    private void DrawScene()
    {
        DrawMapOnCanvas();
        UpdateSidebar();
        UpdateBottomPanel();
        UpdateHeader();
    }

    private void DrawMapOnCanvas()
    {
        GameCanvas.Children.Clear();

        int rows = gameState.Map.GetLength(0);
        int columns = gameState.Map.GetLength(1);

        double step = CellSize + CellMargin;
        GameCanvas.Width = columns * step + CellMargin;
        GameCanvas.Height = rows * step + CellMargin;

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                Rectangle tile = CreateTile(gameState.Map[x, y]);

                double left = CellMargin + y * step;
                double top = CellMargin + x * step;

                Canvas.SetLeft(tile, left);
                Canvas.SetTop(tile, top);
                GameCanvas.Children.Add(tile);

                if (gameState.Player is not null &&
                    gameState.Player.X == x &&
                    gameState.Player.Y == y)
                {
                    Border playerMarker = CreatePlayerMarker();
                    Canvas.SetLeft(playerMarker, left);
                    Canvas.SetTop(playerMarker, top);
                    GameCanvas.Children.Add(playerMarker);
                }
            }
        }
    }

    private void UpdateSidebar()
    {
        HeroNameTextBlock.Text = gameState.Player?.Name ?? "Unknown";
        HeroTypeTextBlock.Text = gameState.Player?.Type.ToString() ?? "Unknown";
        HeroHealthTextBlock.Text = $"Health: {gameState.Player?.Health ?? 0}";
        HeroPositionTextBlock.Text =
            $"Position: ({gameState.Player?.X ?? 0}, {gameState.Player?.Y ?? 0})";
    }

    private void UpdateBottomPanel()
    {
        WoodTextBlock.Text = CountCells(MapCell.Wood).ToString();
        StoneTextBlock.Text = CountCells(MapCell.Stone).ToString();
        GoldTextBlock.Text = CountCells(MapCell.Gold).ToString();
    }

    private void UpdateHeader()
    {
        int rows = gameState.Map.GetLength(0);
        int columns = gameState.Map.GetLength(1);

        GameInfoTextBlock.Text =
            $"Map: {columns} x {rows}   Player: {gameState.Player?.Name} ({gameState.Player?.Type})   Position: ({gameState.Player?.X}, {gameState.Player?.Y})   Status: {statusMessage}";
    }

    private int CountCells(MapCell mapCell)
    {
        int count = 0;

        for (int x = 0; x < gameState.Map.GetLength(0); x++)
        {
            for (int y = 0; y < gameState.Map.GetLength(1); y++)
            {
                if (gameState.Map[x, y] == mapCell)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static Rectangle CreateTile(MapCell cell)
    {
        (Brush fill, Brush stroke) style = cell switch
        {
            MapCell.Empty => (CreateBrush("#3E6B52"), CreateBrush("#577A63")),
            MapCell.Wall => (CreateBrush("#4A4F5A"), CreateBrush("#707887")),
            MapCell.Gold => (CreateBrush("#C79A20"), CreateBrush("#E6BF55")),
            MapCell.Wood => (CreateBrush("#2E7D4A"), CreateBrush("#49A76A")),
            MapCell.Stone => (CreateBrush("#667689"), CreateBrush("#8E9CAF")),
            _ => (CreateBrush("#7A4EA3"), CreateBrush("#A47FD4"))
        };

        return new Rectangle
        {
            Width = CellSize,
            Height = CellSize,
            RadiusX = 6,
            RadiusY = 6,
            Fill = style.fill,
            Stroke = style.stroke,
            StrokeThickness = 1.2
        };
    }

    private static Border CreatePlayerMarker()
    {
        return new Border
        {
            Width = CellSize,
            Height = CellSize,
            CornerRadius = new CornerRadius(6),
            Background = CreateBrush("#D95763"),
            BorderBrush = CreateBrush("#FFCDD2"),
            BorderThickness = new Thickness(1.5),
            Child = new TextBlock
            {
                Text = "@",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center
            }
        };
    }

    private static SolidColorBrush CreateBrush(string hexColor)
    {
        return (SolidColorBrush)new BrushConverter().ConvertFrom(hexColor)!;
    }
}
