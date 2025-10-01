using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(menuName = "Scriptable/CostumeColors")]
public class CostumeColors : ScriptableObject
{
    public List<CostumeColor> colors;


    public Color32 GetColor(byte mapIndex, byte shade)
    {
        int count = colors.Count;
        for (int i = 0; i < count; i++)
            if ((byte) colors[i].partType == mapIndex)
                return colors[i].GetColor(shade);

        return colors[0].GetColor(shade);
    }
    
    
    private static CostumeColors[] resourceSchemes;
    public  static CostumeColors   RandomScheme
    {
        get
        {
            if (resourceSchemes == null)
            {
                Object[] objects = Resources.LoadAll("ColorSchemes");
                resourceSchemes  = new CostumeColors[objects.Length];
                
                for (int i = 0; i < objects.Length; i++)
                    resourceSchemes[i] = objects[i] as CostumeColors;
            }

            return resourceSchemes[Random.Range(0, resourceSchemes.Length - 1)];
        }
    }


    public string Log()
    {
        List<Color> col = new List<Color>();
        for (int i = 0; i < colors.Count; i++)
            col.Add(colors[i].realColor);
        
        return col.Log() + " " + name;
    }


    [System.Serializable]
    public class CostumeColor
    {
        public CharPartType partType;
        public short colorID;
        public byte matCap;
        public Color realColor;

        public Color32 GetColor(byte shade)
        {
            int matCapOffset = Mathf.FloorToInt(colorID / 256f);
            int colorValue   = colorID % 256;
            int matCapValue = matCapOffset * 16 + matCap;

            float r = colorValue / 256.0f;
            float g = matCapValue / 256.0f;

            byte byteR = (byte) Mathf.RoundToInt(r * 255f);
            byte byteG = (byte) Mathf.RoundToInt(g * 255f);
            
            return new Color32(byteR, byteG, 0, shade);
        }
    }
}


#if UNITY_EDITOR

[CustomEditor(typeof(CostumeColors))]
public class CostumeColorsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if(!Palette.Initialized)
            Palette.Load();
        
        GUI.color = Color.white;
        CostumeColors cC = target as CostumeColors;
        
        GUILayout.BeginHorizontal();
        
        GUILayout.BeginVertical();

        for (int i = 0; i < cC.colors.Count; i++)
        {
            GUILayout.BeginHorizontal();

            CostumeColors.CostumeColor part = cC.colors[i];

            EditorGUILayout.ColorField(part.realColor);
            
            EditorGUI.BeginChangeCheck();
            part.partType = (CharPartType)EditorGUILayout.EnumPopup("", part.partType, GUILayout.Width(100));
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(cC);

            if(part.colorID >= Palette.Colors.Length)
                part.colorID = (byte)Mathf.Repeat(part.colorID, Palette.Colors.Length);
            
            GUI.color = Palette.Colors[part.colorID];
            if (GUILayout.Button(part.colorID.ToString(), GUILayout.Width(50), GUILayout.Height(14)))
            {
                /*part.colorID = (byte)Mathf.Repeat(part.colorID + (Event.current.button == 0? -1 : 1), Palette.Colors.Length);
                if (Application.isPlaying)
                    Spinner.SetColorScheme(cC);
                
                EditorUtility.SetDirty(cC);*/
            }
            GUI.color = Color.white;
            
            if (GUILayout.Button(part.matCap.ToString(), GUILayout.Width(50), GUILayout.Height(14)))
            {
                part.matCap = (byte)Mathf.Repeat(part.matCap + (Event.current.button == 0? -1 : 1), 4);
                if (Application.isPlaying)
                    Spinner.SetColorScheme(cC);
                
                EditorUtility.SetDirty(cC);
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        
        GUILayout.EndVertical();
        
        GUILayout.BeginVertical();
       
        if (Application.isPlaying && GUILayout.Button(">>", GUILayout.Width(32), GUILayout.Height(32)))
            Spinner.SetColorScheme(cC);
        
        GUILayout.FlexibleSpace();
        
        GUILayout.EndVertical();
        
        GUILayout.EndHorizontal();
    }
}
#endif