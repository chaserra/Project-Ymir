using System.Collections;
using System.Collections.Generic;

namespace QuaternionGames.BT
{
    public class Inverter : Decorator
    {
        protected override void OnInitialize()
        {
            // Initialize
        }

        protected override Status OnUpdate()
        {
            Status childStatus = child.Update();

            switch(childStatus)
            {
                case Status.RUNNING:
                    return Status.RUNNING;
                case Status.SUCCESS:
                    return Status.FAILURE;
                case Status.FAILURE:
                    return Status.SUCCESS;
                default:
                    return Status.RUNNING;
            }
        }

        protected override void OnTerminate(Status s)
        {
            // Terminate
        }

    }
}