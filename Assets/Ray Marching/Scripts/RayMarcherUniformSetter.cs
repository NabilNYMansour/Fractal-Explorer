using UnityEngine;

[ExecuteInEditMode]
public class RayMarcherUniformSetter : MonoBehaviour
{
    public Material material;
    public Light pointLight;
    public Transform player;

    void Update()
    {
        if (material != null && pointLight != null)
        {
            material.SetVector("_LightPos", pointLight.transform.position);
            material.SetVector("_LightCol", pointLight.color);
            material.SetVector("_PlayerPos", player.position);
            GetComponent<MeshRenderer>().material = material;
        } else
        {
            Debug.Log("TEST");
        }
    }
}