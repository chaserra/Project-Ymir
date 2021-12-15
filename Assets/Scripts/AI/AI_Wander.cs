using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Wander : AI_BaseState
{
    private Vector3 randomFlightVector;

    public override void EnterState(AI_Brain brain)
    {
        // Initialize stuff
        brain.SetAIState(AI_Brain.AI_State.WANDERING);
        randomFlightVector = brain.RandomFrontPosition;     // Initialize flight vector
    }

    public override Vector3 Process(AI_Brain brain)
    {
        float distFromRandomPoint = (randomFlightVector - brain.GetControllerPosition()).magnitude;
        if (distFromRandomPoint < 15f)
        {
            // Get new position if agent reaches previous flight vector
            randomFlightVector = brain.RandomFrontPosition;
        }
        return randomFlightVector;
    }

    public override void ExitState(AI_Brain brain)
    {
        // Do exit stuff
    }

}