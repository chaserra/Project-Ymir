using UnityEngine;

public abstract class AI_BaseState
{
    public abstract void EnterState(AI_Brain brain);
    public abstract Vector3 Process(AI_Brain brain);
    public abstract void ExitState(AI_Brain brain);
}