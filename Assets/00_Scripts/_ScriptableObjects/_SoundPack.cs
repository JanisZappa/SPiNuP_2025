using UnityEngine;


[CreateAssetMenu(menuName = "Scriptable/SoundPack")]
public class _SoundPack : ScriptableObject
    {
        public AudioClip[] musicClips;
        [Space(10)]
        public AudioClip[] ambientClips;
        [Space(10)]
        public AudioClip[] uiClips;
        [Space(10)]
        public AudioClip[] reactorClips;
        [Space(10)]
        public AudioClip[] soundClips;
    }
