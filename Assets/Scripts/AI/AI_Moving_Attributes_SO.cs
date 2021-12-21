using UnityEngine;

[CreateAssetMenu(fileName = "Moving AI Attributes", menuName = "AI Constructor/Moving AI")]
public class AI_Moving_Attributes_SO : ScriptableObject
{
    public float agentReactionTime = 0.275f;
    public float targetScannerRadius = 200f;
    public float forwardTargetSelection = 100f;
    public float forwardDisplacementRadius = 80f;
    public float slowingRadius = 75f;
    public float wanderMaxDistance = 750f;
}