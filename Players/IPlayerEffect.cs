using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Players
{
    public interface IPlayerEffect
    {
        bool CanPlayerMove(Player player);
        void OnTurnPassed();
        bool IsExpired { get; }
    }

}
