using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Flee : AI_BaseState
{
    public override void EnterState(AI_Brain brain)
    {
        // Initialize stuff
        brain.SetAIState(AI_Brain.AI_State.FLEEING);
        brain.ToggleDistanceBeforeTurning(false);
    }

    public override Vector3 Process(AI_Brain brain)
    {
        Vector3 fleeVector = brain.Target.transform.position - brain.GetControllerPosition();
        Vector3 flyToPos = brain.GetControllerPosition() - fleeVector;

        return flyToPos;
    }

    public override void ExitState(AI_Brain brain)
    {
        // Do exit stuff
        brain.ToggleDistanceBeforeTurning(true);
    }

}
