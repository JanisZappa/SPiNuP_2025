using LevelElements;


public abstract class ElementMask
{
    public abstract bool Fits(elementType elementType);
}


public static partial class MaskTypes
{
    public class IsItem_Mask : ElementMask
    {
        public override bool Fits(elementType elementType)
        {
            switch(elementType)
            {
                case elementType.ArrowStick     :
                case elementType.Bouncy         :
                case elementType.Branch         :
                case elementType.EndStick       :
                case elementType.InOutStick     :
                case elementType.Probe          :
                case elementType.Probe_Orange   :
                case elementType.StartStick     :
                case elementType.Stick          :
                case elementType.Stick_SmallTip :
                case elementType.WarpStickA     :
                case elementType.WarpStickB     :
                case elementType.WarpStickC     :
                case elementType.WarpStickD     : return true;

                default: return false;
            }
        }
    }
    public class IsCollectable_Mask : ElementMask
    {
        public override bool Fits(elementType elementType)
        {
            switch(elementType)
            {
                case elementType.Coin       :
                case elementType.Pill       :
                case elementType.Pill_Green : return true;

                default: return false;
            }
        }
    }
    public class IsFluff_Mask : ElementMask
    {
        public override bool Fits(elementType elementType)
        {
            switch(elementType)
            {
                case elementType.DebugCube  :
                case elementType.DebugCube2 :
                case elementType.Flower     :
                case elementType.Flower2    :
                case elementType.Flower3    :
                case elementType.Leaf       :
                case elementType.LeafBlue   :
                case elementType.TrackGap   : return true;

                default: return false;
            }
        }
    }
    public class IsTrack_Mask : ElementMask
    {
        public override bool Fits(elementType elementType)
        {
            switch(elementType)
            {
                case elementType.Arc      :
                case elementType.Circular :
                case elementType.PingPong : return true;

                default: return false;
            }
        }
    }
    public class CanBeGrabbed_Mask : ElementMask
    {
        public override bool Fits(elementType elementType)
        {
            switch(elementType)
            {
                case elementType.ArrowStick     :
                case elementType.Branch         :
                case elementType.EndStick       :
                case elementType.StartStick     :
                case elementType.Stick          :
                case elementType.Stick_SmallTip :
                case elementType.WarpStickA     :
                case elementType.WarpStickB     :
                case elementType.WarpStickC     :
                case elementType.WarpStickD     : return true;

                default: return false;
            }
        }
    }
    public class AnyThing_Mask : ElementMask
    {
        public override bool Fits(elementType elementType)
        {
            switch(elementType)
            {
                case elementType.ArrowStick     :
                case elementType.Bouncy         :
                case elementType.Branch         :
                case elementType.Coin           :
                case elementType.DebugCube      :
                case elementType.DebugCube2     :
                case elementType.EndStick       :
                case elementType.Flower         :
                case elementType.Flower2        :
                case elementType.Flower3        :
                case elementType.InOutStick     :
                case elementType.Leaf           :
                case elementType.LeafBlue       :
                case elementType.Pill           :
                case elementType.Pill_Green     :
                case elementType.Probe          :
                case elementType.Probe_Orange   :
                case elementType.StartStick     :
                case elementType.Stick          :
                case elementType.Stick_SmallTip :
                case elementType.TrackGap       :
                case elementType.WarpStickA     :
                case elementType.WarpStickB     :
                case elementType.WarpStickC     :
                case elementType.WarpStickD     : return true;

                default: return false;
            }
        }
    }
    public class MustShake_Mask : ElementMask
    {
        public override bool Fits(elementType elementType)
        {
            switch(elementType)
            {
                case elementType.ArrowStick     :
                case elementType.Bouncy         :
                case elementType.Branch         :
                case elementType.Coin           :
                case elementType.EndStick       :
                case elementType.Flower         :
                case elementType.Flower2        :
                case elementType.Flower3        :
                case elementType.InOutStick     :
                case elementType.Leaf           :
                case elementType.LeafBlue       :
                case elementType.Pill           :
                case elementType.Pill_Green     :
                case elementType.Probe          :
                case elementType.Probe_Orange   :
                case elementType.StartStick     :
                case elementType.Stick          :
                case elementType.Stick_SmallTip :
                case elementType.WarpStickA     :
                case elementType.WarpStickB     :
                case elementType.WarpStickC     :
                case elementType.WarpStickD     : return true;

                default: return false;
            }
        }
    }
    public class ShakeItem_Mask : ElementMask
    {
        public override bool Fits(elementType elementType)
        {
            switch(elementType)
            {
                case elementType.ArrowStick     :
                case elementType.Bouncy         :
                case elementType.Branch         :
                case elementType.EndStick       :
                case elementType.InOutStick     :
                case elementType.Probe          :
                case elementType.Probe_Orange   :
                case elementType.StartStick     :
                case elementType.Stick          :
                case elementType.Stick_SmallTip :
                case elementType.WarpStickA     :
                case elementType.WarpStickB     :
                case elementType.WarpStickC     :
                case elementType.WarpStickD     : return true;

                default: return false;
            }
        }
    }
    public class ShakeFluff_Mask : ElementMask
    {
        public override bool Fits(elementType elementType)
        {
            switch(elementType)
            {
                case elementType.Flower   :
                case elementType.Flower2  :
                case elementType.Flower3  :
                case elementType.Leaf     :
                case elementType.LeafBlue : return true;

                default: return false;
            }
        }
    }
    public class HidingShakeFluff_Mask : ElementMask
    {
        public override bool Fits(elementType elementType)
        {
            switch(elementType)
            {
                case elementType.Flower  :
                case elementType.Flower2 :
                case elementType.Flower3 : return true;

                default: return false;
            }
        }
    }
    public class Action_Mask : ElementMask
    {
        public override bool Fits(elementType elementType)
        {
            switch(elementType)
            {
                case elementType.Bouncy : return true;

                default: return false;
            }
        }
    }
    public class CanMove_Mask : ElementMask
    {
        public override bool Fits(elementType elementType)
        {
            switch(elementType)
            {
                case elementType.ArrowStick     :
                case elementType.Branch         :
                case elementType.Coin           :
                case elementType.DebugCube      :
                case elementType.DebugCube2     :
                case elementType.EndStick       :
                case elementType.Flower         :
                case elementType.Flower2        :
                case elementType.Flower3        :
                case elementType.Leaf           :
                case elementType.LeafBlue       :
                case elementType.Pill           :
                case elementType.Pill_Green     :
                case elementType.Probe          :
                case elementType.Probe_Orange   :
                case elementType.StartStick     :
                case elementType.Stick          :
                case elementType.Stick_SmallTip :
                case elementType.TrackGap       : return true;

                default: return false;
            }
        }
    }
    public class Hide_Mask : ElementMask
    {
        public override bool Fits(elementType elementType)
        {
            switch(elementType)
            {
                case elementType.DebugCube  :
                case elementType.DebugCube2 :
                case elementType.Flower     :
                case elementType.Flower2    :
                case elementType.Flower3    :
                case elementType.TrackGap   : return true;

                default: return false;
            }
        }
    }
    public class CreatorButton_Mask : ElementMask
    {
        public override bool Fits(elementType elementType)
        {
            switch(elementType)
            {
                case elementType.Arc            :
                case elementType.Bouncy         :
                case elementType.Branch         :
                case elementType.Circular       :
                case elementType.Coin           :
                case elementType.DebugCube      :
                case elementType.DebugCube2     :
                case elementType.Flower         :
                case elementType.Flower2        :
                case elementType.Flower3        :
                case elementType.Leaf           :
                case elementType.LeafBlue       :
                case elementType.Pill           :
                case elementType.Pill_Green     :
                case elementType.PingPong       :
                case elementType.Probe          :
                case elementType.Probe_Orange   :
                case elementType.Stick          :
                case elementType.Stick_SmallTip :
                case elementType.WarpStickA     :
                case elementType.WarpStickC     : return true;

                default: return false;
            }
        }
    }
    public class TrackCanGrow_Mask : ElementMask
    {
        public override bool Fits(elementType elementType)
        {
            switch(elementType)
            {
                case elementType.Arc : return true;

                default: return false;
            }
        }
    }
    public class TrackCanTurn_Mask : ElementMask
    {
        public override bool Fits(elementType elementType)
        {
            switch(elementType)
            {
                case elementType.Arc      :
                case elementType.PingPong : return true;

                default: return false;
            }
        }
    }
        public class Warp_Mask : ElementMask
        {
            public override bool Fits(elementType elementType)
            {
                switch(elementType)
                {
                    case elementType.WarpStickA :
                    case elementType.WarpStickB :
                    case elementType.WarpStickC :
                    case elementType.WarpStickD : return true;

                default: return false;
                }
            }
        }
        public class Warp_Start_Mask : ElementMask
        {
            public override bool Fits(elementType elementType)
            {
                switch(elementType)
                {
                    case elementType.WarpStickA :
                    case elementType.WarpStickC : return true;

                default: return false;
                }
            }
        }
        public class Warp_SideSwitch_Mask : ElementMask
        {
            public override bool Fits(elementType elementType)
            {
                switch(elementType)
                {
                    case elementType.WarpStickA :
                    case elementType.WarpStickB : return true;

                default: return false;
                }
            }
        }
}


