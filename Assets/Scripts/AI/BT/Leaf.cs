using System.Collections;
using System.Collections.Generic;

namespace Ymir.BT
{
    public class Leaf : Node
    {
        public delegate Status Tick();
        public Tick Task;

        public Leaf(string n, Tick t)
        {
            _name = n;
            Task = t;
        }

        protected override void OnInitialize()
        {
            // Initialize
        }

        public override Status Process()
        {
            if (Task != null)
            {
                return Task();
            }
            return Status.FAILURE;
        }

        protected override void OnTerminate(Status s)
        {
            // Terminate
        }

    }
}