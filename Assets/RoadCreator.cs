using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadCreator : MonoBehaviour {

    public List<Transform> points;
    public float resolution;

    void DisplayCatmullRomSpline(int pos)
    {
        //Clamp to allow looping
        Vector3[] pnt = {
            points[ClampListPos(pos - 1)].position, 
            points[pos].position, 
            points[ClampListPos(pos + 1)].position, 
            points[ClampListPos(pos + 2)].position};
        //Just assign a tmp value to this
        Vector3 lastPos = Vector3.zero;
        //t is always between 0 and 1 and determines the resolution of the spline
        //0 is always at p1
        for (float t = 0; t < 1; t += 0.1f)
        {
            //Find the coordinates between the control points with a Catmull-Rom spline
            CatmullRomSpline spline = new CatmullRomSpline();
            Vector3 newPos = spline.GetPoint(t, pnt);
            //Cant display anything the first iteration
            if (t == 0)
            {
                lastPos = newPos;
                continue;
            }
            Gizmos.DrawLine(lastPos, newPos);
            lastPos = newPos;
        }
        //Also draw the last line since it is always less than 1, so we will always miss it
        Gizmos.DrawLine(lastPos, pnt[2]);
    }

    int ClampListPos(int pos)
    {
        if (pos < 0)
        {
            pos = points.Count - 1;
        }

        if (pos > points.Count)
        {
            pos = 1;
        }
        else if (pos > points.Count - 1)
        {
            pos = 0;
        }

        return pos;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        //Draw a sphere at each control point
        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.DrawWireSphere(points[i].position, 0.3f);
        }

        for (int i = 0; i < points.Count; i++)
        {
            //Cant draw between the endpoints
            //Neither do we need to draw from the second to the last endpoint
            //...if we are not making a looping line
            if (i == 0 || i == points.Count - 2 || i == points.Count - 1)
            {
                continue;
            }

            DisplayCatmullRomSpline(i);
        }
    }

	// Use this for initialization
	void Start () {

        Mesh mesh = new Mesh();

        CatmullRomSpline spline = new CatmullRomSpline();

        Vector3[] pos = new Vector3[points.Count];
        Quaternion[] rot = new Quaternion[points.Count];

        for (int i = 0; i < points.Count; i++)
        {
            pos[i] = points[i].transform.position;
            rot[i] = points[i].transform.rotation;
        }

        Vector3 tangent;

        for (int i = 0; i < points.Count; i++)
        {
            tangent = Vector3.Cross(spline.GetTangent(i, pos), Vector3.forward);

            if (tangent.magnitude == 0)
            {
                tangent = Vector3.Cross(spline.GetTangent(i, pos), Vector3.up);
            }
            points[i].transform.rotation = Quaternion.Euler(tangent);
        }

        OrientedPoint[] oPoints = new OrientedPoint[(int)(1/resolution)];
        for (float i = 0f; i < 1.0f; i+=resolution)
        {
            if (i != 0f || i != 1f)
            {
                oPoints[(int)i * 10].position = spline.GetPoint(i, pos);
                tangent = Vector3.Cross(spline.GetTangent(i, pos), Vector3.forward);

                if (tangent.magnitude == 0)
                {
                    tangent = Vector3.Cross(spline.GetTangent(i, pos), Vector3.up);
                }
                oPoints[(int)i * 10].rotation = Quaternion.Euler(tangent);
            }
        }

        Vector2[] verts = { new Vector2(0,0), new Vector2(1,0) };
        Vector2[] normals = { new Vector2(0, 1), new Vector2(0, 1) };
        int[] lines = { 0, 1 };

        Shape shape = new Shape(verts,normals,lines);

        Extrude extrusao = new Extrude(ref mesh, shape, oPoints);
        this.GetComponent<MeshFilter>().sharedMesh = mesh;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
