using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(WeaponsController))]
public class AutoTargetSystem : MonoBehaviour
{
    // Cache
    private WeaponsController weaponsController;
    private Camera cam;
    private Canvas hud;
    private Canvas overlayCanvas;
    private UI_DisplayTargetBox targettingBoxOverlay;

    // Properties
    [Header("AT Weapon")]
    [SerializeField] RectTransform autoTargetBox;
    [SerializeField] float autoTargetRange = 150f;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] RectTransform autoTargetUI;

    // Attributes
    private float forwardOffset = 3f;
    private float offsetMultiplier = 1.25f;
    private float checkSphereOffset;
    public Canvas HUD { get { return hud; } }

    // State
    private List<ITarget> currentActiveTargets = new List<ITarget>();
    [SerializeField] public GameObject primaryTarget = null;
    [SerializeField] private int currentTargetIndex = 0;

    [Header("DEBUG")]
    [SerializeField] TextMeshProUGUI active;

    /* ** DEBUG ** */
    private void OnDrawGizmosSelected()
    {
        checkSphereOffset = autoTargetRange * offsetMultiplier;
        // Display CheckSphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + transform.forward * (checkSphereOffset + forwardOffset), autoTargetRange * offsetMultiplier);
        // Display forward aim
        //Gizmos.color = Color.white;
        //Gizmos.DrawLine(transform.position, transform.position + transform.forward * autoTargetRange * offsetMultiplier);
        //Display OverlapSphere
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * (autoTargetRange + forwardOffset), autoTargetRange);
    }

    private void Awake()
    {
        weaponsController = GetComponent<WeaponsController>();
        // Get HUD and Overlay Canvases
        Canvas[] c = FindObjectsOfType<Canvas>();
        for (int i = c.Length - 1; i >= 0; i--)
        {
            if (c[i].CompareTag("HUD"))
            {
                hud = c[i];
                continue;
            }
            if (c[i].CompareTag("UI Overlay"))
            {
                overlayCanvas = c[i];
                continue;
            }
        }
        targettingBoxOverlay = overlayCanvas.GetComponent<UI_DisplayTargetBox>();
        checkSphereOffset = autoTargetRange * offsetMultiplier;
        Instantiate(autoTargetBox, hud.transform);
    }

    private void Start()
    {
        cam = weaponsController.Camera;
    }

    private void Update()
    {
        // Obtain targets and display HUD icons
        if (TargetsInRange())
        {
            GetTargetsInsideAT(IdentifyTargets());
        }
        DisplayTargetBoxes();
        active.SetText(currentActiveTargets.Count.ToString());

        // Select target
        CycleTargets();
    }

    /* LIGHTWEIGHT DETECT */
    private bool TargetsInRange()
    {
        // Check if a target exists on a large area in front of ship
        // If none found, do nothing to save memory
        if (Physics.CheckSphere(transform.position + transform.forward * (checkSphereOffset + forwardOffset), autoTargetRange * offsetMultiplier, targetLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /* UI-BASED DETECT */
    private void GetTargetsInsideAT(List<ITarget> targets)
    {
        for (int i = targets.Count - 1; i >= 0; i--)
        {
            // Check where the target is in relation to the screen
            Vector3 screenPos = cam.WorldToScreenPoint(targets[i].ThisGameObject().transform.position);

            if (screenPos.z < 0) { continue; }

            // Convert to percentage of screen
            Vector3 delta = new Vector3(screenPos.x / Screen.width, screenPos.y / Screen.height, 0f);

            // Check if target is within AT box and in front of ship
            if (delta.x > autoTargetBox.anchorMin.x &&
                delta.x < autoTargetBox.anchorMax.x &&
                delta.y > autoTargetBox.anchorMin.y &&
                delta.y < autoTargetBox.anchorMax.y)
            {
                // Add to current active targets if inside AT box
                if (!currentActiveTargets.Contains(targets[i]))
                {
                    currentActiveTargets.Add(targets[i]);

                    // Set first target as primary target if no current primary is active
                    if (primaryTarget == null)
                    {
                        SetPrimaryTarget(targets[i].ThisGameObject());
                    }
                }
            }
            // If outside AT box, remove from current targets and trigger UI disable
            else
            {
                RemoveTargetting(targets[i]);
                currentActiveTargets.Remove(targets[i]);
            }
        }
    }

    /* RAYCAST AND SPHERECAST DETECT */
    private List<ITarget> IdentifyTargets()
    {
        // Do a more granular target acquisition via OverlapSphere
        // This has a shorter range and area than the first check above
        List<ITarget> activeTargets = new List<ITarget>();
        Collider[] targets = Physics.OverlapSphere(transform.position + transform.forward * (autoTargetRange + forwardOffset), autoTargetRange, targetLayer);
        for (int i = 0; i < targets.Length; i++)
        {
            // Ignore self
            if (targets[i].gameObject == this.gameObject) { continue; }

            // Check if in line of sight
            RaycastHit hit;
            Vector3 dirToTarget = targets[i].transform.position - transform.position;
            if (Physics.Raycast(transform.position, dirToTarget, out hit, autoTargetRange * 2f, ~ignoreLayer))
            {
                ITarget thisTarget;
                // Check if nothing is blocking the ray
                if (hit.collider.gameObject == targets[i].gameObject)
                {
                    if (hit.collider.TryGetComponent(out thisTarget))
                    {
                        if (!activeTargets.Contains(thisTarget))    // Check if already targetted
                        {
                            activeTargets.Add(thisTarget);          // Add to list
                        }
                    }
                }
                // If something is blocking the target, remove blocked target
                else
                {
                    if (targets[i].TryGetComponent(out thisTarget))
                    {
                        // Only remove if the blocked target is in the currentActiveTargets List
                        if (currentActiveTargets.Contains(thisTarget))
                        {
                            currentActiveTargets.Remove(thisTarget);
                            RemoveTargetting(thisTarget);
                        }
                    }
                }
            }
        }
        return activeTargets;
    }

    /* DISPLAY UI */
    private void DisplayTargetBoxes()
    {
        if (currentActiveTargets.Count <= 0) { return; }

        foreach (ITarget target in currentActiveTargets)
        {
            // If target is destroyed, reset targets list and break loop
            if (target == null || target.Equals(null))
            {
                currentActiveTargets.Clear();
                break;
            }

            // Vector3 Dot to check if object is in front of the ship
            Vector3 dirToTarget = (target.ThisGameObject().transform.position - transform.position).normalized;
            float dotCheck = Vector3.Dot(dirToTarget, transform.forward);

            /*Display UI*/
            // If in front of ship
            if (dotCheck > 0f)
            {
                // Get target's position and size on screen
                Rect screenPos = target.GetTargetScreenPos(this.gameObject);

                // Call overlay canvas and pass on screenPos(Rect) obtained from GetTargetScreenPos
                targettingBoxOverlay.DisplayTargetBox(screenPos, target.ThisGameObject());
            }
            // If behind ship
            else
            {
                // Remove UI then reset targets list and break loop
                RemoveTargetting(target);
                currentActiveTargets.Clear();
                break;
            }
        }
    }

    /* TARGET SELECTION */
    private void CycleTargets()
    {
        // Cycle through targets
        // For single target ATs only!
        if (currentActiveTargets.Count > 1)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                currentTargetIndex++;
                GetNextTarget();
            }
        }
        else
        {
            // Reset index and remove last primary target if no targets are found
            currentTargetIndex = 0;
            if (currentActiveTargets.Count <= 0 && primaryTarget != null)
            {
                primaryTarget = null;
            }
        }
    }

    private void GetNextTarget()
    {
        // If last target or the only target, return first index
        if (currentTargetIndex > currentActiveTargets.Count - 1 ||
            currentActiveTargets.Count <= 1)
        {
            currentTargetIndex = 0;
        }
        // TODO: Find a better way to display primary target. Find alternative to changing image color

        // Deactivate previous primary target
        if (primaryTarget != null)
        {
            targettingBoxOverlay.DisplayPrimaryTarget(primaryTarget, false);
        }

        // Display new target as primary
        SetPrimaryTarget(currentActiveTargets[currentTargetIndex].ThisGameObject());
    }

    /* DATA MANAGEMENT */
    private void SetPrimaryTarget(GameObject target)
    {
        primaryTarget = target;
        targettingBoxOverlay.DisplayPrimaryTarget(primaryTarget, true);
    }

    public void ClearAllTargets()
    {
        if (currentActiveTargets.Count <= 0) { return; }
        foreach (ITarget target in currentActiveTargets)
        {
            // If target is destroyed, reset targets list and break loop
            if (target == null || target.Equals(null))
            {
                currentActiveTargets.Clear();
                break;
            }
            RemoveTargetting(target);
        }
        primaryTarget = null;
        currentActiveTargets.Clear();
        currentTargetIndex = 0;
    }

    private void RemoveTargetting(ITarget target)
    {
        target.NoLongerTargetted();
        targettingBoxOverlay.RemoveTargetting(target.ThisGameObject());

        // If the removed target was the primary target
        if (primaryTarget == target.ThisGameObject())
        {
            // Remove current primary target
            primaryTarget = null;

            // Check for other targets to transfer primary target status
            if (currentActiveTargets.Count > 0)
            {
                GetNextTarget();
            }
        }
    }

}
