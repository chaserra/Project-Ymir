using System.Collections;
using System.Collections.Generic;

namespace Ymir.BT
{
    public class BehaviourTree : Node
    {
        protected List<Node> _children = new List<Node>();
        protected int _currentChild = 0;

        public BehaviourTree () 
        {
            _name = "Behaviour Tree";
        }

        public BehaviourTree(string n)
        {
            _name = n;
        }

        protected override void OnInitialize()
        {
            // Initialize
        }

        public override Status Process()
        {
            return _children[_currentChild].Update();
        }

        protected override void OnTerminate(Status s)
        {
            // Terminate
        }

        /* Class Method */
        public void AddChild(Node n)
        {
            _children.Add(n);
        }

    }
}