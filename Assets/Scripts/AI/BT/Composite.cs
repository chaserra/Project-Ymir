using System.Collections;
using System.Collections.Generic;

namespace Ymir.BT
{
    public abstract class Composite : Node
    {
        protected List<Node> _children = new List<Node>();
        protected int _currentChild = 0;

        /* Class Method */
        public void AddChild(Node n)
        {
            _children.Add(n);
        }

    }
}