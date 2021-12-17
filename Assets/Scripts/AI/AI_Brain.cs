using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Brain
{
    public enum AI_State { SEEKING, PURSUING, FLEEING, EVADING, WANDERING, }

    // Cache
    private AI_Controller controller;
    private Target targettable;

    // AI Behaviours
    public readonly AI_Seek Seek = new AI_Seek();
    public readonly AI_Pursue Pursue = new AI_Pursue();
    public readonly AI_Flee Flee = new AI_Flee();
    public readonly AI_Evade Evade = new AI_Evade();
    public readonly AI_Wander Wander = new AI_Wander();
    //....More AI stuff here

    // Parameters
    private Vector3 initialPos;
    private float targetScannerRadius = 200f;
    private float forwardTargetSelection = 100f;
    private float forwardDisplacementRadius = 80f;
    private float slowingRadius = 50f;
    private float wanderMaxDistance = 1000f;

    // Attributes
    public Target Target { get { return currentTarget; } }
    public Vector3 RandomFrontPosition
    {
        get
        {
            Vector3 forward = controller.transform.position + controller.transform.forward * forwardTargetSelection;
            Vector3 randomPointInSphere = Random.insideUnitSphere * forwardDisplacementRadius + forward;
            float distanceFromInitialPos = (initialPos - randomPointInSphere).magnitude;

            // If outside wander boundaries
            if (distanceFromInitialPos > wanderMaxDistance)
            {
                // Callback this getter
                return RandomFrontPosition;
            }

            return randomPointInSphere;
        }
    }

    // State
    private AI_BaseState currentState;
    private AI_State behaviorState;
    private Vector3 flightVector;
    private Target currentTarget;

    // Constructor
    public AI_Brain (AI_Controller controller, Target targettable, float scanRadius, 
        float forwardDist, float forwardRadius, float slowingRadius, float wanderMaxDistance)
    {
        this.controller = controller;
        this.targettable = targettable;
        targetScannerRadius = scanRadius;
        forwardTargetSelection = forwardDist;
        forwardDisplacementRadius = forwardRadius;
        this.slowingRadius = slowingRadius;
        this.wanderMaxDistance = wanderMaxDistance;
        // wanderMaxDistance should not be lower than randomize position finder
        if (this.wanderMaxDistance < forwardTargetSelection + forwardDisplacementRadius * 2f)
        {
            Debug.LogWarning("wanderMaxDistance is lower than position finder! Adjust values.");
            this.wanderMaxDistance = forwardTargetSelection + forwardDisplacementRadius * 2.5f;
        }

        initialPos = controller.transform.position;

        TransitionState(Wander);
    }

    // Get flight target vector via current behavior state
    // *** This is what is called in Controller Update() method ***
    public Vector3 CalculateFlightTargetVector()
    {
        // Sanity check for current target
        if (currentTarget != controller.CurrentTarget)
        {
            currentTarget = controller.CurrentTarget;
        }

        Think();    // Sets desired behavior of AI
        AdjustSpeed();
        flightVector = currentState.Process(this);

        return flightVector;
    }

    public void Think()
    {
        // This is where the AI decides which state it should be in
        // TODO: FIX condition checks. Make sure behavior does not shift back and forth per frame. Example: if hp < 50 and target is not a MovingTarget, the AI shifts to Wander and Evade every frame
        // TODO: Make this class abstract? So we can implement different personalities per AI.

        // Flee
        if (targettable.GetHealth() < 50)
        {
            if (currentTarget == null)  // If nothing to get away from
            {
                controller.CurrentTarget = AcquireClosestTarget();     // Attempt to get closest
                currentTarget = controller.CurrentTarget;
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
        else if (!controller.TargetIsBehind && currentTarget is MovingTarget)
        {
            // TODO: Create timeout for pursue if stuck without any progress (idea: pick random forward position)
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
            if (controller.debugMode)
            {
                Debug.Log(controller.gameObject.name + " state: " + currentState);
            }
        }
    }

    private Target AcquireClosestTarget()
    {
        Target newTarget = null;
        float closestDist = Mathf.Infinity;

        Collider[] targets = Physics.OverlapSphere(controller.transform.position, targetScannerRadius);
        for (int i = targets.Length - 1; i >= 0; i--)
        {
            if (targets[i].gameObject == controller.gameObject) { continue; }   // Ignore self

            Target t;
            if (!targets[i].TryGetComponent(out t)) { continue; }   // Ignore non-Targets

            float dist = (targets[i].transform.position - controller.transform.position).magnitude;

            if (dist < closestDist)
            {
                closestDist = dist;
                newTarget = t;
            }
        }
        return newTarget;
    }

    private void AdjustSpeed()
    {
        // Apply arrival behavior if Seeking or Pursuing a target
        if (behaviorState == AI_State.SEEKING || behaviorState == AI_State.PURSUING)
        {
            Vector3 velocity = currentTarget.transform.position - controller.transform.position;
            float distance = velocity.magnitude;
            if (distance < slowingRadius)
            {
                bool isBehind = TargetObjectIsBehind(currentTarget);
                if (isBehind)
                {
                    // Adjust speed faster if target is behind
                    controller.AdjustSpeedByDistance(distance / slowingRadius + 0.25f);
                }
                else
                {
                    // Use distance ratio as multiplier
                    controller.AdjustSpeedByDistance(distance / slowingRadius);
                }
            }
            else
            {
                controller.AdjustSpeedByDistance(1f);
            }
        }
        // Use max speed for any other behavior state
        else
        {
            controller.AdjustSpeedByDistance(1f);
        }
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
        return controller.transform.position;
    }

    public Vector3 GetControllerForward()
    {
        return controller.transform.position + controller.transform.forward;
    }

    public float GetCurrentForwardSpeed()
    {
        if (targettable is MovingTarget)
        {
            MovingTarget mt = (MovingTarget)targettable;
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
            return flightVector - controller.transform.position;
        }
        else
        {
            // Return Target object vector
            return currentTarget.transform.position - controller.transform.position;
        }
    }

    public float GetDistanceBeforeTurning()
    {
        return controller.DistanceToEnableTurning;
    }

    public void ToggleDistanceBeforeTurning(bool arg)
    {
        controller.ToggleDistanceBeforeTurning(arg);
    }

    public Vector3 GetFlightTargetVector()
    {
        return flightVector;
    }

    public bool TargetObjectIsBehind(Target targetObject)
    {
        Vector3 dirToObject = targetObject.transform.position - controller.transform.position;
        if (Vector3.Dot(controller.transform.forward, dirToObject) < 0.5f)
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
        Vector3 dirToObject = targetObject.transform.position - controller.transform.position;
        if (Vector3.Dot(controller.transform.forward, dirToObject) < dotValue)
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