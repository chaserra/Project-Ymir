using System.Collections.Generic;

namespace Ymir.BT
{
    public abstract class Node
    {
        public enum Status { SUCCESS, FAILURE, RUNNING };

        public Status status;
        public List<Node> children = new List<Node>();
        public int currentChild = 0;
        public string name;

        /* Constructor */
        public Node() { }
        public Node(string n)
        {
            name = n;
        }

        /* Interface */
        protected abstract void OnInitialize();
        public Status Update()
        {
            if (status != Status.RUNNING) { OnInitialize(); }
            status = Process();
            if (status != Status.RUNNING) { OnTerminate(status); }
            return status;
        }
        protected abstract void OnTerminate(Status s);

        /* Main Process */
        public virtual Status Process()
        {
            return children[currentChild].Update();
        }

        /* Class Method */
        public void AddChild(Node n)
        {
            children.Add(n);
        }

    }
}