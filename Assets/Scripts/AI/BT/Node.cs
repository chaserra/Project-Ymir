using System.Collections.Generic;
using UnityEngine;

namespace QuaternionGames.BT
{
    public abstract class Node : ScriptableObject
    {
        public enum Status { SUCCESS, FAILURE, RUNNING };

        public Status status = Status.RUNNING;
        public string guid;

        private bool _started = false;

        protected abstract void OnInitialize();
        protected abstract Status OnUpdate();
        protected abstract void OnTerminate(Status s);

        // Tick method. Ensures that OnInitialize and OnTerminate are checked during Update
        public Status Update()
        {
            if (!_started) 
            { 
                OnInitialize();
                _started = true;
            }

            status = OnUpdate();

            if (status != Status.RUNNING) 
            { 
                OnTerminate(status);
                _started = false;
            }

            return status;
        }

    }
}