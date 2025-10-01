using System.Collections.Generic;
using Anim;
using GeoMath;
using UnityEngine;


public class RigPart
{
	public readonly Bone rootBone = new Bone();
	public readonly TBone[] bones = CollectionInit.Array<TBone>(30);
	public int boneCount;

	public float rootChainLength, stepMulti, bendyness, height, yMin, yMax;
	public bool  flippedY, noRootBend;

	public PartType partType;
	
	public Quaternion poseRot = Quaternion.identity;


	public virtual void Setup(RigInfo rigInfo, Rig rig)
	{
		partType = rigInfo.partType;
		
		Mesh   mesh   = rigInfo.mesh;
		Bounds bounds = mesh.bounds;
		
		height = bounds.size.y;
		yMin   = bounds.min.y;
		yMax   = bounds.max.y;
		
		bendyness  = rigInfo.bendyness;
		
		flippedY   = partType == PartType.LegL || partType == PartType.LegR;
		noRootBend = partType == PartType.Torso;
		
		
	//  Currently Pivot on Torso or Torso Transform Pos  //
		rootBone.SetupBone(rigInfo.pos);
		
		boneCount = rigInfo.boneCount;
		stepMulti = boneCount > 1? 1f / (boneCount - 1) : 0;
		
	//  Create Bone Chain  //
		Vector3 meshCenter = rigInfo.pos + (rigInfo.pos.x > 0? bounds.center.FlipX() : bounds.center);
		Vector3 top        = new Vector3(meshCenter.x, meshCenter.y + height * .5f, meshCenter.z);
		Vector3 bottom     = new Vector3(meshCenter.x, meshCenter.y - height * .5f, meshCenter.z);
		

		int boneIndex = rig.boneCount;
		rig.boneCount += boneCount;
		
		for (int i = 0; i < boneCount; i++)
		{
			float lerp = i * stepMulti; 
			Vector3 pos = Vector3.Lerp(bottom, top, flippedY? 1 - lerp : lerp);

			int bindIndex = boneIndex + i;
			rig.bindPos[bindIndex] = pos;
			bones[i].SetupBone(rig.bones[bindIndex], pos, lerp);

			if (Application.isEditor)
				rig.bones[boneIndex + i].name = rigInfo.partType + "_" + i.ToString("D2");
		}

		rootChainLength = Mathf.Abs(bones[boneCount - 1].rigPos.y - rootBone.rigPos.y);
		
		
	//  Bake Skinning Gradient into Mesh  //
		mesh.GetVertices(readVertices);
		int verticeCount = readVertices.Count;
		
		mesh.GetColors(readColors);
		readUVs.Clear();
		for (int i = 0; i < verticeCount; i++)
			readUVs.Add(new Vector2(readColors[i].r, readColors[i].a));
		
	
		float step = height * stepMulti;
		for (int i = 0; i < verticeCount; i++)
		{
			byte readPartID = readColors[i].r;

			float verticeY = flippedY? yMax - (readVertices[i].y - yMin) : readVertices[i].y;
			float lerp     = Mathf.InverseLerp(yMin, yMax, verticeY) * (boneCount - 1);
			
			int minIndex = Mathf.FloorToInt(lerp);
			int maxIndex = Mathf.CeilToInt(lerp);
		
			if (minIndex == maxIndex)
			{
				readColors[i] = new Color32(readPartID, (byte)(minIndex + boneIndex), 0, 255);
				continue;
			}
		
			float boneAHeight = yMin + minIndex * step; 
			float boneBHeight = yMin + maxIndex * step;
			float boneLerp    = Mathf.InverseLerp(boneAHeight, boneBHeight, verticeY);
			
			bool firstIsCloser = boneLerp < .5f;
			readColors[i] = new Color32(readPartID, 
				                        (byte)((firstIsCloser? minIndex : maxIndex) + boneIndex), //  Main
				                        (byte)((firstIsCloser? maxIndex : minIndex) + boneIndex), //  Secondary
				                        (byte)(255 * (firstIsCloser? 1 - boneLerp : boneLerp)));  //  Lerp
		}

		mesh.SetColors(readColors);
		mesh.SetUVs(0, readUVs);
	}
	
	
	public virtual void SetTwist(Vector2 poseTwist)
	{
		float angle = poseTwist.y - poseTwist.x;
		const float antiScale = 1f / 360 * -.7f;
		float twist = Mathf.Abs(angle) * antiScale;
		float multi = 1f / (boneCount - 1f);
		
		for (int i = 0; i < boneCount; i++)
		{
			float lerp      = -1 + bones[i].chainLerp * 2;
			float scaleLerp = Mathf.Pow(Mathf.Cos(Mathf.PI * lerp * .5f), 2.5f).NaNChk();

			float twistScale = twist * scaleLerp;
			bones[i].twistScale = new Vector3(twistScale, 0, twistScale);
			bones[i].twistRot   = Rot.Y(poseTwist.x + angle * i * multi);
		}
	}
	

