using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerMoveDEBUG : MonoBehaviour
{
    public GameObject head;
    void Update()
    {
        if (SteamVR_Actions._default.ShootLaserPointerLeft.state)
        {
            this.transform.position += head.transform.forward / 4;
        }
    }
}
