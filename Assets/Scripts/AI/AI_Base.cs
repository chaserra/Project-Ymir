using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float currentSpeed = 2f;
    [SerializeField] private GameObject currentTarget;

    private void Awake()
    {
        
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        if (currentTarget == null) { return; }
        Vector3 distToTarget = currentTarget.transform.position - transform.position;
        Vector3 dirToTarget = distToTarget.normalized;

        // TODO - roll and pitch adjust
        float targetAngleY = Mathf.Atan2(dirToTarget.x, dirToTarget.z) * Mathf.Rad2Deg;

        float angleY = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngleY, ref turnSmoothVelocity, turnSmoothTime / yawSpeed);
        
        transform.rotation = Quaternion.Euler(0f, angleY, 0f);
    }

    //TODO: Create the following Base AI Methods
    //1. Seek()
    //2. Flee() -> Reverse of Seek. Switch around target pos and agent pos when Distance checking
    //3. Wander()
    //4. Pursue() -> Seek with lookahead
    //5. Avoid() -> Reverse of Pursue
    //Don't think of other stuff for now
}
