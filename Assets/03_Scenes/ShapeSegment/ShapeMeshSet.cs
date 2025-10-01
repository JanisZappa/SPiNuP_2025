using UnityEngine;


[CreateAssetMenu]
public class ShapeMeshSet : ScriptableObject
{
	public bool  fill = true;
	public float fillOffset;
	
	[Space(10)]
	
	public EdgeMesh[] top;
	public EdgeMesh[] side;
	public EdgeMesh[] bottom;
	
	[Space(10)]
	public EdgeMesh[] bridges;

	private EdgeMesh[][] edgeMap;

	private readonly float[] randomNumbers = new float[1000];

	
	private void OnEnable()
	{
		edgeMap = new[] { bottom, top, bottom, side };
		NewSeed();
	}


	public void NewSeed()
	{
		for (int i = 0; i < randomNumbers.Length; i++)
			randomNumbers[i] = Random.Range(0f, 1f);
	}

	public EdgeMesh GetEdge(int sideID, int sideIndex, int step)
	{
		EdgeMesh[] edges = edgeMap[sideID];
		return edges[GetRandomIndex(0, edges.Length, sideIndex * 235 + step * 531)];
	}

	
	public EdgeMesh GetBridge(int sideIndex)
	{
		return bridges[GetRandomIndex(0, bridges.Length, sideIndex * 235)];
	}
	

	private int GetRandomIndex(int min, int max, int value)
	{
		int range = max - min;
		return min + Mathf.FloorToInt(randomNumbers[value % randomNumbers.Length] * range);
	}
}
