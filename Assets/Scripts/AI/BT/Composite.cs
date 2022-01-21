using System.Collections;
using System.Collections.Generic;

namespace QuaternionGames.BT
{
    public abstract class Composite : Node
    {
        public List<Node> children = new List<Node>();
        protected int currentChild = 0;

        /* Class Method */
        public void AddChild(Node n)
        {
            children.Add(n);
        }

        public void ResetChildren()
        {
            foreach (Composite n in children)
            {
                n.ResetChildren();
                currentChild = 0;
            }
        }

    }
}