using UnityEngine;

[ExecuteInEditMode]
public class RayMarcherUniformSetter : MonoBehaviour
{
    public Material material;
    public Transform pointLight;

    void Update()
    {
        if (material != null && pointLight != null)
        {
            material.SetVector("_LightPos", pointLight.position);
            GetComponent<MeshRenderer>().material = material;
        }
    }
}