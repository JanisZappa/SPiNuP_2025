namespace Clips
{
    public enum ClipType
    {
        Jump      = 1,
        Swing     = 2,
        Bump      = 3,
        AirLaunch = 4,
        Dead      = 5,
        Spawn     = 6,
        SlipOff   = 7
    }


    public static class ClipTypeFilter
    {
        public static bool IsNotPlaying(this ClipType clipType)
        {
            switch(clipType)
            {
                case 0:
                case ClipType.Dead: return true;
            }

            return false;
        }


        public static bool IsSerializable(this ClipType clipType)
        {
            switch(clipType)
            {
                case ClipType.Jump:
                case ClipType.AirLaunch:
                case ClipType.Dead:
                case ClipType.Spawn:     return true;
            }

            return false;
        }


        public static bool IsAnyJump(this ClipType clipType)
        {
            switch(clipType)
            {
                case ClipType.Jump:
                case ClipType.AirLaunch:
                case ClipType.SlipOff:   return true;
            }

            return false;
        }


        public static bool IsAnySwing(this ClipType clipType)
        {
            switch(clipType)
            {
                case ClipType.Swing:
                case ClipType.Spawn: return true;
            }

            return false;
        }
    }
}
