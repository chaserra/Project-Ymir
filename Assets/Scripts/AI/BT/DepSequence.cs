using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ymir.BT
{
    public class DepSequence : Sequence
    {
        BehaviourTree _dependency;

        public DepSequence(BehaviourTree d)
        {
            nodeName = "Dependency Sequence";
            _dependency = d;
        }
        public DepSequence(string n, BehaviourTree d)
        {
            nodeName = n;
            _dependency = d;
        }

        protected override Status OnUpdate()
        {
            // Check dependencies
            Status dependencyStatus = _dependency.Update();

            if (dependencyStatus == Status.FAILURE)
            {
                _currentChild = 0;
                ResetChildren();
                return Status.FAILURE;
            }

            // Prevent main sequence from running until all dependency nodes are checked
            if (dependencyStatus == Status.RUNNING) { return Status.RUNNING; }

            // Only proceed with main Process sequence if all dependencies return SUCCESS
            return base.OnUpdate();
        }

    }
}