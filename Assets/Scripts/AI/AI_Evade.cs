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
        // Catch null target
        if (brain.Target == null)
        {
            brain.TransitionState(brain.Wander);
            return brain.Wander.Process(brain);
        }

        Vector3 dirToTarget = brain.Target.transform.position - brain.GetControllerPosition();
        float thisSpeed = brain.GetCurrentForwardSpeed();
        float targetSpeed = targetAI.CurrentForwardSpeed;

        // If target is moving, calculate lookahead
        if (targetSpeed > 0f)
        {
            float lookAhead = dirToTarget.magnitude / (thisSpeed + targetSpeed);
            // Get predicted vector position
            Vector3 predictedPos = brain.Target.transform.position + brain.Target.transform.forward * lookAhead;

            // Reverse the predicted vector position
            Vector3 fleeVector = predictedPos - brain.GetControllerPosition();
            // Then reverse the direction
            flyToPos = brain.GetControllerPosition() - fleeVector;
        }
        // If target is not moving, simply return the flee position (Flee)
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