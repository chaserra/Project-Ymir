using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Seek : AI_BaseState
{
    public override void EnterState(AI_Brain brain)
    {
        // Initialize stuff
    }

    public override Vector3 Process(AI_Brain brain)
    {
        Vector3 flyToPos;

        flyToPos = brain.Target.transform.position;

        return flyToPos;
    }

    public override void ExitState(AI_Brain brain)
    {
        throw new System.NotImplementedException();
    }

}