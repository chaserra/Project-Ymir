using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Flee : AI_BaseState
{
    Vector3 distanceToTarget;

    public override void EnterState(AI_Brain brain)
    {
        // Initialize stuff
        if (currentState == AI_Brain.AI_State.FLEEING) { return; }
        // TODO: If no target, get target to flee from
        currentState = AI_Brain.AI_State.FLEEING;
        brain.SetAIState(AI_Brain.AI_State.FLEEING);
    }

    public override Vector3 Process(AI_Brain brain)
    {
        Vector3 dist = brain.GetControllerPosition() - brain.Target.transform.position;
        Vector3 dir = dist.normalized;

        Vector3 flyToPos = brain.GetControllerPosition() + (dir * brain.GetDistanceBeforeTurning());

        return flyToPos;
    }

    public override void ExitState(AI_Brain brain)
    {
        // Do exit stuff
    }

}
