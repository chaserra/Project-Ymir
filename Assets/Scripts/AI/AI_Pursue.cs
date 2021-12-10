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
        // Null target catch
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
            float lookAhead = dirToTarget.magnitude / (thisSpeed / targetSpeed);
            // Get predicted vector position
            flyToPos = brain.Target.transform.position + brain.Target.transform.forward * lookAhead;
        }
        // If target is not moving, simply return the position (Seek)
        else
        {
            flyToPos = brain.Target.transform.position;
        }

        return flyToPos;
    }

    public override void ExitState(AI_Brain brain)
    {
        // Do exit stuff
    }

}