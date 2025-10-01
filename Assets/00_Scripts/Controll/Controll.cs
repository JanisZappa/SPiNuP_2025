using UnityEngine;


public static class Controll
{
    public static void Update()
    {
	//  UI  //
		UI_Manager.CheckUI();
	    
    //  Mouse  //
        ScrollWheelDelta = !Application.isEditor || OnScreenInEditor ? Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime : 0;
        
        ZoomUpdate();
		TouchUpdate();
		ShakeUpdate();
    }
    
    
#region Mouse

    public static float ScrollWheelDelta;
	
    private static bool OnScreenInEditor
    {
        get
        {
            Vector2 viewPos = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
            return viewPos.x > 0 && viewPos.x < 1 && viewPos.y > 0 && viewPos.y < 1;
        }
    }
    
#endregion



#region Touches

	private static float PixelPerMilimeter => Screen.dpi * .048f;

	public delegate void Swipe(bool start);  public static event Swipe onSwipe;
	public delegate void Zoom(bool start);   public static event Zoom  onZoom;
	public delegate void Shake();            public static event Shake onShake;

	private static Vector2 startPos;
	private static float touchStartTime;
	private static bool canSwipe, swiping, zooming;
	
	private static int TouchID = NoTouchID;
	
	private const int MouseID   = int.MaxValue, 
		              NoTouchID = int.MinValue;
	
	public static Vector2 TouchPos;
	public static float ZoomLength;
	
	


	private static void TouchUpdate()
	{
		if (TouchStart)
			if (!UI_Manager.HitUI && !zooming)
			{
				swiping = false;
				canSwipe = true;
				startPos = TouchPos;
				touchStartTime = Time.realtimeSinceStartup;
			}
			else
				canSwipe = false;
		
		

		if (Touching && !swiping && canSwipe && !zooming)
			if (Time.realtimeSinceStartup - touchStartTime >= .1f || Vector2.Distance(TouchPos, startPos) / PixelPerMilimeter > 0)
			{
				onSwipe?.Invoke(true);

				swiping = true;
			}
		
		
		
		if (TouchStop && swiping)
		{
			onSwipe?.Invoke(false);

			swiping = false;
		}
	}
	

	private static bool TouchStart
	{
		get
		{
			if (Application.isMobilePlatform)
			{
				if (!zooming && Input.touchCount > 0 && TouchID == NoTouchID)
					for (int i = 0; i < Input.touchCount; i++)
					{
						Touch touch = Input.touches[i];
						
						if (touch.phase != TouchPhase.Canceled && touch.phase != TouchPhase.Ended)
						{
							TouchPos = touch.position;
							TouchID  = touch.fingerId;
							return true;
						}
					}
						
			}
			else
			{
				if (Input.GetMouseButtonDown(0))
				{
					TouchPos = Input.mousePosition;
					TouchID  = MouseID;
					return true;
				}
			}


			return false;
		}
	}
	
	
	private static bool Touching
	{
		get
		{
			if (Application.isMobilePlatform)
			{
				if (!zooming && Input.touchCount > 0)
					for (int i = 0; i < Input.touchCount; i++)
					{
						Touch touch = Input.touches[i];
						
						if (touch.fingerId == TouchID
						    && touch.phase != TouchPhase.Canceled 
						    && touch.phase != TouchPhase.Ended)
						{
							TouchPos = touch.position;
							return true;
						}
					}	
			}
			else
				if (Input.GetMouseButton(0))
				{
					TouchPos = Input.mousePosition;
					return true;
				}
			

			return false;
		}
	}
	
	
	private static bool TouchStop
	{
		get
		{
			if (Application.isMobilePlatform)
			{
				if (!zooming && Input.touchCount > 0)
					for (int i = 0; i < Input.touchCount; i++)
						if (Input.touches[i].fingerId == TouchID
						    && (Input.touches[i].phase == TouchPhase.Canceled || Input.touches[i].phase == TouchPhase.Ended))
						{
							TouchID = NoTouchID;
							return true;
						}
				
				if (zooming || Input.touchCount == 0 && TouchID != NoTouchID && TouchID != MouseID)
				{
					TouchID = NoTouchID;
					return true;
				}
			}
			else
				if (Input.GetMouseButtonUp(0))
				{
					TouchID = NoTouchID;
					return true;
				}
			
			
			return false;
		}
	}


	private static void ZoomUpdate()
	{
		bool isZooming = Application.isMobilePlatform && Input.touchCount == 2;
		
		if (isZooming)
			ZoomLength = Vector3.Distance(Input.touches[0].position, Input.touches[1].position) / PixelPerMilimeter;

		if (isZooming && !zooming)
		{
			zooming = true;
			onZoom?.Invoke(true);
		}
		
		if (!isZooming && zooming)
		{
			zooming = false;
			onZoom?.Invoke(false);
		}
	}

#endregion



#region Shake

	private static Vector3 lowPassValue;
	
	private static void ShakeUpdate()
	{
		if (!Application.isMobilePlatform)
		{
			if (KeyMap.Down(Key.Rewind) && onShake != null)
				onShake();
		}
		else
		{
			Vector3 acceleration = Input.acceleration;
			lowPassValue      = Vector3.Lerp(lowPassValue, acceleration, Time.deltaTime * 4);
			Vector3 deltaAcceleration = acceleration - lowPassValue;
		
			const float threshold = .75f * .75f;
			if(deltaAcceleration.sqrMagnitude >= threshold && onShake != null)
				onShake();
		}
	}

#endregion
}