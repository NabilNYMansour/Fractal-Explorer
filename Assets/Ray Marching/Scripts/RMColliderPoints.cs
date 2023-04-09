using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteAlways]
[RequireComponent(typeof(Rigidbody))]
public class RMColliderPoints : MonoBehaviour
{
    public float colliderRadius = 1f;
    public float verticalOffset = 0f;
    public int numPoints = 100;

    private List<Vector3> colliderPoints = new List<Vector3>();
    private Vector3 centerPoint = Vector3.zero;

    void PopulateUniformPoints(int numPoints)
    {
        this.colliderPoints = new List<Vector3>();

        float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
        float off = 2.0f / numPoints;
        for (int i = 0; i < numPoints; i++)
        {
            float y = i * off - 1 + (off / 2);
            float r = Mathf.Sqrt(1 - y * y);
            float phi = i * inc;
            float x = Mathf.Cos(phi) * r;
            float z = Mathf.Sin(phi) * r;
            Vector3 p = new Vector3(x, y, z) * this.colliderRadius;
            this.colliderPoints.Add(p);
        }
    }

    void OnDrawGizmosSelected() // For debug purposes
    {
        this.centerPoint = this.transform.position + this.transform.up * verticalOffset;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.centerPoint, colliderRadius);

        for (int i = 0; i < this.colliderPoints.Count; i++)
        {
            Vector3 p = this.centerPoint - this.colliderPoints[i];
            float hDis = this.GetHit(p);
            if (hDis < 0) Gizmos.color = Color.red;
            else Gizmos.color = Color.green;
            Gizmos.DrawSphere(this.centerPoint - this.colliderPoints[i], 0.05f);
        }
    }

    void Start()
    {
        PopulateUniformPoints(this.numPoints);
    }

    void Update()
    {
        this.centerPoint = this.transform.position + this.transform.up * verticalOffset;

        Vector3 moveDir = Vector3.zero;
        for (int i = 0; i < this.colliderPoints.Count; i++)
        {
            Vector3 p = this.centerPoint - this.colliderPoints[i];
            float hDis = this.GetHit(p);
            if (hDis < 0)
            {
                moveDir += this.colliderPoints[i] * Mathf.Sign(hDis);
            }
        }
        Vector3 forceAmount = -moveDir.normalized * Mathf.Min(0.5f, this.colliderRadius - this.GetHit(this.centerPoint));
        this.GetComponent<Rigidbody>().AddForce(forceAmount*20, ForceMode.Impulse); ;
    }

    // from http://blog.hvidtfeldts.net/index.php/2011/08/distance-estimated-3d-fractals-iii-folding-space/
    static Vector3 planeFold(Vector3 p, Vector3 n)
    {
        return p - 2.0f * Mathf.Min(0f, Vector3.Dot(p, n)) * n;
    }

    static Vector3 rotateX(float theta, Vector3 p)
    {
        float c = Mathf.Cos(theta);
        float s = Mathf.Sin(theta);

        Matrix4x4 m = new Matrix4x4(
            new Vector4(1, 0, 0, 0),
            new Vector4(0, c, -s, 0),
            new Vector4(0, s, c, 0),
            new Vector4(0, 0, 0, 1)
        );

        Vector4 res = m * new Vector4(p.x, p.y, p.z, 1);
        return new Vector3(res.x, res.y, res.z);
    }


    static Vector3 rotateY(float theta, Vector3 p)
    {
        float c = Mathf.Cos(theta);
        float s = Mathf.Sin(theta);

        Matrix4x4 m = new Matrix4x4(
            new Vector4(c, 0, s, 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(-s, 0, c, 0),
            new Vector4(0, 0, 0, 1)
        );

        Vector4 res = m * new Vector4(p.x, p.y, p.z, 1);
        return new Vector3(res.x, res.y, res.z);
    }

    static Vector3 rotateZ(float theta, Vector3 p)
    {
        float c = Mathf.Cos(theta);
        float s = Mathf.Sin(theta);

        Matrix4x4 m = new Matrix4x4(
            new Vector4(c, -s, 0, 0),
            new Vector4(s, c, 0, 0),
            new Vector4(0, 0, 1, 0),
            new Vector4(0, 0, 0, 1)
        );

        Vector4 res = m * new Vector4(p.x, p.y, p.z, 1);
        return new Vector3(res.x, res.y, res.z);
    }

    static Vector3 Abs(Vector3 v)
    {
        return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }

    static float modf(float a, float n) => (a % n + n) % n;

    static Vector3 mod(Vector3 p, float c)
    {
        return new Vector3(modf(p.x, c), modf(p.y, c), modf(p.z, c));
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

    static float MengerSpongeFolded(Vector3 pos, float os, int details)
    {
        float PI = -3.1415926538f; // minus due to hlsl to cs translation

        float b = SDFbox(pos, new Vector3(os * 10, os * 10, os * 10));

        float s = os / 3f;

        float c = os / 3f;
        Vector3 pmod;

        int i1 = 0;

        Vector3 n1;

        for (int i = 0; i < details; i++)
        {
            i1++;

            pos = rotateX(PI / (i1 * 2), pos);
            pos = rotateY(PI / (i1 * 2), pos);
            pos = rotateZ(PI / (i1 * 2), pos);

            n1 = (new Vector3(1, -1, 1)).normalized;
            pos = planeFold(pos, n1);

            s /= 3f;
            pmod = mod(Abs(pos + Vector3.one * c), c * 2f) - Vector3.one * c;

            b = Mathf.Max(b, -SDFbox(pmod, new Vector3(os, s, s)));
            b = Mathf.Max(b, -SDFbox(pmod, new Vector3(s, os, s)));
            b = Mathf.Max(b, -SDFbox(pmod, new Vector3(s, s, os)));
            c /= 3f;
        }

        s = os / 3f;

        b = Mathf.Max(b, -SDFbox(pos, new Vector3(os, s, s)));
        b = Mathf.Max(b, -SDFbox(pos, new Vector3(s, os, s)));
        b = Mathf.Max(b, -SDFbox(pos, new Vector3(s, s, os)));

        b -= 0.01f; // fix for rotation flicker

        return b;
    }

    float GetHit(Vector3 p) // Must be the same as in the shader.
    {
        //return SDFbox(p, new Vector3(16, 16, 16));
        //return SDFsphere(p, 16);
        return MengerSpongeFolded(p, 100f, 2);
    }

}
