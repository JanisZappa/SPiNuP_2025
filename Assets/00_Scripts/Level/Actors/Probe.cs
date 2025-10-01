using UnityEngine;


public class Probe : Stick 
{
    public override void SetTransform(bool forcedUpdate)
    {
        if(!HasToUpdate() && false && !forcedUpdate)
        {
            ShadowUpdate(false);
            return;
        }
        
        Vector2 pos     = item.GetPos(GTime.Now);
        Vector2 leanPos = item.GetLagPos(GTime.Now);
        Vector2 newLean = leanPos - pos + (anim?.GetLeanNow() ?? V2.zero);
    
        _transform.position = (pos + newLean).V3(Level.GetPlaneDist(item.side));


        float tiltTime = GTime.Now + .2f;
        Vector2 tiltLean = (item.GetLagPos(tiltTime) - item.GetPos(tiltTime)) * -2 
                         + (anim?.GetLean(GTime.Now - .2f) ?? V2.zero);
        
        
        Quaternion sideRot = Rot.Y(item.side.front? 0 : 180);
        Quaternion leanRot = (tiltLean * 3).LeanRotLocal(item);
        
        pivot.localRotation = leanRot;
        TipOffset = sideRot * leanRot * (Vector3.forward * Level.PlaneOffset);
        
        ShadowUpdate(true);
    }
}
