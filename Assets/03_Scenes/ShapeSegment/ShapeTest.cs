using ShapeStuff;
using UnityEngine;


public class ShapeTest : MonoBehaviour
{
	public Shape shape;
	private readonly ShapeLineDrawer lineDrawer = new ShapeLineDrawer();
	private readonly ShapeMesher     mesher     = new ShapeMesher();
	private ShapeFacingDrawer facingDrawer;
	
	public Camera orthoCam;
	
	[Range(.1f, 1)]
	public  float speed;
	private float time;
	
	private readonly Shape shellShape = new Shape();
	private readonly ShapeLineDrawer shellDrawer = new ShapeLineDrawer(400);
	
	private readonly Shape collisionShape = new Shape();
	private readonly ShapeLineDrawer collisionDrawer = new ShapeLineDrawer();

	private readonly Shape collisionShellShape = new Shape();
	private readonly ShapeLineDrawer collisionShellDrawer = new ShapeLineDrawer(400);

	private Vector2 p1, p2;

	[Space(10)] 
	public EdgeMesher edgeMesher;
	
	
	private Shape[] allShapes;
	private int shapePick, undos;
	private bool selfIntersecting;


	private void Awake()
	{
		allShapes = new Shape[20];
		for (int i = 0; i < 20; i++)
			allShapes[i] = new Shape();
		
		facingDrawer = new ShapeFacingDrawer();
	}


