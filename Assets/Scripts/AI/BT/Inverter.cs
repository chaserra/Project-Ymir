using System.Collections;
using System.Collections.Generic;

namespace Ymir.BT
{
    public class Inverter : Decorator
    {
        public Inverter()
        {
            _name = "Inverter Node";
        }
        public Inverter(string n)
        {
            _name = n;
        }

        protected override void OnInitialize()
        {
            // Initialize
        }

        public override Status Process()
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