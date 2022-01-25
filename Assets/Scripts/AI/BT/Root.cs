using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuaternionGames.BT
{
    public class Root : Node
    {
        public Node child;

        protected override void OnInitialize()
        {

        }

        protected override Status OnUpdate()
        {
            return child.Update();
        }

        protected override void OnTerminate(Status s)
        {

        }

        public override Node Clone()
        {
            Root node = Instantiate(this);
            node.child = child.Clone();
            return node;
        }

    }
}