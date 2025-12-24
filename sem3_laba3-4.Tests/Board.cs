using System;
using System.Linq;
using Xunit;
using WpfApp1;

namespace WpfApp1.Tests
{
    public class Board
    {
        private ICell[] _cells;
        public ICell[] Cells => _cells;
        public int Size => _cells.Length;

        public Board(int size)
        {
            _cells = new ICell[size];
        }

        public void PlaceCell(int index, ICell cell)
        {
            if (index >= 0 && index < _cells.Length)
            {
                _cells[index] = cell;
            }
        }
    }

    public class BoardTests
    {
        [Fact]
        public void Board_Creation_HasCorrectSize()
        {
            var board = new Board(5);

            Assert.Equal(5, board.Size);
            Assert.Equal(5, board.Cells.Length);
        }

        [Fact]
        public void Board_CanPlaceCell()
        {
            var board = new Board(5);
            var cell = new SimpleCell();

            board.PlaceCell(2, cell);

            Assert.Equal(cell, board.Cells[2]);
        }

        [Fact]
        public void Board_PlaceCell_InvalidIndex_DoesNothing()
        {
            var board = new Board(3);

            board.PlaceCell(-1, new SimpleCell());
            board.PlaceCell(3, new SimpleCell());

            Assert.All(board.Cells, c => Assert.Null(c));
        }

        [Fact]
        public void Board_CanPlaceDifferentCellTypes()
        {
            var board = new Board(4);

            var forward = new ForwardCell();
            var back = new BackCell();
            var skip = new SkipCell();

            board.PlaceCell(0, forward);
            board.PlaceCell(1, back);
            board.PlaceCell(2, skip);

            Assert.Equal(forward, board.Cells[0]);
            Assert.Equal(back, board.Cells[1]);
            Assert.Equal(skip, board.Cells[2]);

            Assert.Equal(2, board.Cells[0].Resolve().PositionDelta);
            Assert.Equal(-3, board.Cells[1].Resolve().PositionDelta);
            Assert.Single(board.Cells[2].Resolve().Effects);
        }
    }
}
