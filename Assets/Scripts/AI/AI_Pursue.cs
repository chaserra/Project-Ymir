using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Pursue : AI_BaseState
{
    private Vector3 flyToPos;
    private MovingTarget targetAI;

    public override void EnterState(AI_Brain brain)
    {
        // Initialize stuff
        brain.SetAIState(AI_Brain.AI_State.PURSUING);
        targetAI = brain.Target.GetComponent<MovingTarget>();
    }

    public override Vector3 Process(AI_Brain brain)
    {
        if (brain.Target == null)
        {
            return brain.RandomFrontPosition;
        }
        Vector3 dirToTarget = brain.Target.transform.position - brain.GetControllerPosition();

        float thisSpeed = brain.GetCurrentForwardSpeed();
        float targetSpeed = targetAI.CurrentForwardSpeed;
        float lookAhead = dirToTarget.magnitude / (thisSpeed + targetSpeed);

        flyToPos = brain.Target.transform.position + brain.Target.transform.forward * lookAhead;

        // TODO: Do something about the flickering. And also about stopped target prediction

        return flyToPos;
    }

    public override void ExitState(AI_Brain brain)
    {
        // Do exit stuff
    }

}