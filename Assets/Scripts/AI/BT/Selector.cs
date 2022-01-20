using System.Collections;
using System.Collections.Generic;

namespace Ymir.BT
{
    public class Selector : Composite
    {
        public Selector()
        {
            nodeName = "Selector Node";
        }
        public Selector(string n)
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

            if (childStatus == Status.SUCCESS) 
            { 
                _currentChild = 0;
                return Status.SUCCESS; 
            }

            _currentChild++;

            if (_currentChild >= _children.Count)
            {
                _currentChild = 0;
                return Status.FAILURE;
            }

            return Status.RUNNING;
        }

        protected override void OnTerminate(Status s)
        {
            // Terminate
            // Maybe add here if the selector resets, then reset to 0
            _currentChild = 0;
        }

    }
}