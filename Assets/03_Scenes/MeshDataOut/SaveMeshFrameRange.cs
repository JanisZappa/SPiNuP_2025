using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class SaveMeshFrameRange : Singleton<SaveMeshFrameRange>
{
    public int frames;
    public int fps;
    public string fileName;
    
    public static bool Saving;
    private static float EndTime;
    
    private readonly HashSet<string> collectedMeshes = new HashSet<string>();
    private readonly Dictionary<string, int> meshMap = new Dictionary<string, int>(100000);
    private readonly List<Mesh> objectMeshes = new List<Mesh>(100000);
    
    private readonly List<int> recIds = new List<int>(100000);
    private readonly List<Matrix4x4> recMatrices = new List<Matrix4x4>(100000);
    private Vector2Int[] frameOffsets;
    private Mesh[] characterMeshes;
    private Matrix4x4[] characterMatrices;
    private static int SaveFrame;
    

    private void Start()
    {
        characterMeshes = new Mesh[frames];
        for (int i = 0; i < frames; i++)
            characterMeshes[i] = new Mesh();

        characterMatrices = new Matrix4x4[frames];
        frameOffsets      = new Vector2Int[frames];
    }


    public static float GetFrameTime
    {
        get
        {
            float step = 1f / Inst.fps;
            return EndTime + (-Inst.frames + SaveFrame) * step;
        }
    }
    
    
    private void Update()
    {
        if (!Saving && Input.GetKeyDown(KeyCode.J))
        {
            EndTime = GTime.Now;
            Saving = true;
            SaveFrame = 0;
        }
    }


    public static void WriteToFile()
    {
        string path = Inst.fileName + "." + $"{(SaveFrame++ + 1):D5}";
        MeshDataOut.GetAndSave(path);
        if (SaveFrame == Inst.frames)
            Saving = false;
    }


    private IEnumerator DoingIt()
    {
        Saving = true;
        int meshID = 0;
        
        Camera cam = Camera.main;

        for (int frame = 0; frame < frames; frame++)
        {
            MeshRenderer[] mR = FindObjectsOfType<MeshRenderer>();
            int count = mR.Length;

            int recStart = recIds.Count;

            for (int i = 0; i < count; i++)
            {
                MeshRenderer mRi = mR[i];
                if(!mRi.enabled || (cam.cullingMask & (1 << mRi.gameObject.layer)) == 0)
                    continue;
                
                MeshFilter mF = mRi.GetComponent<MeshFilter>();
                Mesh mesh = mF.mesh;
                if(!mesh.isReadable)
                    continue;

                string meshName = mesh.name;

                if (collectedMeshes.Add(meshName))
                {
                    meshMap.Add(meshName, meshID++);
                    objectMeshes.Add(mesh);
                }

                recIds.Add(meshMap[meshName]);
                recMatrices.Add(mF.transform.localToWorldMatrix);
            }
            
            frameOffsets[frame] = new Vector2Int(recStart, recIds.Count - recStart);
            
            
        //  Character  //
            SkinnedMeshRenderer[] sMR = FindObjectsOfType<SkinnedMeshRenderer>();
            count = sMR.Length;
            
            for (int i = 0; i < count; i++)
            {
                SkinnedMeshRenderer mRi = sMR[i];
                if(!mRi.enabled)
                    continue;

                mRi.BakeMesh(characterMeshes[frame]);
                characterMatrices[frame] = mRi.transform.localToWorldMatrix;
            }
            
            yield return null;
        }
        
        
    //  Saving  //
        List<int> tri = new List<int>();
        List<Vector3> pts  = new List<Vector3>();
        List<Vector3> nrms = new List<Vector3>();
        List<Color32> cols = new List<Color32>();

        for (int frame = 0; frame < frames; frame++)
        {
            tri.Clear();
            pts.Clear();
            nrms.Clear();
            cols.Clear();

            int triOffset = 0;
            
            Vector2Int offsets = frameOffsets[frame];

            for (int i = 0; i < offsets.y; i++)
            {
                int index = recIds[offsets.x + i];
                Mesh mesh = objectMeshes[index];
                Matrix4x4 mtrx = recMatrices[offsets.x + i];
                
                Vector3[] verts  = mesh.vertices;
                Vector3[] norms  = mesh.normals;
                  Color[] colors = mesh.colors;
                int vCount = verts.Length;
                
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
            
        //  Character  // 
            {
                Mesh mesh = characterMeshes[frame];
                Matrix4x4 mtrx = characterMatrices[frame];
                
                Vector3[] verts  = mesh.vertices;
                Vector3[] norms  = mesh.normals;
                Color[] colors = mesh.colors;
                int vCount = verts.Length;
                
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
            }


            int pCount = pts.Count;
            int tCount = tri.Count;

            string path = "MeshData/" + fileName + "." + $"{(frame + 1):D5}";

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
            yield return null;
        }
        


        Saving = false;
    }
}
