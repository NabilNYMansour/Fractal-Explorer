using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class JoystickDriver : MonoBehaviour
{
    public GameObject ship;
    private Rigidbody shipRb;

    public Transform lever;
    public Transform baseHandle;
    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Boolean gripClick;
    // public Hand handRef;

    public GameObject handObj;
    public Transform pinchPoint;

    private float angle;
    public bool selected = false;
    private bool allowControls = false;



    // Start is called before the first frame update
    void Start()
    {
        shipRb = ship.GetComponent<Rigidbody>();
    }

    private void OnEnable() {
        GameSingleton.gameStart += setAllowControls;
    }
    private void OnDisable() {
        GameSingleton.gameStart += setAllowControls;
    }

    public void setAllowControls(){
        allowControls = true;
    }
    private void FixedUpdate() {
        if (selected && gripClick.GetStateUp(handType)){
            selected = false;
            handObj.transform.position = transform.position;

            shipRb.velocity = new Vector3(0,0,0);
            shipRb.angularVelocity = new Vector3(0,0,0);
            lever.transform.localRotation = Quaternion.identity;
            
        }
  
        if (selected && gripClick.GetState(handType)){
            angle = calculateAngle();
            handObj.transform.position = pinchPoint.position;
        
            if (allowControls){
                shipRb.AddTorque(ship.transform.up * -angle * 2);
                shipRb.velocity = ship.transform.forward * 5f;
            }

            lever.transform.Rotate(new Vector3(0,0,1), angle);
        }

    }

    private float calculateAngle(){
        Vector3 dir = baseHandle.transform.InverseTransformPoint(transform.position).normalized;
        return Mathf.Clamp(Vector2.SignedAngle(Vector2.up, dir), -1, 1);
    }

    private void OnTriggerStay(Collider other) {
        if (gripClick.GetStateDown(handType) && other.name == "SteerTop" ){
            selected = true;
            angle = calculateAngle();
        }
    }


}
