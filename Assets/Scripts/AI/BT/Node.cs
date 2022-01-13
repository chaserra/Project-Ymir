using System.Collections.Generic;

namespace Ymir.BT
{
    public abstract class Node
    {
        public enum Status { SUCCESS, FAILURE, RUNNING };

        protected Status _status;
        protected string _name;

        protected abstract void OnInitialize();
        public abstract Status Process();
        protected abstract void OnTerminate(Status s);

        // TODO [BT]: Create a delegated task for doing stuff on OnInitialize and OnTerminate
        // [Cont] assign these Init and Terminate tasks on Node instantiation via constructors
        // [Cont] make this as simple as possible (ie. tasks like getting a reference to the blackboard, etc)

        // Tick method. Ensures that OnInitialize and OnTerminate are checked during Process method
        public Status Update()
        {
            if (_status != Status.RUNNING) { OnInitialize(); }
            _status = Process();
            if (_status != Status.RUNNING) { OnTerminate(_status); }
            return _status;
        }

    }
}