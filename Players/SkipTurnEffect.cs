using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Players
{
    public class SkipTurnEffect : IPlayerEffect
    {
        private int turnsLeft;
        public bool IsExpired => turnsLeft <= 0;

        public SkipTurnEffect(int turns = 1)
        {
            turnsLeft = turns;
        }

        public bool CanPlayerMove(Player player)
        {
            return false;
        }

        public void OnTurnPassed()
        {
            turnsLeft--;
        }
    }
}
