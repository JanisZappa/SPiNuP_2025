using UnityEngine;
using UnityEngine.UI;


public class NonDrawingGraphic : Graphic
{
	public override void SetMaterialDirty() {}
	public override void SetVerticesDirty() {}

	private int count;

	protected override void OnPopulateMesh(VertexHelper vh) 
	{
		color = new Color(0,0,0,0);
		raycastTarget = true;
	}
}