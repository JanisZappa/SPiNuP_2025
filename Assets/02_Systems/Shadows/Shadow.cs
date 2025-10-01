using UnityEngine;


public abstract class Shadow : MonoBehaviour 
{
	public SpriteRenderer spriteRenderer;
	public Sprite[]       sprites;
	public bool isLight;
	
	protected bool front = true;
	
	protected Transform _transform;

	
	private void Awake()
	{
		_transform = transform;
		
		SetLayerAndSprite();
	}


	public void SetSide(bool front)
	{
		if (this.front != front)
		{
			this.front = front;
			
			SetLayerAndSprite();
		}
	}


	protected virtual void SetLayerAndSprite()
	{
		if (sprites.Length > 0)
			spriteRenderer.sprite = sprites[front ? 0 : 1];
		
		gameObject.layer = isLight? front ? Layers.LightA  : Layers.LightB : 
			                        front ? Layers.ShadowA : Layers.ShadowB;
	}


	protected void ShadowUpdate(bool updateTransform)
	{
		if(true || front == GameCam.CurrentSide.front && updateTransform)
			UpdateTransform();
	}


	protected virtual void UpdateTransform(){}
	
	
	protected static Quaternion VectorRot(Vector2 vector)
	{
		return Rot.Z(Mathf.Atan2(vector.x, vector.y) * -Mathf.Rad2Deg);
	}
}
