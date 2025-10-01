using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;


public static class MeshDataOut
{
    [MenuItem("MeshData/Out")]
    public static void GrabMeshData()
    {
        GetAndSave("DataOut");
    }
    
    
    public static void GetAndSave(string name)
    {
        string path = "MeshData/" + name;
        
        Camera cam = Camera.main;
        
        MeshRenderer[] mR = Object.FindObjectsOfType<MeshRenderer>();
        List<int> tri = new List<int>();
        List<Vector3> pts  = new List<Vector3>();
        List<Vector3> nrms = new List<Vector3>();
        List<Color32> cols = new List<Color32>();
        int offset = 0;
        int count = mR.Length;
        
        for (int i = 0; i < count; i++)
        {
            MeshRenderer mRi = mR[i];
            GameObject gO = mRi.gameObject;
            if(!mRi.enabled || (cam.cullingMask & (1 << gO.layer)) == 0 || gO.name == "Piece")
                continue;
            
            

            MeshFilter mF = mRi.GetComponent<MeshFilter>();
            Mesh m = mF.mesh;
            if(!m.isReadable)
                continue;
            
            Vector3[] verts  = m.vertices;
            Vector3[] norms  = m.normals;
              Color[] colors = m.colors;
            int vCount = verts.Length;
            
            int[] tris = m.triangles;
            
            Transform trans = mF.transform;
            Matrix4x4 mtrx = trans.localToWorldMatrix;

            for (int e = 0; e < vCount; e++)
                pts.Add(mtrx.MultiplyPoint(verts[e]));
            
            for (int e = 0; e < vCount; e++)
                nrms.Add(mtrx.MultiplyVector(norms[e]));
            
            for (int e = 0; e < vCount; e++)
                cols.Add(Palette.GetColor(colors[e]));

            int triCount = tris.Length / 3;
            Vector3 s = trans.localScale;
            bool reversed = Mathf.Sign(s.x * s.y * s.z) < 0;
            for (int e = 0; e < triCount; e++)
            {
                tri.Add(offset + tris[e * 3 + (reversed ? 2 : 0)]);
                tri.Add(offset + tris[e * 3 + 1]);
                tri.Add(offset + tris[e * 3 + (reversed ? 0 : 2)]);
            }
            
            offset += vCount;
        }
        
        SkinnedMeshRenderer[] sMR = Object.FindObjectsOfType<SkinnedMeshRenderer>();
        count = sMR.Length;

        Mesh bakeMesh = new Mesh();
        for (int i = 0; i < count; i++)
        {
            SkinnedMeshRenderer mRi = sMR[i];
            if(!mRi.enabled)
                continue;

            mRi.BakeMesh(bakeMesh);
            Vector3[] verts  = bakeMesh.vertices;
            Vector3[] norms  = bakeMesh.normals;
              Color[] colors = bakeMesh.colors;
            
            int vCount = verts.Length;
            int[] tris = bakeMesh.triangles;

            Transform trans = mRi.transform;
            Matrix4x4 mtrx = trans.localToWorldMatrix;

            for (int e = 0; e < vCount; e++)
                pts.Add(mtrx.MultiplyPoint(verts[e]));
            
            for (int e = 0; e < vCount; e++)
                nrms.Add(mtrx.MultiplyVector(norms[e]));
            
            for (int e = 0; e < vCount; e++)
                cols.Add(Palette.GetColor(colors[e]));
            
            int triCount = tris.Length / 3;
            Vector3 s = trans.localScale;
            bool reversed = Mathf.Sign(s.x * s.y * s.z) < 0;
            for (int e = 0; e < triCount; e++)
            {
                tri.Add(offset + tris[e * 3 + (reversed ? 2 : 0)]);
                tri.Add(offset + tris[e * 3 + 1]);
                tri.Add(offset + tris[e * 3 + (reversed ? 0 : 2)]);
            }

            offset += vCount;
        }
        
        
        int pCount = pts.Count;
        int tCount = tri.Count;

        using (MemoryStream mem = new MemoryStream())
        using (BinaryWriter w = new BinaryWriter(mem))
        {
            w.Write(pCount);
            for (int i = 0; i < pCount; i++)
            {
                Vector3 v = pts[i];
                w.Write(v.x);
                w.Write(v.y);
                w.Write(v.z);
                
                Vector3 n = nrms[i];
                w.Write(n.x);
                w.Write(n.y);
                w.Write(n.z);
                
                Color32 c = cols[i];
                w.Write(c.r);
                w.Write(c.g);
                w.Write(c.b);
            }
            
            w.Write(tCount);
            for (int i = 0; i < tCount; i++)
                w.Write(tri[i]);
            
            DesktopBytes.Write(path, mem.GetBuffer());
        }
        
        
        Debug.Log("Saved: " + path);
    }
}
