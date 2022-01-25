using System.Collections;
using System.Collections.Generic;

namespace QuaternionGames.BT
{
    public abstract class Decorator : Node
    {
        public Node child;

        public void SetChild(Node n)
        {
            child = n;
        }

        public override Node Clone()
        {
            Decorator node = Instantiate(this);
            node.child = child.Clone();
            return node;
        }
    }
}