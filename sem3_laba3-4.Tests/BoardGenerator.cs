using System.Linq;
using Xunit;
using WpfApp1.GameCore;

namespace WpfApp1.Tests
{
    public class BoardGenerator
    {
        private readonly Random _random = new Random();

        public void Generate(Board board, int simple, int forward, int back, int skip)
        {
            for (int i = 0; i < board.Size; i++)
                board.Cells[i] = null;

            board.PlaceCell(0, new SimpleCell());
            board.PlaceCell(board.Size - 1, new SimpleCell());

            var availableIndices = Enumerable.Range(1, board.Size - 2).ToList();

            FillCells(board, forward, () => new ForwardCell(), availableIndices.Where(i => i < board.Size - 2).ToList());
            FillCells(board, back, () => new BackCell(), availableIndices.Where(i => i > 2).ToList());

            availableIndices = availableIndices.Where(i => board.Cells[i] == null).ToList();
            FillCells(board, simple, () => new SimpleCell(), availableIndices);
            availableIndices = availableIndices.Where(i => board.Cells[i] == null).ToList();
            FillCells(board, skip, () => new SkipCell(), availableIndices);
        }


        private void FillCells(Board board, int count, Func<ICell> createCell, List<int> indices)
        {
            var rnd = new Random();
            for (int i = 0; i < count && indices.Count > 0; i++)
            {
                int idx = rnd.Next(indices.Count);
                board.PlaceCell(indices[idx], createCell());
                indices.RemoveAt(idx);
            }
        }
    }

    public class BoardGeneratorTests
    {
        [Fact]
        public void Generate_Board_HasFirstAndLastSimpleCells()
        {
            var board = new Board(10);
            var generator = new BoardGenerator();

            generator.Generate(board, simple: 3, forward: 2, back: 2, skip: 1);

            Assert.IsType<SimpleCell>(board.Cells[0]);
            Assert.IsType<SimpleCell>(board.Cells[board.Size - 1]);
        }

    }
}
