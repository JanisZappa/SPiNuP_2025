using LevelElements;
using UnityEngine;


public class ArrowStick : Stick
{
    private Quaternion pointRot = Quaternion.identity;
    
    public override void SetItem(Item item)
    {
        base.SetItem(item);
        
        if(item != null)
            pointRot = Quaternion.AngleAxis(-item.angle * item.side.Sign, Vector3.forward);
    }
    
    protected override void SetLeanRot(Quaternion rot)
    {
        pivot.localRotation = rot * pointRot;
    }
}
