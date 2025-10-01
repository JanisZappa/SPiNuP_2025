public class NewContactShadow : Shadow 
{
	protected override void UpdateTransform(){}

	protected override void SetLayerAndSprite()
	{
		gameObject.layer = front ? Layers.ShadowA : Layers.ShadowB;
	}
}
