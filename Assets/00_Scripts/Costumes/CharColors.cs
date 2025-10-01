using System.Collections.Generic;
using UnityEngine;


public static class CharColors
{
    private static readonly List<Color32> colors = new List<Color32>(short.MaxValue);
    public static void Colorize(this Mesh mesh, CostumeColors costumeColors = null, List<byte> colorMask = null, List<byte> shadeMask = null)
    {
        if (costumeColors == null)
            costumeColors = CostumeColors.RandomScheme;
                
        mesh.GetColors(colors);
        bool useMask  = colorMask != null,
             useShade = shadeMask != null;
        
        int  count = useMask? colorMask.Count : colors.Count;

        for (int i = 0; i < count; i++)
            colors[i] = costumeColors.GetColor(colorMask[i], useShade? shadeMask[i] : (byte)255);

        mesh.SetColors(colors);
    }
}
