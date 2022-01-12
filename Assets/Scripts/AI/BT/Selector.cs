using System.Collections;
using System.Collections.Generic;

namespace Ymir.BT
{
    public class Selector : Node
    {
        public Selector()
        {
            _name = "Selector Node";
        }
        public Selector(string n)
        {
            _name = n;
        }

        protected override void OnInitialize()
        {
            // Initialize
        }

        public override Status Process()
        {
            Status childStatus = _children[_currentChild].Update();

            if (childStatus == Status.RUNNING) { return childStatus; }
            if (childStatus == Status.SUCCESS) { return Status.SUCCESS; }

            _currentChild++;

            if (_currentChild > _children.Count)
            {
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