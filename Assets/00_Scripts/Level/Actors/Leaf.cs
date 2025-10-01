using LevelElements;
using UnityEngine;


public class Leaf : Stick
{
	private Quaternion itemRot;
        
	public override void SetItem(Item item)
	{
		base.SetItem(item);
            
		/*if(item != null)
			itemRot = Quaternion.Euler(
				Mth.SmoothPP(-20, 20, item.ID * 13.161f),
				Mth.SmoothPP(-25, 25, item.ID * 27.161f),
				Mth.SmoothPP(-10, 10, item.ID * 39.161f));*/
		
		if(item != null)
			itemRot = Quaternion.Euler(
				Mth.Repeat(-20, 20, item.ID * 13.161f),
				Mth.Repeat(-35, 35, item.ID * 27.161f),
				Mth.Repeat( -5,  5, item.ID * 39.161f));
	}
        
	protected override void SetLeanRot(Quaternion rot)
	{
		pivot.localRotation = rot * itemRot;
	}
}
