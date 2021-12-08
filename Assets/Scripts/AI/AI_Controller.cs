using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(MovingTarget))]
public class AI_Controller : MonoBehaviour
{
    // Cache
    private MovingTarget ship;
    private ShipStats shipStats;
    private AI_Brain ai;

    // Parameters
    [Header("AI Behavior")]
    [SerializeField] float maxDistanceBeforeTurning = 150f;
    [SerializeField] float randomizeFactor = 0.35f;
    [SerializeField] float targetScannerRadius = 50f;
    [SerializeField] float forwardTargetSelection = 30f;
    [SerializeField] float forwardDisplacementRadius = 10f;

    // Attributes
    public Target CurrentTarget { get { return currentTarget; } set { currentTarget = value; } }
    public float DistanceToEnableTurning { get { return distanceToEnableTurning; } }
    public bool TargetIsBehind { get { return targetIsBehind; } }

    // State
    [Header("Ship Status")]
    [SerializeField] private float currentForwardSpeed = 50f;
    [SerializeField] private Target currentTarget;
    private Vector3 targetFlightVector;
    private float distanceFromTarget;
    private bool targetIsBehind;

    /* Momentum turning (get to distance before maneuvering towards target) */
    private bool enableDistanceBeforeTurning = true;
    private float distanceToEnableTurning;
    private bool hasRandomized = false;
    private float randomizeTimer = 0f;

    // DEBUG
    [Header("DEBUG")]
    [SerializeField] bool debugMode = false;
    [SerializeField] TextMeshProUGUI rightDotText;
    [SerializeField] TextMeshProUGUI upDotText;
    [SerializeField] TextMeshProUGUI upToDirDotText;
    [SerializeField] TextMeshProUGUI rightToDirDotText;
    [SerializeField] TextMeshProUGUI targetAngle;
    [SerializeField] TextMeshProUGUI targetBehindText;
    [SerializeField] TextMeshProUGUI crossText;
    [SerializeField] bool roll = true;
    [SerializeField] bool pitch = true;
    [SerializeField] bool yaw = true;
    [SerializeField] float rollAngleDebug;
    [SerializeField] float pitchAngleDebug;
    [SerializeField] float yawAngleDebug;
    bool rolling = false;
    bool pitching = false;
    bool yawing = false;
    [SerializeField] TextMeshProUGUI rollingText;
    [SerializeField] TextMeshProUGUI pitchingText;
    [SerializeField] TextMeshProUGUI yawingText;
    [SerializeField] TextMeshProUGUI forwardDotText;

