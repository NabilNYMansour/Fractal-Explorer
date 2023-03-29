using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshRenderer))]
public class ShowOutline : MonoBehaviour
{
    public Material outlineMat;
    public Material selectOutlineMat;

    private MeshRenderer mr;
    private bool isSelected = false;

    private void Start()
    {
        this.mr = GetComponent<MeshRenderer>();
    }

    public void ShowO()
    {
        if (isSelected)
        {
            this.ShowSO();
        }
        else
        {
            Material[] newMats = { mr.materials[0], outlineMat };
            mr.materials = newMats;
        }
    }

    public void ShowSO()
    {
        Material[] newMats = { mr.materials[0], selectOutlineMat };
        mr.materials = newMats;
        this.isSelected = true;
    }

    public void HideO(bool isSelected)
    {
        if (isSelected)
        {
            this.ShowSO();
        }
        else
        {
            Material[] newMats = { mr.materials[0] };
            mr.materials = newMats;
            this.isSelected = false;
        }
    }
}
