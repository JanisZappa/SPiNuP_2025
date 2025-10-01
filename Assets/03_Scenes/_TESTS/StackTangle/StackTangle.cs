 using System.Collections.Generic;
using GeoMath;
using UnityEngine;

namespace Test
{
	public class StackTangle : MonoBehaviour
	{
		private class FallRect
		{
			private readonly Color color;
			private readonly Bounds2D bounds;

			private const int xMin = -25, xMax = 25;
			public static float maxY;

			public FallRect()
			{
				bool horizontal = rects.Count > 2 && Random.Range(0, 4) == 0;

				if (!horizontal)
				{
					float x1, x2;
					int trys = 0;
					while (true)
					{
						x1 = Random.Range(xMin, xMax - 2);
						x2 = Random.Range(x1, Mathf.Min(xMax, x1 + 10));

						if (Mathf.Abs(x2 - x1) > 2)
							break;

						trys++;
						if (trys == 100)
						{
							if (Application.isEditor)
								Debug.Log("Horizontal didn't work");
							return;
						}
					}

					bounds.minX = x1;
					bounds.maxX = x2;
					bounds.minY = 0;
					bounds.maxY = Random.Range(2, 10f);

					color = Color.Lerp(Color.yellow, Color.green, Random.Range(0, 1f));

					if (rects.Count > 0)
					{
						for (int i = 0; i < rects.Count; i++)
						{
							Bounds2D other = rects[i].bounds;

							if (bounds.XAxisOverlap(other) && other.maxY > bounds.minY)
							{
								float offset = other.maxY - bounds.minY;
								bounds.maxY += offset;
								bounds.minY += offset;

								maxY = Mathf.Max(maxY, bounds.maxY);
							}
						}
					}
				}
				else
				{
					float y1, y2;
					int trys = 0;
					while (true)
					{
						y1 = Random.Range(Mathf.Max(0, maxY - 100), maxY);
						y2 = Random.Range(y1, Mathf.Min(maxY, y1 + 10));

						if (Mathf.Abs(y2 - y1) > 2)
							break;

						trys++;
						if (trys == 100)
						{
							if (Application.isEditor)
								Debug.Log("Horizontal didn't work");
							return;
						}
					}

					bool fromRight = Random.Range(0, 2) == 0;

					if (fromRight)
					{
						bounds.minX = -1000;
						bounds.maxX = bounds.minX + Random.Range(2, 10f);
						bounds.minY = y1;
						bounds.maxY = y2;

						color = Color.Lerp(Color.red, Color.cyan, Random.Range(0, 1f));

						if (rects.Count > 0)
						{
							for (int i = 0; i < rects.Count; i++)
							{
								Bounds2D other = rects[i].bounds;

								if (bounds.YAxisOverlap(other) && other.maxX > bounds.minX)
								{
									float offset = other.maxX - bounds.minX;
									bounds.maxX += offset;
									bounds.minX += offset;
								}
							}
						}
					}
					else
					{
						bounds.maxX = 1000;
						bounds.minX = bounds.maxX - Random.Range(2, 10f);
						;

						bounds.minY = y1;
						bounds.maxY = y2;

						color = Color.Lerp(Color.red, Color.cyan, Random.Range(0, 1f));

						if (rects.Count > 0)
						{
							for (int i = 0; i < rects.Count; i++)
							{
								Bounds2D other = rects[i].bounds;

								if (bounds.YAxisOverlap(other) && other.minX < bounds.maxX)
								{
									float offset = other.minX - bounds.maxX;
									bounds.maxX += offset;
									bounds.minX += offset;
								}
							}
						}
					}
				}



				rects.Add(this);
			}


			private void Draw(float min, float max)
			{
				if (bounds.minY <= max && bounds.maxY >= min)
					bounds.Draw().SetColor(color).Fill();
			}

			private static readonly List<FallRect> rects = new List<FallRect>();


			public static void DrawAll(float min, float max)
			{
				for (int i = 0; i < rects.Count; i++)
					rects[i].Draw(min, max);
			}

			public static void Clear()
			{
				rects.Clear();
				maxY = 0;
			}
		}


		public Transform cam;

		private float camAnim;
		private string buildTime;

		private void OnEnable()
		{
			BuildTower();
		}


		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
				BuildTower();

			camAnim = Mathf.Clamp(camAnim + Input.GetAxis("Vertical") / (1 + FallRect.maxY) * 2, 0, 1);

			float y = Mathf.Lerp(0, FallRect.maxY, camAnim);
			cam.position = new Vector3(0, y, -10);


			//  Floor  //
			const int lineCount = 10;
			for (int i = 0; i < lineCount; i++)
			{
				float horizon = y - Mathf.Lerp(0, 30 + i * i * 30, camAnim);
				DRAW.Vector(new Vector3(-1000, horizon), new Vector3(2000, 0))
					.SetColor(Color.white.A(.15f * (1 - (float)i / lineCount)));
			}

			//  Draw  //
			float camY = cam.position.y;
			FallRect.DrawAll(camY - 40, camY + 40);
		}


		private void BuildTower()
		{
			FallRect.Clear();

			float t = Time.realtimeSinceStartup;
			for (int i = 0; i < 1000; i++)
				new FallRect();

			t = Time.realtimeSinceStartup - t;
			int frames = Mathf.FloorToInt(t * 60);
			buildTime = "BuildTime: " + t.ToString("F4") + "\n" + frames + (frames > 1 ? " Frames" : " Frame") +
			            "\n\n" + "Height: " + FallRect.maxY.ToString("F1");
		}


		private void OnGUI()
		{
			GUI.Label(new Rect(Vector2.zero, new Vector2(Screen.width, Screen.height)), buildTime);
		}
	}
}
