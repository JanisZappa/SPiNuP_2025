using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class Fade : Singleton<Fade>
{
    public Image image;
    private static Image Image => Inst.image;

    private static bool visible;
    private const float fadeSpeed = .8f;

    public static void SetVisibility(bool visible)
    {
        Image.transform.parent.gameObject.SetActive(true);
        Image.enabled = visible;
        Color c = Image.color;
        Image.color = new Color(c.r,c.g,c.b,visible ? 1 : 0);
        Fade.visible = visible;
    }

    
    public static void Transition(Action<bool> setActive, Action<bool> setInactive)
    {
        setActive(true);
        setInactive(false);
        
        //Inst.StartCoroutine(Inst.TransitionFade(setActive, setInactive));
    }


    private IEnumerator TransitionFade(Action<bool> setActive, Action<bool> setInactive)
    {
        if (!visible)
            yield return StartCoroutine(FadeIn());

        setActive(true);
        setInactive(false);

        StartCoroutine(FadeOut());
    }
    
    
    private static IEnumerator FadeIn()
    {
        visible = true;
        
        float start = Image.color.a;
        float speed = 1f / Mathf.Abs(start - 1);
        Image.enabled = true;
        Image.transform.parent.gameObject.SetActive(true);
        
        float lerp = 0;

        Color c = Image.color;

        while ( lerp < 1 && visible)
        {
            lerp += Time.deltaTime * fadeSpeed * speed;
            Image.color = new Color(c.r, c.g, c.b, Mathf.SmoothStep(start, 1, lerp));
            yield return null;
        }
    }
    
    
    private static IEnumerator FadeOut()
    {
        visible = false;
        
        float start = Image.color.a;
        float speed = 1f / Mathf.Abs(start - 0);
        Image.enabled = true;
        Image.transform.parent.gameObject.SetActive(true);
        
        float lerp = 0;

        Color c = Image.color;

        while ( lerp < 1 && !visible)
        {
            lerp += Time.deltaTime * fadeSpeed * speed;
            Image.color = new Color(c.r, c.g, c.b, Mathf.Lerp(start, 0, lerp));
            yield return null;
        }

        if(visible)
            yield break;
        
        Image.enabled = false;
        Image.transform.parent.gameObject.SetActive(false);
    }
}
