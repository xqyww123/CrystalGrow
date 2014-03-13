using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystallGrow
{
    struct Pair<T1, T2>
    {
        public Pair(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
        public T1 Item1;
        public T2 Item2;
    }
}

