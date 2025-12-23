using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Players
{
    public static class PlayerEffectFactory
    {
        private static readonly Dictionary<string, Func<IPlayerEffect>> _effects
            = new Dictionary<string, Func<IPlayerEffect>>
        {
            { nameof(SkipTurnEffect), () => new SkipTurnEffect() },
        };

        public static IPlayerEffect Create(string key)
        {
            return _effects.TryGetValue(key, out var creator)
                ? creator()
                : null;
        }
    }
}
