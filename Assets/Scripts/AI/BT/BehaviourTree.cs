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

            Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
            nodes.Add(node);

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(node, this);
            }
            Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");
            AssetDatabase.SaveAssets();

            return node;
        }

        public void DeleteNode(Node node)
        {
            Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");
            nodes.Remove(node);

            //AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(Node parent, Node child)
        {
            Root root = parent as Root;
            if (root)
            {
                Undo.RecordObject(root, "Behaviour Tree (AddChild)");
                root.child = child;
                EditorUtility.SetDirty(root);
            }

            Decorator decorator = parent as Decorator;
            if (decorator)
            {
                Undo.RecordObject(decorator, "Behaviour Tree (AddChild)");
                decorator.child = child;
                EditorUtility.SetDirty(decorator);
            }

            Composite composite = parent as Composite;
            if (composite)
            {
                Undo.RecordObject(composite, "Behaviour Tree (AddChild)");
                composite.children.Add(child);
                EditorUtility.SetDirty(composite);
            }
        }

        public void RemoveChild(Node parent, Node child)
        {
            Root root = parent as Root;
            if (root)
            {
                Undo.RecordObject(root, "Behaviour Tree (RemoveChild)");
                root.child = null;
                EditorUtility.SetDirty(root);
            }

            Decorator decorator = parent as Decorator;
            if (decorator)
            {
                Undo.RecordObject(decorator, "Behaviour Tree (RemoveChild)");
                decorator.child = null;
                EditorUtility.SetDirty(decorator);
            }

            Composite composite = parent as Composite;
            if (composite)
            {
                Undo.RecordObject(composite, "Behaviour Tree (RemoveChild)");
                composite.children.Remove(child);
                EditorUtility.SetDirty(composite);
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

        public void Traverse(Node node, System.Action<Node> visitor)
        {
            if (node)
            {
                visitor.Invoke(node);
                var children = GetChildren(node);
                children.ForEach((n) => Traverse(n, visitor));
            }
        }

        public BehaviourTree Clone()
        {
            BehaviourTree tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone();
            tree.nodes = new List<Node>();
            Traverse(tree.rootNode, (n) =>
            {
                tree.nodes.Add(n);
            });
            return tree;
        }

    }
}