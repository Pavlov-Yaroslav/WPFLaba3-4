using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Cells;
using WpfApp1.Players;

namespace WpfApp1
{
    public interface ICell
    {
        CellEffectResult Resolve();
    }

    public class SimpleCell : ICell
    {
        public CellEffectResult Resolve()
        {
            return new CellEffectResult
            {
                PositionDelta = 0
            };
        }
    }

    public class ForwardCell : ICell
    {
        public CellEffectResult Resolve()
        {
            return new CellEffectResult
            {
                PositionDelta = 2
            };
        }
    }

    public class BackCell : ICell
    {
        public CellEffectResult Resolve()
        {
            return new CellEffectResult
            {
                PositionDelta = -3
            };
        }
    }

    public class SkipCell : ICell
    {
        public CellEffectResult Resolve()
        {
            var result = new CellEffectResult();
            result.Effects.Add(new SkipTurnEffect(1));
            return result;
        }
    }
}
