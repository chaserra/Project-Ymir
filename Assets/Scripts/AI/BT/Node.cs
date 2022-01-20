using System.Collections.Generic;
using UnityEngine;

namespace Ymir.BT
{
    public abstract class Node : ScriptableObject
    {
        public enum Status { SUCCESS, FAILURE, RUNNING };

        public Status status = Status.RUNNING;
        public string nodeName;
        public bool started = false;

        protected abstract void OnInitialize();
        protected abstract Status OnUpdate();
        protected abstract void OnTerminate(Status s);

        // TODO [BT]: Create a delegated task for doing stuff on OnInitialize and OnTerminate
        // [Cont] assign these Init and Terminate tasks on Node instantiation via constructors
        // [Cont] make this as simple as possible (ie. tasks like getting a reference to the blackboard, etc)

        // Tick method. Ensures that OnInitialize and OnTerminate are checked during Process method
        public Status Update()
        {
            if (!started) 
            { 
                OnInitialize();
                started = true;
            }

            status = OnUpdate();

            if (status != Status.RUNNING) 
            { 
                OnTerminate(status);
                started = false;
            }

            return status;
        }

        // TODO [BT MED]: Add a Parallel composite node (new script).
        // [Cont] Checks children status, tallies up all success and failures, then acts according to
        // [Cont] it's own policy (RequireOne or RequireAll)

    }
}