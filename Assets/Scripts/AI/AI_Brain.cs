using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Brain
{
    public enum AI_State { SEEKING, FLEEING, }

    // Cache
    private AI_Controller controller;
    private Target ship;
    private AI_Seek Seek = new AI_Seek();
    private AI_Flee Flee = new AI_Flee();
    //....More AI stuff here

    // Attributes
    public GameObject Target { get { return currentTarget; } }

    // State
    private AI_BaseState currentState;
    private AI_State state;
    private Vector3 flightVector;
    private GameObject currentTarget;

    public AI_Brain (AI_Controller c, Target t)
    {
        controller = c;
        ship = t;
        TransitionState(Seek);  // TODO: This should change to Wander once implemented
    }

    public void Think()
    {
        // TODO: This is where the AI should decide which state it should be in
        // TODO: Make this abstract? So we can implement different personalities per AI.

        // Wander
        if (currentTarget == null || 
            (state == AI_State.FLEEING && GetDistanceToTargetObject() > GetDistanceBeforeTurning() * 1.5f))
        {
            Debug.Log("Should wander");
        }

        // Seek

        // Pursue / Attack

        // Flee
        if (ship.GetHealth() < 50)
        {
            TransitionState(Flee);
        }
    }

    // Get flight target vector via current behavior state
    public Vector3 CalculateFlightTargetVector()
    {
        if (currentTarget != controller.CurrentTarget)
        {
            currentTarget = controller.CurrentTarget;
        }

        flightVector = currentState.Process(this);

        return flightVector;
    }

    public void TransitionState(AI_BaseState state)
    {
        if (currentState != state)
        {
            if (currentState != null)
            {
                currentState.ExitState(this);
            }
            currentState = state;
            currentState.EnterState(this);
        }
    }

    public void SetAIState(AI_State newState)
    {
        if (state != newState)
        {
            state = newState;
            Debug.Log(controller.gameObject.name + " state: " + state);
        }
    }

    public Vector3 GetControllerPosition()
    {
        return controller.transform.position;
    }

    public float GetDistanceToTargetObject()
    {
        if (currentTarget == null) { return 0f; }
        return (currentTarget.transform.position - controller.transform.position).magnitude;
    }

    public Vector3 GetDistanceToTargetVector()
    {
        return flightVector - controller.transform.position;
    }

    public float GetDistanceBeforeTurning()
    {
        return controller.DistanceToEnableTurning;
    }

    public Vector3 GetFlightTargetVector()
    {
        return flightVector;
    }

    //TODO: Create the following Base AI Methods
    //1. Seek()
    //2. Flee() -> Reverse of Seek. Switch around target pos and agent pos when Distance checking
    //3. Wander()
    //4. Pursue() -> Seek with lookahead
    //5. Avoid() -> Reverse of Pursue
    //Don't think of other stuff for now

}