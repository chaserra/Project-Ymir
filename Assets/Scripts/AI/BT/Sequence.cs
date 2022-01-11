using System.Collections;
using System.Collections.Generic;

namespace Ymir.BT
{
    public class Sequence : Node
    {
        public Sequence()
        {
            name = "Sequence Node";
        }
        public Sequence(string n)
        {
            name = n;
        }

        protected override void OnInitialize()
        {
            // Initialize
        }

        public override Status Process()
        {
            Status childStatus = children[currentChild].Update();

            if (childStatus == Status.RUNNING) { return childStatus; }
            if (childStatus == Status.FAILURE) { return Status.FAILURE; }

            currentChild++;

            if (currentChild > children.Count)
            {
                return Status.SUCCESS;
            }

            return Status.RUNNING;
        }

        protected override void OnTerminate(Status s)
        {
            // Terminate
            if (s == Status.SUCCESS)
            {
                currentChild = 0;
            }
        }
    }
}