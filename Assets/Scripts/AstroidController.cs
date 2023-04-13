using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AstroidController : MonoBehaviour
{

    public static event Action damageEvent;
    private Vector3 targetDir;
    private float startingRand;
    // Start is called before the first frame update
    void Start()
    {
        startingRand = UnityEngine.Random.Range(0, 20);
        targetDir = startingRand > 10 ? Vector3.up : Vector3.forward;
    }

    private void FixedUpdate() {
        transform.RotateAround(new Vector3(0,0,0), targetDir, 10f * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {
        if (damageEvent != null && other.name == "PlayerParent"){
            damageEvent.Invoke();
        }
    }
}
