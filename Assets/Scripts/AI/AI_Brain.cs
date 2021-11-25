using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Brain
{
    // Cache
    private AI_Controller controller;
    private AI_Seek Seek = new AI_Seek();

    // State
    private AI_BaseState currentState;
    private GameObject currentTarget;
    public GameObject Target { get { return currentTarget; } }
    private Vector3 distToTarget;
    public Vector3 DistToTarget { get { return distToTarget; } }

    public AI_Brain (AI_Controller c)
    {
        controller = c;
        TransitionState(Seek);  // TODO: This should change to Wander once implemented
    }

    // Get flight target vector via current behavior state
    public Vector3 FlightTargetVector()
    {
        // TODO: This is where the AI should decide which state it should be in
        if (currentTarget != controller.CurrentTarget)
        {
            currentTarget = controller.CurrentTarget;
        }

        // TODO: distToTarget should be processed in the state itself
        // Get distance to target vector
        // TODO: current target should be flight vector
        distToTarget = currentTarget.transform.position - controller.transform.position;

        return currentState.Process(this);
    }

    public void TransitionState(AI_BaseState state)
    {
        if (currentState != state)
        {
            //currentState.ExitState(this);
            currentState = state;
            currentState.EnterState(this);
        }
    }

}