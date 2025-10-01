using System.Collections.Generic;
using GeoMath;
using UnityEngine;


public class RigHead
{
	private static readonly BoolSwitch goCrazy = new("Char/Face Test", false);
	
	public readonly Bone rootBone = new();
	public readonly TBone[] bones = CollectionInit.Array<TBone>(30);
	
	public Quaternion poseRot = Quaternion.identity;
	
	public float height, yMin, yMax;
	
	public int boneCount;
	
	
	public void Setup(RigInfo[] headInfos, Rig rig)
	{
		boneCount = headInfos.Length;
		
		for (int i = 0; i < boneCount; i++)
		{
			RigInfo rigInfo = headInfos[i];
			
			Mesh mesh  = rigInfo.mesh;
			Bounds bounds = mesh.bounds;

			if (i == 0)
			{
				height = bounds.size.y;
				yMin   = bounds.min.y;
				yMax   = bounds.max.y;
				
				rootBone.SetupBone(rigInfo.pos);
			}
		
			int boneIndex = rig.boneCount;
			rig.boneCount++;
		
			rig.bindPos[boneIndex] = rigInfo.pos;
			bones[i].SetupBone(rig.bones[boneIndex], rigInfo.pos);

			if (Application.isEditor)
				rig.bones[boneIndex].name = rigInfo.partType.ToString();
		
		
			//  Bake Skinning Gradient into Mesh  //
			mesh.GetVertices(readVertices);
			int verticeCount = readVertices.Count;
		
			mesh.GetColors(readColors);
			readUVs.Clear();
			for (int e = 0; e < verticeCount; e++)
				readUVs.Add(new Vector2(readColors[e].r, readColors[e].a));
		
	
			for (int e = 0; e < verticeCount; e++)
			{
				byte readPartID = readColors[e].r;
				readColors[e] = new Color32(readPartID, (byte)(boneIndex), 0, 255);
			}

			mesh.SetColors(readColors);
			mesh.SetUVs(0, readUVs);
		}
	}
	

	public void UpdateBones(RigPoser poser)
	{
	//  Set Turn and Squash  //
		rootBone.squashPos = rootBone.rigPos.MultiBy(poser.squashScale);
		rootBone.turnRot   = poser.GetTurnRot(rootBone.squashPos.y);
		rootBone.turnPos   = rootBone.turnRot * rootBone.squashPos;
		
		float rootRadius  = poser.GetBendRadius(rootBone.turnPos);
		float noBendAngle = Circle.GetSegmentRad(height * poser.squashScale.y, rootRadius) * Mathf.Rad2Deg;
		
		Placement rootPlacement = poser.GetBendPlacement(rootBone.turnPos);
		rootBone.bendPos = rootPlacement.pos;
		rootBone.bendRot = Rot.Z(noBendAngle) * rootPlacement.rot * rootBone.turnRot;

		for (int i = 0; i < boneCount; i++)
		{
			TBone b = bones[i];
			b.squashPos = b.rigPos.MultiBy(poser.squashScale);
			b.turnRot = rootBone.turnRot;

			bool crazyEye = goCrazy && i is 1 or 2;
			
			b.bendRot = !crazyEye? rootBone.bendRot : rootBone.bendRot * Rot.Z(GTime.Now * 600 * (i == 1? 1 : -1));
			b.bendPos = rootBone.bendPos + rootBone.bendRot * (b.squashPos - rootBone.squashPos);

			float blink = !(i is 1 or 2) || Mathf.PerlinNoise(GTime.Now * 3, .24f) > .35f? 1 : 0;
			b.updateScale = !crazyEye? V3.one * blink : new Vector3(3, 1.5f, 1);
		
			b.SimpleUpdateTransform();
		}
	}
	
	
	private static readonly List<Vector3> readVertices = new(2000);
	private static readonly List<Color32> readColors   = new(2000);
	private static readonly List<Vector2> readUVs      = new(2000);
}
