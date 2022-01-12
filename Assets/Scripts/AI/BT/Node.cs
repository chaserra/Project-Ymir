using System.Collections.Generic;

namespace Ymir.BT
{
    public abstract class Node
    {
        public enum Status { SUCCESS, FAILURE, RUNNING };

        protected Status _status;
        protected List<Node> _children = new List<Node>();
        protected int _currentChild = 0;
        protected string _name;

        /* Constructor */
        public Node() { }
        public Node(string n)
        {
            _name = n;
        }

        /* Interface */
        protected abstract void OnInitialize();
        protected abstract void OnTerminate(Status s);

        /* Main Process */
        public virtual Status Process()
        {
            return _children[_currentChild].Update();
        }

        // Tick method. Ensures that OnInitialize and OnTerminate are checked during Process method
        public Status Update()
        {
            if (_status != Status.RUNNING) { OnInitialize(); }
            _status = Process();
            if (_status != Status.RUNNING) { OnTerminate(_status); }
            return _status;
        }

        /* Class Method */
        public void AddChild(Node n)
        {
            _children.Add(n);
        }

    }
}