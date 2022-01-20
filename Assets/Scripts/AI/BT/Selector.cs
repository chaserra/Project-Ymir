using System.Collections;
using System.Collections.Generic;

namespace Ymir.BT
{
    public class Selector : Composite
    {
        protected override void OnInitialize()
        {
            // Initialize
        }

        protected override Status OnUpdate()
        {
            Status childStatus = children[currentChild].Update();

            if (childStatus == Status.RUNNING) { return childStatus; }

            if (childStatus == Status.SUCCESS) 
            { 
                currentChild = 0;
                return Status.SUCCESS; 
            }

            currentChild++;

            if (currentChild >= children.Count)
            {
                currentChild = 0;
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