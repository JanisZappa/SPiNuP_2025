using System.Collections;
using UnityEngine;


public class AnimationHandler : MonoBehaviour {

    private ISimpleAnim animType;
    public AnimationCurve[] aC;

    private Vector3 realPos;


    private void Start()
    {
        realPos = transform.localPosition;
        
        SetAnimation();
    }


    private IEnumerator CoolAnim()
    {
        float waitFor = Random.Range(0,2.0f);
        yield return new WaitForSeconds(waitFor);
        
        float count = 0;

        float speed = Random.Range(1f, 5.0f);

        AnimationCurve animCurve = aC[Random.Range(0, aC.Length)];
  
        while(count <1)
        {
            count += Time.deltaTime * speed;
            animType.Anim(transform, count, animCurve);
            yield return null;
        }

        SetAnimation();
    }


    private void SetAnimation()
    {
        Transform  t        = transform;
        Quaternion rot      = t.rotation;
        Vector3    localPos = t.localPosition;
        
        switch(Random.Range(0, 15))
        {
            case 0:  animType = new TurnLeft(localPos, rot); break;
            case 1:  animType = new TurnRight(localPos, rot); break;
            case 2:  animType = new TurnForward(localPos, rot); break;
            case 3:  animType = new TurnBack(localPos, rot); break;
            case 4:  animType = new SpinLeft(localPos, rot); break;
            case 5:  animType = new SpinRight(localPos, rot); break;
            case 6:  animType = new Move(localPos, rot); break;
            case 7:  animType = new ResetHeight(localPos, rot); break;
            case 8:  animType = new ResetHeight2(localPos, rot); break;
            case 9:  animType = new ResetHeight3(localPos, rot); break;
            case 10: animType = new ResetHeight(localPos, rot); break;
            case 11: animType = new ResetHeight2(localPos, rot); break;
            case 12: animType = new ResetHeight3(localPos, rot); break;
            case 13: animType = new Move2(localPos, rot,realPos); break;
            case 14: animType = new Move3(localPos, rot, realPos); break;
        }

        StartCoroutine(CoolAnim());
    }
}
