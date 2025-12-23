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
    }
    public class CellSave
    {
        public string Type { get; set; }
    }

    public class GameSave
    {
        public List<PlayerSave> Players { get; set; }
        public List<CellSave> CellSaves { get; set; }
        public List<PlayerSave> FinishPlayers { get; set; }
    }

}
