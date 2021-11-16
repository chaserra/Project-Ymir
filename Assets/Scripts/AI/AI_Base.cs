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

    // DEBUG
    [SerializeField] TextMeshProUGUI rightDotText;
    [SerializeField] TextMeshProUGUI upDotText;
    [SerializeField] TextMeshProUGUI upToDirDotText;
    [SerializeField] TextMeshProUGUI rightToDirDotText;
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

        if (currentTarget == null) { return; }
        Vector3 distToTarget = currentTarget.transform.position - transform.position;
        Vector3 dirToTarget = distToTarget.normalized;

        // TODO - roll and pitch adjust

        // Get 'center' angle between agent and target
        Vector3 cross = Vector3.Cross(transform.position, currentTarget.transform.position);
        // Determine if agent's up is perpendicular to target's position
        float rightDot = Vector3.Dot(transform.right, cross.normalized);
        float upDot = Vector3.Dot(transform.up, cross.normalized);
        float upToDirDot = Vector3.Dot(transform.up, dirToTarget);
        float rightToDirDot = Vector3.Dot(transform.right, dirToTarget);
        rightDotText.SetText(rightDot.ToString());
        upDotText.SetText(upDot.ToString());
        upToDirDotText.SetText(upToDirDot.ToString());
        rightToDirDotText.SetText(rightToDirDot.ToString());

        /* ***ROLL*** */
        /***********************************************************************
        Rotate Z Axis to:
        FOLLOW = Roll until agent Y axis is parallel to target's position
        AVOID = Roll until agent Y axis is perpendicular to target's position
        ========================================================================
        RollDot at 1 means agent's up is parallel to target's position
        RollDot at 0 means agent's up is perpendicular to target's position
        RollDot at -1 means agent's up is opposite to target's position
        Maintain at 1 to bank up towards target, 0 to bunk up away from target
        ************************************************************************/

        // Get amount of angle to rotate
        float rollAngle = Vector3.SignedAngle(transform.right, cross, Vector3.forward);
        rollAngleDebug = rollAngle;
        // Roll ship towards target
        if (roll)
        {
            if (Mathf.Abs(upDot) > 0.05f)
            {
                transform.Rotate(Vector3.back * rollAngle * rollSpeed * Time.deltaTime);
            }
        }

        /* ***PITCH*** */
        // Pitch up or down to align agent z axis to target
        float pitchAmount = Vector3.SignedAngle(transform.forward, cross, Vector3.forward);
        pitchAngleDebug = pitchAmount;
        if (pitch)
        {
            if (Mathf.Abs(upToDirDot) > 0.05f)
            {
                // TODO: FIX THIS. Make sure forward always looks AT target
                transform.Rotate(Vector3.left * pitchAmount * pitchSpeed * Time.deltaTime);
            }
        }

        /* ***YAW*** */
        float yawAmount = Vector3.SignedAngle(transform.forward, dirToTarget, Vector3.forward);
        yawAngleDebug = yawAmount;
        if (yaw)
        {
            if (Mathf.Abs(rightToDirDot) > 0.05f)
            {
                transform.Rotate(Vector3.down * yawAmount * yawSpeed * Time.deltaTime);
            }
        }
        //float targetAngleY = Mathf.Atan2(dirToTarget.x, dirToTarget.z) * Mathf.Rad2Deg;
        //float angleY = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngleY, ref turnSmoothVelocity, turnSmoothTime / yawSpeed);

        //transform.rotation = Quaternion.Euler(transform.rotation.x, angleY, transform.rotation.z);

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
