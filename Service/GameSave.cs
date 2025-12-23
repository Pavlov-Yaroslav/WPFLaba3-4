using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WpfApp1.GameCore
{
    public class PlayerSave
    {
        public string Name { get; set; }
        public int Position { get; set; }
        public string Color { get; set; }
        public List<string> EffectsTypes { get; set; } = new List<string>();
    }

    public class CellSave
    {
        public string Type { get; set; }
    }

    public class FinishedPlayerSave
    {
        public string PlayerName { get; set; }

        public int FinishRound { get; set; }
    }

    public class GameSave
    {
        public List<PlayerSave> Players { get; set; }
        public List<CellSave> CellSaves { get; set; }
        public List<FinishedPlayerSave> FinishedPlayers { get; set; }
        public int SizeBoard { get; set; }

        public int CurrentPlayerIndex { get; set; }
        public int RoundMoveCounter { get; set; }
        public int RoundActivePlayersCount { get; set; }
        public int CurrentRound { get; set; }
    }

}