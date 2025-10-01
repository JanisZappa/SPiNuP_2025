using System.Collections.Generic;
using Clips;
using LevelElements;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Profiling;


public static partial class Sound 
{
    static Sound()
    {
        GameCam.OnNewCamSide += GameCamOnOnNewCamSide;
    }

    private static void GameCamOnOnNewCamSide(bool front)
    {
        mixer.SetFloat("Front Volume", front ? 0 : -5);
        mixer.SetFloat("Back Volume", !front ? 0 : -5);
            
        mixer.SetFloat("Front Low", front ? 22000 : 1500);
        mixer.SetFloat("Back Low", !front ? 22000 : 1500);
            
        mixer.SetFloat("Front High", front ? 0 : 500);
        mixer.SetFloat("Back High", !front ? 0 : 500);
    }

    private static bool CanPlay => !SetVolume.Mute && GameManager.Running && !GTime.Paused;

    private static AudioMixer mixer;

    private static SoundObject[] soundObjects;
    private static Dictionary<int, AudioClip>   audioClipDict;
    private static Dictionary<int, AudioMixerGroup> mixerDict;
    
        
    public static void GameLoad()
    {
        const int nrOfSounds = 40;
        soundObjects = new SoundObject[nrOfSounds];
        
        GameObject  dummy   = new GameObject("SoundObject", typeof(AudioSource));
        AudioSource source  = dummy.GetComponent<AudioSource>();
        source.rolloffMode  = AudioRolloffMode.Linear;
        source.dopplerLevel = 0;
        source.playOnAwake  = false;
        
        Transform group = Application.isEditor? new GameObject("SoundObjects").transform : null;
        
        if(group)
            group.SetParent(SceneLocator.Sound);
        
        for ( int i = 0; i < nrOfSounds; i++ )
            soundObjects[i] = new SoundObject(Object.Instantiate(dummy, group));
        
        Object.Destroy(dummy);

        
        _SoundPack soundPack = Resources.Load("Audio/SoundPack_Game") as _SoundPack;
    
        audioClipDict = new Dictionary<int, AudioClip>();
        for (int i = 0; i < soundPack.musicClips.Length; i++)
            audioClipDict.Add(soundPack.musicClips[i].name.GetHashCode(), soundPack.musicClips[i]);
        
        for (int i = 0; i < soundPack.ambientClips.Length; i++)
            audioClipDict.Add(soundPack.ambientClips[i].name.GetHashCode(), soundPack.ambientClips[i]);
        
        for (int i = 0; i < soundPack.uiClips.Length; i++)
            audioClipDict.Add(soundPack.uiClips[i].name.GetHashCode(), soundPack.uiClips[i]);
        
        for (int i = 0; i < soundPack.reactorClips.Length; i++)
            audioClipDict.Add(soundPack.reactorClips[i].name.GetHashCode(), soundPack.reactorClips[i]);
        
        for (int i = 0; i < soundPack.soundClips.Length; i++)
            audioClipDict.Add(soundPack.soundClips[i].name.GetHashCode(), soundPack.soundClips[i]);

        Resources.UnloadAsset(soundPack);
        
        
        mixer = Resources.Load("Audio/Everything") as AudioMixer;
        
        mixerDict = new Dictionary<int, AudioMixerGroup>();
        AudioMixerGroup[] allMixerGroups = mixer.FindMatchingGroups(string.Empty);
        for (int i = 0; i < allMixerGroups.Length; i++)
            mixerDict.Add(allMixerGroups[i].name.GetHashCode(), allMixerGroups[i]);
    }
    

