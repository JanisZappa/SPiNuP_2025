using GeoMath;
using UnityEngine;


//    http://csharphelper.com/blog/2016/08/convert-between-rgb-and-hls-color-models-in-c/

public struct HLS
{
    public readonly float h, l, s, a;
    
    public HLS(float h, float l, float s, float a = 1)
    {
        this.h = h;
        this.l = l;
        this.s = s;
        this.a = a;
    }

    public HLS(float hue)
    {
        h = hue;
        l = .5f;
        s = 1;
        a = 1;
    }


    public Vector2 HueDir { get { return Rot.Z(h * 360) * Vector2.up; } }


    public static Color Get(float hue)
    {
        return new HLS(Mathf.Repeat(hue, 1), .5f, 1);
    }
    
    
    public static implicit operator Color(HLS hls)
    {
        float h = hls.h;
        float luminance  = hls.l;
        float saturation = hls.s;
       
        if (Mathf.Approximately(saturation, 0))
            return new Color(luminance, luminance, luminance, hls.a);
       
        float p2 = luminance <= .5f? luminance * (1 + saturation) : luminance + saturation - luminance * saturation;
        float p1 = 2 * luminance - p2;
        
        const float fraction = 1f / 3;
        return new Color(QqhToRgb(p1, p2, h + fraction), QqhToRgb(p1, p2, h), QqhToRgb(p1, p2, h - fraction), hls.a);
    }

    
    public static implicit operator Color32(HLS hls)
    {
        float h = hls.h;
        float luminance  = hls.l;
        float saturation = hls.s;
       
        if (Mathf.Approximately(saturation, 0))
            return new Color(luminance, luminance, luminance, hls.a);
       
        float p2 = luminance <= .5f? luminance * (1 + saturation) : luminance + saturation - luminance * saturation;
        float p1 = 2 * luminance - p2;
        
        const float fraction = 1f / 3;
        return new Color(QqhToRgb(p1, p2, h + fraction), QqhToRgb(p1, p2, h), QqhToRgb(p1, p2, h - fraction), hls.a);
    }
    
    
    private static float QqhToRgb(float q1, float q2, float hue)
    {
        hue = Mathf.Repeat(hue, 1);

        if (hue < 1f / 6) 
            return q1 + (q2 - q1) * hue * 6;
        
        if (hue < .5f) 
            return q2;
        
        const float multiB = 1f / 6 * 4;
        if (hue < multiB) 
            return q1 + (q2 - q1) * (multiB - hue) * 6;
        
        return q1;
    }
}


public static class HLSext
{
    public static HLS ToHLS(this Color color)
    {
        float r = color.r, g = color.g, b = color.b;
        
        float max = Mathf.Max(Mathf.Max(r, g), b);
        float min = Mathf.Min(Mathf.Min(r, g), b);
        
        float diff = max - min;
        float luminance = (max + min) * .5f;
        
        if (Mathf.Abs(diff) < 0.00001)
            return new HLS(0, luminance, 0, color.a);
        
        float saturation = luminance <= .5? diff / (max + min) : diff / (2 - max - min);
        float hue;
        
        float distMulti = 1f / diff;
        float r_dist = (max - r) * distMulti;
        float g_dist = (max - g) * distMulti;
        float b_dist = (max - b) * distMulti;

        if (Mathf.Approximately(r, max)) 
            hue = b_dist - g_dist;
        else 
            hue = g > b ? 2 + r_dist - b_dist : 4 + g_dist - r_dist;

        const float multi = 1f / 6;
        hue = Mathf.Repeat(hue * multi, 1);
        
        return new HLS(hue, luminance, saturation, color.a);
    }
    
    
    public static HLS ShiftHue(this HLS color, float shift)
    {
        return new HLS(Mathf.Repeat(color.h + shift, 1), color.l, color.s, color.a);
    }
    
    
    public static HLS SetLuminace(this HLS color, float value)
    {
        return new HLS(color.h, value, color.s, color.a);
    }
    
    
    public static HLS SetSaturation(this HLS color, float value)
    {
        return new HLS(color.h, color.l, value, color.a);
    }


    public static HLS MapGamut(this HLS hls, Line a, Line b, Line c)
    {
       Line dir = new Line(Vector2.zero, hls.HueDir);
       Vector2 hitPoint;
       if(dir.Contact(a, out hitPoint) || dir.Contact(b, out hitPoint) || dir.Contact(c, out hitPoint))
           return new HLS(hls.h, hls.l, hitPoint.magnitude, hls.a);

       return hls;
    }


    public static HLS HueSlerp(this HLS hls, HLS other, float lerp)
    {
        float angle = hls.HueDir.Angle_Sign(other.HueDir) / 360;
        float curve = 1 - Mathf.Pow(Mathf.Abs(angle * 2), 2);
              curve = Mathf.SmoothStep(0, 1, 1 - Mathf.Abs(angle * 2));
        
        //return new HLS(0, Mathf.Abs(angle) * 2, 0, 1);
        
        
        return new HLS(Mathf.Repeat(hls.h - angle * lerp, 1), hls.l, hls.s, hls.a);
    }


    public static HLS SetA(this HLS hls, float a)
    {
        return new HLS(hls.h, hls.l, hls.s, a);
    }
}