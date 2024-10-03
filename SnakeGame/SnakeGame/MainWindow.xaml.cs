using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using MySql.Data.MySqlClient;

namespace SnakeGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private const string ConnectionString = "Server=localhost;Database=snake_game;Uid=root;";

        private bool isDatabaseAvailable = true;

        private readonly Dictionary<Griglia, ImageSource> gridValToImage = new()
        {
            { Griglia.Empty, Immagini.Empty},
            { Griglia.Snake, Immagini.Body },
            { Griglia.Food, Immagini.Food }
        };

        private readonly Dictionary<Direzioni, int> dirToRotation = new()
        {
            { Direzioni.Up, 0 },
            { Direzioni.Right, 90 },
            { Direzioni.Down, 180 },
            { Direzioni.Left, 270 },
        };

        private readonly int rows = 15, cols = 15;
        private readonly Image[,] gridImages;
        private GameState gameState;
        private bool gameRunning;

        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            gameState = new GameState(rows, cols);
        }

        private async Task RunGame()
        {
            Draw();
            await Countdown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            await Gameover();
            gameState = new GameState(rows, cols);
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }

            if (!gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return; //partita finita
            }

            switch (e.Key)
            {
                case Key.Left:
                    gameState.ChangeDirection(Direzioni.Left);
                    break;
                case Key.Right:
                    gameState.ChangeDirection(Direzioni.Right);
                    break;
                case Key.Up:
                    gameState.ChangeDirection(Direzioni.Up);
                    break;
                case Key.Down:
                    gameState.ChangeDirection(Direzioni.Down);
                    break;
            }
        }

        //dobbiamo fare muovere il serpente a intervalli regolari fino a quando non finisce il gioco
        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                await Task.Delay(100); //può essere modificato per rendere il gioco più veloce o lento
                gameState.Move();
                Draw();
            }
        }

        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            GameGrid.Width = GameGrid.Height * (cols / (double)rows); //mi permette di non avere per forza griglie quadrate
            
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Immagini.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5) //risolve bug creato per far girare la testa
                    };

                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }

            return images;
        }

        private void Draw()
        {
            DrawGrid();
            snakeHead();
            ScoreText.Text = $"SCORE {gameState.Score}";
        }

        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Griglia gridVal = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridVal];
                    gridImages[r, c].RenderTransform = Transform.Identity; //risolve bug creato per far girare la testa
                }
            }
        }

        private async Task Countdown()
        {
            for (int i=3; i >= 1; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }
        private async Task snakeDead()
        {
            List<Posizioni> positions = new List<Posizioni>(gameState.SnakePositions());

            for (int i = 0; i < positions.Count; i++)
            {
                Posizioni pos = positions[i];
                ImageSource source = (i == 0) ? Immagini.DeadHead : Immagini.DeadBody;
                gridImages[pos.Row, pos.Col].Source = source;
                await Task.Delay(50);
            }
        }

        private async Task Gameover()
        {
            await snakeDead();

            if (isDatabaseAvailable)
            {
                await SaveScore(gameState.Score);
                await Task.Delay(1000);

                await DisplayLeaderboard();
            }
            else
            {
                await Task.Delay(1000);
            }

            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "PREMI UN TASTO PER GIOCARE";
        }



        private void snakeHead()
        {
            Posizioni headPos = gameState.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Col];
            image.Source = Immagini.Head;

            int rotation = dirToRotation[gameState.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }

        private async Task SaveScore(int score)
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    string query = "INSERT INTO leaderboard (score, date) VALUES (@score, @date)";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@score", score);
                        command.Parameters.AddWithValue("@date", DateTime.Now);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (MySqlException)
            {
                //Ignora l'errore se il database non è disponibilie
                isDatabaseAvailable = false;
            }
        }

        private async Task<List<(int Score, DateTime Date)>> GetTopScores(int limit = 10)
        {
            var scores = new List<(int Score, DateTime Date)>();
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT score, date FROM leaderboard ORDER BY score DESC LIMIT @limit";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@limit", limit);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                scores.Add((reader.GetInt32(0), reader.GetDateTime(1)));
                            }
                        }
                    }
                }
                isDatabaseAvailable = true;
            }
            catch (MySqlException)
            {
                // Ignora l'errore se il database non è disponibile
                isDatabaseAvailable = false;
            }

            return scores;
        }

        private async Task DisplayLeaderboard()
        {
            if (!isDatabaseAvailable)
            {
                return;
            }

            var topScores = await GetTopScores();
            var leaderboardText = "\n";
            for (int i = 0; i < topScores.Count; i++)
            {
                leaderboardText += $"{i + 1}. Score: {topScores[i].Score} - Data: {topScores[i].Date:dd/MM/yyyy HH:mm:ss}\n";
            }

            // Aggiorna l'UI per mostrare la classifica
            Dispatcher.Invoke(() =>
            {
                LeaderboardText.Text = leaderboardText;
                LeaderboardOverlay.Visibility = Visibility.Visible;
            });
        }

        private void CloseLeaderboard_Click(object sender, RoutedEventArgs e)
        {
            LeaderboardOverlay.Visibility = Visibility.Hidden;
        }

    }
}
