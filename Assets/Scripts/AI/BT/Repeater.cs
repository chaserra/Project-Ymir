using System.Collections;
using System.Collections.Generic;

namespace QuaternionGames.BT
{
    public class Repeater : Decorator
    {
        private int _loopCounter = 0;

        public int numLoop;

        protected override void OnInitialize()
        {
            // Initialize
            if (numLoop <= 0)
            {
                numLoop = 5;
            }
        }

        protected override Status OnUpdate()
        {
            // TODO [HIGH] : Loop on ONE FRAME (?? not sure)
            if (_loopCounter < numLoop)
            {
                child.Update();

                // If child is still running, don't iterate loop.
                if (child.status == Status.RUNNING) { return Status.RUNNING; }

                // If child fails, stop and return failure
                if (child.status == Status.FAILURE) { return Status.FAILURE; }

                _loopCounter++;
                return Status.RUNNING;
            }
            else
            {
                // Loops completed successfully
                return Status.SUCCESS;
            }
        }

        protected override void OnTerminate(Status s)
        {
            // Terminate
            _loopCounter = 0;
        }
    }
}