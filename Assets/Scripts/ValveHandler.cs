using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(HingeJoint))]
public class ValveHandler : MonoBehaviour
{
    public bool beingUsed = false;
    public float angle = 0f;

    private HingeJoint joint;
    private void Start()
    {
        joint = GetComponent<HingeJoint>();
    }

    private void OnCollisionStay(Collision collision)
    {
        SteamVR_Actions._default.Haptic.Execute(0f, 0.1f, 100f, 0.1f, SteamVR_Input_Sources.LeftHand); // haptic input
        SteamVR_Actions._default.Haptic.Execute(0f, 0.1f, 100f, 0.1f, SteamVR_Input_Sources.RightHand); // haptic input

        beingUsed = true;
        angle = joint.angle;
    }

    private void OnCollisionExit(Collision collision)
    {
        beingUsed = false;
    }
}
