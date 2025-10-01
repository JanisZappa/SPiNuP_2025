using UnityEngine;


public class Curves : Singleton<Curves> 
{
    public AnimationCurve swingLeanAnim;
    
    public static AnimationCurve SwingLeanAnim  { get { return Inst.swingLeanAnim; } }
}
