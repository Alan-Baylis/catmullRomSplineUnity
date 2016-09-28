using UnityEngine;
using System.Collections;

public struct Shape
{
    public Vector2[] verts;
    public Vector2[] normals;
    public float[] us;
    public int[] lines;

    public Shape(Vector2[] verts, Vector2[] normals, int[] lines) : this()
    {
        this.verts = verts;
        this.normals = normals;
        this.lines = lines;
    }
}