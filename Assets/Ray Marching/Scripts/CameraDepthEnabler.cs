using UnityEngine;

[ExecuteInEditMode]
public class CameraDepthEnabler : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
    }
}
