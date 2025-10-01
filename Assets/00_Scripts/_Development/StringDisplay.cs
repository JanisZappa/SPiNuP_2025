using UnityEngine;
using UnityEngine.UI;


public class StringDisplay : MonoBehaviour
{
    public Text[] texts;

    public void SetTexts(string[] strings)
    {
        for (int i = 0; i < texts.Length; i++)
            texts[i].text = i < strings.Length ? strings[i] : "";
    }
}
