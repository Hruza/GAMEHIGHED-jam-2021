using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public enum SoundType{music, sound};
    public SoundType soundType=SoundType.sound;
    public string soundName;

    public AudioClip clip;

    [Range(0,1)] public float volume=0.5f;

    [HideInInspector] public AudioSource source;
}
