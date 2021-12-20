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
    /* Brain constructor stuff */
    // TODO: Probably make this a ScriptableObject if number of parameters start to bloat
    [SerializeField] float targetScannerRadius = 200f;
    [SerializeField] float forwardTargetSelection = 100f;
    [SerializeField] float forwardDisplacementRadius = 80f;
    [SerializeField] float slowingRadius = 75f;
    [SerializeField] float wanderMaxDistance = 1000f;

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
    public bool debugMode = false;
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
        ai = new AI_Brain(this, ship, targetScannerRadius, forwardTargetSelection, 
            forwardDisplacementRadius, slowingRadius, wanderMaxDistance);
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

        if (debugMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Time.timeScale = .25f;
            }
            if (Input.GetMouseButtonDown(1))
            {
                Time.timeScale = 1f;
            }
        }
    }

    private void Move(Vector3 vectorToSteerTowards)
    {
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

        #region ROLL
        // Roll ship towards target
        // Get amount of angle to rotate
        float rollAngle = Vector3.Angle(transform.right, cross);
        rollAngleDebug = rollAngle; // Debug only
        if (roll)
        {
            // Stop rotating when near target angle to prevent stutter
            if (Mathf.Abs(upDot) > 0.015f) // Number closer to 0 makes adjustment more accurate
            {
                // Roll Left
                if (upDot > 0f)
                {
                    transform.Rotate(Vector3.back * shipStats.RollSpeed * Time.deltaTime);
                }
                // Roll Right
                else
                {
                    transform.Rotate(Vector3.forward * shipStats.RollSpeed * Time.deltaTime);
                }
                rolling = true;
            }
            else
            {
                rolling = false;
            }
        }
        #endregion

        #region PITCH
        // Pitch up or down to align agent z axis to target
        float pitchAngle = Vector3.Angle(transform.forward, cross);
        pitchAngleDebug = pitchAngle; // Debug only
        if (pitch)
        {
            // Rotate if target is behind or if target pitch angle is not yet near target angle
            if (Mathf.Abs(upToDirDot) > 0.015f || targetIsBehind) // Number closer to 0 makes adjustment more accurate
            {
                // Rotate if max distance reached. Makes flight look more convincing
                // Also rotate while target is in front and target pitch angle is not yet reached
                if (!enableDistanceBeforeTurning || 
                    distanceFromTarget > distanceToEnableTurning || 
                    !targetIsBehind)
                {
                    // Pitch Up
                    if (upToDirDot > 0f)
                    {
                        transform.Rotate(Vector3.left * shipStats.PitchSpeed * Time.deltaTime);
                    }
                    // Pitch Down
                    else
                    {
                        transform.Rotate(Vector3.right * shipStats.PitchSpeed * Time.deltaTime);
                    }
                    pitching = true;
                }
            }
            else
            {
                pitching = false;
            }
        }
        #endregion

        #region YAW
        // Rotate agent towards target
        float yawAngle = Vector3.Angle(transform.forward, dirToTarget);
        yawAngleDebug = yawAngle; // Debug only
        if (yaw)
        {
            // Rotate if target yaw angle is not yet near target angle
            if (Mathf.Abs(rightToDirDot) > 0.01f || targetIsBehind) // Number closer to 0 makes adjustment more accurate
            {
                // Rotate if max distance reached. Makes flight look more convincing
                // Also rotate while target is in front and target yaw angle is not yet reached
                if (!enableDistanceBeforeTurning || 
                    distanceFromTarget > distanceToEnableTurning || 
                    !targetIsBehind)
                {
                    // Yaw Left
                    if (rightToDirDot > 0f)
                    {
                        transform.Rotate(Vector3.up * shipStats.YawSpeed * Time.deltaTime);
                    }
                    // Yaw Right
                    else
                    {
                        transform.Rotate(Vector3.down * shipStats.YawSpeed * Time.deltaTime);
                    }
                    yawing = true;
                }
            }
            else
            {
                yawing = false;
            }
        }
        #endregion

        #region THRUST
        transform.Translate(Vector3.forward * currentForwardSpeed * Time.deltaTime);
        ship.SetForwardSpeed = currentForwardSpeed;
        #endregion

        #region DEBUG
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
        if (currentTarget != null)
        {
            Vector3 dir = (currentTarget.transform.position - transform.position).normalized;
            Debug.DrawLine(transform.position, transform.position + (dir * 15f), Color.yellow);
        }
        // Cross (of this object to target)
        Debug.DrawLine(transform.position, transform.position + (cross.normalized * 15f), Color.cyan);
        // Line to target flight vector
        Debug.DrawLine(transform.position, ai.GetFlightTargetVector(), Color.magenta);
        #endregion
    }

    private IEnumerator RandomizeDistance()
    {
        while (true)
        {
            // If momentum turning is disabled, allow to turn immediately
            //if (!enableDistanceBeforeTurning)
            //{
            //    distanceToEnableTurning = 0f;
            //    yield return null;
            //    continue;
            //}
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

    public void AdjustSpeedByDistance(float distanceMultiplier)
    {
        // Multiply max possible speed to ratio (0~1) between ship position and target position
        float newSpeed = shipStats.MaxSpeed * distanceMultiplier;
        currentForwardSpeed = Mathf.Clamp(newSpeed, shipStats.MinSpeed, shipStats.MaxSpeed);
    }

    public void AdjustSpeedByThrusterSpeed(bool sign)
    {
        float multiplier;
        if (sign) { multiplier = 1f; } else { multiplier = -1f; }

        float newSpeed = currentForwardSpeed + shipStats.ThrusterSpeed * multiplier * Time.deltaTime;
        currentForwardSpeed = Mathf.Clamp(newSpeed, shipStats.MinSpeed, shipStats.MaxSpeed);
    }

}