    private static readonly SoundObject NoSound = new SoundObject(null);
    public static SoundObject Get(AudioID audioID)
    {
        audioClipDict.TryGetValue(audioID.clipHash, out AudioClip clip);
        if (!clip)
            return NoSound;

        mixerDict.TryGetValue(audioID.mixerHash, out AudioMixerGroup mixerGroup);
        if (!mixerGroup)
            mixerGroup = mixerDict[Mixer.Master];
        
        int soundObjectsLength = soundObjects.Length;
        for ( int i = 0; i < soundObjectsLength; i++ )
            if (!soundObjects[i].isPlaying)
                return soundObjects[i].Setup(clip, audioID.volume, mixerGroup); 
        
        return NoSound;
    }
    
    
    public static void SoundUpdate()
    {
        Profiler.BeginSample("Sound.SoundUpdate()");
        
        int soundObjectsLength = soundObjects.Length;
        for ( int i = 0; i < soundObjectsLength; i++ )
            if ( soundObjects[i].isPlaying)
                soundObjects[i].Update();

        if (SoundObject.Visualize)
        {
            Vector2 hitPoint = Level.HitPoint;
            float z = (Level.WallDepth + .05f) * GameCam.CurrentSide.Sign;
            float volume = SoundObject.GetVolumeAndPan(hitPoint.V3(z)).x;
            DRAW.Text(volume.ToString("F3"), hitPoint.V3(z), COLOR.red.tomato, 2, offset: V2.up * 4);
            DRAW.MultiCircle(hitPoint, .7f, 4, 1, 20).SetColor(f.Same(volume, 0)? COLOR.purple.violet : COLOR.red.tomato).SetDepth(z);
        }
        Profiler.EndSample();
    }
}




public static partial class Sound
{
    public class SoundObject
    {
        public static readonly bool Visualize = new BoolSwitch("Audio/Visualize", false);

        private readonly GameObject obj;
        private readonly AudioSource source;

        private Item    item;
        private Spinner spinner;
     
        private float startTime, length, fadeTime, volume, pitch, volMulti;
        
        public bool isPlaying;
        
        private bool Muted => !obj || !CanPlay;


        public SoundObject(GameObject obj)
        {
            if (!obj)    return;
            
            this.obj  = obj;
            source    = obj.GetComponent<AudioSource>();
            obj.SetActive(false);
        }
        
        
        public SoundObject Setup(AudioClip audioClip, float volume, AudioMixerGroup mixerGroup)
        {
            source.clip = audioClip;
            this.volume = volume;
            length      = audioClip.length;

            source.outputAudioMixerGroup = mixerGroup;
            source.loop = false;
            
            pitch    = 1;
            item     = null;
            spinner  = null;
            fadeTime = 0;
            volMulti = 1;
            return this;
        }


        public SoundObject Volume(float volume, float randomRange = 0)
        {
            if (Muted)    
                return this;
            
            this.volume *= volume * 1 + Random.Range(-randomRange, randomRange);
            
            return this;
        }
        
        
        public SoundObject PlayerMulti(Spinner spinner)
        {
            if (Muted)    
                return this;

            volume *= spinner.isFocus? 1 : .6f;
            
            return this;
        }
        
        
        public SoundObject Pitch(float pitch, float randomRange = 0)
        {
            if (Muted)    
                return this;
            
            this.pitch *= pitch * 1 + Random.Range(-randomRange, randomRange);
            
            return this;
        }


        public SoundObject Loop()
        {
            if (Muted)    
                return this;
            
            source.loop = true;
            
            return this;
        }
        
        
        public SoundObject Fade(float fadeTime)
        {
            if (Muted)    
                return this;
            
            this.fadeTime = fadeTime;
            
            return this;
        }


        public SoundObject SetSide(Side side)
        {
            if (Muted)    
                return this;
            
            source.outputAudioMixerGroup = mixerDict[side.front? Mixer.Front : Mixer.Back];
            
            return this;
        }


        public SoundObject SetItem(Item item)
        {
            if (Muted)    
                return this;
            
            Vector2 volPan = GetVolumeAndPan(item.GetPos(GTime.Now).V3(Level.GetWallDist(item.side)));
            if(f.Same(volPan.x, 0))
                return NoSound;

            volMulti         = volPan.x;
            source.panStereo = volPan.y;
            
            this.item = item;

            return SetSide(item.side);
        }


