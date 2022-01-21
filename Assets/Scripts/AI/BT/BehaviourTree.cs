using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace QuaternionGames.BT
{
    [CreateAssetMenu()]
    public class BehaviourTree : ScriptableObject
    {
        public Node rootNode;
        public Node.Status treeStatus = Node.Status.RUNNING;
        public List<Node> nodes = new List<Node>();

        public Node.Status Update()
        {
            if (rootNode.status == Node.Status.RUNNING)
            {
                treeStatus = rootNode.Update();
            }
            //Debug.Log($"Root Node Status: {rootNode.status}");
            return treeStatus;
        }

        public Node CreateNode<T>(System.Type type) where T : Node
        {
            Node node = ScriptableObject.CreateInstance(type) as Node;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();
            nodes.Add(node);

            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();

            return node;
        }

        public void DeleteNode(Node node)
        {
            nodes.Remove(node);
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
        }

    }
}