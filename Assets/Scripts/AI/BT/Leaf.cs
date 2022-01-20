using System.Collections;
using System.Collections.Generic;

namespace Ymir.BT
{
    public abstract class Leaf : Node
    {
        public delegate Status Tick();
        public Tick Task;

    }
}