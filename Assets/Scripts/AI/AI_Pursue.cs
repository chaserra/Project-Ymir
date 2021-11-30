using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Pursue : AI_BaseState
{
    private AI_Controller targetAI;
    public override void EnterState(AI_Brain brain)
    {
        // Initialize stuff
        if (currentState == AI_Brain.AI_State.PURSUING) { return; }
        currentState = AI_Brain.AI_State.PURSUING;
        brain.SetAIState(AI_Brain.AI_State.PURSUING);
        targetAI = brain.Target.GetComponent<AI_Controller>();
    }

    public override Vector3 Process(AI_Brain brain)
    {
        if (brain.Target == null)
        {
            return brain.RandomFrontPosition;
        }
        Vector3 flyToPos;
        Vector3 dirToTarget = brain.Target.transform.position - brain.GetControllerPosition();

        float thisSpeed = brain.GetCurrentForwardSpeed();
        float targetSpeed = targetAI.CurrentForwardSpeed;

        float lookAhead = dirToTarget.magnitude / (thisSpeed + targetSpeed);
        flyToPos = brain.Target.transform.position + brain.Target.transform.forward * lookAhead;

        // TODO: This works BUT FIX HOW TO GET SPEEDS!!

        return flyToPos;
    }

    public override void ExitState(AI_Brain brain)
    {
        // Do exit stuff
    }

}