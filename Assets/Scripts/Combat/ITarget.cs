using UnityEngine;

public interface ITarget
{
    GameObject ThisGameObject();
    void IsHit(int damage);
    Rect GetTargetScreenPos(GameObject source);
    void NoLongerTargetted();
}
