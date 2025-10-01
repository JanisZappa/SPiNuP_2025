using System.Collections.Generic;
using UnityEngine;


//https://nothings.org/gamedev/herringbone/herringbone_tiles.html
namespace Test
{
	public class HeringboneWang : MonoBehaviour
	{
		private const int width = 10;

		public class Brick
		{
			public Vector2Int pos;
			public readonly bool horizontal;
			public float lerp;

			public Brick(Vector2Int pos, bool horizontal)
			{
				this.pos = pos;
				this.horizontal = horizontal;
			}

			public void Draw(Brick hoverBrick)
			{
				bool thisIsIt = this == hoverBrick;
				lerp = Mathf.Lerp(lerp, thisIsIt ? 1 : 0, Time.deltaTime * (thisIsIt ? 16 : 6));
				float size = 1 - lerp;
				Vector2 margin = new Vector2(.45f, .45f) * size;
				float fill = .5f + lerp * .5f;

				if (horizontal)
					DRAW.Rectangle(pos + new Vector2(.5f, 0), new Vector2(2, 1) - margin)
						.SetColor(pos.y % 2 == 0 ? COLOR.red.tomato : COLOR.yellow.fresh).Fill(fill, true);
				else
					DRAW.Rectangle(pos + new Vector2(0, .5f), new Vector2(1, 2) - margin)
						.SetColor(pos.y % 2 == 0 ? COLOR.turquois.aquamarine : COLOR.green.spring).Fill(fill, true);
			}
		}


		private List<Brick> bricks;

		private Brick[] brickGrid;


		private void Awake()
		{
			bricks = new List<Brick>();

			brickGrid = new Brick[width * width];

			{
				Brick brick = new Brick(new Vector2Int(0, 0), false);
				bricks.Add(brick);

				brickGrid[0] = brick;
				brickGrid[width] = brick;
			}

			for (int y = 0; y < width; y++)
			for (int x = 0; x < width; x++)
			{
				int index = y * width + x;
				Brick brick = brickGrid[index];
				if (brick != null)
					continue;

				Brick leftBrick = x > 0 ? brickGrid[index - 1] : null;
				if (leftBrick != null)
				{
					if (leftBrick.horizontal)
					{
						brick = new Brick(new Vector2Int(x, y), false);
						if (y < width - 1)
							brickGrid[index + width] = brick;
					}
					else if (leftBrick.pos.y == y)
					{
						brick = new Brick(new Vector2Int(x, y - 1), false);
						if (y > 0)
							brickGrid[index - width] = brick;
					}
					else
					{
						brick = new Brick(new Vector2Int(x, y), true);
						if (x < width - 1)
							brickGrid[index + 1] = brick;
					}

					bricks.Add(brick);
					brickGrid[index] = brick;

					continue;
				}


				Brick downBrick = y > 0 ? brickGrid[index - width] : null;
				if (downBrick != null)
				{
					if (!downBrick.horizontal)
					{
						brick = new Brick(new Vector2Int(x, y), true);
						if (x < width - 1)
							brickGrid[index + 1] = brick;
					}
					else if (downBrick.pos.x == x)
					{
						brick = new Brick(new Vector2Int(x - 1, y), true);
						if (x > 0)
							brickGrid[index - 1] = brick;
					}
					else
					{
						brick = new Brick(new Vector2Int(x, y), false);
						if (y < width - 1)
							brickGrid[index + width] = brick;
					}

					bricks.Add(brick);
					brickGrid[index] = brick;
				}
			}
		}


		private void Update()
		{
			Vector2 hoverPos = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
			Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(hoverPos.x), Mathf.RoundToInt(hoverPos.y));


			Brick hoverBrick = null;
			if (gridPos.x >= 0 && gridPos.x < width &&
			    gridPos.y >= 0 && gridPos.y < width)
				hoverBrick = brickGrid[gridPos.y * width + gridPos.x];


			for (int i = 0; i < bricks.Count; i++)
				bricks[i].Draw(hoverBrick);
		}
	}
}