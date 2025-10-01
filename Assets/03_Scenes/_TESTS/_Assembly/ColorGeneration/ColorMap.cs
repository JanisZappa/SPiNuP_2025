using UnityEngine;


public class ColorMap : MonoBehaviour
{
	public PaletteSource source;
	public Color color;

	private int editPick, currentEditPick;
	private int editMapping, savedMapping;
	

	private void Start () 
	{
		if (source.mapping.Length == 0)
		{
			source.mapping = new int[source.oldColors.Length];

			for (int i = 0; i < source.oldColors.Length; i++)
			{
				Color oldColor = source.oldColors[i];
				float bestDist = float.MaxValue;
				int pick = 0;

				for (int e = 0; e < source.newColors.Length; e++)
				{
					Color newColor = source.newColors[e];
					float dist = newColor.Dist(oldColor);
					if (dist < bestDist)
					{
						bestDist = dist;
						pick = e;
					}
				}
        
				source.mapping[i] = pick;
			}
		}

		currentEditPick = -1;
	}
	
	
	private void Update ()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow))
			editPick = (editPick + 1).Repeat(source.oldColors.Length);
		
		if (Input.GetKeyDown(KeyCode.DownArrow))
			editPick = (editPick - 1).Repeat(source.oldColors.Length);
		
		
		if (currentEditPick != editPick)
		{
			currentEditPick = editPick;

			editMapping = savedMapping = source.mapping[currentEditPick];
			color = source.newColors[savedMapping];
		}

		
		float bestDist = float.MaxValue;
		editMapping = 0;

		for (int e = 0; e < source.newColors.Length; e++)
		{
			Color newColor = source.newColors[e];
			float dist = newColor.Dist(color);
			if (dist < bestDist)
			{
				bestDist = dist;
				editMapping = e;
			}
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			savedMapping = editMapping;
			source.mapping[editPick] = savedMapping;
		}
		
		DRAW.Rectangle(Vector3.right * -1, Vector2.one * 2).SetColor(source.oldColors[currentEditPick]).Fill(1, true);
		DRAW.Rectangle(Vector3.right *  1, Vector2.one * 2).SetColor(source.newColors[editMapping]).Fill(1, true);
	}
}
