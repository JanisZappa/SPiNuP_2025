using UnityEngine;


public interface ISimpleAnim
{
    void Anim(Transform trans, float lerp, AnimationCurve curve);
}

public class TurnLeft : ISimpleAnim 
{
    Quaternion startRot;

    public TurnLeft(Vector3 startPos, Quaternion startRot)
    {
        this.startRot = startRot;
    }
    
    public void Anim(Transform trans, float lerp, AnimationCurve curve)
    {
        float animLerp = curve.Evaluate(lerp);

        Quaternion goalRot = new Quaternion();
        goalRot.eulerAngles = startRot.eulerAngles + new Vector3(0, 0, 90);

        trans.rotation = Quaternion.Slerp(startRot, goalRot, animLerp);
    }
}

public class TurnRight : ISimpleAnim
{
    Quaternion startRot;

    public TurnRight(Vector3 startPos, Quaternion startRot)
    {
        this.startRot = startRot;
    }

    public void Anim(Transform trans, float lerp, AnimationCurve curve)
    {
        float animLerp = curve.Evaluate(lerp);

        Quaternion goalRot = new Quaternion();
        goalRot.eulerAngles = startRot.eulerAngles + new Vector3(0, 0, -90);

        trans.rotation = Quaternion.Slerp(startRot, goalRot, animLerp);
    }
}

public class TurnForward : ISimpleAnim
{
    Quaternion startRot;

    public TurnForward(Vector3 startPos, Quaternion startRot)
    {
        this.startRot = startRot;
    }

    public void Anim(Transform trans, float lerp, AnimationCurve curve)
    {
        float animLerp = curve.Evaluate(lerp);

        Quaternion goalRot = new Quaternion();
        goalRot.eulerAngles = startRot.eulerAngles + new Vector3(90, 0, 0);

        trans.rotation = Quaternion.Slerp(startRot, goalRot, animLerp);
    }
}

public class TurnBack : ISimpleAnim
{
    Quaternion startRot;

    public TurnBack(Vector3 startPos, Quaternion startRot)
    {
        this.startRot = startRot;
    }

    public void Anim(Transform trans, float lerp, AnimationCurve curve)
    {
        float animLerp = curve.Evaluate(lerp);

        Quaternion goalRot = new Quaternion();
        goalRot.eulerAngles = startRot.eulerAngles + new Vector3(-90, 0, 0);

        trans.rotation = Quaternion.Slerp(startRot, goalRot, animLerp);
    }
}

public class SpinLeft : ISimpleAnim
{
    Quaternion startRot;

    public SpinLeft(Vector3 startPos, Quaternion startRot)
    {
        this.startRot = startRot;
    }

    public void Anim(Transform trans, float lerp, AnimationCurve curve)
    {
        float animLerp = curve.Evaluate(lerp);

        Quaternion goalRot = new Quaternion();
        goalRot.eulerAngles = startRot.eulerAngles + new Vector3(0, 90, 0);

        trans.rotation = Quaternion.Slerp(startRot, goalRot, animLerp);
    }
}

public class SpinRight : ISimpleAnim
{
    Quaternion startRot;

    public SpinRight(Vector3 startPos, Quaternion startRot)
    {
        this.startRot = startRot;
    }

    public void Anim(Transform trans, float lerp, AnimationCurve curve)
    {
        float animLerp = curve.Evaluate(lerp);

        Quaternion goalRot = new Quaternion();
        goalRot.eulerAngles = startRot.eulerAngles + new Vector3(0, -90, 0);

        trans.rotation = Quaternion.Slerp(startRot, goalRot, animLerp);
    }
}

public class Move : ISimpleAnim
{
    Vector3 startPos;
    float goalHeight;

    public Move(Vector3 startPos, Quaternion startRot)
    {
        this.startPos = startPos;
        goalHeight = Random.Range(0.0f, 2.0f);
    }

    public void Anim(Transform trans, float lerp, AnimationCurve curve)
    {
        float yPos = Mathf.Lerp(startPos.y,startPos.y + goalHeight, curve.Evaluate(lerp));
        trans.localPosition = new Vector3(trans.localPosition.x,yPos,trans.localPosition.z);
    }
}

public class Move2 : ISimpleAnim
{
    Vector3 startPos;
    Vector3 endPos;

    public Move2(Vector3 startPos, Quaternion startRot,Vector3 realPos)
    {
        this.startPos = startPos;
        endPos = new Vector3(realPos.x + Random.Range(-0.4f, 0.4f), startPos.y, realPos.z + Random.Range(-0.4f, 0.4f));
    }

