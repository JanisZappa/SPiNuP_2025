using Anim;
using GeoMath;
using UnityEngine;


public class BendPart
{
	private readonly Bone   rootBone = new Bone();
	          public Bone   Tip;
	public  readonly Bone[] bones = CollectionInit.Array<Bone>(30);
	          
	
	public  int   boneCount;
	private bool  flippedY, noRootBend;
	private float meshHeight, rootChainLength, stepMulti, bendyness;
	

	public void CreateBendChain(RigPart limb)
	{
		boneCount       = limb.boneCount;
		flippedY        = limb.flippedY;
		noRootBend      = limb.noRootBend;
		bendyness       = limb.bendyness;
		rootChainLength = limb.rootChainLength;
		meshHeight      = limb.height;
		stepMulti       = limb.stepMulti;
		
		
		rootBone.SetupBone(limb.rootBone.rigPos.SetX(0));

		for (int i = 0; i < boneCount; i++)
			bones[i].SetupBone(limb.bones[i].rigPos.SetX(0));
		
		
		Tip = bones[boneCount - 1];
	}
	
	
	public void CalculateBend(RigPoser poser)
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
		
		
	//  Bend Chain  //
		if (noRootBend)
		{
			for (int i = boneCount - 2; i < boneCount; i++)
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
			float chainLength = meshHeight * poser.squashScale.y;
			float stepLength  = chainLength * stepMulti;
			float bendLength  = Mathf.Abs(Circle.GetSegmentRad(stepLength, chainRadius) * chainRadius * (boneCount - 1) * 2);
		
			chainLength = bendLength.NaNChk(chainLength);
		
			float   angle     = chainLength / (2 * Mathf.PI * chainRadius) * 360 * flippedY.SignFloat(1) * bendyness;
			Vector3 parentPos = rootBone.bendPos, localParentPos = rootBone.turnPos;
			for (int i = 0; i < boneCount; i++)
			{
				bones[i].bendPos = parentPos + rootBone.bendRot * (bones[i].turnPos - localParentPos).RotZ(angle * stepMulti * i);
				
				parentPos      = bones[i].bendPos;
				localParentPos = bones[i].turnPos;
			}
		}
		
		
	//  Align Tip Bone  //
		{
			Vector3 posA = bones[boneCount - 2].bendPos;
			Vector3 posB = bones[boneCount - 1].bendPos;
			Vector3 dir  = flippedY? posA - posB : posB - posA;
			bones[boneCount - 1].bendRot = Quaternion.FromToRotation(V3.up, dir);
		}
	}
}
