using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SpeedDriver : MonoBehaviour
{

    public GameObject ship;
    public float speed = 2f;
    public LinearMapping lm;

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log("SAM: " + lm.value);
        if (lm.value > 0){
            
            ship.transform.position += ship.transform.forward * lm.value * speed;
        }
    }
}
