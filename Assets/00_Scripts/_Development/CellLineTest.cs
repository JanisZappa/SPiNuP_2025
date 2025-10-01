using UnityEngine;


public class CellLineTest : MonoBehaviour
{
	public Transform p1, p2;
	
	private void LateUpdate ()
	{
		if (!GameManager.Running)
			return;

		Vector3 pos1 = p1.position, 
			    pos2 = p2.position;
		
		DRAW.Vector(pos1, pos2 - pos1).SetColor(Color.white).SetDepth(Z.P);

		int count;
		Vector2Int[] cells = SuperCover.GetLineCells(pos1 / Level.CellSize, pos2 / Level.CellSize, out count);

		for (int i = 0; i < count; i++)
			DRAW.Rectangle(cells[i] * Level.CellSize + Vector2.one * Level.CellHalfSize, Vector2.one * Level.CellHalfSize * 1.8f).
				SetColor(Color.white).SetDepth(Z.P);
	}
}
