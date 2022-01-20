using System.Collections;
using System.Collections.Generic;

namespace Ymir.BT
{
    public abstract class Decorator : Node
    {
        public Node child;

        public void SetChild(Node n)
        {
            child = n;
        }
    }
}