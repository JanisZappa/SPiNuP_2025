using System.IO;


namespace Clips
{
    public class Dead : None 
    {
        public Dead () : base(ClipType.Dead) {}
        
        
        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(startTime);
        }
    
            
        public static Clip DeserializeClip(BinaryReader reader, Spinner spinner, Side side, float timeShift)
        {
            return PoolClip(ClipType.Dead, spinner, reader.ReadSingle() + timeShift, side);
        }


        /*public override bool GetClipBounds(ClipBoundPool pool)
        {
            pool.Get.Set(startTime, pool.max, Type, startSide);
            
            return true;
        }*/
        
        
        protected override Clip Setup(Spinner spinner, float startTime, Side startSide)
        {
            duration = GTime.RewindTime;

            return base.Setup(spinner, startTime, startSide);
        }
    }
}
