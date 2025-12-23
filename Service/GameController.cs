using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WpfApp1.GameCore;
using WpfApp1.Service;

namespace WpfApp1.Services
{
    public class GameController
    {
        private Board gameBoard;
        private BoardGenerator boardGenerator = new BoardGenerator();
        private List<Player> players;
        private List<FinishedPlayerInfo> finishedPlayers = new List<FinishedPlayerInfo>();
        private List<Player> finishedThisRound = new List<Player>();
        private Dice dice = new Dice();
        private int currentPlayerIndex = 0;
        private bool gameActive = false;
        private int roundMoveCounter = 0;
        private int roundActivePlayersCount = 0;
        private int currentRound = 1;


        private static readonly string[] PlayerColorsString =
        {
            "Red",
            "Blue",
            "Green",
            "Orange"
        };

        public bool IsGameActive => gameActive;
        public Player CurrentPlayer => GetNextActivePlayer();
        public Board GameBoard => gameBoard;
        public List<Player> Players => players;
        public List<FinishedPlayerInfo> FinishedPlayers => finishedPlayers;
        public Dice GameDice => dice;
        public bool AllPlayersFinished => finishedPlayers.Count == players?.Count;

        public void StartNewGame(int boardSize = 40, int playerCount = 2)
        {
            if (playerCount < 2 || playerCount > 4)
                throw new ArgumentException("Количество игроков должно быть от 2 до 4");

            if (boardSize < 10 || boardSize > 100)
                throw new ArgumentException("Размер поля должен быть от 10 до 100 ячеек");

            finishedPlayers.Clear();
            finishedThisRound.Clear();
            roundMoveCounter = 0;
            roundActivePlayersCount = 0;
            currentRound = 1;

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
                players.Add(new Player($"Игрок {i + 1}", PlayerColorsString[i]));
            }
            currentPlayerIndex = 0;
            gameActive = true;
        }

        public void ContinueGame(List<PlayerSave> loadPlayers)
        {
            int playerCount = loadPlayers.Count;
            int boardSize = 20;

            if (playerCount < 2 || playerCount > 4)
                throw new ArgumentException("Количество игроков должно быть от 2 до 4");

            if (boardSize < 10 || boardSize > 100)
                throw new ArgumentException("Размер поля должен быть от 10 до 100 ячеек");

            finishedPlayers.Clear();
            finishedThisRound.Clear();
            roundMoveCounter = 0;
            roundActivePlayersCount = 0;
            currentRound = 1;

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

            players = loadPlayers.Select(p => new Player(p.Name, p.Color)
            {
                Position = p.Position,
            }).ToList();

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

        public bool MakePlayerTurn()
        {
            var player = GetNextActivePlayer();
            if (player == null)
                return false;

            if (roundActivePlayersCount == 0)
            {
                roundActivePlayersCount = players.Count - finishedPlayers.Count;
                if (roundActivePlayersCount < 0) roundActivePlayersCount = 0;
            }

            dice.Roll();

            bool finished = MakePlayerMove(player);

            if (finished && !finishedPlayers.Any(f => f.Player == player))
            {
                var finishedInfo = new FinishedPlayerInfo
                {
                    Player = player,
                    FinishRound = currentRound
                };
                finishedPlayers.Add(finishedInfo);
                finishedThisRound.Add(player);
            }

            MoveToNextActivePlayer();
            roundMoveCounter++;

            if (IsRoundFinished())
            {
                EndRound();
            }

            return finished;
        }


        public bool IsRoundFinished()
        {
            return roundActivePlayersCount > 0 && roundMoveCounter >= roundActivePlayersCount;
        }

        public void EndRound()
        {
            finishedThisRound.Clear();
            roundMoveCounter = 0;
            currentRound++;
            roundActivePlayersCount = players.Count - finishedPlayers.Count;
            if (roundActivePlayersCount < 0) roundActivePlayersCount = 0;
        }

        private bool MakePlayerMove(Player player)
        {
            if (!player.CanMove()) return false;

            player.MoveBy(dice.Edge);

            if (player.Position >= gameBoard.Size)
                return true;

            var cell = gameBoard.Cells[player.Position];
            var result = cell?.Resolve();
            if (result != null)
                player.ApplyCellResult(result);

            return player.Position >= gameBoard.Size;
        }



        private Player GetNextActivePlayer()
        {
            if (players == null || players.Count == 0) return null;

            for (int i = 0; i < players.Count; i++)
            {
                int index = (currentPlayerIndex + i) % players.Count;
                var player = players[index];
                if (!finishedPlayers.Any(f => f.Player == player))
                    return player;
            }

            return null;
        }

        private void MoveToNextActivePlayer()
        {
            if (players == null || players.Count == 0) return;

            for (int i = 1; i <= players.Count; i++)
            {
                int nextIndex = (currentPlayerIndex + i) % players.Count;
                var nextPlayer = players[nextIndex];
                if (!finishedPlayers.Any(f => f.Player == nextPlayer))
                {
                    currentPlayerIndex = nextIndex;
                    return;
                }
            }
        }


        public List<string> GetResults()
        {
            var results = new List<string>();

            var groupedByRound = finishedPlayers
                .OrderBy(f => f.FinishRound)
                .GroupBy(f => f.FinishRound)
                .ToList();

            int place = 1;

            foreach (var group in groupedByRound)
            {
                foreach (var info in group)
                {
                    results.Add($"{place} место: {info.Player.Name}");
                }
                place += group.Count();
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
            finishedThisRound.Clear();
            currentPlayerIndex = 0;
            roundMoveCounter = 0;
            roundActivePlayersCount = 0;
            currentRound = 1;
            gameActive = false;
        }
    }
}