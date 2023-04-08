using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SteeringWheel : MonoBehaviour
{
    public GameObject ship;
    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Boolean gripClick;
    public Transform wheel;
    public Transform centerWheel;
    private bool isGrabbing;
    public float damp = 1f;
    private float angle = 0.0f;
    private float offset;


    void resetRotation(){
        offset = angle;
    }

    private void FixedUpdate() {
        if (isGrabbing && gripClick.GetStateUp(handType)){
            isGrabbing = false;
            resetRotation();
        }
        if (isGrabbing && gripClick.GetState(handType)){
            float continuedAngle =  Mathf.Clamp(offset-angle, -45, 45);
            // Offset is used to remember the last held location and offset the rotation so it doesn't instantly spin to your hands rotation
            wheel.Rotate(new Vector3(0,1,0), -continuedAngle, Space.Self);
            if (offset != angle){
                // ship.transform.Rotate(new Vector3(0,1,0), -(continuedAngle), Space.World);
            }
            offset = angle;
        }
    }

    private void OnTriggerStay(Collider other) {
        if (gripClick.GetStateDown(handType) && other.name == "Wheel_Model"){
            Debug.Log("Steering Wheel contact");
            // The first time we press down store offset, otherwise continue calculating point to follow
            if (gripClick.GetStateDown(handType)){
                offset = angle;
            }
            isGrabbing = true;
            Vector3 dir = centerWheel.InverseTransformPoint(transform.position).normalized;
            // Steering wheel relative to up vector will rotate
            angle = Vector2.SignedAngle(Vector2.up, dir)/damp;
        }
        // // If we let go of the wheel, store the last location as offset
        // else if(SteamVR_Actions._default.GrabGrip.stateUp){
        //     resetRotation();
        // }
    }

    // private void OnTriggerExit(Collider other) {
    //     resetRotation();
    // }

}
