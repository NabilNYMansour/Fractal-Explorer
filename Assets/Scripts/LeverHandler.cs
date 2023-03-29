using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverHandler : MonoBehaviour
{
    public LeverHeadController head;
    public float ratio = 0f;

    public bool beingUsed = false;
    void FixedUpdate()
    {
        if (head.beingUsed)
        {
            Vector3 forwardVector = this.transform.forward;
            Vector3 direction = (head.transform.position - this.transform.position).normalized;
            ratio = Vector3.Dot(forwardVector, direction);
        }

        this.beingUsed = head.beingUsed;
    }
}
