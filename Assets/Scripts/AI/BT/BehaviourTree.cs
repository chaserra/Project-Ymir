using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ymir.BT
{
    [CreateAssetMenu()]
    public class BehaviourTree : ScriptableObject
    {
        public Node rootNode;
        public Node.Status treeState = Node.Status.RUNNING;

        public Node.Status Update()
        {
            return rootNode.Update();
        }

    }
}