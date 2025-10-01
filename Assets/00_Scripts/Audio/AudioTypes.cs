public static class Audio 
{
    public static class Music
    {
        public static readonly AudioID 
            Menu      = new AudioID("", 1), 
            Pause     = new AudioID("", 1), 
            Morricone = new AudioID("m_morricone", 1), 
            Moriat    = new AudioID("m_moriat", 1), 
            Katamari  = new AudioID("m_katamari", 1), 
            Plaid     = new AudioID("m_plaid", 1);
    }


    public static class Ambient
    {
        public static readonly AudioID 
            Pause    = new AudioID("", 1), 
            CityLoop = new AudioID("a_cityloop", 1), 
            Birds    = new AudioID("a_birds", 1);
    }


    public static class UI
    {
        public static readonly AudioID 
            ScoreUp   = new AudioID("u_scoreUp", .5f), 
            OldStick  = new AudioID("u_oldStick", .7f),
            HighScore = new AudioID("u_highscore", 1);
    }
    
    
    public static class Reaction
    {
        public static readonly AudioID 
            DrummRoll = new AudioID("r_drumRoll", 1), 
            SnareHit  = new AudioID("r_snareHit", 1), 
            CymbalHit = new AudioID("r_cymbalHit", .6f);
    }


    public static class Sound
    {
        public static readonly AudioID 
            StickContact = new AudioID("s_stickContact", .9f), 
            StickJump    = new AudioID("s_stickJump", .9f), 
            StickWarp    = new AudioID("s_stickWarp", .3f), 
            DeathHit     = new AudioID("s_deathHit", .4f), 
            DeathAir     = new AudioID("s_deathAir", 1);
    }
}


public class AudioID
{
    public readonly int   clipHash;
    public readonly float volume;
    public readonly int   mixerHash;
    
    
    public AudioID(string clipName, float volume, int mixerHash = 0)
    {
        clipHash = clipName.GetHashCode();
        this.volume = volume;

        if (mixerHash == 0 && clipName.Length > 0)
            switch (clipName[0])
            {
                default:  this.mixerHash = Mixer.Master;  break;
                case 'm': this.mixerHash = Mixer.Music;   break;
                case 'u': this.mixerHash = Mixer.UI;      break;
                case 'r': this.mixerHash = Mixer.Reactor; break;
                case 'a':
                case 's': this.mixerHash = Mixer.All;     break;   
            }
        else
            this.mixerHash = mixerHash;
    }
}


public static class Mixer
{
    public static readonly int
        Master  = "Master".GetHashCode(),
        Music   = "Music".GetHashCode(),
        UI      = "UI".GetHashCode(),
        Reactor = "Reactor".GetHashCode(),
        Front   = "Front".GetHashCode(),
        Back    = "Back".GetHashCode(),
        All     = "All".GetHashCode();
}