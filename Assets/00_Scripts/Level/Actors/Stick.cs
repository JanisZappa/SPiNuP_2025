using LevelElements;
using UnityEngine;


public class Stick : Actor
{
    public override void SetTransform(bool forcedUpdate)
    {
        if (!HasToUpdate() && !forcedUpdate && false)
        {
            ShadowUpdate(false);
            return;
        }
        
        if(anim != null && anim.item != item)
            Debug.LogFormat("Wrong Anim! {0} is getting Anim from {1}", item.ID, anim.item.ID);

        Vector2 moveLean;
        if (forcedUpdate || item.parent != null)
        {
            Vector2 pos     = item.GetPos(GTime.Now);
            Vector2 leanPos = item.GetLagPos(GTime.Now);
            
            SetDepthPos(pos);
            
            moveLean = leanPos - pos;
        }
        else
            moveLean = Vector2.zero;
        
        Vector2 lean = moveLean + (anim?.GetLeanNow() ?? V2.zero);

        Quaternion sideRot = Rot.Y(item.side.front ? 0 : 180);
        Quaternion leanRot = lean.LeanRotLocal(item);
        
        SetLeanRot(leanRot);
        TipOffset = sideRot * leanRot * new Vector3(0, 0, 5);
                  
        ShadowUpdate(true);
    }


    protected virtual void SetLeanRot(Quaternion rot)
    {
        pivot.localRotation = rot;
    }
}


public static class RotHelp
{
    public static Quaternion LeanRotLocal(this Vector2 lean, Item item)
    {
        float offset = Level.PlaneOffset - item.depth;
        return Quaternion.FromToRotation(new Vector3(0, 0, offset), 
                                         new Vector3(lean.x * item.side.Sign, -lean.y, offset));
    }
    public static Quaternion LeanRotGlobal(this Vector2 lean, Item item)
    {
        float offset = Level.PlaneOffset - item.depth;
        return Quaternion.FromToRotation(new Vector3(0, 0, offset), 
                                         new Vector3(lean.x * item.side.Sign, lean.y * item.side.Sign, offset));
    }
}