public static partial class Mask
{
    public static readonly MaskTypes.IsItem_Mask           IsItem           = new MaskTypes.IsItem_Mask();
    public static readonly MaskTypes.IsCollectable_Mask    IsCollectable    = new MaskTypes.IsCollectable_Mask();
    public static readonly MaskTypes.IsFluff_Mask          IsFluff          = new MaskTypes.IsFluff_Mask();
    public static readonly MaskTypes.IsTrack_Mask          IsTrack          = new MaskTypes.IsTrack_Mask();
    public static readonly MaskTypes.CanBeGrabbed_Mask     CanBeGrabbed     = new MaskTypes.CanBeGrabbed_Mask();
    public static readonly MaskTypes.AnyThing_Mask         AnyThing         = new MaskTypes.AnyThing_Mask();
    public static readonly MaskTypes.MustShake_Mask        MustShake        = new MaskTypes.MustShake_Mask();
    public static readonly MaskTypes.ShakeItem_Mask        ShakeItem        = new MaskTypes.ShakeItem_Mask();
    public static readonly MaskTypes.ShakeFluff_Mask       ShakeFluff       = new MaskTypes.ShakeFluff_Mask();
    public static readonly MaskTypes.HidingShakeFluff_Mask HidingShakeFluff = new MaskTypes.HidingShakeFluff_Mask();
    public static readonly MaskTypes.Action_Mask           Action           = new MaskTypes.Action_Mask();
    public static readonly MaskTypes.CanMove_Mask          CanMove          = new MaskTypes.CanMove_Mask();
    public static readonly MaskTypes.Hide_Mask             Hide             = new MaskTypes.Hide_Mask();
    public static readonly MaskTypes.CreatorButton_Mask    CreatorButton    = new MaskTypes.CreatorButton_Mask();
    public static readonly MaskTypes.TrackCanGrow_Mask     TrackCanGrow     = new MaskTypes.TrackCanGrow_Mask();
    public static readonly MaskTypes.TrackCanTurn_Mask     TrackCanTurn     = new MaskTypes.TrackCanTurn_Mask();
    public static readonly MaskTypes.Warp_Mask             Warp             = new MaskTypes.Warp_Mask();
    public static readonly MaskTypes.Warp_Start_Mask       Warp_Start       = new MaskTypes.Warp_Start_Mask();
    public static readonly MaskTypes.Warp_SideSwitch_Mask  Warp_SideSwitch  = new MaskTypes.Warp_SideSwitch_Mask();
}
