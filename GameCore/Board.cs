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
}