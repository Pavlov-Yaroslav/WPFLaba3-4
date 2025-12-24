using System;
using System.Collections.Generic;
using System.Linq;
using WpfApp1.GameCore;
using WpfApp1.Players;
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
        private bool gameActive = false;

        public int CurrentPlayerIndex { get; set; } = 0;
        public int RoundMoveCounter { get; set; } = 0;
        public int RoundActivePlayersCount { get; set; } = 0;
        public int CurrentRound { get; set; } = 1;

        private static readonly string[] PlayerColorsString = { "Red", "Blue", "Green", "Orange" };

        private static readonly Dictionary<string, Func<ICell>> CellFactory = new Dictionary<string, Func<ICell>>
        {
            { nameof(SimpleCell), () => new SimpleCell() },
            { nameof(ForwardCell), () => new ForwardCell() },
            { nameof(BackCell), () => new BackCell() },
            { nameof(SkipCell), () => new SkipCell() },
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
            if (playerCount < 2 || playerCount > 4) throw new ArgumentException("Количество игроков должно быть от 2 до 4");
            if (boardSize < 10 || boardSize > 100) throw new ArgumentException("Размер поля должен быть от 10 до 100 ячеек");

            finishedPlayers.Clear();
            finishedThisRound.Clear();
            CurrentPlayerIndex = 0;
            RoundMoveCounter = 0;
            RoundActivePlayersCount = 0;
            CurrentRound = 1;

            gameBoard = new Board(boardSize);

            int simpleCells = CalculateSimpleCells(boardSize);
            int forwardCells = CalculateForwardCells(boardSize);
            int backCells = CalculateBackCells(boardSize);
            int skipCells = CalculateSkipCells(boardSize);

            boardGenerator.Generate(gameBoard, simpleCells, forwardCells, backCells, skipCells);

            players = new List<Player>();
            for (int i = 0; i < playerCount; i++)
                players.Add(new Player($"Игрок {i + 1}", PlayerColorsString[i]));

            gameActive = true;
        }

        public void LoadGame(GameSave save)
        {
            if (save == null) throw new ArgumentNullException(nameof(save));

            finishedPlayers.Clear();
            finishedThisRound.Clear();

            gameBoard = new Board(save.SizeBoard);
            for (int i = 0; i < save.CellSaves.Count; i++)
            {
                var type = save.CellSaves[i].Type;
                gameBoard.Cells[i] = CellFactory.TryGetValue(type, out var constructor) ? constructor() : null;
            }

            players = save.Players.Select(p => new Player(p.Name, p.Color) { Position = p.Position }).ToList();

            foreach (var pSave in save.Players)
            {
                var player = players.First(p => p.Name == pSave.Name);
                foreach (var effectType in pSave.EffectsTypes)
                {
                    var effect = PlayerEffectFactory.Create(effectType);
                    if (effect != null) player.AddEffect(effect);
                }
            }

            foreach (var f in save.FinishedPlayers)
            {
                var player = players.First(p => p.Name == f.PlayerName);
                finishedPlayers.Add(new FinishedPlayerInfo { Player = player, FinishRound = f.FinishRound });
            }

            RoundMoveCounter = save.RoundMoveCounter;
            RoundActivePlayersCount = save.RoundActivePlayersCount;
            CurrentRound = save.CurrentRound;

            CurrentPlayerIndex = save.CurrentPlayerIndex;
            var nextActive = GetNextActivePlayer();
            if (nextActive != null)
                CurrentPlayerIndex = players.IndexOf(nextActive);

            gameActive = true;
        }


        private int CalculateSimpleCells(int boardSize) => (int)(boardSize * 0.6);
        private int CalculateForwardCells(int boardSize) => (int)(boardSize * 0.2);
        private int CalculateBackCells(int boardSize) => (int)(boardSize * 0.1);
        private int CalculateSkipCells(int boardSize) => (int)(boardSize * 0.1);

        public bool MakePlayerTurn(out List<Player> newlyFinishedPlayers)
        {
            newlyFinishedPlayers = new List<Player>();
            var player = GetNextActivePlayer();
            if (player == null) return false;

            if (RoundActivePlayersCount == 0)
                RoundActivePlayersCount = players.Count - finishedPlayers.Count;

            dice.Roll();
            bool finished = MakePlayerMove(player);

            if (finished && !finishedPlayers.Any(f => f.Player == player))
            {
                finishedThisRound.Add(player);
                newlyFinishedPlayers.Add(player);
            }

            MoveToNextActivePlayer();
            RoundMoveCounter++;

            if (IsRoundFinished())
                EndRound();

            return finished;
        }

        public bool IsRoundFinished() => RoundActivePlayersCount > 0 && RoundMoveCounter >= RoundActivePlayersCount;

        private void EndRound()
        {
            foreach (var p in finishedThisRound)
            {
                if (!finishedPlayers.Any(f => f.Player == p))
                    finishedPlayers.Add(new FinishedPlayerInfo { Player = p, FinishRound = CurrentRound });
            }

            finishedThisRound.Clear();
            RoundMoveCounter = 0;
            RoundActivePlayersCount = players.Count - finishedPlayers.Count;
            if (RoundActivePlayersCount < 0) RoundActivePlayersCount = 0;
            CurrentRound++;
        }


        private bool MakePlayerMove(Player player)
        {
            if (!player.CanMove())
            {
                player.CanMove();
                return false;
            }

            player.MoveBy(dice.Edge);

            if (player.Position >= gameBoard.Size)
            {
                player.Position = gameBoard.Size - 1;
                return true;
            }

            if (player.Position < 0)
            {
                player.Position = 0;
                return false;
            }

            var cell = gameBoard.Cells[player.Position];
            if (cell != null)
            {
                var result = cell.Resolve();

                player.Position += result.PositionDelta;

                if (player.Position < 0)
                    player.Position = 0;
                if (player.Position >= gameBoard.Size)
                {
                    player.Position = gameBoard.Size - 1;
                    return true;
                }

                foreach (var effect in result.Effects)
                {
                    player.AddEffect(effect);
                }
            }

            return player.Position >= gameBoard.Size;
        }



        private Player GetNextActivePlayer()
        {
            if (players == null || players.Count == 0) return null;
            for (int i = 0; i < players.Count; i++)
            {
                int index = (CurrentPlayerIndex + i) % players.Count;
                var player = players[index];
                if (!finishedPlayers.Any(f => f.Player == player)) return player;
            }
            return null;
        }

        private void MoveToNextActivePlayer()
        {
            if (players == null || players.Count == 0) return;

            for (int i = 1; i <= players.Count; i++)
            {
                int nextIndex = (CurrentPlayerIndex + i) % players.Count;
                var nextPlayer = players[nextIndex];
                if (!finishedPlayers.Any(f => f.Player == nextPlayer))
                {
                    CurrentPlayerIndex = nextIndex;
                    return;
                }
            }
        }

        public List<string> GetResults()
        {
            var results = new List<string>();
            var groupedByRound = finishedPlayers
                .OrderBy(f => f.FinishRound)
                .GroupBy(f => f.FinishRound);

            int place = 1;
            foreach (var group in groupedByRound)
            {
                foreach (var info in group)
                    results.Add($"{place} место: {info.Player.Name}");
                place += group.Count();
            }

            return results;
        }

        public void EndGame() => gameActive = false;

        public void ResetGame()
        {
            gameBoard = null;
            players = null;
            finishedPlayers.Clear();
            finishedThisRound.Clear();
            CurrentPlayerIndex = 0;
            RoundMoveCounter = 0;
            RoundActivePlayersCount = 0;
            CurrentRound = 1;
            gameActive = false;
        }
    }
}
