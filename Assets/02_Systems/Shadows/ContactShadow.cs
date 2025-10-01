public class ContactShadow : ActorShadow 
{
	protected override void UpdateTransform(){_transform.localPosition = depthOffset;}
}