    /* DEBUG */
    private void OnDrawGizmosSelected()
    {
        // Wander
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * forwardTargetSelection);
        Gizmos.DrawWireSphere(transform.position + transform.forward * forwardTargetSelection, forwardDisplacementRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetFlightVector, 2f);
    }

    private void Awake()
    {
        ship = GetComponent<MovingTarget>();
        shipStats = (ShipStats)ship.TargetStats;
        ai = new AI_Brain(this, ship, targetScannerRadius, forwardTargetSelection, forwardDisplacementRadius);
        maxDistanceBeforeTurning = shipStats.MaxSpeed * 3.5f;
        distanceToEnableTurning = maxDistanceBeforeTurning;
    }

    private void Start()
    {
        StartCoroutine(RandomizeDistance());
    }

    private void Update()
    {
        targetFlightVector = ai.CalculateFlightTargetVector();
        Move(targetFlightVector);

        //if (debugMode)
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        Time.timeScale = .25f;
        //    }
        //    if (Input.GetMouseButtonDown(1))
        //    {
        //        Time.timeScale = 1f;
        //    }
        //}
    }

    private void Move(Vector3 vectorToSteerTowards)
    {
        /* THRUST */
        transform.Translate(Vector3.forward * currentForwardSpeed * Time.deltaTime);

        // Compute for Distance and Direction to target
        Vector3 distToTarget = ai.GetVelocityVector();
        Vector3 dirToTarget = distToTarget.normalized;
        distanceFromTarget = distToTarget.magnitude;

        // Get 'center' angle between agent and target
        Vector3 cross = Vector3.Cross(transform.position, vectorToSteerTowards);
        // Dot product computations (can safely remove rightDot)
        float rightDot = Vector3.Dot(transform.right, cross.normalized);
        float upDot = Vector3.Dot(transform.up, cross.normalized);
        float upToDirDot = Vector3.Dot(transform.up, dirToTarget);
        float rightToDirDot = Vector3.Dot(transform.right, dirToTarget);
        float forwardDot = Vector3.Dot(transform.forward, dirToTarget);

        // Check if target is behind the agent.
        targetIsBehind = forwardDot < 0.25f ? true : false;

        /* ***ROLL*** */
        // Get amount of angle to rotate
        float rollAngle = Vector3.Angle(transform.right, cross);
        rollAngleDebug = rollAngle; // Debug only
        // Roll ship towards target
        if (roll)
        {
            // Stop rotating when near target angle to prevent stutter
            if (Mathf.Abs(upDot) > 0.015f) // Number closer to 0 makes adjustment more accurate
            {
                // Roll Left
                if (upDot > 0f)
                {
                    transform.Rotate(Vector3.back * rollAngle * shipStats.RollSpeed * Time.deltaTime);
                }
                // Roll Right
                else
                {
                    transform.Rotate(Vector3.forward * rollAngle * shipStats.RollSpeed * Time.deltaTime);
                }
                rolling = true;
            }
            else
            {
                rolling = false;
            }
        }

        /* ***PITCH*** */
        // Pitch up or down to align agent z axis to target
        //float pitchAmount = Vector3.SignedAngle(transform.forward, cross, Vector3.forward);
        float pitchAmount = Vector3.Angle(transform.forward, cross);
        pitchAngleDebug = pitchAmount; // Debug only
        if (pitch)
        {
            // Rotate if target is behind or if target pitch angle is not yet near target angle
            if (Mathf.Abs(upToDirDot) > 0.015f || targetIsBehind) // Number closer to 0 makes adjustment more accurate
            {
                // Rotate if max distance reached. Makes flight look more convincing
                // Also rotate while target is in front and target pitch angle is not yet reached
                if (distanceFromTarget > distanceToEnableTurning || !targetIsBehind)
                {
                    // Pitch Up
                    if (upToDirDot > 0f)
                    {
                        transform.Rotate(Vector3.left * pitchAmount * shipStats.PitchSpeed * Time.deltaTime);
                    }
                    // Pitch Down
                    else
                    {
                        transform.Rotate(Vector3.right * pitchAmount * shipStats.PitchSpeed * Time.deltaTime);
                    }
                    pitching = true;
                }
            }
            else
            {
                pitching = false;
            }
        }

        /* ***YAW*** */
        // Rotate agent towards target
        //float yawAmount = Vector3.SignedAngle(transform.forward, dirToTarget, Vector3.forward);
        float yawAmount = Vector3.Angle(transform.forward, dirToTarget);
        yawAngleDebug = yawAmount; // Debug only
        if (yaw)
        {
            // Rotate if target yaw angle is not yet near target angle
            if (Mathf.Abs(rightToDirDot) > 0.01f || targetIsBehind) // Number closer to 0 makes adjustment more accurate
            {
                // Rotate if max distance reached. Makes flight look more convincing
                // Also rotate while target is in front and target yaw angle is not yet reached
                if (distanceFromTarget > distanceToEnableTurning || !targetIsBehind)
                {
                    // Yaw Left
                    if (rightToDirDot > 0f)
                    {
                        transform.Rotate(Vector3.up * yawAmount * shipStats.YawSpeed * Time.deltaTime);
                    }
                    // Yaw Right
                    else
                    {
                        transform.Rotate(Vector3.down * yawAmount * shipStats.YawSpeed * Time.deltaTime);
                    }
                    yawing = true;
                }
            }
            else
            {
                yawing = false;
            }
        }

        /* ***THRUST*** */
        ship.SetForwardSpeed = currentForwardSpeed;

        /* ***UNSTUCK*** */
        // Instant look to target to prevent death spiral bug
        //if (distanceFromTarget > 1000f && targetIsBehind && currentTarget != null)
        //{
        //    Debug.Log(gameObject.name + " prevented death spiral!");
        //    transform.LookAt(currentTarget.transform, transform.up);
        //}

        /** !DEBUG UI TEXT! **/
        if (debugMode)
        {
            rightDotText.SetText("RDot: " + rightDot.ToString());
            upDotText.SetText("UDot: " + upDot.ToString());
            upToDirDotText.SetText("UDirDot: " + upToDirDot.ToString());
            rightToDirDotText.SetText("RDirDot: " + rightToDirDot.ToString());
            targetBehindText.SetText("Target Behind?: " + targetIsBehind.ToString());
            targetAngle.SetText("Angle: " + forwardDot.ToString());
            rollingText.SetText("Rolling: " + rolling.ToString());
            pitchingText.SetText("Pitching: " + pitching.ToString());
            yawingText.SetText("Turning: " + yawing.ToString());
            forwardDotText.SetText("FDot: " + forwardDot.ToString());
            //crossText.SetText(cross.x + "\n" + cross.y + "\n" + cross.z);
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
        // Cross (of this object to target)
        Debug.DrawLine(transform.position, transform.position + (cross.normalized * 15f), Color.cyan);
        // Line to target flight vector
        Debug.DrawLine(transform.position, ai.GetFlightTargetVector(), Color.magenta);
    }

    private IEnumerator RandomizeDistance()
    {
        while (true)
        {
            if (!enableDistanceBeforeTurning)
            {
                distanceToEnableTurning = 0f;
                yield return null;
                continue;
            }
            if (!hasRandomized && !targetIsBehind)
            {
                float randomFactor = maxDistanceBeforeTurning - (maxDistanceBeforeTurning * randomizeFactor);
                distanceToEnableTurning = Random.Range(randomFactor, maxDistanceBeforeTurning);
                randomizeTimer = 0f;
                hasRandomized = true;
            }
            if (targetIsBehind)
            {
                hasRandomized = false;
                if (distanceFromTarget < distanceToEnableTurning)
                {
                    randomizeTimer += Time.deltaTime;
                }
            }
            if (randomizeTimer > 5f) // Timeout. Force AI to turn towards target vector.
            {
                distanceToEnableTurning = 0f;
                hasRandomized = true;
                randomizeTimer = 0f;
            }
            yield return null;
        }
    }

    public void ToggleDistanceBeforeTurning(bool arg)
    {
        enableDistanceBeforeTurning = arg;
    }

}
