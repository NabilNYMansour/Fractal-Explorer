using System;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class BakeSDF : MonoBehaviour
{
    public ComputeShader bakeShader;
    public float size;

    private void Bake()
    {
        int size = (int)this.size;
        TextureFormat format = TextureFormat.RGBA32;
        TextureWrapMode wrapMode = TextureWrapMode.Clamp;

        Texture3D texture = new Texture3D(size, size, size, format, false);
        texture.wrapMode = wrapMode;

        int colorSize = sizeof(float) * 4;
        ComputeBuffer SDFBuffer = new ComputeBuffer(size * size * size, colorSize);

        bakeShader.SetBuffer(0, "SDFBuffer", SDFBuffer);
        bakeShader.SetInt("Size", size);
        bakeShader.SetInt("SizeH", size/2);
        bakeShader.SetInt("Size2", size * size);

        float numThreads = 10;
        bakeShader.Dispatch(0, Mathf.CeilToInt(size / numThreads), Mathf.CeilToInt(size / numThreads), Mathf.CeilToInt(size / numThreads));

        Color[] sdfs = new Color[size * size * size];

        SDFBuffer.GetData(sdfs);
        SDFBuffer.Release();

        texture.SetPixels(sdfs);
        texture.Apply();

        AssetDatabase.CreateAsset(texture, "Assets/Ray Marching Shaders/Output.asset");

        Debug.Log("TEXTURE CREATED");
    }

    private void Start()
    {
        Bake();
    }
}


/*
static Vector3 Abs(Vector3 v)
    {
        return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }

    static float SDFsphere(Vector3 p, float r)
    {
        return p.magnitude - r;
    }

    static float SDFbox(Vector3 p, Vector3 b)
    {
        Vector3 q = Abs(p) - b;
        return Vector3.Magnitude(Vector3.Max(q, Vector3.zero)) + Mathf.Min(Mathf.Max(q.x, Mathf.Max(q.y, q.z)), 0.0f);
    }

    static Vector3 mod(Vector3 p, float c)
    {
        return new Vector3(p.x % c, p.y % c, p.z % c);
    }

    static float MengerSponge(Vector3 pos, float os)
    {
        float b = SDFbox(pos, new Vector3(os, os, os));
        float s = os / 3f;

        float c = os / 3f;
        Vector3 pmod;

        for (int i = 0; i < 3; i++)
        {
            s /= 3f;
            pmod = mod(Abs(pos + Vector3.one * c), c * 2.0f) - Vector3.one * c;
            b = Mathf.Max(b, -SDFbox(pmod, new Vector3(os, s, s)));
            b = Mathf.Max(b, -SDFbox(pmod, new Vector3(s, os, s)));
            b = Mathf.Max(b, -SDFbox(pmod, new Vector3(s, s, os)));
            c /= 3f;
        }

        s = os / 3f;
        os += 1f;

        b = Mathf.Max(b, -SDFbox(pos, new Vector3(os, s, s)));
        b = Mathf.Max(b, -SDFbox(pos, new Vector3(s, os, s)));
        b = Mathf.Max(b, -SDFbox(pos, new Vector3(s, s, os)));

        return b;
    }

    // from https://iquilezles.org/articles/normalsSDF/
    Vector3 GetNormal(Vector3 pos)
    {
        Vector3 n = new Vector3(0, 0, 0);
        Vector3 e;

        float SLOPE_EPS = 0.0001f;

        for (int i = 0; i < 4; i++)
        {
            e = 0.5773f * (2.0f * new Vector3((((i + 3) >> 1) & 1), ((i >> 1) & 1), (i & 1)) - Vector3.one);
            n += e * GetHit(pos + e * SLOPE_EPS);
        }
        return n.normalized;
    }

    float GetHit(Vector3 pos)
    {
        float s = Mathf.Ceil(size / 2f) - 2f;
        return MengerSponge(pos, s);
    }

        //RenderTexture renderTexture = new RenderTexture(size, size, size);
        //renderTexture.enableRandomWrite = true;
        //renderTexture.Create();
        //float offset = size / 2.0f;

        //float inverseResolution = 1.0f / (size - 1.0f);

        //for (int z = 0; z < size; z++)
        //{
        //    int zOffset = z * size * size;
        //    for (int y = 0; y < size; y++)
        //    {
        //        int yOffset = y * size;
        //        for (int x = 0; x < size; x++)
        //        {
                    //Vector3 pos = new Vector3(x - offset, y - offset, z - offset);

                    //float dis = GetHit(pos);
                    //Vector3 normal = GetNormal(pos);
                    //sdfs[x + yOffset + zOffset] = new Color(dis, normal.x, normal.y, normal.z);
        //        }
        //    }
        //}
 */