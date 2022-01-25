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

        public void AddChild(Node parent, Node child)
        {
            Root root = parent as Root;
            if (root)
            {
                root.child = child;
            }

            Decorator decorator = parent as Decorator;
            if (decorator)
            {
                decorator.child = child;
            }

            Composite composite = parent as Composite;
            if (composite)
            {
                composite.children.Add(child);
            }
        }

        public void RemoveChild(Node parent, Node child)
        {
            Root root = parent as Root;
            if (root)
            {
                root.child = null;
            }

            Decorator decorator = parent as Decorator;
            if (decorator)
            {
                decorator.child = null;
            }

            Composite composite = parent as Composite;
            if (composite)
            {
                composite.children.Remove(child);
            }
        }

        public List<Node> GetChildren(Node parent)
        {
            List<Node> children = new List<Node>();

            Root root = parent as Root;
            if (root && root.child != null)
            {
                children.Add(root.child);
            }

            Decorator decorator = parent as Decorator;
            if (decorator && decorator.child != null)
            {
                children.Add(decorator.child);
            }

            Composite composite = parent as Composite;
            if (composite)
            {
                return composite.children;
            }

            return children;
        }

        public BehaviourTree Clone()
        {
            BehaviourTree tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone();
            return tree;
        }

    }
}