using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    public class BoardGenerator
    {
        private readonly Random _random = new Random();

        public void Generate(Board board, int simple, int forward, int back, int skip)
        {
            for (int i = 0; i < board.Size; i++)
            {
                board.Cells[i] = null;
            }

            board.PlaceCell(0, new SimpleCell());
            board.PlaceCell(board.Size - 1, new SimpleCell());

            var availableIndices = new List<int>();
            for (int i = 1; i < board.Size - 1; i++)
            {
                availableIndices.Add(i);
            }

            FillCells(board, forward, () => new ForwardCell(), availableIndices);
            FillCells(board, back, () => new BackCell(), availableIndices);
            FillCells(board, skip, () => new SkipCell(), availableIndices);
            FillCells(board, simple, () => new SimpleCell(), availableIndices);
        }

        private void FillCells(Board board, int count, Func<ICell> createCell, List<int> availableIndices)
        {
            for (int i = 0; i < count && availableIndices.Count > 0; i++)
            {
                int randomIndex = _random.Next(0, availableIndices.Count);
                int boardIndex = availableIndices[randomIndex];

                board.PlaceCell(boardIndex, createCell());
                availableIndices.RemoveAt(randomIndex);
            }
        }
    }
}