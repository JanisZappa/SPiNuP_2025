using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FindBestPalette : MonoBehaviour
{
    public int bestHueSteps = 0, bestSaturationSteps = 0;
    public float bestMinTint, bestMaxTint;
    public float bestDist;
    public float[] bestHueOffsets;
    public NewColorGen gen;

    [Space] 
    public int tests;
    public int currentHue, currentSat;

    [Space] public TextMeshPro tmp;

    public bool justInit;

    
	private void Start ()
    {
        UpdateGen();
        
        if(!justInit)
            StartCoroutine(YieldSearch());
    }


    private void UpdateGen()
    {
        gen.hueSteps = bestHueSteps;
        gen.satSteps = bestSaturationSteps;
        gen.hueOffsets = new float[bestHueOffsets.Length];
        gen.tintMin = bestMinTint;
        gen.tintMax = bestMaxTint;
        for (int i = 0; i < bestHueSteps; i++)
            gen.hueOffsets[i] = bestHueOffsets[i];
        
        gen.Init();
        
    #if UNITY_EDITOR
        PaletteSource.Get.hues = NewColorGen.colors;
        EditorUtility.SetDirty(PaletteSource.Get);
    #endif
        
        {
            List<string> lines = new List<string>();
            lines.Add(bestHueSteps.ToString());
            lines.Add(bestSaturationSteps.ToString());
            
            string array = "";
            for (int i = 0; i < bestHueSteps; i++)
                array += bestHueOffsets[i].ToString(CultureInfo.InvariantCulture) + " ";
            lines.Add(array);
            
            DesktopTxt.Write("ColorGenValues", lines.ToArray(), ".spn");
        }
    }

    
	private IEnumerator YieldSearch()
	{
        float[] hueOffsets = new float[60];
        Color[] old = NewColorGen.HoudiniUsed();
        
        List<Color> used = new List<Color>(), not = new List<Color>();

        for (int i = 0; i < old.Length; i++)
        {
            Color color = old[i];

            float nonGreyness = Mathf.Abs(color.r - color.g) + Mathf.Abs(color.r - color.b);

            if (nonGreyness < .2f)
            {
                not.Add(color);
                continue;
            }
            
            used.Add(color);
        }
        
        Debug.Log("Used: " + used.Log());
        Debug.Log("Not: " + not.Log());
        
        while (true)
        {
            tests = 0;
            int test = 0;

            for (int hueTest = 16; hueTest < 50; hueTest++) //16
            for (int satTest = 3; satTest < 15; satTest++)
            {
                for (int hueOff = 0; hueOff < 10000; hueOff++)
                {
                    for (int i = 0; i < hueTest; i++)
                        hueOffsets[i] = Random.Range(-.5f, .5f);
                    
                    float totalDist = 0;
                
                    Color[] newColors = NewColorGen.GeneratCrazy(hueTest, satTest, hueOffsets);
                
                    for (int i = 0; i < used.Count; i++)
                        totalDist += used[i].ClosestColorDist(newColors);

                    if (bestDist >= totalDist)
                    {
                        bestDist = totalDist;
                        bestHueSteps = hueTest;
                        bestSaturationSteps = satTest;

                        for (int i = 0; i < hueTest; i++)
                            bestHueOffsets[i] = hueOffsets[i];
                        
                        
                        UpdateGen();
                        
                        
                        newColors = NewColorGen.GeneratCrazy(bestHueSteps, bestSaturationSteps, bestHueOffsets);
                        
                        Dictionary<int, int> valueRemap = new Dictionary<int, int>();
                        for (int i = 0; i < used.Count; i++)
                            valueRemap.Add(i, used[i].ClosestColorIndex(newColors));

                        foreach (KeyValuePair<int, int> VARIABLE in valueRemap)
                        {
                            Debug.Log(VARIABLE.Key.ToString().PadLeft(4) + " " + FancyString.Block.B_Color(used[VARIABLE.Key]) + 
                                      FancyString.Block.B_Color(newColors[VARIABLE.Value]) + " " + VARIABLE.Value.ToString().PadLeft(4));
                        }
                    }

                    test++;
                    tests++;

                    if (test == 100)
                    {
                        yield return null;

                        tmp.text = "H: " + hueTest + " S: " + satTest + "\nT: " + tests.ToString(); 

                        test = 0;
                        currentHue = hueTest;
                        currentSat = satTest;
                    }
                }
            }
        }
	}
}
