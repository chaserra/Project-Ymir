using System.Collections;
using System.Collections.Generic;

namespace Ymir.BT
{
    public abstract class Decorator : Node
    {
        protected Node _child;

        /* Class Method */
        public void AddChild(Node n)
        {
            _child = n;
        }

    }
}