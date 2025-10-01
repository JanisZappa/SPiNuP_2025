using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Exp
{
    public static float T(float delta)
    {
        //return delta;
        return 1.0f - Mathf.Exp(-delta);
    }
}
