using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Players;

namespace WpfApp1.Cells
{
    public class CellEffectResult
    {
        public int PositionDelta { get; set; }
        public List<IPlayerEffect> Effects { get; } = new List<IPlayerEffect>();
    }

}
