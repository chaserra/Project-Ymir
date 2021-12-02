using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Ymir.ScreenHelper;

[RequireComponent(typeof(Target))]
public class ShipController : MonoBehaviour
{
    // Cache
    private Target ship;
    private ShipStats shipStats;
    private Camera cam;
    private CinemachineImpulseSource impulseSource;
    private ScreenSizeHelper screen;

    // Properties
    [Header("Controls")]
    [SerializeField] bool mouseControls = false;
    [SerializeField] bool invertY = false;
    [Tooltip("% of screen from the middle where movement will not be detected.")]
    [SerializeField] float screenDeadZone = 0.01f;

    // Attributes
    private float multFactor = 10f;     // Multiplier factor so variable inputs can be smaller numbers        

    // State
    [SerializeField] private float activeForwardSpeed = 12f;    // TODO: Remove SerializeField
    public float CurrentForwardSpeed { get { return activeForwardSpeed * Time.deltaTime; } }
    [Header("DEBUG")]
    private float roll, pitch, thrust, yaw;

    private void Awake()
    {
        ship = GetComponent<Target>();
        shipStats = (ShipStats)ship.TargetStats;
        cam = Camera.main;
        impulseSource = GetComponent<CinemachineImpulseSource>();
        screen = new ScreenSizeHelper();
    }

    private void Start()
    {
        if (mouseControls) { Cursor.lockState = CursorLockMode.Confined; }
        screen.InitializeScreenCenter();    // Aiming reticle center. Used for mouse movement delta.
    }

    private void Update()
    {
        screen.AdjustForScreenResolutionChange();   // Check for any changes in the screen size. Prevents mouse delta position bugs.
        GetInput();
        Move();
    }

    private void LateUpdate()
    {
        
    }

    private void GetInput()
    {
        // Mouse Controls
        if (mouseControls)
        {
            Vector3 rawDelta = Input.mousePosition - screen.ScreenCenter;
            Vector3 delta = new Vector3(rawDelta.x / Screen.width, rawDelta.y / Screen.height, rawDelta.z);
            
            roll = Input.GetAxis("Horizontal");         // Roll
            if (Mathf.Abs(delta.y) > screenDeadZone)    // Pitch
            { pitch = Mathf.Clamp((delta.y * 2f) - screenDeadZone, -1f, 1f); }
            if (Mathf.Abs(delta.x) > screenDeadZone)    // Yaw
            { yaw = Mathf.Clamp((delta.x * 2f) - screenDeadZone, -1f, 1f); }
            thrust = Input.GetAxis("Vertical");         // Thrust
            if (!invertY) { pitch *= -1; }              // Invert pitch
        }
        // Keyboard Controls
        else
        {
            roll = Input.GetAxis("Horizontal");
            pitch = Input.GetAxis("Vertical");
            yaw = Input.GetAxis("Yaw");
            thrust = Input.GetAxis("Thrust");
            if (invertY) { pitch *= -1; }              // Invert pitch
        }

        // Thrust camera noise effect
        if (thrust > 0f && activeForwardSpeed != shipStats.MaxSpeed)
        {
            impulseSource.GenerateImpulse(cam.transform.forward);
        }
    }

    private void Move()
    {
        transform.Rotate(Vector3.back * roll * shipStats.RollSpeed * multFactor * Time.deltaTime);     // Roll
        transform.Rotate(Vector3.right * pitch * shipStats.PitchSpeed * multFactor * Time.deltaTime);  // Pitch
        transform.Rotate(Vector3.up * yaw * shipStats.YawSpeed * multFactor * Time.deltaTime);         // Yaw

        activeForwardSpeed += thrust * shipStats.ThrusterSpeed * Time.deltaTime;   // Add thrust
        activeForwardSpeed = Mathf.Clamp(activeForwardSpeed, shipStats.MinSpeed, shipStats.MaxSpeed);   // Clamp current speed
        transform.Translate(Vector3.forward * activeForwardSpeed * Time.deltaTime); // Thrust
    }

}