using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ButtonHandler : MonoBehaviour
{
    public BoxCollider mainBody;
    public BoxCollider triggerBody;
    public static event Action startGame;

    public bool isPressed;
    void FixedUpdate()
    {
        isPressed = mainBody.bounds.Intersects(triggerBody.bounds);
        if (isPressed){
            startGame.Invoke();
            Destroy(gameObject);
        }
    }
}