	private void OnEnable()
	{
		NewShape(Random.Range(0, 10));
	}
	
	
	private void Update()
	{
		time += Time.deltaTime * speed;

	#region Input Handling
	
	//  New Shape  //
		if (Input.GetKeyDown(KeyCode.Space))	NewShape(Random.Range(0, 10));
		if (Input.GetKeyDown(KeyCode.Alpha0))	NewShape(0);
		if (Input.GetKeyDown(KeyCode.Alpha1))	NewShape(1);
		if (Input.GetKeyDown(KeyCode.Alpha2))	NewShape(2);
		if (Input.GetKeyDown(KeyCode.Alpha3))	NewShape(3);
		if (Input.GetKeyDown(KeyCode.Alpha4))	NewShape(4);
		if (Input.GetKeyDown(KeyCode.Alpha5))	NewShape(5);
		if (Input.GetKeyDown(KeyCode.Alpha6))	NewShape(6);
		if (Input.GetKeyDown(KeyCode.Alpha7))	NewShape(7);
		if (Input.GetKeyDown(KeyCode.Alpha8))	NewShape(8);
		if (Input.GetKeyDown(KeyCode.Alpha9))	NewShape(9);
		
		if (Input.GetKeyDown(KeyCode.Backspace))	Undo();


	//  Rotate Shape //
		{
			float rad = (Input.GetKey(KeyCode.Q)? 1 : 0) + (Input.GetKey(KeyCode.Q)? -1 : 0);
			      
			if (!f.Same(rad, 0))
			{
				shape.RotateAround(MousePoint, rad * Time.deltaTime);
				UpdateShapeStuff(false);
			}
		}


		     drawLine.KeySwitch(KeyCode.L);
		     drawMesh.KeySwitch(KeyCode.M);
		    drawArrow.KeySwitch(KeyCode.A);
		   drawPoints.KeySwitch(KeyCode.P);
		showCollision.KeySwitch(KeyCode.C);
		drawIndicator.KeySwitch(KeyCode.I);
		   drawFacing.KeySwitch(KeyCode.F);
		
		drawBounds.Set(Input.GetKeyDown(KeyCode.B) ? (drawBounds + 1) % 3 : drawBounds);
		
		
	#endregion
		
		
		facingDrawer.UpdateRatios(shape);

		float tipLerp = Mathf.Repeat(time / GTime.LoopTime, 1);
		
		if (drawBounds == 1)
			ShapeLineDrawer.DrawBounds(shape);
		if(drawBounds == 2)
			lineDrawer.DrawFractions(tipLerp);
		
		if(drawPoints)
			ShapeLineDrawer.DrawPoints(shape);

		if (drawLine && !(drawFacing && facingDrawer.Active && shape.loop))
		{
			lineDrawer.DrawLine();
			shellDrawer.DrawLine();

			for (int i = 0; i < shape.segmentCount; i++)
				shape.segments[i].DrawLine();


			float thickness = Mathf.Min(10, shape.PossibleThickness) * Mth.SmoothPP(.1f, 1, Time.realtimeSinceStartup);

			for (int i = 0; i < shape.segmentCount; i++)
				shape.segments[i].DrawShell(thickness);
		}

		if (drawArrow)
			lineDrawer.DrawArrows();

		if(drawMesh)
			mesher.DrawTriangles();

		if (drawFacing)
		{
			facingDrawer.DrawFacings(shape);
			facingDrawer.DrawStartDir(shape);
		}

		if (showCollision)
		{
			Vector2 movement = new Vector2((Input.GetKey(KeyCode.RightArrow) ? 1 : 0) + (Input.GetKey(KeyCode.LeftArrow) ? -1 : 0), 
				                           (Input.GetKey(KeyCode.UpArrow)    ? 1 : 0) + (Input.GetKey(KeyCode.DownArrow) ? -1 : 0));

			if (movement != Vector2.zero)
			{
				collisionShape.Move(movement.normalized * Time.deltaTime * 4);

				bool intersection = collisionShape.Intersects(shape);
				bool inside = !intersection && shape.IsInside(collisionShape.segments[0].pointA);
				collisionDrawer.SetShape(collisionShape, intersection ? COLOR.green.spring : (inside ? COLOR.yellow.golden : COLOR.turquois.bright.A(.2f)));

				collisionShellShape.CreateExtrude(collisionShape);
				collisionShellDrawer.SetShape(collisionShellShape, COLOR.grey.dark);
			}

		//  Rotate  //
			float rad = (Input.GetKey(KeyCode.Z)? 1 : 0) + (Input.GetKey(KeyCode.X)? -1 : 0);
			
			if (!f.Same(rad, 0))
			{
				collisionShape.RotateAround(MousePoint, rad * Time.deltaTime);
				CheckCollision();
			}

			collisionDrawer.DrawLine();
			ShapeLineDrawer.DrawPoints(collisionShape, true, collisionDrawer);


			if (collisionShape.GetClosestDistance(shape, out p1, out p2) > 0)
			{
				Vector2 dir = p2 - p1;
				DRAW.TwoWayArrow(p1, dir, .1f).SetColor(Color.white).Fill(1);
				DRAW.Circle(Vector2.Lerp(p1, p2, .5f), dir.magnitude * .5f, 30).SetColor(Color.white.A(.2f));
			}
		}

	
	//  Indicator  //
		if(drawIndicator && OnScreen(Input.mousePosition))
		{
			Vector2 mousePoint = MousePoint;
			Color   pointColor = shape.IsInside(MousePoint)? Color.yellow : Color.cyan;
			
			
			Vector2 closestPoint = shape.GetClosestPoint(mousePoint);
			float   distance     = Vector2.Distance(mousePoint, closestPoint);
			const float max = 5;
			if (distance < max)
			{
				float lerp = 1 - distance / max;
				
				DRAW.MultiCircle(closestPoint, .3f * lerp, 3, .08f * lerp, 16).SetColor(pointColor).Fill(.1f);
				DRAW.Vector(mousePoint, closestPoint - mousePoint).SetColor(pointColor);
			}

			if(drawPoints)
				ShapeLineDrawer.DrawCenters(shape);
			
			if (selfIntersecting)
			{
				DRAW.Circle(shape.intersectionPoint, .05f, 20).SetColor(COLOR.red.firebrick).Fill(1);
				DRAW.GapVector(mousePoint, shape.intersectionPoint - mousePoint, 20).SetColor(pointColor);
			}
		}
		
		
		#region SNAKE OR STICKS
		
		drawSnake.Set(Input.GetKeyDown(KeyCode.S) ? (drawSnake + 1) % 3 : drawSnake);
		
		Color grey = new Color(.98f, .98f, .98f, 1);
		if (drawSnake == 1)
		{
			float length = shape.length * .1f;

			int steps = Mathf.CeilToInt(length / .2f);
			float lerpStep = .1f / steps;

			for (int i = 0; i < steps; i++)
			{
				float lerp = Mathf.Repeat(time / GTime.LoopTime - lerpStep * i, 1);
				DRAW.Circle(shape.GetPoint(lerp), .05f, 20).SetColor(grey).Fill(1);
				DRAW.Circle(shape.GetPoint(lerp, .08f), .02f, 20).SetColor(COLOR.red.tomato).Fill(1);
				DRAW.Circle(shape.GetPoint(lerp, -.08f), .02f, 20).SetColor(COLOR.green.spring).Fill(1);
			}

			Vector2 dir = shape.GetDir(tipLerp);
			DRAW.Arrow(shape.GetPoint(tipLerp) + dir * .05f, dir * .2f, .05f).SetColor(COLOR.blue.deepsky).Fill(1);
		}
		if (drawSnake == 0)
		{
			Vector2 dir = V2.up.Rot(54);
			Color c = shape.loop? (shape.clockwise ? COLOR.green.spring : COLOR.red.tomato) : COLOR.blue.deepsky;
			for (int i = 0; i < 8; i++)
			{
				float   lerp   = Mathf.Repeat(time / GTime.LoopTime + .125f * i, 1);
				Vector2 point  = shape.GetPoint(lerp);
				float   extend = shape.loop? 1 : (lerp < .5f ? Mathf.SmoothStep(0, 1, lerp * 20) 
					: Mathf.SmoothStep(1, 0, (lerp - .95f) * 20));
					
				DRAW.Circle(point + dir * .8f * extend, .05f, 20).SetColor(c).Fill(1);
				DRAW.Circle(point, .025f, 20).SetColor(grey).Fill(1);
				DRAW.Vector(shape.GetPoint(lerp), dir * .75f * extend).SetColor(grey);
			}
		}
		
		#endregion
	}


