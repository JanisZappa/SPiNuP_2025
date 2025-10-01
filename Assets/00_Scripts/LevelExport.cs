using System.Collections.Generic;
using System.IO;
using Generation;
using LevelElements;
using UnityEngine;

public class LevelExport : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            GetLevelStuff();
            GetBuildingMesh();
        }
    }


    private void GetBuildingMesh()
    {
        List<int> tri = new List<int>();
        List<Vector3> pts  = new List<Vector3>();
        List<Vector3> nrms = new List<Vector3>();
        List<Color32> cols = new List<Color32>();
        
        int triOffset = 0;
        
        MeshPlacement[] placements = PlacerMeshes.placements;
        int count = placements.Length;
        for (int i = 0; i < count; i++)
        {
            MeshPlacement placement = placements[i];
            if(placement.pieceInfo == 0)
                continue;
            
            PlacerMeshes.PieceAndShadow ps = PlacerMeshes.PieceObjects.Pieces.GetMesh(placement.pieceInfo);
            if(ps == null)
                continue;

            Mesh mesh = ps.piece;
            if(mesh == null)
                continue;

            Vector3[] verts  = mesh.vertices;
            Vector3[] norms  = mesh.normals;
            Color[] colors = mesh.colors;
            int vCount = verts.Length;
            
            Matrix4x4 mtrx = Matrix4x4.Translate(placement.pos);
            
            int[] tris = mesh.triangles;

            for (int e = 0; e < vCount; e++)
                pts.Add(mtrx.MultiplyPoint(verts[e]));
            
            for (int e = 0; e < vCount; e++)
                nrms.Add(mtrx.MultiplyVector(norms[e]));
            
            for (int e = 0; e < vCount; e++)
                cols.Add(Palette.GetColor(colors[e]));
            
            int triCount = tris.Length / 3;
            //Vector3 s = trans.localScale;
            bool reversed = false;// Mathf.Sign(s.x * s.y * s.z) < 0;
            for (int e = 0; e < triCount; e++)
            {
                tri.Add(triOffset + tris[e * 3 + (reversed ? 2 : 0)]);
                tri.Add(triOffset + tris[e * 3 + 1]);
                tri.Add(triOffset + tris[e * 3 + (reversed ? 0 : 2)]);
            }
            
            triOffset += vCount;
        }
        
        int pCount = pts.Count;
        int tCount = tri.Count;

        string path = "MeshData/Level";

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


    private void GetLevelStuff()
    {
        List<Item> activeItems = Item.active;
       
        int iCount = activeItems.Count;
        for (int i = 0; i < iCount; i++)
            if (activeItems[i].parent != null)
            {
                activeItems.RemoveAt(i);
                iCount--;
                i--;
            }

        
        
        for (int i = 0; i < iCount; i++)
        {
            Item item = activeItems[i];
            Debug.Log(item.elementType);
            
            
            
        }
        
        List<Track> activeTracks = Track.active;
    }
}
