using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ExitManager : MonoBehaviour
{
    public static event Action gameEnd;

 

    private void OnTriggerEnter(Collider other) {
        if (other.name == "PlayerParent"){
            gameEnd.Invoke();
        }
    }
}
