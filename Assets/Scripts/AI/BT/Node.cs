using System.Collections.Generic;
using UnityEngine;

namespace QuaternionGames.BT
{
    public abstract class Node : ScriptableObject
    {
        public enum Status { SUCCESS, FAILURE, RUNNING };

        [HideInInspector] public Status status = Status.RUNNING;
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;

        private bool _started = false;
        public bool Started { get { return _started; } }

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

        public virtual Node Clone()
        {
            return Instantiate(this);
        }

    }
}