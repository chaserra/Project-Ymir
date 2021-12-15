using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Brain
{
    public enum AI_State { SEEKING, PURSUING, FLEEING, EVADING, WANDERING, }

    // Cache
    private AI_Controller _controller;
    private Target _thisTargettableObject;

    // AI Behaviours
    public readonly AI_Seek Seek = new AI_Seek();
    public readonly AI_Pursue Pursue = new AI_Pursue();
    public readonly AI_Flee Flee = new AI_Flee();
    public readonly AI_Evade Evade = new AI_Evade();
    public readonly AI_Wander Wander = new AI_Wander();
    //....More AI stuff here

    // Parameters
    private float _targetScannerRadius = 50f;
    private float _forwardTargetSelection = 85f;
    private float _forwardDisplacementRadius = 50f;

    // Attributes
    public Target Target { get { return currentTarget; } }
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
    private Target currentTarget;

    // Constructor
    public AI_Brain (AI_Controller controller, Target targettable, float scanRadius, float forwardDist, float forwardRadius)
    {
        _controller = controller;
        _thisTargettableObject = targettable;
        _targetScannerRadius = scanRadius;
        _forwardTargetSelection = forwardDist;
        _forwardDisplacementRadius = forwardRadius;

        TransitionState(Wander);
    }

    // Get flight target vector via current behavior state
    // *** This is what is called in Controller Update() method ***
    public Vector3 CalculateFlightTargetVector()
    {
        // Sanity check for current target
        if (currentTarget != _controller.CurrentTarget)
        {
            currentTarget = _controller.CurrentTarget;
        }

        Think();    // Sets desired behavior of AI
        flightVector = currentState.Process(this);

        return flightVector;
    }

    public void Think()
    {
        // This is where the AI decides which state it should be in
        // TODO: FIX condition checks. Make sure behavior does not shift back and forth per frame. Example: if hp < 50 and target is not a MovingTarget, the AI shifts to Wander and Evade every frame
        // TODO: Make this class abstract? So we can implement different personalities per AI.

        // Flee
        if (_thisTargettableObject.GetHealth() < 50)
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
            TransitionState(Evade);
            return;
        }

        // Avoid
        // Like flee but with wander randomness

        // Wander
        else if (currentTarget == null || 
                ((behaviorState == AI_State.FLEEING || behaviorState == AI_State.EVADING) && 
                GetVelocityVector().magnitude > GetDistanceBeforeTurning() * 1.5f))
        {
            TransitionState(Wander);
            return;
        }

        // Pursue / Attack
        // Like seek but with lookAhead
        else if (!_controller.TargetIsBehind && currentTarget is MovingTarget)
        {
            TransitionState(Pursue);
            return;
        }

        // Seek
        else
        {
            TransitionState(Seek);
        }
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

    private Target GetClosestTarget()
    {
        Target newTarget = null;
        float closestDist = Mathf.Infinity;

        Collider[] targets = Physics.OverlapSphere(_controller.transform.position, _targetScannerRadius);
        for (int i = targets.Length - 1; i >= 0; i--)
        {
            if (targets[i].gameObject == _controller.gameObject) { continue; }   // Ignore self

            Target t;
            if (!targets[i].TryGetComponent(out t)) { continue; }   // Ignore non-Targets

            float dist = (targets[i].transform.position - _controller.transform.position).magnitude;

            if (dist < closestDist)
            {
                closestDist = dist;
                newTarget = t;
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

    public Vector3 GetControllerForward()
    {
        return _controller.transform.position + _controller.transform.forward;
    }

    public float GetCurrentForwardSpeed()
    {
        if (_thisTargettableObject is MovingTarget)
        {
            MovingTarget mt = (MovingTarget)_thisTargettableObject;
            return mt.CurrentForwardSpeed;
        }
        else
        {
            return 0f;
        }
    }

    public Vector3 GetVelocityVector()
    {
        if (currentTarget == null || 
            behaviorState == AI_State.PURSUING ||
            behaviorState == AI_State.FLEEING || 
            behaviorState == AI_State.EVADING)
        {
            // Return flight vector
            return flightVector - _controller.transform.position;
        }
        else
        {
            // Return Target object vector
            return currentTarget.transform.position - _controller.transform.position;
        }
    }

    public float GetDistanceBeforeTurning()
    {
        return _controller.DistanceToEnableTurning;
    }

    public void ToggleDistanceBeforeTurning(bool arg)
    {
        _controller.ToggleDistanceBeforeTurning(arg);
    }

    public Vector3 GetFlightTargetVector()
    {
        return flightVector;
    }

    public bool TargetObjectIsBehind(Target targetObject)
    {
        Vector3 dirToObject = targetObject.transform.position - _controller.transform.position;
        if (Vector3.Dot(_controller.transform.forward, dirToObject) < 0.5f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Overload method
    public bool TargetObjectIsBehind(Target targetObject, float dotValue)
    {
        Vector3 dirToObject = targetObject.transform.position - _controller.transform.position;
        if (Vector3.Dot(_controller.transform.forward, dirToObject) < dotValue)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //TODO: Create the following Base AI Methods
    //1. Seek()
    //2. Flee() -> Reverse of Seek. Switch around target pos and agent pos when Distance checking
    //3. Wander()
    //4. Pursue() -> Seek with lookahead
    //5. Avoid() -> Reverse of Pursue
    //Don't think of other stuff for now

}