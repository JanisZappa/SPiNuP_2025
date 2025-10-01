using System.Collections.Generic;
using UnityEngine;


public class MeshTest : MonoBehaviour
{
    public MeshFilter mF;
    private Transform[] points;
    private Mesh mesh;
    
    private Vector3[] verts;
    
    
    private void Start()
    {
        points = transform.GetChildren();
        verts = new Vector3[points.Length];
        
        mesh = new Mesh();
        mF.mesh = mesh;
    }

    
    private void Update()
    {
        if (false)
        {
            for (int i = 0; i < points.Length; i++)
                verts[i] = points[i].position;
        
            List<int> tris = Triangulator.Fill(verts, points.Length);
        
            mesh.Clear();
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateBounds();
        }
        else
        {
            int count = points.Length + 1;
            DRAW.Shape shape = DRAW.Shape.Get(count);
    
            for (int i = 0; i < count; i++)
                shape.Set(i, points[i % points.Length].position);
                        
            shape.SetColor(Color.red).TriFill(.5f, true);
        }
    }
}
