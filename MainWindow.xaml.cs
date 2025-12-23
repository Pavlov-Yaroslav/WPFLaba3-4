using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
using WpfApp1.GameCore;
using WpfApp1.Service;
using WpfApp1.Services;
using Path = System.IO.Path;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private GameController gameController;
        private BoardRenderer boardRenderer = new BoardRenderer();

        public MainWindow(GameController controller = null)
        {
            InitializeComponent();
            gameController = controller ?? new GameController();
            GameStatusText.Text = "Настройте параметры и нажмите 'Новая игра'\nВсе игроки должны дойти до финиша";
        }

        private void RenderGame(int playerCount, int boardSize)
        { 
                boardRenderer.RenderBoard(GameBoardPanel, gameController.GameBoard);
                boardRenderer.UpdatePlayerMarkers(GameBoardPanel, gameController.GameBoard, gameController.Players);

                UpdateGameUI();

                GameStatusText.Text = $"Игра началась! {playerCount} игрока, поле {boardSize} ячеек\nИгра продолжается до финиша всех игроков!";

                FinishedPlayersText.Text = "";
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int playerCount = GetSelectedPlayerCount();
                int boardSize = GetSelectedBoardSize();

                gameController.StartNewGame(boardSize, playerCount);

                RenderGame(playerCount, boardSize);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private int GetSelectedPlayerCount()
        {
            if (Player2Radio.IsChecked == true) return 2;
            if (Player3Radio.IsChecked == true) return 3;
            if (Player4Radio.IsChecked == true) return 4;
            return 2;
        }

        private int GetSelectedBoardSize()
        {
            if (BoardSizeCombo.SelectedItem is ComboBoxItem item && item.Tag != null)
            {
                if (int.TryParse(item.Tag.ToString(), out int size))
                {
                    return size;
                }
            }
            return 40;
        }

        private void UpdateGameUI()
        {
            RollDiceButton.IsEnabled = true;
            NewGameButton.IsEnabled = false;
            SaveGame.IsEnabled = true;

            Player2Radio.IsEnabled = false;
            Player3Radio.IsEnabled = false;
            Player4Radio.IsEnabled = false;
            BoardSizeCombo.IsEnabled = false;

            var currentPlayer = gameController.CurrentPlayer;
            if (currentPlayer != null)
            {
                CurrentPlayerLabel.Text = currentPlayer.Name;
                CurrentPlayerLabel.Foreground = BrushHelper.GetBrush(currentPlayer.Color);
            }
            DiceResultText.Text = "-";
        }

        private void RollDiceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!gameController.IsGameActive)
                    return;

                List<Player> newlyFinishedPlayers;
                bool roundEnded = gameController.MakePlayerTurn(out newlyFinishedPlayers);

                DiceResultText.Text = gameController.GameDice.Edge.ToString();

                boardRenderer.UpdatePlayerMarkers(GameBoardPanel, gameController.GameBoard, gameController.Players);

                UpdateFinishedPlayersDisplay();

                if (roundEnded)
                {
                    if (gameController.AllPlayersFinished)
                    {
                        EndGame();
                    }
                    else
                    {
                        ShowIntermediateResults();
                    }

                    if (newlyFinishedPlayers.Count > 0)
                    {
                        var names = string.Join(", ", newlyFinishedPlayers.Select(p => p.Name));
                    }

                    return;
                }

                UpdateNextPlayerUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }


        private void UpdateFinishedPlayersDisplay()
        {
            int finishedCount = gameController.FinishedPlayers.Count;
            if (finishedCount > 0)
            {
                FinishedPlayersText.Text = $"Финишировало: {finishedCount}";
            }
            else
            {
                FinishedPlayersText.Text = "";
            }
        }

        private void ShowIntermediateResults()
        {
            var results = gameController.GetResults();
            var currentPlayer = gameController.CurrentPlayer;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("🎉 Игрок достиг финиша! 🎉");
            sb.AppendLine();
            sb.AppendLine("Текущие результаты:");
            sb.AppendLine();

            foreach (var result in results)
            {
                sb.AppendLine(result);
            }

            if (currentPlayer != null)
            {
                sb.AppendLine();
                sb.AppendLine($"Следующий ходит: {currentPlayer.Name}");
                GameStatusText.Text = $"Игрок достиг финиша! Ходит {currentPlayer.Name}";
            }

            MessageBox.Show(sb.ToString(), "Промежуточные результаты",
                MessageBoxButton.OK, MessageBoxImage.Information);

            UpdateNextPlayerUI();
        }

        private void EndGame()
        {
            var results = gameController.GetResults();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("ИГРА ОКОНЧЕНА!");
            sb.AppendLine();
            sb.AppendLine("Все игроки достигли финиша!");
            sb.AppendLine();
            sb.AppendLine("Итоговые результаты:");
            sb.AppendLine();

            for (int i = 0; i < results.Count; i++)
            {
                string placeEmoji = "";
                if (i == 0) placeEmoji = " ";
                else if (i == 1) placeEmoji = " ";
                else if (i == 2) placeEmoji = " ";
                else if (i == 3) placeEmoji = " ";

                sb.AppendLine($"{placeEmoji}{results[i]}");
            }

            GameStatusText.Text = "Игра окончена! Все игроки достигли финиша!";
            MessageBox.Show(sb.ToString(), "Игра окончена",
                MessageBoxButton.OK, MessageBoxImage.Information);

            RollDiceButton.IsEnabled = false;
            NewGameButton.IsEnabled = true;
            SaveGame.IsEnabled = false;

            Player2Radio.IsEnabled = true;
            Player3Radio.IsEnabled = true;
            Player4Radio.IsEnabled = true;
            BoardSizeCombo.IsEnabled = true;

            UpdateFinishedPlayersDisplay();

            boardRenderer.UpdatePlayerMarkers(GameBoardPanel, gameController.GameBoard, gameController.Players);
        }

        private void UpdateNextPlayerUI()
        {
            var nextPlayer = gameController.CurrentPlayer;
            if (nextPlayer != null)
            {
                GameStatusText.Text = $"Ходит {nextPlayer.Name}";
                CurrentPlayerLabel.Text = nextPlayer.Name;
                CurrentPlayerLabel.Foreground = BrushHelper.GetBrush(nextPlayer.Color);

                int finishedCount = gameController.FinishedPlayers.Count;
                if (finishedCount > 0)
                {
                    GameStatusText.Text += $"\nУже финишировало: {finishedCount} игроков";
                }
            }
            else
            {
                if (gameController.AllPlayersFinished)
                {
                    EndGame();
                }
            }
        }

        private void MainMenu_Click(object sender, RoutedEventArgs e)
        {
            StartWindow startWindow = new StartWindow();
            startWindow.Show();
            Close();
        }

        private void SaveGame_Click(object sender, RoutedEventArgs e)
        {
            SaveGameToFile(gameController);
        }
        private void SaveGameToFile(GameController gameController)
        {
            if (gameController == null || !gameController.IsGameActive)
            {
                MessageBox.Show("Игра ещё не начата", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var save = new GameSave
            {
                Players = gameController.Players.Select(p => new PlayerSave
                {
                    Name = p.Name,
                    Position = p.Position,
                    Color = p.Color,
                    EffectsTypes = p.Effects
                        .Select(e => e.GetType().Name)
                        .ToList()
                }).ToList(),

                CellSaves = gameController.GameBoard.Cells.Select(s => new CellSave
                {
                    Type = s.GetType().Name,
                }).ToList(),

                FinishedPlayers = gameController.FinishedPlayers
                    .Select(f => new FinishedPlayerSave
                    {
                        PlayerName = f.Player.Name,
                        FinishRound = f.FinishRound
                    }).ToList(),

                CurrentPlayerIndex = gameController.CurrentPlayerIndex,
                RoundMoveCounter = gameController.RoundMoveCounter,
                RoundActivePlayersCount = gameController.RoundActivePlayersCount,
                CurrentRound = gameController.CurrentRound,

                SizeBoard = gameController.GameBoard.Size,

            };
            SaveService.Save(save);
        }

        private void LoadGameFromFile()
        {
            var save = SaveService.Load();

            if (save == null)
                return;

            gameController.LoadGame(save);

            boardRenderer.UpdatePlayerMarkers(
                GameBoardPanel,
                gameController.GameBoard,
                gameController.Players
            );
            RenderGame(gameController.Players.Count, gameController.GameBoard.Size);
        }


        private void LoadGameButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadGameFromFile();
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }
    }
}