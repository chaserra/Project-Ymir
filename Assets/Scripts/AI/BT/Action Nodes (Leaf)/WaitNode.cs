using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ymir.BT
{
    public class WaitNode : Leaf
    {
        private float _startTime;

        public float duration = 1f;

        protected override void OnInitialize()
        {
            // Initialize
            _startTime = Time.time;
        }

        protected override Status OnUpdate()
        {
            if (Time.time - _startTime > duration)
            {
                return Status.SUCCESS;
            }
            return Status.RUNNING;
        }

        protected override void OnTerminate(Status s)
        {
            // Terminate
        }

    }
}