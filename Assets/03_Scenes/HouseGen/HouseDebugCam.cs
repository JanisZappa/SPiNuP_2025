using GeoMath;
using UnityEngine;


public class HouseDebugCam : MonoBehaviour 
{
	public Color a, b;
	
	public float pad;
	

	[HideInInspector] 	public bool back;
	
	private Camera cam;
	private bool grab;
	
	private Vector2 grabPos;
	[HideInInspector] public Vector2 mousePos;
	
	public static Bounds2D CamBounds;


	public float OrthoSize
	{
		get
		{
			return cam.orthographicSize;
		}
	}


	private void Awake()
	{
		cam = GetComponentInChildren<Camera>();
	}


	public void UpdatePos()
	{
		mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
		
		
		if (!grab && Input.GetKeyDown(KeyCode.Mouse0))
		{
			grab = true;
			grabPos = Input.mousePosition;
		}
		if(grab && Input.GetKeyUp(KeyCode.Mouse0))
			grab = false;

		if (grab)
		{
			Vector2 grabPosNow = Input.mousePosition;
			Vector3 grabOffset = (grabPosNow - grabPos) / Screen.height* (cam.orthographicSize * 2);
			
			if(back)
				grabOffset.x *= -1;
			
			transform.position -= grabOffset;
			
			grabPos = grabPosNow;
		}
		else
		{
			float orthoSize = cam.orthographicSize;
			cam.orthographicSize = Mathf.Clamp(orthoSize - Input.mouseScrollDelta.y * Time.deltaTime * orthoSize * 3, 20, 1000);
		}
		
		UpdateBounds();
	}


	public void SwitchSide()
	{
		back = !back;
		transform.rotation = Quaternion.AngleAxis(back? 180 : 0, Vector3.up);
		cam.backgroundColor = back ? b : a;
	}


	public void FrameBuilding(Bounds2D bounds)
	{
		bounds = bounds.Pad(pad);
		
		transform.position = bounds.Center;
		
		float aspect = cam.aspect;
		float verticalSize   = bounds.Size.y * .5f;
		float horizontalSize = bounds.Size.x * .5f;
		float aspectHorizontalSize = verticalSize * aspect;
		
		cam.orthographicSize = Mathf.Max(verticalSize, verticalSize * (horizontalSize / aspectHorizontalSize));
		grab = false;
	}
	
	
	public void FrameHorizontal(Bounds2D bounds)
	{
		bounds = bounds.Pad(pad);
		
		transform.position = bounds.Center;
		
		float aspect = cam.aspect;
		float verticalSize   = bounds.Size.y * .5f;
		float horizontalSize = bounds.Size.x * .5f;
		float aspectHorizontalSize = verticalSize * aspect;
		
		cam.orthographicSize = verticalSize * (horizontalSize / aspectHorizontalSize);
		grab = false;
	}
	
	
	public void FrameVertical(Bounds2D bounds)
	{
		bounds = bounds.Pad(pad);
		
		transform.position   = bounds.Center;
		cam.orthographicSize = bounds.Size.y * .5f;
		grab = false;
	}


	private void UpdateBounds()
	{
		Vector2 center = transform.position;
		float verticalSize   = cam.orthographicSize * 2;
		float horizontalSize = verticalSize * cam.aspect;
		
		Vector2 halfSize = new Vector2(horizontalSize, verticalSize) * .5f;
		
		CamBounds = new Bounds2D(center - halfSize).Add(center + halfSize).Pad(pad);;
	}

	
	public static bool IsVisible(Bounds2D bounds)
	{
		return CamBounds.Intersects(bounds);
	}
}
