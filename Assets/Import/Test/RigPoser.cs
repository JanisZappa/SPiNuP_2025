using GeoMath;
using LevelElements;
using UnityEngine;
using Pose = Anim.Pose;


public class RigPoser
{
	private static readonly BoolSwitch twistTest = new ("Anim/Twist Test", false);
	protected static readonly BoolSwitch turnIt  = new ("Anim/Turn Test", false);
	
	private float tBend, bBend, tRadius, bRadius, turn, tTurn, bTurn, btTurnDir;
	
	public  Vector3 squashScale;
	private bool    tStraight, bStraight;
	
	
//  Rig Info  //
	public readonly Rig rig;
	private readonly GameObject gameObject;
	public readonly Transform  transform;

	public readonly BendPart[] mainChains = CollectionInit.Array<BendPart>(2);
	

	public  float   height;
	private float   min, max, torsoMin, torsoMax;
	public  Vector3 size;
	private float   handZ, footZ;

	private RigPart torso, armL, armR, legL, legR;
	private RigHead head;
	private const int armC = 0, legC = 1;

	public float handRadius, footRadius;

	public readonly Pose currentPose = new Pose();
	
	
	private FloatForce turnA = new FloatForce(GPhysics.ForceFPS).SetSpeed(40).SetDamp(3f), 
		               turnB = new FloatForce(GPhysics.ForceFPS).SetSpeed(40).SetDamp(4);
	private FloatForce bendA = new FloatForce(GPhysics.ForceFPS).SetSpeed(120).SetDamp(12f), 
		               bendB = new FloatForce(GPhysics.ForceFPS).SetSpeed(120).SetDamp(8);		
	private Vector3Force squashForce = new Vector3Force(GPhysics.ForceFPS).SetSpeed(220).SetDamp(5);	

//  TODO The Size is hardcoded ...  //
	private readonly SpinnerShadow[] shadows;


	public RigPoser(Rig rig, Spinner spinner)
	{
		this.rig  = rig;

		transform  = rig.transform;
		gameObject = transform.gameObject;

		
		Transform spinnerShadow = "SpinnerShadow".ResourceInst(transform.parent).SetName("SpinnerShadow").transform;

		shadows = spinnerShadow.GetComponentsInChildren<SpinnerShadow>();
		for (int s = 0; s < shadows.Length; s++)
			shadows[s].spinner = spinner;
	}
	
	
	public void FinishSetup(Vector3 size, Vector3 hands, Vector3 feet)
	{
		head  = rig.rigHead;
		
		torso = GetPart(PartType.Torso);
		armL  = GetPart(PartType.ArmL);
		armR  = GetPart(PartType.ArmR);
		legL  = GetPart(PartType.LegL);
		legR  = GetPart(PartType.LegR);
		
		
		height = hands.y - feet.y - Item.DefaultRadius * 2;
		   min = height * -.5f;
		   max = height *  .5f;
		
		
		this.size = size.SetY(height);
		
	//  TODO I fucked this  //
		torsoMin = (torso.yMin + legL.yMax) * .5f;
		torsoMax = (torso.yMax + armL.yMin) * .5f;
		
		
		mainChains[armC].CreateBendChain(armL);
		mainChains[legC].CreateBendChain(legL);

		
		handZ = hands.z - mainChains[armC].Tip.rigPos.z;
		footZ =  feet.z - mainChains[legC].Tip.rigPos.z;

		for (int s = 0; s < shadows.Length; s++)
			shadows[s].Setup(this.size);
	}


	private RigPart GetPart(PartType partType)
	{
		for (int i = 0; i < rig.rigParts.Length; i++)
			if (rig.rigParts[i].partType == partType)
				return rig.rigParts[i];
		
		return null;
	}


	private void SetPoseValues(Pose pose, float squash, bool life = false)
	{
		tBend = bBend = pose[Pose.Lean] * .3f + pose[Pose.Bend];
		tTurn = bTurn = pose[Pose.Turn] * 360;
		
		
		if (life)
		{
			float pivot = pose[Pose.Pivot];
		
			float bottomP    = Mathf.Clamp01(1f - pivot * 2);
			float topP = Mathf.Clamp01((pivot -.5f) * 2);
			
			//Debug.Log(pivot.ToString("F3") + " | " + topP + " - " + bottomP);

			if (false)
			{
				tBend = Mathf.Lerp(bendA.Update(tBend, Time.deltaTime), tBend, topP);
				bBend = Mathf.Lerp(bendB.Update(bBend, Time.deltaTime), bBend, bottomP);
			
				tTurn = turnA.Update(tTurn, Time.deltaTime);
				bTurn = turnB.Update(bTurn, Time.deltaTime);
			}
		}
		
		
		if(tBend >= -.0001f && tBend <= .0001f)
			tBend = 0;
		if(bBend >= -.0001f && bBend <= .0001f)
			bBend = 0;
		
		tStraight = f.Same(tBend, 0);
		bStraight = f.Same(bBend, 0);

		turn = (tTurn + bTurn) * .5f;

		btTurnDir = tTurn - bTurn;
		
		Vector3 p = Vector3.zero;
		p.x += 1;
		
		
		bRadius = 1 / bBend / 2 / Mth.π * height;
		tRadius = 1 / tBend / 2 / Mth.π * height;

		squashScale = size.GetFactors(size.VolumeScaleY(squash));

		if (life)
		{
			//squashScale = squashForce.Update(squashScale, Time.deltaTime);
		}
	}

	
	private void TumbleAnim(float tumble, float pivot)
	{
		float bottomP = Mathf.Clamp01(1f - pivot * 2);
		float topP    = Mathf.Clamp01((pivot -.5f) * 2);
		
		float tumbleT = tumble * (1f - topP);
		float tumbleB = tumble * (1f - bottomP);
		
		float tumbleTime = GameManager.Running ? GTime.Now : Time.realtimeSinceStartup;
		
	//  Arms  //	
		armL.poseRot = Quaternion.Euler(16 * tumbleT, 0,  14 * tumbleT) * Quaternion.FromToRotation(V3.up, V3.up.RotZ( 14 * tumbleT).RotY(tumbleTime *  540));
		armR.poseRot = Quaternion.Euler(16 * tumbleT, 0, -14 * tumbleT) * Quaternion.FromToRotation(V3.up, V3.up.RotZ(-14 * tumbleT).RotY(tumbleTime * -540));

	//  Legs  //
		legL.poseRot = Quaternion.Euler((11 + Mathf.Sin(tumbleTime * 8) *  38) * tumbleB, 0, -10 * tumbleB);
		legR.poseRot = Quaternion.Euler((11 + Mathf.Sin(tumbleTime * 8) * -38) * tumbleB, 0,  10 * tumbleB);
	}
	
	
	public Placement GetBendPlacement(Vector3 point, bool getRot = true)
	{
		bool top = point.y > 0;
		
		if(top && tStraight || !top && bStraight)
			return new Placement(point, Rot.Zero);
		
		
		float checkBend   = top? tBend   : bBend;
		float checkRadius = top? tRadius : bRadius;
		
		
		float bodyLerp    = Mathf.InverseLerp(min, max, point.y);
		float radFraction = checkBend * (1 - bodyLerp - .5f) * Mth.π * 2;

		Vector3 circlePos = Circle.GetPos(checkRadius - point.x, Mth.π + radFraction);
		
		return new Placement(new Vector3(checkRadius + circlePos.x, circlePos.y, point.z), 
			                 getRot? Rot.Z(radFraction * Mathf.Rad2Deg) : Quaternion.identity);
	}


