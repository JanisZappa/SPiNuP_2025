using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
public class Curves : Singleton<Curves> 
{
    public AnimationCurve swingLeanAnim;

    public static AnimationCurve SwingLeanAnim
    {
        get
        {
            if (Inst == null)
            {
                Debug.Log("Fuuuck");
            }
            return Inst.swingLeanAnim;
        }
    }
}
