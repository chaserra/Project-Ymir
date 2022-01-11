using System.Collections;
using System.Collections.Generic;

namespace Ymir.BT
{
    public class Selector : Node
    {
        public Selector()
        {
            name = "Sequence Node";
        }
        public Selector(string n)
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
            if (childStatus == Status.SUCCESS) { return Status.SUCCESS; }

            currentChild++;

            if (currentChild > children.Count)
            {
                return Status.FAILURE;
            }

            return Status.RUNNING;
        }

        protected override void OnTerminate(Status s)
        {
            // Terminate
            // Maybe add here if the selector resets, then reset to 0
            currentChild = 0;
        }
    }
}