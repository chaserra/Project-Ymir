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
        // Null target catch
        if (brain.Target == null || targetAI == null)
        {
            brain.TransitionState(brain.Wander);
            return brain.Wander.Process(brain);
        }

        Vector3 dirToTarget = brain.Target.transform.position - brain.GetControllerPosition();
        float thisSpeed = brain.GetCurrentSpeed();
        float targetSpeed = targetAI.GetSpeed();

        // If target is moving, calculate lookahead
        if (targetSpeed > 0f && thisSpeed > 0f)
        {
            // Calculate lookahead value || d = ((Vf + Vi)/2) * t ||
            float lookAhead = dirToTarget.magnitude / ((thisSpeed + targetSpeed) / 2);
            // Get predicted vector position
            Vector3 predictedPos = brain.Target.transform.position + brain.Target.transform.forward * lookAhead;

            // TODO: Clean up jitter!
            if (brain.TargetObjectIsBehind(brain.Target))
            {
                // Reverse the predicted vector position
                Vector3 fleeVector = predictedPos - brain.Target.transform.position;
                // Then flee from the reversed predicted position
                flyToPos = brain.GetControllerForward() - fleeVector;
            }
            else
            {
                // If pursuer is in front, change direction towards the predicted position
                flyToPos = brain.GetControllerForward() + predictedPos;
            }

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