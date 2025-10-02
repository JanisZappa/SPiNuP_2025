using UnityEngine;

public class Curves : Singleton<Curves>
{
    public AnimationCurve swingLeanAnim;

    public static AnimationCurve SwingLeanAnim => Inst.swingLeanAnim;
}
