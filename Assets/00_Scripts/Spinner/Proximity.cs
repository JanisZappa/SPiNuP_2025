using LevelElements;
using UnityEngine;


namespace Future
{
	public class ProxItem
	{
		public  Item    item;
		public  float   startTime, endTime, closestTime, closestDist;
		public  Vector2 closestDir, closestMotion;
		
		public bool upMotion;

		public int Importance => (int)Mathf.Clamp((endTime - startTime) * 4, 0, 3);

		private const float Step = JumpInfo.StepLength;

		public bool IsTheSame(float time, bool motionUp)
		{
			return Mathf.Approximately(time, endTime) && upMotion == motionUp;
		}


		public void New(Item item, float time, Vector2 dir, Vector2 motion)
		{
			this.item = item;
			
			startTime     = closestTime = time;
			endTime       = time + Step;
			closestDir    = dir;
		    closestMotion = motion;
			closestDist   = dir.sqrMagnitude;

			upMotion = motion.y > 0;
		}


		public void Update(float time, Vector2 dir, Vector2 motion)
		{
			endTime = time + Step;
			
			float dist = dir.sqrMagnitude;
			
			if (closestDist > dist)
			{
				closestDist   = dist;
				closestTime   = time;
				closestDir    = dir;
				closestMotion = motion;
			}
		}
		
		
		public bool DebugIsGoingOn(float time)
		{
			return startTime <= time && endTime > time;
		}
	}
}