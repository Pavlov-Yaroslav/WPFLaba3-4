using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public interface ICell
    {
        int Action();
    }

    public class SimpleCell : ICell
    {
        public int Action() => 0;
    }

    public class ForwardCell : ICell
    {
        public int Action() => 2;
    }

    public class BackCell : ICell
    {
        public int Action() => -3;
    }

    public class SkipCell : ICell
    {
        public int Action() => 0;
    }
}
