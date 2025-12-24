using WpfApp1;
using WpfApp1.Players;
using Xunit;

namespace WpfApp1.Tests
{
    public class CellEffectResult
    {
        public int PositionDelta { get; set; } = 0;
        public List<object> Effects { get; set; } = new List<object>();
    }

    public class SkipTurnEffect
    {
        public int Turns { get; }
        public SkipTurnEffect(int turns) => Turns = turns;
    }

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

    public class CellsTests
    {
        [Fact]
        public void SimpleCell_Resolve_ReturnsZeroDeltaAndNoEffects()
        {
            var cell = new SimpleCell();
            var result = cell.Resolve();

            Assert.Equal(0, result.PositionDelta);
            Assert.Empty(result.Effects);
        }

        [Fact]
        public void ForwardCell_Resolve_ReturnsPlusTwoDelta()
        {
            var cell = new ForwardCell();
            var result = cell.Resolve();

            Assert.Equal(2, result.PositionDelta);
            Assert.Empty(result.Effects);
        }

        [Fact]
        public void BackCell_Resolve_ReturnsMinusThreeDelta()
        {
            var cell = new BackCell();
            var result = cell.Resolve();

            Assert.Equal(-3, result.PositionDelta);
            Assert.Empty(result.Effects);
        }

        [Fact]
        public void SkipCell_Resolve_ReturnsSkipTurnEffect()
        {
            var cell = new SkipCell();
            var result = cell.Resolve();

            Assert.Equal(0, result.PositionDelta);
            Assert.Single(result.Effects);
            Assert.IsType<SkipTurnEffect>(result.Effects[0]);
        }
    }
}
