using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SteeringWheel : MonoBehaviour
{
    public GameObject ship;
    public Transform rightHand;
    public LinearMapping lm;
    public float damp = 1f;
    private float angle = 0.0f;
    private float offset;


    void resetRotation(){
        offset=angle;
    }

    private void FixedUpdate() {
        Debug.Log(offset - angle);
        float continuedAngle =  Mathf.Clamp(offset-angle, -45, 45);
        
        // Offset is used to remember the last held location and offset the rotation so it doesn't instantly spin to your hands rotation
        transform.Rotate(new Vector3(0,0,1), -continuedAngle, Space.Self);
        if (offset != angle){
            ship.transform.Rotate(new Vector3(0,1,0), -(continuedAngle), Space.World);
        }
        offset = angle;
    }

    private void OnTriggerStay(Collider other) {
        if (SteamVR_Actions._default.GrabGrip.state){
            // The first time we press down store offset, otherwise continue calculating point to follow
            if (SteamVR_Actions._default.GrabGrip.stateDown){
                offset = angle;
            }
            Vector3 dir = transform.InverseTransformPoint(rightHand.position);
            // Steering wheel relative to up vector will rotate
            angle = Vector2.SignedAngle(Vector2.up, dir)/damp;
        }
        // If we let go of the wheel, store the last location as offset
        else if(SteamVR_Actions._default.GrabGrip.stateUp){
            resetRotation();

        }
    }

    private void OnTriggerExit(Collider other) {
        resetRotation();
    }

}
