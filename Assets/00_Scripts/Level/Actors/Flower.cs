using LevelElements;
using UnityEngine;


public class Flower : Stick
{
    private Quaternion itemRot;
        
        public override void SetItem(Item item)
        {
            base.SetItem(item);
            
            if(item != null)
                itemRot = Quaternion.AngleAxis(Mth.SmoothPP(-50, 50, item.ID * 13.161f), Vector3.forward);
        }
        
    protected override void SetLeanRot(Quaternion rot)
    {
        pivot.localRotation = rot * itemRot;
    }
}
