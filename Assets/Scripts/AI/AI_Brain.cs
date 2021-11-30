using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Brain
{
    public enum AI_State { SEEKING, PURSUING, FLEEING, WANDERING, }

    // Cache
    private AI_Controller _controller;
    private Target _ship;

    // AI Behaviours
    private AI_Seek Seek = new AI_Seek();
    private AI_Pursue Pursue = new AI_Pursue();
    private AI_Flee Flee = new AI_Flee();
    private AI_Wander Wander = new AI_Wander();
    //....More AI stuff here

    // Parameters
    private float _frontDistanceTargetSelection = 30f;
    private float _frontDistanceDisplacementRadius = 10f;

    // Attributes
    public GameObject Target { get { return currentTarget; } }
    public Vector3 RandomFrontPosition
    {
        get
        {
            Vector3 forward = _controller.transform.position + _controller.transform.forward * _frontDistanceTargetSelection;
            Vector3 randomPointInSphere = Random.insideUnitSphere * _frontDistanceDisplacementRadius + forward;

            return randomPointInSphere;
        }
    }

    // State
    private AI_BaseState currentState;
    private AI_State state;
    private Vector3 flightVector;
    private GameObject currentTarget;

    public AI_Brain (AI_Controller controller, Target ship, float forwardDist, float forwardRadius)
    {
        _controller = controller;
        _ship = ship;
        _frontDistanceTargetSelection = forwardDist;
        _frontDistanceDisplacementRadius = forwardRadius;

        TransitionState(Wander);  // TODO: This should change to Wander once implemented
    }

    public void Think()
    {
        // TODO: This is where the AI should decide which state it should be in
        // TODO: Make this class abstract? So we can implement different personalities per AI.

        // Flee
        if (_ship.GetHealth() < 50)
        {
            TransitionState(Flee);
        }

        // Avoid
        // Like flee but with wander randomness

        // Wander
        else if (currentTarget == null || 
            (state == AI_State.FLEEING && GetDistanceToTargetObject() > GetDistanceBeforeTurning() * 1.5f))
        {
            TransitionState(Wander);
        }

        // Pursue / Attack
        // Like seek but with lookAhead

        // Seek
        else
        {
            //TransitionState(Seek);
            TransitionState(Pursue);
        }
    }

    // Get flight target vector via current behavior state
    public Vector3 CalculateFlightTargetVector()
    {
        if (currentTarget != _controller.CurrentTarget)
        {
            currentTarget = _controller.CurrentTarget;
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
            Debug.Log(_controller.gameObject.name + " state: " + state);
        }
    }

    public Vector3 GetControllerPosition()
    {
        return _controller.transform.position;
    }

    public float GetCurrentForwardSpeed()
    {
        return _controller.CurrentForwardSpeed;
    }

    public float GetDistanceToTargetObject()
    {
        if (currentTarget == null) { return 0f; }
        return (currentTarget.transform.position - _controller.transform.position).magnitude;
    }

    public Vector3 GetDistanceToTargetVector()
    {
        return flightVector - _controller.transform.position;
    }

    public float GetDistanceBeforeTurning()
    {
        return _controller.DistanceToEnableTurning;
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