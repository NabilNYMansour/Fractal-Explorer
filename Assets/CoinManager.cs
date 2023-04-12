using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CoinManager : MonoBehaviour
{

    public static event Action collectEvent;

    private void Awake() {
       // Set  
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(new Vector3(0,1,0), Time.deltaTime * 20f, Space.World);
    }

    private void OnTriggerEnter(Collider other) {
        if (collectEvent != null && other.name == "PlayerParent"){
            collectEvent.Invoke();
        }
        Destroy(gameObject);
    }
}
