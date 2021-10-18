using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DisplayTargetBox : MonoBehaviour
{
    // Cache
    private ObjectPooler autoTargetBoxPool;

    // Parameters
    [SerializeField] RectTransform autoTargetUI;
    public RectTransform SetAutoTargetUI { set { autoTargetUI = value; } }

    // State
    private Dictionary<GameObject, RectTransform> activeTargets = new Dictionary<GameObject, RectTransform>();

    private void Awake()
    {
        autoTargetBoxPool = new ObjectPooler(autoTargetUI.gameObject, this.gameObject);
    }

    private void Update()
    {
        CleanInactiveTargets();     // Remove targetting for destroyed targets
    }

    public void DisplayTargetBox(Rect sizeAndPosOnScreen, GameObject target)
    {
        RectTransform autoTarget;   // Reference only

        // If target is not found in the dictionary (not an already-active target)
        if (!activeTargets.ContainsKey(target))
        {
            // Get a pooled auto-target box
            GameObject obj = autoTargetBoxPool.GetPooledObject();
            // Get the auto-target box's RectTransform
            autoTarget = obj.GetComponent<RectTransform>();

            // If this is the first active target, set as primary
            if (activeTargets.Count < 1)
            {
                autoTarget.GetComponent<Image>().color = Color.green;
            }

            // Add this target-ui pair to the dictionary
            activeTargets.Add(target, autoTarget);
        }
        // If target is already in the dictionary
        else
        {
            // Get the auto-target box's RectTransform
            activeTargets.TryGetValue(target, out autoTarget);
        }

        // Adjust Auto Target Box position and size to display on the screen
        autoTarget.position = new Vector2(sizeAndPosOnScreen.xMin, sizeAndPosOnScreen.yMin);
        autoTarget.sizeDelta = new Vector2(sizeAndPosOnScreen.width, sizeAndPosOnScreen.height);
        autoTarget.gameObject.SetActive(true);
    }

    public void DisplayPrimaryTarget(GameObject target, bool isPrimary)
    {
        if (activeTargets.ContainsKey(target))
        {
            RectTransform autoTarget;
            activeTargets.TryGetValue(target, out autoTarget);
            if (isPrimary)
            {
                autoTarget.GetComponent<Image>().color = Color.green;
            }
            else
            {
                autoTarget.GetComponent<Image>().color = Color.red;
            }
        }
    }

    public void RemoveTargetting(GameObject target)
    {
        // Disable UI target box display on screen for a specific target
        if (activeTargets.ContainsKey(target))
        {
            RectTransform autoTarget;
            activeTargets.TryGetValue(target, out autoTarget);
            autoTarget.GetComponent<Image>().color = Color.red;
            autoTarget.gameObject.SetActive(false);
            activeTargets.Remove(target);
        }
    }

    private void CleanInactiveTargets()
    {
        if (activeTargets.Count <= 0) { return; }
        // Check dictionary for any destroyed (null) targets. Remove targetting for each found.
        foreach (KeyValuePair<GameObject, RectTransform> item in activeTargets)
        {
            if (item.Key == null)
            {
                RemoveTargetting(item.Key);
                break;
            }
        }
    }

}