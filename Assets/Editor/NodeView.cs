using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using QuaternionGames.BT;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public Action<NodeView> OnNodeSelected;
    public QuaternionGames.BT.Node node;
    public Port input;
    public Port output;

    public NodeView(QuaternionGames.BT.Node node) : base("Assets/Editor/NodeView.uxml")
    {
        this.node = node;
        this.title = node.name;
        this.viewDataKey = node.guid;

        style.left = node.position.x;
        style.top = node.position.y;

        CreateInputPorts();
        CreateOutputPorts();
        SetupClasses();
    }

    private void SetupClasses()
    {
        if (node is Leaf)
        {
            AddToClassList("action");
        }
        else if (node is Composite)
        {
            AddToClassList("composite");
        }
        else if (node is Decorator)
        {
            AddToClassList("decorator");
        }
        else if (node is Root)
        {
            AddToClassList("root");
        }
    }

    private void CreateInputPorts()
    {
        if (node is Leaf)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (node is Composite)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (node is Decorator)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (node is Root)
        {

        }

        if (input != null)
        {
            input.portName = "";
            input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(input);
        }
    }

    private void CreateOutputPorts()
    {
        if (node is Leaf)
        {

        }
        else if (node is Composite)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        }
        else if (node is Decorator)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }
        else if (node is Root)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }

        if (output != null)
        {
            output.portName = "";
            output.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(output);
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(node, "Behaviour Tree (Set Position)");
        node.position.x = newPos.xMin;
        node.position.y = newPos.yMin;
        EditorUtility.SetDirty(node);
    }

    public override void OnSelected()
    {
        base.OnSelected();
        OnNodeSelected?.Invoke(this);
    }

    public void SortChildren()
    {
        Composite composite = node as Composite;
        if (composite)
        {
            composite.children.Sort(SortByHorizontalPosition);
        }
    }

    private int SortByHorizontalPosition(QuaternionGames.BT.Node left, QuaternionGames.BT.Node right)
    {
        return left.position.x < right.position.x ? -1 : 1;
    }
}
