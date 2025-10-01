using System.IO;


namespace Clips
{
	public class Spawn : Swing 
	{
		public Spawn () : base(ClipType.Spawn) {}

		protected override Clip Setup(Spinner spinner, float startTime, Side startSide)
		{
			duration = Prep.duration;
			base.Setup(spinner, startTime, startSide);
			return this;
		}

		#region Serialization
	
		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(startTime);
			writer.Write(duration);
		}
	
			
		public static Clip DeserializeClip(BinaryReader reader, Spinner spinner, Side side, float timeShift)
		{
			Prep.Jump  = null;
			Prep.startTime = reader.ReadSingle() + timeShift;
			Prep.duration  = reader.ReadSingle();
			
			return PoolClip(ClipType.Spawn, spinner, Prep.startTime, side);
		}
		
		
		/*public override bool GetClipBounds(ClipBoundPool pool)
		{
			float runTime = duration > 0 ? duration : ClipBounds.Forever;

			pool.Get.Set(startTime, startTime + runTime, Type, startSide).
				bounds.Set(Level.StartStick.bounds).Pad(spinner.size.y);

			return true;
		}*/
		
		
		#endregion
	}




	public abstract partial class Clip
	{
		public static Clip Get_Clip_Spawn(Spinner spinner, float startTime, Side side)
		{
			Prep.Jump  = null;
			Prep.duration = 0;
			return PoolClip(ClipType.Spawn, spinner, startTime, side);
		}
	}
}
