using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ymir.BT;

public class BT_Tester : MonoBehaviour
{
    BehaviourTree tree;

    private void Awake()
    {
        tree = ScriptableObject.CreateInstance<BehaviourTree>();

        var log1 = ScriptableObject.CreateInstance<DebugNode>();
        log1.message = "Heya! 11111";
        var pause1 = ScriptableObject.CreateInstance<WaitNode>();
        var log2 = ScriptableObject.CreateInstance<DebugNode>();
        log2.message = "Heya! 22222";
        var pause2 = ScriptableObject.CreateInstance<WaitNode>();
        var log3 = ScriptableObject.CreateInstance<DebugNode>();
        log3.message = "Heya! 33333";
        var pause3 = ScriptableObject.CreateInstance<WaitNode>();

        var sequence = ScriptableObject.CreateInstance<Sequence>();
        sequence.AddChild(log1);
        sequence.AddChild(pause1);
        sequence.AddChild(log2);
        sequence.AddChild(pause2);
        sequence.AddChild(log3);
        sequence.AddChild(pause3);

        var repeater = ScriptableObject.CreateInstance<Repeater>();
        repeater.numLoop = 2;
        repeater.SetChild(sequence);

        tree.rootNode = repeater;
    }

    private void Start()
    {

    }

    private void Update()
    {
        tree.Update();
    }

}
