using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Evade : AI_BaseState
{
    private Vector3 flyToPos;
    private MovingTarget targetAI;

    public override void EnterState(AI_Brain brain)
    {
        // Initialize stuff
        brain.SetAIState(AI_Brain.AI_State.EVADING);
        targetAI = brain.Target.GetComponent<MovingTarget>();
        brain.ToggleDistanceBeforeTurning(false);
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

        if (targetSpeed > 0f)
        {
            // TODO: Do something about the flickering?
            float lookAhead = dirToTarget.magnitude / (thisSpeed + targetSpeed);
            Vector3 predictedPos = brain.Target.transform.position + brain.Target.transform.forward * lookAhead;

            Vector3 fleeVector = predictedPos - brain.GetControllerPosition();
            flyToPos = brain.GetControllerPosition() - fleeVector;
        }
        else
        {
            flyToPos = brain.Flee.Process(brain);
        }

        return flyToPos;
    }

    public override void ExitState(AI_Brain brain)
    {
        // Do exit stuff
        brain.ToggleDistanceBeforeTurning(true);
    }

}