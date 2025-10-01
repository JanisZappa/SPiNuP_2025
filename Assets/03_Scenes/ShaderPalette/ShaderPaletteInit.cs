using UnityEngine;
using UnityEngine.UI;


public class ShaderPaletteInit : ActiveUI
{
    [Space(10)] public GameObject[] button;

    private Image[] images;
    private Color[] colors;

    public delegate void SwitchActorColor();

    public static event SwitchActorColor onSwitchActorColor;

    private readonly string[] Keywords = { "normals", "grey", "desaturate", "test", "cheap" };
    private string[] PrefKeywords;


    private void Awake()
    {
        PrefKeywords = new string[Keywords.Length];
        for (int i = 0; i < Keywords.Length; i++)
            PrefKeywords[i] = "ShaderVar_" + Keywords[i];
		
		
        Palette.Load();

        images = new Image[button.Length];
        colors = new Color[button.Length];
        for (int i = 0; i < button.Length; i++)
        {
            images[i] = button[i].transform.parent.GetComponent<Image>();
            colors[i] = images[i].color;
        }

        for (int i = 0; i < button.Length; i++)
            SetValue(i);
    }
	

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Slash))
            SetValue(0, true);
		
        if (Input.GetKeyDown(KeyCode.Quote))
            SetValue(1, true);
		
        if (Input.GetKeyDown(KeyCode.BackQuote))
            SetValue(2, true);
		
        /*if (Input.GetKeyDown(KeyCode.N))
            SetValue(Test, true);*/

        if (KeyMap.Down(Key.Dev_ColorShift) && onSwitchActorColor != null)
            onSwitchActorColor();
    }


    private void SetValue(int index, bool toggle = false)
    {
        string varName  =     Keywords[index];
        string prefName = PrefKeywords[index];
		
        bool value = PlayerPrefs.GetInt(prefName) == 1;
		
        if (toggle)
        {
            value = !value;
            PlayerPrefs.SetInt(prefName, value? 1 : 0);
        }
        else
        {
            PlayerPrefs.SetInt(prefName, 0);
            value = false;
        }
			

        string bigString = varName.ToUpper();
        Shader.DisableKeyword(bigString + ( value? "_OFF" : "_ON"));
        Shader.EnableKeyword( bigString + (!value? "_OFF" : "_ON"));


        images[index].color = (colors[index] * (value ? 1 : .75f)).A(1);
    }
	
	
    public override bool HitUI(int click)
    {
        for (int i = 0; i < button.Length; i++)
            if (UI_Manager.ImPointedAt(button[i]))
            {
                if(click == 1)	
                    SetValue(i, true);
                return true;
            }

        return false;
    }
}