using System.Collections;
using System.Collections.Generic;

namespace Ymir.BT
{
    public class Sequence : Composite
    {
        public Sequence()
        {
            nodeName = "Sequence Node";
        }
        public Sequence(string n)
        {
            nodeName = n;
        }

        protected override void OnInitialize()
        {
            // Initialize
        }

        protected override Status OnUpdate()
        {
            Status childStatus = _children[_currentChild].Update();

            if (childStatus == Status.RUNNING) { return childStatus; }

            if (childStatus == Status.FAILURE) 
            {
                _currentChild = 0;
                ResetChildren();
                return Status.FAILURE; 
            }

            _currentChild++;

            if (_currentChild >= _children.Count)
            {
                _currentChild = 0;
                return Status.SUCCESS;
            }

            return Status.RUNNING;
        }

        protected override void OnTerminate(Status s)
        {
            // Terminate
            if (s == Status.SUCCESS)
            {
                _currentChild = 0;
            }
        }

    }
}