	public float GetBendRadius(Vector3 point)	
	{
		bool top = point.y > 0;
		return (top ? tRadius : bRadius) - point.x;
	}


	public Quaternion GetTurnRot(float y)
	{
		float lerp  = Mathf.InverseLerp(torsoMin, torsoMax, y);
		float angle = bTurn + btTurnDir * lerp;
		
		return Rot.Y(angle);
	}


	public Placement GetSwingOffset(Pose pose, float squash, bool hands, float itemRadius)
	{
    //  Set Debug Radiuses	//
		handRadius =  hands? itemRadius : Item.DefaultRadius * .5f;
		footRadius = !hands? itemRadius : Item.DefaultRadius * .5f;
		
		
		SetPoseValues(pose, squash);
		
		
	//  Bend  //
		for (int i = 0; i < mainChains.Length; i++)
			mainChains[i].CalculateBend(this);
	

	//  Calculate Offset  //
		Vector3   grabOffset = (hands? new Vector3(0, itemRadius, handZ) : new Vector3(0, -itemRadius, footZ)).RotY(turn);
		Placement grabPoint  = mainChains[hands? armC : legC].Tip.GetBendPlacement(grabOffset).ZeroZ();
		
		const float limit = .6666667f; 
		return new Placement(-grabPoint.pos, Quaternion.FromToRotation(grabPoint.pos, hands? V3.up : V3.down).Limit(limit));
	}
	
	
	public void SetPose(Placement placement, Pose pose, float squash)
	{
		if (turnIt)
		{
			float baseTurn = GTime.Now * -2.2f;
			const float extra = .03f;

			float extraAngle = Mth.SmoothPP(extra, -extra, GTime.Now * 3f);
			pose.Set(Pose.Turn, pose[Pose.Turn] + baseTurn + extraAngle);
		}

		currentPose.Copy(pose);
		
		SetPoseValues(currentPose, squash, true);
		
		if (true)
		{
			TumbleAnim(pose[Pose.Tumble], pose[Pose.Pivot]);

		//  Turn Head  //
			const float headRange = 18;
			Quaternion headRot = Quaternion.FromToRotation(V3.forward, V3.forward.RotX(-headRange).RotZ(GTime.Now * 250));
			head.poseRot = Rot.X(-headRange) * headRot;
			
		//  TwistTest  //
			if (twistTest)
			{
				Vector2 testTwist = new Vector2(0, Mth.SmoothPP(GTime.Now) * 180);
				
				armL.SetTwist(testTwist);
				armR.SetTwist(testTwist);
				legL.SetTwist(testTwist);
				legR.SetTwist(testTwist);
			}
		}
		
		head.UpdateBones(this);
		
		for (int i = 0; i < rig.rigParts.Length; i++)
			rig.rigParts[i].UpdateBones(this);
		
		transform.SetPlacement(placement);
		
		for (int s = 0; s < shadows.Length; s++)
			shadows[s].ManualUpdate(transform.position.z < 0, turn, squashScale);
	}

	
	public Placement HandRootPos 
	{ 
		get 
		{ 
			Placement p = mainChains[armC].Tip.GetBendPlacement(new Vector3(0, handRadius, handZ).RotY(turn)).ZeroZ();
			return new Placement(transform.TransformPoint(p.pos), transform.rotation * p.rot);	
		}
	} 
	public Placement FootRootPos 
	{
		get
		{
			Placement p = mainChains[legC].Tip.GetBendPlacement(new Vector3(0, -footRadius, footZ).RotY(turn)).ZeroZ();
			return new Placement(transform.TransformPoint(p.pos), transform.rotation * p.rot);
		}
	}


	public void SetActive(bool active)
	{
		if(!active)
			SetPose(Placement.OutOfSight, Pose.Reader.Zero(), 1);
		
		gameObject.SetActive(active);

		for (int i = 0; i < shadows.Length; i++)
			shadows[i].gameObject.SetActive(active);
	}
}