using LevelElements;
using ShapeStuff;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Test
{
public class TrackMesh : MonoBehaviour
{
	public int trackToSkin;
	private Track track;
	
	private readonly Shape shape = new Shape();

	[Space(10)] 
	public Outline outline;
	public Outline shadowOutline;
	public Mesh tip;
	
	[Range(0, 100)] public float tesselation;
	public byte matCap;
	

	private GameObject shadow;

	private Mesh mesh, shadowMesh;
	

	private void Awake()
	{
		mesh = new Mesh();
		shadowMesh = new Mesh();
		
		GetComponent<MeshFilter>().mesh = mesh;

		shadow = transform.GetChild(0).gameObject;
		shadow.GetComponent<MeshFilter>().mesh = shadowMesh;

		const float m = 1.2f;
		shadowOutline.vertices[0] = new Vector3(outline.width *  .5f + m, shadowOutline.vertices[0].y, shadowOutline.vertices[0].z);
		shadowOutline.vertices[1] = new Vector3(outline.width *  .5f    , shadowOutline.vertices[1].y, shadowOutline.vertices[1].z);
		shadowOutline.vertices[2] = new Vector3(outline.width * -.5f    , shadowOutline.vertices[2].y, shadowOutline.vertices[2].z);
		shadowOutline.vertices[3] = new Vector3(outline.width * -.5f - m, shadowOutline.vertices[3].y, shadowOutline.vertices[3].z);
	}
	
	
	public void Skin(Track track)
	{
		this.track = track;

		track.TellShapeWhatToDo(shape);


		byte shadowMask = ((byte) 0).Set(true, false, true);
		Color32 c = new Color32(GetHeightColor(track), matCap, shadowMask, 0);
		
		outline.GenerateShapeMesh(shape, mesh, tesselation, c,  Level.WallDepth * (track.side.front? -1 : 1), tip);
		
		shadowOutline.GenerateShadowMesh(shape, shadowMesh, tesselation / 2);
		
		transform.localScale = new Vector3(1, 1, 1);

		shadow.gameObject.layer = track.side.front ? Layers.ShadowA : Layers.ShadowB;
	}


	private void Update()
	{
		if(GameManager.Running && track == null && trackToSkin != 0)
			Skin(Track.Get(trackToSkin));
	}
	
	
	private static byte GetHeightColor(Track track)
	{
		return LevelColors[(int) Mathf.Repeat(track.ID * .5f, LevelColors.Length)];
	}
	
	
	private static readonly byte[] LevelColors  = { 19, 20, 21, 23, 24, 25 };
}


#if UNITY_EDITOR
[CustomEditor(typeof(TrackMesh))]
public class TrackMeshEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		
		GUILayout.Space(10);
		if (Application.isPlaying && GUILayout.Button("Skin Track"))
		{
			TrackMesh tM = target as TrackMesh;
			
			Track track = Track.Get(tM.trackToSkin);
			
			if(track != null)
				tM.Skin(track);
		}		
	}
}
#endif
}