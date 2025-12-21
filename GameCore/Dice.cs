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
    public class Dice
    {
        public int Edge { get; private set; }
        private readonly Random _rand = new Random();

        public void Roll()
        {
            Edge = _rand.Next(1, 7);
        }
    }
}
