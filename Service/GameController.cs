using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfApp1.Services
{
    public class GameController
    {
        private Board gameBoard;
        private BoardGenerator boardGenerator = new BoardGenerator();
        private List<Player> players;
        private List<Player> finishedPlayers = new List<Player>();
        private Dice dice = new Dice();
        private int currentPlayerIndex = 0;
        private bool gameActive = false;
        private bool allPlayersMode = true;

        private static readonly Brush[] PlayerColors =
        {
            Brushes.Red,
            Brushes.Blue,
            Brushes.Green,
            Brushes.Orange
        };

        public bool IsGameActive => gameActive;
        public Player CurrentPlayer => GetNextActivePlayer();
        public Board GameBoard => gameBoard;
        public List<Player> Players => players;
        public List<Player> FinishedPlayers => finishedPlayers;
        public Dice GameDice => dice;
        public bool AllPlayersFinished => finishedPlayers.Count == players?.Count;

        public void StartNewGame(int boardSize = 40, int playerCount = 2)
        {
            if (playerCount < 2 || playerCount > 4)
                throw new ArgumentException("Количество игроков должно быть от 2 до 4");

            if (boardSize < 10 || boardSize > 100)
                throw new ArgumentException("Размер поля должен быть от 10 до 100 ячеек");

            finishedPlayers.Clear();

            gameBoard = new Board(boardSize);

            int simpleCells = CalculateSimpleCells(boardSize);
            int forwardCells = CalculateForwardCells(boardSize);
            int backCells = CalculateBackCells(boardSize);
            int skipCells = CalculateSkipCells(boardSize);

            boardGenerator.Generate(gameBoard,
                simpleCells,
                forwardCells,
                backCells,
                skipCells);

            players = new List<Player>();
            for (int i = 0; i < playerCount; i++)
            {
                players.Add(new Player($"Игрок {i + 1}", PlayerColors[i]));
            }
            currentPlayerIndex = 0;
            gameActive = true;
        }

        private int CalculateSimpleCells(int boardSize)
        {
            return (int)(boardSize * 0.6);
        }

        private int CalculateForwardCells(int boardSize)
        {
            return (int)(boardSize * 0.2);
        }

        private int CalculateBackCells(int boardSize)
        {
            return (int)(boardSize * 0.1);
        }

        private int CalculateSkipCells(int boardSize)
        {
            return (int)(boardSize * 0.1);
        }

        public bool MakeMove()
        {
            if (!gameActive || players == null)
                return false;

            var currentPlayer = GetNextActivePlayer();
            if (currentPlayer == null)
            {
                gameActive = false;
                return true;
            }

            dice.Roll();

            bool playerWon = currentPlayer.MakeMove(gameBoard.Cells, dice);

            if (playerWon)
            {
                currentPlayer.Position = gameBoard.Size - 1;
                if (!finishedPlayers.Contains(currentPlayer))
                {
                    finishedPlayers.Add(currentPlayer);
                }

                if (AllPlayersFinished)
                {
                    gameActive = false;
                    return true;
                }
            }

            MoveToNextActivePlayer();

            return false;
        }

        private Player GetNextActivePlayer()
        {
            if (players == null || players.Count == 0)
                return null;

            for (int i = 0; i < players.Count; i++)
            {
                int index = (currentPlayerIndex + i) % players.Count;
                var player = players[index];
                if (!finishedPlayers.Contains(player))
                {
                    return player;
                }
            }

            return null;
        }

        private void MoveToNextActivePlayer()
        {
            if (players == null || players.Count == 0)
                return;

            for (int i = 1; i <= players.Count; i++)
            {
                int nextIndex = (currentPlayerIndex + i) % players.Count;
                var nextPlayer = players[nextIndex];
                if (!finishedPlayers.Contains(nextPlayer))
                {
                    currentPlayerIndex = nextIndex;
                    return;
                }
            }
        }

        public List<string> GetResults()
        {
            var results = new List<string>();

            for (int i = 0; i < finishedPlayers.Count; i++)
            {
                results.Add($"{i + 1} место: {finishedPlayers[i].Name}");
            }

            var remainingPlayers = players.Where(p => !finishedPlayers.Contains(p)).ToList();
            foreach (var player in remainingPlayers)
            {
                results.Add($"В игре: {player.Name} (позиция: {player.Position + 1})");
            }

            return results;
        }

        public void EndGame()
        {
            gameActive = false;
        }

        public void ResetGame()
        {
            gameBoard = null;
            players = null;
            finishedPlayers.Clear();
            currentPlayerIndex = 0;
            gameActive = false;
        }
    }
}
