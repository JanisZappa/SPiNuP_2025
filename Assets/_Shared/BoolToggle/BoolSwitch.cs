using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class BoolSwitch
{
    public readonly string id;
    private readonly Action<bool> setAction;
	
    
    public BoolSwitch(string id, bool defaultValue, Action<bool> setAction = null)
    {
        this.id = id;
        this.setAction = setAction;

        if (HasStoreV(id))
            setAction?.Invoke(GetStoreV(id));
        else
        {
            SetStoreV(id, defaultValue);
            setAction?.Invoke(defaultValue);
        }
        
        ToggleMap.Add(id, this);
        Switches.Add(this);
    }

    public void Toggle()
    {
        bool v = !GetStoreV(id);
        SetStoreV(id, v);
        setAction?.Invoke(v);
        //Debug.Log("Jo toggling " + id);
    }

    public void Set(bool value)
    {
        SetStoreV(id, value);
    }
    
	
    public static implicit operator bool(BoolSwitch d)
    {
        return GetStoreV(d.id);
    }
    

    private static readonly Dictionary<string, BoolSwitch> ToggleMap = new();
    private static readonly List<BoolSwitch> Switches = new();
    public static List<BoolSwitch> SortedSwitches => Switches.OrderBy(b => b.id).ToList();

    
    public static void ToggleValue(string id)
    {
        if(ToggleMap.TryGetValue(id, out BoolSwitch toggle))
            toggle.Toggle();
    }
    
    
    public static bool GetValue(string id)
    {
        return ToggleMap.TryGetValue(id, out BoolSwitch toggle) ? toggle : false;
    }

    private static Dictionary<string, bool> storedValues;


    private static bool GetStoreV(string id)
    {
        if (storedValues == null || storedValues.Count == 0)
            LoadStore();

        return storedValues[id];
    }


    private static bool HasStoreV(string id)
    {
        if (storedValues == null || storedValues.Count == 0)
            LoadStore();

        return storedValues.ContainsKey(id);
    }


    private static void SetStoreV(string id, bool value)
    {
        if (storedValues == null || storedValues.Count == 0)
            LoadStore();

        if (storedValues.TryGetValue(id, out bool v))
        {
            if (v != value)
            {
                storedValues[id] = value;
                SaveStore();
            }
        }
        else
        {
            storedValues.Add(id, value);
            SaveStore();
        }
    }


    private static void LoadStore()
    {
        storedValues = new Dictionary<string, bool>();
        byte[] b = Read("SpinUpBools");
        if(b is { Length: > 0 })
            using (MemoryStream m = new MemoryStream(b))
            using (BinaryReader r = new BinaryReader(m))
            {
                int count = r.ReadInt32();
                //Debug.LogFormat("Loading {0} Bools", count);
                for (int i = 0; i < count; i++)
                    storedValues.Add(r.ReadString(), r.ReadBoolean());
            } 
    }

    private static void SaveStore()
    {
        using (MemoryStream m = new MemoryStream())
        using (BinaryWriter w = new BinaryWriter(m))
        {
            w.Write(storedValues.Count);
            
            foreach (KeyValuePair<string, bool> kvp in storedValues)
            {
                w.Write(kvp.Key);
                w.Write(kvp.Value);
            }
            
            Write("SpinUpBools", m.ToArray());
        }
    }
    
    private static void Write(string name, byte[] bytes)
    {
        string path = GetPath(name);
        
        File.WriteAllBytes(path, bytes);
    }


    private static byte[] Read(string name)
    {
        string path = GetPath(name);
        return File.Exists(path) ? File.ReadAllBytes(path) : Array.Empty<byte>();
    }

    private static string GetPath(string name)
    {
        string path = Path.Combine(Application.streamingAssetsPath, name + ".bytes");
        return path;
    }
}
