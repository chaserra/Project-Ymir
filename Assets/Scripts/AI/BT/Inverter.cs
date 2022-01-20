using System.Collections;
using System.Collections.Generic;

namespace Ymir.BT
{
    public class Inverter : Decorator
    {
        public Inverter()
        {
            nodeName = "Inverter Node";
        }
        public Inverter(string n)
        {
            nodeName = n;
        }

        protected override void OnInitialize()
        {
            // Initialize
        }

        protected override Status OnUpdate()
        {
            Status childStatus = _child.Update();

            if (childStatus == Status.RUNNING) { return Status.RUNNING; }
            else if (childStatus == Status.FAILURE) { return Status.SUCCESS; }
            else { return Status.FAILURE; }
        }

        protected override void OnTerminate(Status s)
        {
            // Terminate
        }

    }
}