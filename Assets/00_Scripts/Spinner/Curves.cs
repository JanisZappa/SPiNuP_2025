using UnityEngine;

public class Curves : MonoBehaviour
{
    public AnimationCurve swingLeanAnim;

    public static AnimationCurve SwingLeanAnim;

    private void Awake()
    {
        SwingLeanAnim = swingLeanAnim;
    }
}
