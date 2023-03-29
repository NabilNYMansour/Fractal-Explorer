using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    public BoxCollider mainBody;
    public BoxCollider triggerBody;

    public bool isPressed;
    void FixedUpdate()
    {
        isPressed = mainBody.bounds.Intersects(triggerBody.bounds);
    }
}
