using System.Collections;
using System.Collections.Generic;

namespace QuaternionGames.BT
{
    public class Sequence : Composite
    {
        protected override void OnInitialize()
        {
            // Initialize
        }

        protected override Status OnUpdate()
        {
            Status childStatus = children[currentChild].Update();

            if (childStatus == Status.RUNNING) { return childStatus; }

            if (childStatus == Status.FAILURE) 
            {
                ResetChildren();
                return Status.FAILURE; 
            }

            currentChild++;

            if (currentChild >= children.Count)
            {
                return Status.SUCCESS;
            }

            return Status.RUNNING;
        }

        protected override void OnTerminate(Status s)
        {
            // Terminate
            currentChild = 0;
        }

    }
}