using System.Collections.Generic;
using UnityEngine;


public static class ListUniqueMeshes
{
    private static int max;

    private static readonly List<UniqueMesh> meshes = new List<UniqueMesh>();
    private static readonly List<string> lines = new List<string>();
    
    
    public static void CountEm()
    {
        for (int i = 0; i < meshes.Count; i++)
            meshes[i].Reset();

        MeshFilter[] filter = SceneLocator.Level.GetComponentsInChildren<MeshFilter>();

        for (int i = 0; i < filter.Length; i++)
            if (filter[i].gameObject.activeInHierarchy)
            {
                string name = filter[i].mesh.name;

                bool newMesh = true;

                int uniqueMeshCount = meshes.Count;
                for (int e = 0; e < uniqueMeshCount; e++)
                    if (meshes[e].SameMesh(name))
                    {
                        newMesh = false;
                        break;
                    }
                
                if(newMesh)
                    meshes.Add(new UniqueMesh(name));
            }

        if (filter.Length > max)
        {
            max = Mathf.Max(max, filter.Length);
            
            int uniqueMeshCount = meshes.Count;
            for (int i = 0; i < uniqueMeshCount; i++)
               meshes[i].Save();
        }
    }


    public static void ListEm()
    {
        lines.Clear();
        lines.Add("Total Mesh Count: " + max);
        lines.Add("");
            
        int uniqueMeshCount = meshes.Count;
        
        int longestName = 0;
        for (int i = 0; i < uniqueMeshCount; i++)
            longestName = Mathf.Max(longestName, meshes[i].name.Length);
            
        int meshesToList = uniqueMeshCount;
        while (meshesToList > 0)
        {
            int most = -1;
            UniqueMesh mostInstances = null;

            for (int i = 0; i < uniqueMeshCount; i++)
                if (meshes[i].saved > most)
                {
                    most = meshes[i].saved;
                    mostInstances = meshes[i];
                }
                
            lines.Add(mostInstances.GetLine(longestName));
            mostInstances.saved = -1;
            meshesToList--;
        }
            
        DesktopTxt.Write("Unique Meshes", lines.ToArray(), ".spn");
    }


    private class UniqueMesh
    {
        public readonly string name;
        private int max;
        public int count, saved;

        public UniqueMesh(string name)
        {
            this.name = name;
            count = 1;
            max = 1;
        }

        
        public void Reset()
        {
            count = 0;
        }

        
        public bool SameMesh(string name)
        {
            if (this.name == name)
            {
                count++;
                max = Mathf.Max(max, count);
                
                return true;
            }

            return false;
        }


        public void Save()
        {
            saved = count;
        }


        public string GetLine(int longestName)
        {
            return name.PadRight(longestName) + " | Count: " + saved.ToString().PadLeft(2) + " | Max: " + max.ToString().PadLeft(2);
        }
    }
}