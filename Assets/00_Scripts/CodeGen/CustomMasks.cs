using LevelElements;


public static partial class MaskTypes
{
    public class Debug_Mask : ElementMask
    {
        public override bool Fits(elementType elementType)
        {
            switch(elementType)
            {
                case elementType.Bouncy         :
                case elementType.Branch         :
                //case elementType.Coin           :
                //case elementType.DebugCube      :
                //case elementType.DebugCube2     :
                case elementType.Flower         :
                case elementType.Flower2        :
                case elementType.Flower3        :
                case elementType.InOutStick     :
                //case elementType.Leaf           :
                //case elementType.LeafBlue       :
                //case elementType.Pill           :
                //case elementType.Pill_Green     :
                //case elementType.Probe          :
                //case elementType.Probe_Orange   :
                case elementType.Stick          :
                case elementType.Stick_SmallTip :
                case elementType.StartStick:
                case elementType.EndStick:
                case elementType.ArrowStick:
                //case elementType.TrackGap       :
                case elementType.WarpStickA     :
                case elementType.WarpStickB     :
                case elementType.WarpStickC     :
                case elementType.WarpStickD     : return true;

                default: return false;
            }
        }
    }
}


public static partial class Mask
{
    public static readonly MaskTypes.Debug_Mask  Debug = new MaskTypes.Debug_Mask();
}