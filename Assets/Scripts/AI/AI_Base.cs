using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AI_Base : MonoBehaviour
{
    // Parameters
    [Header("Ship Properties")]
    [SerializeField] float minSpeed = 2f;
    [SerializeField] float maxSpeed = 5f;
    [Header("Ship Controls")]
    [SerializeField] float yawSpeed = .45f;
    [SerializeField] float pitchSpeed = .8f;
    [SerializeField] float rollSpeed = 2f;
    [Header("AI Behavior")]
    [SerializeField] float maxDistanceBeforeTurning = 150f;
    [SerializeField] float randomizeFactor = 0.35f;

    // State
    [Header("Ship Status")]
    [SerializeField] private float currentSpeed = 5f;
    [SerializeField] private GameObject currentTarget;
    private bool targetIsBehind;
    private float distanceToEnableTurning;
    private bool hasRandomized = false;

    // DEBUG
    [Header("DEBUG")]
    [SerializeField] bool debugMode = false;
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
    [SerializeField] float distanceFromTarget;

    private void Awake()
    {
        distanceToEnableTurning = maxDistanceBeforeTurning;
    }

    private void Start()
    {
        StartCoroutine(RandomizeDistance());
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        /* THRUST */
        // TODO - make AI adjust thrust when banking. Do this after finishing roll and pitch code
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        // Target check
        if (currentTarget == null) { return; }

        // Compute for Distance and Direction to target
        Vector3 distToTarget = currentTarget.transform.position - transform.position;
        Vector3 dirToTarget = distToTarget.normalized;
        distanceFromTarget = distToTarget.magnitude;

        // Get 'center' angle between agent and target
        Vector3 cross = Vector3.Cross(transform.position, currentTarget.transform.position);
        // Dot product computations (can safely remove rightDot)
        float rightDot = Vector3.Dot(transform.right, cross.normalized);
        float upDot = Vector3.Dot(transform.up, cross.normalized);
        float upToDirDot = Vector3.Dot(transform.up, dirToTarget);
        float rightToDirDot = Vector3.Dot(transform.right, dirToTarget);
        float forwardDot = Vector3.Dot(transform.forward, dirToTarget);

        // Check if target is behind the agent.
        targetIsBehind = forwardDot < 0.5f ? true : false;

        /* ***ROLL*** */
        // Get amount of angle to rotate
        float rollAngle = Vector3.SignedAngle(transform.right, cross, Vector3.forward);
        rollAngleDebug = rollAngle; // Debug only
        // Roll ship towards target
        if (roll)
        {
            // Stop rotating when near target angle to prevent stutter
            if (Mathf.Abs(upDot) > 0.05f) // Number closer to 0 makes adjustment more accurate
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
            // TODO: Check how to make this rotate downwards. It seems like ai only pitches up
            // Rotate if target is behind or if target pitch angle is not yet near target angle
            if (Mathf.Abs(upToDirDot) > 0.035f || targetIsBehind) // Number closer to 0 makes adjustment more accurate
            {
                // Rotate if max distance reached. Makes flight look more convincing
                // Also rotate while target is in front and target pitch angle is not yet reached
                if (distanceFromTarget > distanceToEnableTurning || !targetIsBehind)
                {
                    transform.Rotate(Vector3.left * Mathf.Abs(pitchAmount) * pitchSpeed * Time.deltaTime);
                }
            }
        }

        /* ***YAW*** */
        // Rotate agent towards target
        float yawAmount = Vector3.SignedAngle(transform.forward, dirToTarget, Vector3.forward);
        yawAngleDebug = yawAmount; // Debug only
        if (yaw)
        {
            // Rotate if target is behind or if target yaw angle is not yet near target angle
            if (Mathf.Abs(rightToDirDot) > 0.035f || targetIsBehind) // Number closer to 0 makes adjustment more accurate
            {
                // Rotate if max distance reached. Makes flight look more convincing
                // Also rotate while target is in front and target yaw angle is not yet reached
                if (distanceFromTarget > distanceToEnableTurning || !targetIsBehind)
                {
                    transform.Rotate(Vector3.down * Mathf.Abs(yawAmount) * yawSpeed * Time.deltaTime);
                }
            }
        }

        /* ***UNSTUCK*** */
        // Instant look to target to prevent death spiral bug
        if (distanceFromTarget > 500f && targetIsBehind)
        {
            Debug.Log(gameObject.name + " prevented death spiral!");
            transform.LookAt(currentTarget.transform, transform.up);
        }

        /** !DEBUG UI TEXT! **/
        if (debugMode)
        {
            rightDotText.SetText("RDot: " + rightDot.ToString());
            upDotText.SetText("UDot: " + upDot.ToString());
            upToDirDotText.SetText("UDirDot: " + upToDirDot.ToString());
            rightToDirDotText.SetText("RDirDot: " + rightToDirDot.ToString());
            targetBehind.SetText("Target Behind?: " + targetIsBehind.ToString());
            targetAngle.SetText("Angle: " + forwardDot.ToString());
        }

        /** !DEBUG LINES! **/
        // Local Up
        Debug.DrawLine(transform.position, transform.position + transform.up * 15f, Color.green);
        // Local Right
        Debug.DrawLine(transform.position, transform.position + transform.right * 15f, Color.red);
        // Local Forward
        Debug.DrawLine(transform.position, transform.position + transform.forward * 15f, Color.blue);
        // Dir to target
        Debug.DrawLine(transform.position, transform.position + (dirToTarget * 15f), Color.yellow);
        //Debug.DrawLine(transform.position, currentTarget.transform.position, Color.yellow);
        // Cross (of this object to target)
        Debug.DrawLine(transform.position, transform.position + (cross.normalized * 15f), Color.cyan);
    }

    private IEnumerator RandomizeDistance()
    {
        while (true)
        {
            if (!hasRandomized && !targetIsBehind)
            {
                float randomFactor = maxDistanceBeforeTurning - (maxDistanceBeforeTurning * randomizeFactor);
                distanceToEnableTurning = Random.Range(randomFactor, maxDistanceBeforeTurning);
                hasRandomized = true;
            }
            if (targetIsBehind)
            {
                hasRandomized = false;
            }
            yield return null;
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