        public SoundObject SetSpinner(Spinner spinner)
        {
            if (Muted)    
                return this;

            Clip    clip   = spinner.currentClip;
            Vector3 pos    = clip.BasicPlacement(GTime.Now).pos;
            Side    side   = clip.GetSide(GTime.Now);
            Vector2 volPan = GetVolumeAndPan(pos.SetZ(Level.GetWallDist(side)));
            
            volMulti         = volPan.x;
            source.panStereo = volPan.y;

            this.spinner = spinner;
            
            return SetSide(clip.GetSide(GTime.Now));
        }
        
        

        public SoundObject Play()
        {
            if (Muted)    
                return this;
            
            obj.SetActive(true);
            isPlaying = true;

            source.volume = (!f.Same(fadeTime, 0) ? 0 : volume) * volMulti;
            source.pitch  = pitch;
            source.Play();
            startTime = Time.realtimeSinceStartup;
            
            return this;
        }
        
        
        public void Update()
        {
            const float adjustSpeed = 3;
            
            if (item != null)
            {
                Vector2 pos    = item.GetPos(GTime.Now);
                Vector2 volPan = GetVolumeAndPan(pos.V3(Level.GetWallDist(item.side)));
                
                volMulti = Mathf.Lerp(volMulti, volPan.x, Time.deltaTime * adjustSpeed);
                source.panStereo = volPan.y;
                
                if (Visualize && item.side.IsCamSide)
                    SoundDraw(pos);
            }


            if (spinner != null)
            {
                Clip    clip   = spinner.currentClip;
                Vector3 pos    = clip.BasicPlacement(GTime.Now).pos;
                Side    side   = clip.GetSide(GTime.Now);
                Vector2 volPan = GetVolumeAndPan(pos.SetZ(Level.GetPlaneDist(side)));
                
                volMulti = Mathf.Lerp(volMulti, volPan.x, Time.deltaTime * adjustSpeed);
                source.panStereo = volPan.y;
                
                SetSide(side);
                
                
                if (Visualize && side.IsCamSide)
                    SoundDraw(pos);
            }

            
            if (!f.Same(fadeTime, 0))
            {
                float lerp = Mathf.InverseLerp(startTime, startTime + fadeTime, Time.realtimeSinceStartup);
                source.volume = volume * lerp * volMulti;
            }
            else
                source.volume = volume * volMulti;

            {
                float lerp = Mathf.Lerp(length, length / pitch, .01f);
                if (!source.loop && Time.realtimeSinceStartup > startTime + lerp)
                    Stop();
            }
        }


        private void SoundDraw(Vector2 pos)
        {
            float lerp  = Mathf.InverseLerp(startTime, startTime + length, Time.realtimeSinceStartup);
            Color color = COLOR.red.tomato.A(1 - lerp);
            DRAW.Text(volMulti.ToString("F3"), pos.V3(Z.W05), color, 2, offset: V2.up * 4);
            DRAW.MultiCircle(pos, .7f, 4, 1, 20).SetColor(color).SetDepth(Z.W05);
        }

        
        public void Stop()
        {
            if (Muted)    
                return;
            
            source.Stop();
            obj.SetActive(false);
            isPlaying = false;
            item = null;
        }


        
        public static Vector2 GetVolumeAndPan(Vector3 worldPos)
        {
            Vector2 vPos   = GameCam.Cam.WorldToViewportPoint(worldPos).V2() * 2 - V2.one;
            float   pan    = vPos.x * .4f;
            float   factor = ScreenControll.Landscape ? 1f / GameCam.Cam.aspect : 1;
           
            return new Vector2(Mathf.Min(ToVolume(vPos.x, factor), ToVolume(vPos.y)), pan);
        }


        private static float ToVolume(float value, float factor = 1)
        {
            const float iMargin = 1f, oMargin = .5f;

            float linear = 1 - Mathf.InverseLerp(1 - iMargin * factor, 1 + oMargin * factor, Mathf.Abs(value));
            return 1 - Mathf.Pow(1 - linear, 1.5f);
        }
    }
}