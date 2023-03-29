using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Valve.VR;

public class TableController : MonoBehaviour
{
    public PlayerController player;

    // Button
    public ButtonHandler buttonIS; // increase size
    private bool hasPressedIS = false;


    public ButtonHandler buttonDS; // decrease size
    private bool hasPressedDS = false;

    // Lever
    public LeverHandler leverForward;

    // Valve
    public ValveHandler valve;

    // Screen
    public TextMeshProUGUI textMeshPro;

    void Update()
    {
        if (player.selectedObject != null)
        {
            HandleButtonSizeHit(buttonIS, ref hasPressedIS, 1.1f);
            HandleButtonSizeHit(buttonDS, ref hasPressedDS, 0.9f);

            if (leverForward.beingUsed)
            {
                player.selectedObject.transform.position += player.selectedObject.transform.forward * leverForward.ratio / 10f;
            }

            if (valve.beingUsed)
            {
                Vector3 rotation = player.selectedObject.transform.rotation.eulerAngles;
                player.selectedObject.transform.eulerAngles = new Vector3(rotation.x, valve.angle, rotation.z);
            }

            textMeshPro.text = "name:" + player.selectedObject.name + "\n" +
                               "position:" + player.selectedObject.position + "\n" +
                               "rotation:" + player.selectedObject.rotation.eulerAngles + "\n" +
                               "scale:" + player.selectedObject.localScale;
        } else
        {
            textMeshPro.text = "No Object Selected";
        }
    }

    void HandleButtonSizeHit(ButtonHandler button, ref bool hasPressed, float scale)
    {
        if (button.isPressed && !hasPressed)
        {
            SteamVR_Actions._default.Haptic.Execute(0f, 0.1f, 100f, 0.1f, SteamVR_Input_Sources.LeftHand); // haptic input
            SteamVR_Actions._default.Haptic.Execute(0f, 0.1f, 100f, 0.1f, SteamVR_Input_Sources.RightHand); // haptic input

            player.selectedObject.localScale *= scale;
            hasPressed = true;
        }
        else if (!button.isPressed)
        {
            hasPressed = false;
        }
    }
}
