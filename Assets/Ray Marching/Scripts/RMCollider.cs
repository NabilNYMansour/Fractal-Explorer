using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class RMCollider : MonoBehaviour
{
    public float colliderRadius = 1f;

    private bool isColliding = false;
    private float hitDis = 1f;
    private Vector3 hitPoint = Vector3.zero;
    private Vector3 previousV = Vector3.forward;
    private bool maxIterReached = false;
    private int currentGuess = 0;
    void OnDrawGizmosSelected() // For debug purposes
    {
        if (isColliding) Gizmos.color = Color.red;
        else Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, colliderRadius);
        Gizmos.DrawSphere(hitPoint, 0.1f);
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
        return new Vector3(modf(p.x ,c), modf(p.y, c), modf(p.z, c));
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

        int i1 = 1;

        Vector3 n1;

        for (int i = 0; i < details; i++)
        {
            i1++;

            //pos = rotateX(-PI / 3f, pos);
            pos = rotateX(PI / (i1 * 2), pos);
            pos = rotateY(PI / (i1 * 2), pos);
            pos = rotateZ(PI / (i1 * 2), pos);

            n1 = (new Vector3(1, -1, 1)).normalized;
            //pos = planeFold(pos, n1);

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
        return MengerSpongeFolded(p, 100f, 5);
    }

    Vector3 GetGradientVector(Vector3 cv, float eps, bool up)
    {

        Vector3 v;
        if (up) v = Quaternion.AngleAxis(eps, Vector3.right) * cv;
        else v = Quaternion.AngleAxis(eps, Vector3.up) * cv;
        return v;
    }

    void GradientDescent(Vector3 p)
    {
        float eps = 0.5f;
        Vector3 vO = previousV; // make guess previous found vector
        float dO = this.GetHit(p);


        // update isColliding
        this.isColliding = dO < colliderRadius;
        this.hitDis = dO;

        if (!this.isColliding) { return; } // optimization: dont do gradient descent if not colliding

        Vector3 v1;
        Vector3 v2;
        Vector3 v3;
        Vector3 v4;

        Vector3 p1;
        Vector3 p2;
        Vector3 p3;
        Vector3 p4;

        float d1;
        float d2;
        float d3;
        float d4;

        List<float> distances;

        float dm = 0f; // minimum distance
        float dOsign = Mathf.Sign(dO); // To be used to correct nearest point if inside the SDF.


        int maxIter = 500;
        int i = 0;
        for (; i <= maxIter; i++)
        {
            v1 = GetGradientVector(vO, eps, false);
            v2 = GetGradientVector(vO, -eps, false);
            v3 = GetGradientVector(vO, eps, true);
            v4 = GetGradientVector(vO, -eps, true);

            p1 = p - dO * v1;
            p2 = p - dO * v2;
            p3 = p - dO * v3;
            p4 = p - dO * v4;

            d1 = this.GetHit(p1) * dOsign;
            d2 = this.GetHit(p2) * dOsign;
            d3 = this.GetHit(p3) * dOsign;
            d4 = this.GetHit(p4) * dOsign;

            distances = new List<float> { d1, d2, d3, d4 };

            dm = distances.Min();

            if (d1 == dm) { vO = v1; }
            else if (d2 == dm) { vO = v2; }
            else if (d3 == dm) { vO = v3; }
            else if (d4 == dm) { vO = v4; }

            if (dm < eps) break;

            this.maxIterReached = i == maxIter;
        }
        // update hit point
        this.hitPoint = p - dO * vO;

        // update guess to previous found vector or to a new guess
        if (this.maxIterReached)
        {
            switch (currentGuess)
            {
                case 0:
                    previousV = Vector3.forward;
                    currentGuess++;
                    break;
                case 1:
                    previousV = Vector3.left;
                    currentGuess++;
                    break;
                case 2:
                    previousV = Vector3.up;
                    currentGuess++;
                    break;
                case 3:
                    previousV = Vector3.right;
                    currentGuess++;
                    break;
                case 4:
                    previousV = Vector3.down;
                    currentGuess++;
                    break;
                case 5:
                    previousV = Vector3.back;
                    currentGuess++;
                    break;
                default:
                    previousV = Vector3.forward;
                    currentGuess = 0;
                    break;
            }
        } else
        {
            previousV = vO;
        }
        //previousV = this.maxIterReached ? previousV = (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f))).normalized : vO;
    }

    void Update()
    {
        this.GradientDescent(transform.position);
        if (this.isColliding && !this.maxIterReached) this.transform.position += (this.transform.position - this.hitPoint * Mathf.Sign(hitDis)).normalized * Mathf.Min(this.colliderRadius - this.hitDis, 0.1f);
        //if (isColliding) this.GetComponent<Rigidbody>().AddForce(this.transform.position - this.hitPoint * 1000);
    }
}
