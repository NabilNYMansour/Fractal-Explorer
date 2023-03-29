using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Collider))]
public class ColliderTrigger : MonoBehaviour
{
    public Collider other;
    public int inputSource;
    private void OnTriggerStay(Collider other)
    {
        this.other = other;
        Transform parent = other.gameObject.transform.parent;

        if (parent != null && parent.parent != null)
        {
            string grandParentName = parent.parent.name;
            Debug.Log(grandParentName);

            if (grandParentName.Contains("left", StringComparison.OrdinalIgnoreCase))
            {
                SteamVR_Actions._default.Haptic.Execute(0f, 0.1f, 100f, 0.1f, SteamVR_Input_Sources.LeftHand); // haptic input
                inputSource = -1;
            }
            else if (grandParentName.Contains("right", StringComparison.OrdinalIgnoreCase))
            {
                SteamVR_Actions._default.Haptic.Execute(0f, 0.1f, 100f, 0.1f, SteamVR_Input_Sources.RightHand); // haptic input
                inputSource = 1;
            }
            else
            {
                inputSource = 0;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        this.other = null;
        inputSource = 0;
    }
}
