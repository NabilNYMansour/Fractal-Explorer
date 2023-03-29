using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Valve.VR;

public class PlayerController : MonoBehaviour
{
    public Transform handL;
    public Transform handR;

    public LineRenderer lineL;
    public LineRenderer lineR;

    private float laserDis = 100f;
    private ShowOutline prevSO = null;
    private ShowOutline selectedSO = null;
    public Transform selectedObject = null;

    private void Start()
    {
        lineL.enabled = false;
        lineR.enabled = false;
    }

    void Update()
    {
        // left
        if (!lineR.enabled)
        {
            if (SteamVR_Actions._default.ShootLaserPointerLeft.stateDown)
            {
                lineL.enabled = true;
            }
            if (SteamVR_Actions._default.ShootLaserPointerLeft.state)
            {
                lineL.enabled = true;
                shootLaser(handL, lineL, SteamVR_Actions._default.SelectLeft.state);
            }
            // left up
            if (SteamVR_Actions._default.ShootLaserPointerLeft.stateUp)
            {
                lineL.enabled = false;
                if (prevSO != null)
                {
                    prevSO.HideO(prevSO == selectedSO);
                    prevSO = null;
                }
            }
        }

        if (!lineL.enabled)
        {
            // right
            if (SteamVR_Actions._default.ShootLaserPointerRight.stateDown)
            {
                lineR.enabled = true;
            }
            if (SteamVR_Actions._default.ShootLaserPointerRight.state)
            {
                lineR.enabled = true;
                shootLaser(handR, lineR, SteamVR_Actions._default.SelectRight.state);
            }
            // right up
            if (SteamVR_Actions._default.ShootLaserPointerRight.stateUp)
            {
                lineR.enabled = false;
                if (prevSO != null)
                {
                    prevSO.HideO(prevSO == selectedSO);
                    prevSO = null;
                }
            }
        }

        if (SteamVR_Actions._default.Deselect.stateDown)
        {
            if (selectedSO != null)
            {
                selectedSO.HideO(prevSO == selectedSO);
                selectedSO = null;
                selectedObject = null;
            }
        }
    }

    private void shootLaser(Transform hand, LineRenderer line, bool isSelecting)
    {
        Vector3 laserDir = Quaternion.AngleAxis(-30, hand.right) * hand.up;

        Vector3 lo = hand.position - hand.forward / 32f; //origin
        Vector3 le = hand.position - laserDir * laserDis; //end
        line.SetPosition(0, lo);
        line.SetPosition(1, le);

        RaycastHit hit;
        if (Physics.Raycast(lo, -laserDir, out hit, laserDis))
        {
            ShowOutline newSO = hit.transform.gameObject.GetComponent<ShowOutline>();
            if (prevSO == null && newSO != null)
            {
                newSO.ShowO();
                prevSO = newSO;
            }

            else if (isSelecting && newSO != null)
            {
                if (selectedSO != null) selectedSO.HideO(prevSO == selectedSO);
                newSO.ShowSO();
                selectedSO = newSO;
                selectedObject = hit.transform;
            }
        }
        else
        {
            if (prevSO != null)
            {
                prevSO.HideO(prevSO == selectedSO);
                prevSO = null;
            }
        }
    }
}
