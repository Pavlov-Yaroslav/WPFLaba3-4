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
    public class Player
    {
        public int Position { get; set; }
        public string Name { get; set; }
        public Brush Color { get; set; }

        public Player(string name, Brush color)
        {
            Position = 0;
            Name = name;
            Color = color;
        }

        public bool MakeMove(ICell[] cells, Dice dice)
        {
            dice.Roll();
            Position += dice.Edge;

            if (Position >= 0 && Position < cells.Length)
            {
                var cell = cells[Position];
                if (cell != null)
                {
                    Position += cell.Action();
                }

                if (Position < 0) Position = 0;
                if (Position >= cells.Length) return true;

                return false;
            }
            return true;
        }
    }
}