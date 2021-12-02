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
    private float _targetScannerRadius = 50f;
    private float _forwardTargetSelection = 85f;
    private float _forwardDisplacementRadius = 50f;

    // Attributes
    public GameObject Target { get { return currentTarget; } }
    public Vector3 RandomFrontPosition
    {
        get
        {
            Vector3 forward = _controller.transform.position + _controller.transform.forward * _forwardTargetSelection;
            Vector3 randomPointInSphere = Random.insideUnitSphere * _forwardDisplacementRadius + forward;

            return randomPointInSphere;
        }
    }

    // State
    private AI_BaseState currentState;
    private AI_State behaviorState;
    private Vector3 flightVector;
    private GameObject currentTarget;

    // Constructor
    public AI_Brain (AI_Controller controller, Target ship, float scanRadius, float forwardDist, float forwardRadius)
    {
        _controller = controller;
        _ship = ship;
        _targetScannerRadius = scanRadius;
        _forwardTargetSelection = forwardDist;
        _forwardDisplacementRadius = forwardRadius;

        TransitionState(Wander);
    }

    public void Think()
    {
        // This is where the AI decides which state it should be in
        // TODO: Make this class abstract? So we can implement different personalities per AI.

        // Flee
        if (_ship.GetHealth() < 50)
        {
            if (currentTarget == null)  // If nothing to get away from
            {
                _controller.CurrentTarget = GetClosestTarget();     // Attempt to get closest
                currentTarget = _controller.CurrentTarget;
                if (currentTarget == null)      // If still no targets around, just wander
                {
                    TransitionState(Wander);
                    return;
                }
            }
            TransitionState(Flee);
            return;
        }

        // Avoid
        // Like flee but with wander randomness

        // Wander
        else if (currentTarget == null || 
            (behaviorState == AI_State.FLEEING && GetDistanceToTargetObject() > GetDistanceBeforeTurning() * 1.5f))
        {
            TransitionState(Wander);
            return;
        }

        // Pursue / Attack
        // Like seek but with lookAhead
        else if (!_controller.TargetIsBehind)
        {
            TransitionState(Pursue);
        }

        // Seek
        else
        {
            // TODO: Differentiate between moving and non-moving object. Pursue does not work if target has no speed
            TransitionState(Seek);
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
            Debug.Log(_controller.gameObject.name + " state: " + currentState);
        }
    }

    private GameObject GetClosestTarget()
    {
        GameObject newTarget = null;
        float closestDist = Mathf.Infinity;
        Collider[] targets = Physics.OverlapSphere(_controller.transform.position, _targetScannerRadius);
        for (int i = targets.Length - 1; i >= 0; i--)
        {
            if (targets[i].gameObject == _controller.gameObject) { continue; }
            if (targets[i].GetComponent<Target>() == null) { continue; }

            float dist = (targets[i].transform.position - _controller.transform.position).magnitude;

            if (dist < closestDist)
            {
                closestDist = dist;
                newTarget = targets[i].gameObject;
            }
        }
        return newTarget;
    }

    /* Getters and Setters */
    public void SetAIState(AI_State newState)
    {
        if (behaviorState != newState)
        {
            behaviorState = newState;
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