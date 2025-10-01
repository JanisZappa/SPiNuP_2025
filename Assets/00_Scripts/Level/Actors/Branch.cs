using UnityEngine;


public class Branch : Stick
{
	public  Transform[]       subPivots;
	private QuaternionForce[] forces;
	private Quaternion[]      bRot;
	

	public override void Setup()
	{
		base.Setup();
		
		int fCount = subPivots.Length - 1;
		
		forces = new QuaternionForce[fCount];
		for (int i = 0; i < fCount; i++)
			forces[i] = new QuaternionForce(GPhysics.ForceFPS).SetDamp(6 + i * 4).SetSpeed(440);

		
		bRot = new Quaternion[fCount];
		for (int i = 0; i < fCount; i++)
		{
			Vector3 r = Quaternion.AngleAxis(Random.Range(-50f, 50f), Vector3.forward) * Vector3.right;
			bRot[i] = Quaternion.AngleAxis((i + 1) * -20 + i * -20, r);
		}	
	}
	
	
	public override void SetTransform(bool forcedUpdate)
	{
		if(!HasToUpdate() && !forcedUpdate && false)
		{
			ShadowUpdate(false);
			return;
		}

		if (false)
		{
			Quaternion baseRot = _transform.rotation;
		
			for (int i = 0; i < subPivots.Length; i++)
			{
				float time = GTime.Now - .075f * i;

				Vector2 pos     = item.GetPos(time);
				Vector2 leanPos = item.GetLagPos(time);
				Vector2 newLean = leanPos - pos + (anim?.GetLean(GTime.Now) ?? V2.zero);

				if (i == 0)
				{
					SetDepthPos(pos);
					subPivots[i].rotation = baseRot *  newLean.LeanRotLocal(item);
					continue;
				}
			
				const bool dip = true;
				Vector2 dipDown = dip? new Vector2(0, i * i * -.15f) : V2.zero;
				Quaternion leanRot = (newLean * (1 + i * 2) + dipDown).LeanRotLocal(item);
        
				subPivots[i].rotation = baseRot * leanRot;
			}
		}
		else
		{
			Quaternion baseRot = _transform.rotation;
			
			Vector2 pos     = item.GetPos(GTime.Now);
			Vector2 leanPos = item.GetLagPos(GTime.Now);
			Vector2 newLean = leanPos - pos + (anim?.GetLean(GTime.Now) ?? V2.zero);
			
			SetDepthPos(pos);
			Quaternion rootRot = baseRot * newLean.LeanRotLocal(item);
			subPivots[0].rotation = rootRot;
			
			Quaternion addRot = rootRot;
			if (forcedUpdate)
			{
				int fCount = forces.Length;
				for (int i = 0; i < fCount; i++)
				{
					addRot = addRot * bRot[i];
					forces[i].SetValue(addRot).SetForce(Quaternion.identity);
					
				}
					
			}
			
			addRot = rootRot;
			for (int i = 1; i < subPivots.Length; i++)
			{
				addRot = addRot * bRot[i - 1];
				Quaternion rot = forces[i - 1].Update(addRot, Time.deltaTime);
				subPivots[i].rotation = rot;
				addRot = rot;
			}
		}
		
		
		ShadowUpdate(true);
	}
	
	
	public static Quaternion LeanRotLocal(Vector2 lean, float offset, float sign)
	{
		return Quaternion.FromToRotation(new Vector3(0, 0, offset), 
			new Vector3(lean.x * sign, -lean.y, offset));
	}
}
