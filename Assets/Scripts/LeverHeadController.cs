using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(ShowOutline))]
public class LeverHeadController : MonoBehaviour
{
    public ColliderTrigger selectCollider;
    public bool beingUsed = false;

    private Rigidbody head;
    private ShowOutline outline;

    private bool physicsCollided = false;
    private void Start()
    {
        head = GetComponent<Rigidbody>();
        outline = GetComponent<ShowOutline>();
        StartCoroutine(LateStart(0.001f));
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        head.constraints = RigidbodyConstraints.FreezePosition;
    }

    private void OnCollisionStay(Collision collision)
    {
        head.constraints = RigidbodyConstraints.None;
        physicsCollided = true;

        beingUsed = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        head.constraints = RigidbodyConstraints.FreezePosition;
        physicsCollided = false;

        beingUsed = false;
    }

    void FixedUpdate()
    {
        if (selectCollider.other != null && 
            (SteamVR_Actions._default.SelectLeft.state && selectCollider.inputSource == -1) || 
            (SteamVR_Actions._default.SelectRight.state && selectCollider.inputSource == 1))
        {
            outline.ShowO();
            head.constraints = RigidbodyConstraints.None;
            Vector3 handMotion = selectCollider.other.transform.position - head.transform.position;
            head.AddForce(handMotion * 1000f);

            beingUsed = true;
        }
        else
        {
            if (!physicsCollided) {
                head.constraints = RigidbodyConstraints.FreezePosition;
            }
            outline.HideO(false);
            beingUsed = false;
        }
    }
}
