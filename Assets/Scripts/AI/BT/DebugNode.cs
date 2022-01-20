using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ymir.BT
{
    public class DebugNode : Leaf
    {
        public string message;

        protected override void OnInitialize()
        {
            Debug.Log($"OnInitialize{message}");
        }

        protected override Status OnUpdate()
        {
            Debug.Log($"OnUpdate{message}");
            return Status.SUCCESS;
        }

        protected override void OnTerminate(Status s)
        {
            Debug.Log($"OnStart{message}");
        }
    }
}