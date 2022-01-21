using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuaternionGames.BT
{
    [CreateAssetMenu()]
    public class BehaviourTree : ScriptableObject
    {
        public Node rootNode;
        public Node.Status treeStatus = Node.Status.RUNNING;

        public Node.Status Update()
        {
            if (rootNode.status == Node.Status.RUNNING)
            {
                treeStatus = rootNode.Update();
            }
            //Debug.Log($"Root Node Status: {rootNode.status}");
            return treeStatus;
        }

    }
}