	private void NewShape(int shapeNumber)
	{
		shapePick = (shapePick + 1) % 20;
		shape = allShapes[shapePick];
		undos = Mathf.Min(undos + 1, 20);

		shape.CreateByNumber(shapeNumber);
		
		UpdateShapeStuff(true);
	}
	
	
	private void UpdateShapeStuff(bool newShape)
	{
		  lineDrawer.SetShape(shape, COLOR.red.tomato);
		      mesher.SetShape(shape);
		facingDrawer.UpdateFacings(shape);
		  edgeMesher.SetShape(shape);
		  shellShape.CreateExtrude(shape);
		 shellDrawer.SetShape(shellShape, COLOR.grey.mid);
		
		
		if (newShape)
		{
			selfIntersecting = shape.Intersects(shape);
			SetOrthoCam();
			
			collisionShape.CreateByNumber(Random.Range(0, 10));
		}
		
		CheckCollision();
	}


	private void CheckCollision()
	{
		bool intersection = collisionShape.Intersects(shape);
		bool inside = !intersection && shape.IsInside(collisionShape.segments[0].pointA);
		collisionDrawer.SetShape(collisionShape, intersection ? COLOR.green.spring : (inside? COLOR.yellow.golden : COLOR.turquois.bright.A(.2f)));

		collisionShellShape.CreateExtrude(collisionShape);
		collisionShellDrawer.SetShape(collisionShellShape, COLOR.grey.dark);
	}


	private void Undo()
	{
		if (undos == 0)
			return;

		undos--;

		shapePick--;
		if (shapePick < 0)
			shapePick = 19;
		
		shape = allShapes[shapePick];
		
		UpdateShapeStuff(true);
	}


	private void SetOrthoCam()
	{
		orthoCam.transform.parent.position = shape.bounds.Center;
		orthoCam.orthographicSize   = shape.bounds.Size.y * .5f + 2;
	}


	private static bool OnScreen(Vector2 pos)
	{
		float margin = Screen.height / 30f;
		return pos.x > margin && pos.x < Screen.width - margin && pos.y > margin && pos.y < Screen.height - margin;
	}
	
	
	private static readonly prefBool 
		drawLine      = new prefBool("st_drawLine"),
		drawMesh      = new prefBool("st_drawMesh"),
		drawPoints    = new prefBool("st_drawPoints"),
		drawArrow     = new prefBool("st_drawArrow"),
		showCollision = new prefBool("st_showCollision"),
		drawIndicator = new prefBool("st_drawIndicator"),
		drawFacing    = new prefBool("st_drawFacing");
	
	
	private static readonly prefInt 
		drawBounds = new prefInt("st_drawBounds"),
		drawSnake  = new prefInt("st_drawSnake");
	

	private Vector2 MousePoint
	{
		get
		{
			Ray ray = orthoCam.ScreenPointToRay(Input.mousePosition);
			float dist;
			new Plane(Vector3.forward, Vector3.zero).Raycast(ray, out dist);
			return ray.origin + ray.direction * dist;
		}
	}
}
