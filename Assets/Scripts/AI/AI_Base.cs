using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AI_Base : MonoBehaviour
{
    // Parameters
    [SerializeField] float minSpeed = 2f;
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float pitchSpeed = 5f;
    [SerializeField] float rollSpeed = 5f;
    [SerializeField] float yawSpeed = 5f;
    [SerializeField] float turnSmoothTime = 5f;

    // State
    private float turnSmoothVelocity;
    [SerializeField] private float currentSpeed = 5f;
    [SerializeField] private GameObject currentTarget;
    private bool targetIsBehind;

    // DEBUG
    [SerializeField] TextMeshProUGUI rightDotText;
    [SerializeField] TextMeshProUGUI upDotText;
    [SerializeField] TextMeshProUGUI upToDirDotText;
    [SerializeField] TextMeshProUGUI rightToDirDotText;
    [SerializeField] TextMeshProUGUI targetAngle;
    [SerializeField] TextMeshProUGUI targetBehind;
    [SerializeField] bool roll = true;
    [SerializeField] bool pitch = true;
    [SerializeField] bool yaw = true;
    [SerializeField] float rollAngleDebug;
    [SerializeField] float pitchAngleDebug;
    [SerializeField] float yawAngleDebug;

    private void Awake()
    {
        
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        /* THRUST */
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        // TODO - make AI adjust thrust when banking. Do this after finishing roll and pitch code

        // Target check
        if (currentTarget == null) { return; }

        // Compute for Distance and Direction to target
        Vector3 distToTarget = currentTarget.transform.position - transform.position;
        Vector3 dirToTarget = distToTarget.normalized;

        // Get 'center' angle between agent and target
        Vector3 cross = Vector3.Cross(transform.position, currentTarget.transform.position);
        // Dot product computations (can safely remove rightDot)
        float rightDot = Vector3.Dot(transform.right, cross.normalized);
        float upDot = Vector3.Dot(transform.up, cross.normalized);
        float upToDirDot = Vector3.Dot(transform.up, dirToTarget);
        float rightToDirDot = Vector3.Dot(transform.right, dirToTarget);
        float forwardDot = Vector3.Dot(transform.forward, dirToTarget);

        // Check if target is behind the agent.
        targetIsBehind = forwardDot < 0f ? true : false;

        /* ***ROLL*** */
        // Get amount of angle to rotate
        float rollAngle = Vector3.SignedAngle(transform.right, cross, Vector3.forward);
        rollAngleDebug = rollAngle; // Debug only
        // Roll ship towards target
        if (roll)
        {
            // Stop rotating when near target angle to prevent stutter
            if (Mathf.Abs(upDot) > 0.05f)
            {
                transform.Rotate(Vector3.back * rollAngle * rollSpeed * Time.deltaTime);
            }
        }

        /* ***PITCH*** */
        // Pitch up or down to align agent z axis to target
        float pitchAmount = Vector3.SignedAngle(transform.forward, cross, Vector3.forward);
        pitchAngleDebug = pitchAmount; // Debug only
        if (pitch)
        {
            // Stop rotating when near target angle to prevent stutter
            // Only stop rotating if target is in front of the object (ensures target is moving TO target)
            if (Mathf.Abs(upToDirDot) > 0.05f || targetIsBehind)
            {
                transform.Rotate(Vector3.left * Mathf.Abs(pitchAmount) * pitchSpeed * Time.deltaTime);
            }
        }

        /* ***YAW*** */
        // Rotate agent towards target
        float yawAmount = Vector3.SignedAngle(transform.forward, dirToTarget, Vector3.forward);
        yawAngleDebug = yawAmount; // Debug only
        if (yaw)
        {
            // Stop rotating when near target angle to prevent stutter
            // Only stop rotating if target is in front of the object (ensures target is moving TO target)
            if (Mathf.Abs(rightToDirDot) > 0.05f || targetIsBehind)
            {
                transform.Rotate(Vector3.down * Mathf.Abs(yawAmount) * yawSpeed * Time.deltaTime);
            }
        }

        /** !DEBUG UI TEXT! **/
        rightDotText.SetText("RDot: " + rightDot.ToString());
        upDotText.SetText("UDot: " + upDot.ToString());
        upToDirDotText.SetText("UDirDot: " + upToDirDot.ToString());
        rightToDirDotText.SetText("RDirDot: " + rightToDirDot.ToString());
        targetBehind.SetText("Target Behind?: " + targetIsBehind.ToString());
        targetAngle.SetText("Angle: " + forwardDot.ToString());

        /** !DEBUG LINES! **/
        // Local Up
        Debug.DrawLine(transform.position, transform.position + transform.up * 15f, Color.green);
        // Local Right
        Debug.DrawLine(transform.position, transform.position + transform.right * 15f, Color.red);
        // Local Forward
        Debug.DrawLine(transform.position, transform.position + transform.forward * 15f, Color.blue);
        // Dir to target
        Debug.DrawLine(transform.position, transform.position + (dirToTarget * 15f), Color.yellow);
        // Cross (of this object to target)
        Debug.DrawLine(transform.position, transform.position + (cross.normalized * 15f), Color.cyan);
    }

    //TODO: Create the following Base AI Methods
    //1. Seek()
    //2. Flee() -> Reverse of Seek. Switch around target pos and agent pos when Distance checking
    //3. Wander()
    //4. Pursue() -> Seek with lookahead
    //5. Avoid() -> Reverse of Pursue
    //Don't think of other stuff for now
}