	public virtual void UpdateBones(RigPoser poser)
	{
	//  Set Turn and Squash  //
		rootBone.squashPos = rootBone.rigPos.MultiBy(poser.squashScale);
		rootBone.turnRot   = poser.GetTurnRot(rootBone.squashPos.y);
		rootBone.turnPos   = rootBone.turnRot * rootBone.squashPos;

		for (int i = 0; i < boneCount; i++)
		{
			bones[i].squashPos = bones[i].rigPos.MultiBy(poser.squashScale);
			bones[i].turnRot   = poser.GetTurnRot(bones[i].squashPos.y);
			bones[i].turnPos   = bones[i].turnRot * bones[i].squashPos;
		}

		Quaternion animPoseRot = Quaternion.FromToRotation(V3.up, rootBone.turnRot * poseRot * V3.up);
		
		
	//  Bend Chain  //
		if (noRootBend)
		{
			for (int i = 0; i < boneCount; i++)
				bones[i].bendPos = poser.GetBendPlacement(bones[i].turnPos, false).pos;
		}
		else
		{
			float rootRadius  = poser.GetBendRadius(rootBone.turnPos);
			float noBendAngle = Circle.GetSegmentRad(rootChainLength * poser.squashScale.y, rootRadius) * Mathf.Rad2Deg * flippedY.SignFloat(1);


			Placement rootPlacement = poser.GetBendPlacement(rootBone.turnPos);
			rootBone.bendPos = rootPlacement.pos;
			rootBone.bendRot = Rot.Z(noBendAngle * (1 - bendyness)) * rootPlacement.rot;
			
			float chainRadius = poser.GetBendRadius(bones[0].turnPos);
			float chainLength = height * poser.squashScale.y;
			float stepLength  = chainLength * stepMulti;
			float bendLength  = Mathf.Abs(Circle.GetSegmentRad(stepLength, chainRadius) * chainRadius * (boneCount - 1) * 2);
				
			chainLength = bendLength.NaNChk(chainLength);
		
			
			float   angle     = chainLength / (2 * Mathf.PI * chainRadius) * 360 * flippedY.SignFloat(1) * bendyness;
			Vector3 parentPos = rootBone.bendPos, localParentPos = rootBone.turnPos;
			for (int i = 0; i < boneCount; i++)
			{
				Quaternion boneRot = Rot.Z(angle * bones[i].chainLerp);
				bones[i].bendPos = parentPos + animPoseRot * (rootBone.bendRot * boneRot) * (bones[i].turnPos - localParentPos);
				
				parentPos      = bones[i].bendPos;
				localParentPos = bones[i].turnPos;
			}
		}
		
		
	//  Align Chain Bones  //
		if (boneCount > 1)
			for (int i = 0; i < boneCount; i++)
			{
				Vector3 dirA = i > 0 ? bones[i].bendPos - bones[i - 1].bendPos : bones[i + 1].bendPos - bones[i].bendPos;
				Vector3 dirB = i < boneCount - 1 ? bones[i + 1].bendPos - bones[i].bendPos : bones[i].bendPos - bones[i - 1].bendPos;
	
				if (flippedY)
				{
					dirA *= -1;
					dirB *= -1;
				}
	
				bones[i].bendRot = Quaternion.FromToRotation(V3.up, Vector3.Lerp(dirA, dirB, .5f));
			}
		else
			bones[0].bendRot = rootBone.bendRot;

		
	//  Apply to Transforms  //
		for (int i = 0; i < boneCount; i++)
		{
			bones[i].SetXZScale(poser.squashScale);
			bones[i].UpdateTransform();
		}
	}
	
	
	private static readonly List<Vector3> readVertices = new List<Vector3>(2000);
	private static readonly List<Color32> readColors   = new List<Color32>(2000);
	private static readonly List<Vector2> readUVs      = new List<Vector2>(2000);
}