    public void Anim(Transform trans, float lerp, AnimationCurve curve)
    {
        trans.localPosition = Vector3.Lerp(startPos, endPos, curve.Evaluate(lerp));
    }
}

public class Move3 : ISimpleAnim
{
    Vector3 startPos;
    Vector3 endPos;

    public Move3(Vector3 startPos, Quaternion startRot, Vector3 realPos)
    {
        this.startPos = startPos;
        endPos = new Vector3(realPos.x + Random.Range(-0.4f, 0.4f), startPos.y + Random.Range(0.0f, 5.0f), realPos.z + Random.Range(-0.4f, 0.4f));
    }

    public void Anim(Transform trans, float lerp, AnimationCurve curve)
    {
        trans.localPosition = Vector3.Lerp(startPos, endPos, curve.Evaluate(lerp));
    }
}

public class ResetHeight : ISimpleAnim
{
    Vector3 startPos;
    float goalHeight;

    public ResetHeight(Vector3 startPos, Quaternion startRot)
    {
        this.startPos = startPos;
        goalHeight = 0;
    }

    public void Anim(Transform trans, float lerp, AnimationCurve curve)
    {
        float yPos = Mathf.Lerp(startPos.y, goalHeight, curve.Evaluate(lerp));
        trans.localPosition = new Vector3(trans.localPosition.x, yPos, trans.localPosition.z);
    }
}

public class ResetHeight2 : ISimpleAnim
{
    Vector3 startPos;
    Quaternion startRot;
    float goalHeight;

    Quaternion goalRot;

    public ResetHeight2(Vector3 startPos, Quaternion startRot)
    {
        this.startPos = startPos;
        this.startRot = startRot;
        goalHeight = 0;

        goalRot = new Quaternion();
        int pick = Random.Range(0, 6);

        switch(pick)
        {
            case 0: goalRot.eulerAngles = startRot.eulerAngles + new Vector3(0, -90, 0); break;
            case 1: goalRot.eulerAngles = startRot.eulerAngles + new Vector3(0, 90, 0); break;
            case 2: goalRot.eulerAngles = startRot.eulerAngles + new Vector3(90, 0, 0); break;
            case 3: goalRot.eulerAngles = startRot.eulerAngles + new Vector3(-90,0, 0); break;
            case 4: goalRot.eulerAngles = startRot.eulerAngles + new Vector3(0, 0, 90); break;
            case 5: goalRot.eulerAngles = startRot.eulerAngles + new Vector3(0, 0, -90); break;
        }
       
    }

    public void Anim(Transform trans, float lerp, AnimationCurve curve)
    {
        float animLerp = curve.Evaluate(lerp);
        trans.rotation = Quaternion.Slerp(startRot, goalRot, animLerp);

        float yPos = Mathf.Lerp(startPos.y, goalHeight, animLerp);
        trans.localPosition = new Vector3(trans.localPosition.x, yPos, trans.localPosition.z);
    }
}

public class ResetHeight3 : ISimpleAnim
{
    Vector3 startPos;
    Quaternion startRot;
    float goalHeight;

    Quaternion goalRot;

    public ResetHeight3(Vector3 startPos, Quaternion startRot)
    {
        this.startPos = startPos;
        this.startRot = startRot;
        goalHeight = 0;

        goalRot = new Quaternion();
        int pick = Random.Range(0, 6);

        switch (pick)
        {
            case 0: goalRot.eulerAngles = startRot.eulerAngles + new Vector3(270, -90, 0); break;
            case 1: goalRot.eulerAngles = startRot.eulerAngles + new Vector3(0, 90, 180); break;
            case 2: goalRot.eulerAngles = startRot.eulerAngles + new Vector3(90, 90, 90); break;
            case 3: goalRot.eulerAngles = startRot.eulerAngles + new Vector3(-90, 360, 0); break;
            case 4: goalRot.eulerAngles = startRot.eulerAngles + new Vector3(0, 90, 90); break;
            case 5: goalRot.eulerAngles = startRot.eulerAngles + new Vector3(90, 0, -90); break;
        }

    }

    public void Anim(Transform trans, float lerp, AnimationCurve curve)
    {
        float animLerp = curve.Evaluate(lerp);
        trans.rotation = Quaternion.Slerp(startRot, goalRot, animLerp);

        float yPos = Mathf.Lerp(startPos.y, goalHeight, animLerp);
        trans.localPosition = new Vector3(trans.localPosition.x, yPos, trans.localPosition.z);
    }
}
