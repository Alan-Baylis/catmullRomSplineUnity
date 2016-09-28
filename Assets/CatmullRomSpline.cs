using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct CatmullRomSpline{

    public Vector3 GetPoint(float t, Vector3[] points)
    {
        Vector3 a = 0.5f * (2f * points[1]);
        Vector3 b = 0.5f * (points[2] - points[0]);
        Vector3 c = 0.5f * (2f * points[0] - 5f * points[1] + 4f * points[2] - points[3]);
        Vector3 d = 0.5f * (-points[0] + 3f * points[1] - 3f * points[2] + points[3]);
        Vector3 pos = a + (b * t) + (c * t * t) + (d * t * t * t);
        return pos;
    }

    public Vector3 GetTangent(float t, Vector3[] points)
    {
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;
        Vector3 tangent = points[0] * (-omt2) + points[1] * (3 * omt2 - 2 * omt) + points[2] * (-3 * t2 + 2 * t) + points[3] * (t2);
        return tangent.normalized;
    }

    public Vector3 GetNormal2D(Vector3[] pts, float t)
    {
        Vector3 tng = GetTangent(t, pts);
        return new Vector3(-tng.y, tng.x, 0f);
    }

    public Vector3 GetNormal3D(Vector3[] pts, float t, Vector3 up)
    {
        Vector3 tng = GetTangent(t, pts);
        Vector3 binormal = Vector3.Cross(up, tng).normalized;
        return Vector3.Cross(tng, binormal);
    }

    public Quaternion GetOrientation2D(Vector3[] pts, float t)
    {
        Vector3 tng = GetTangent(t, pts);
        Vector3 nrm = GetNormal2D(pts, t);
        return Quaternion.LookRotation(tng, nrm);
    }

    public Quaternion GetOrientation3D(Vector3[] pts, float t, Vector3 up)
    {
        Vector3 tng = GetTangent(t, pts);
        Vector3 nrm = GetNormal3D(pts, t, up);
        return Quaternion.LookRotation(tng, nrm);
    }


}