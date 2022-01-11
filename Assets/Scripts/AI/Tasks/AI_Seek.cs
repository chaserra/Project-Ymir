using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Seek : AI_BaseState
{
    public override void EnterState(AI_Brain brain)
    {
        // Initialize stuff
        brain.SetAIState(AI_Brain.AI_State.SEEKING);
    }

    public override Vector3 Process(AI_Brain brain)
    {
        /* Keeping this in case code below this doesn't work */
        //if (brain.Target == null)
        //{
        //    return brain.RandomFrontPosition;
        //}

        // Null target catch
        if (brain.Target == null)
        {
            brain.TransitionState(brain.Wander);
            return brain.Wander.Process(brain);
        }

        // Return target Vector
        return brain.Target.transform.position;
    }

    public override void ExitState(AI_Brain brain)
    {
        // Do exit stuff
    }

}