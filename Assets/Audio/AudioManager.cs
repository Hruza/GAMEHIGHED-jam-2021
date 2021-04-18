using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    static private AudioManager instance;
    public Sound[] sounds;

    private void Awake() {
        instance=this;
        foreach (Sound sound in sounds){
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;   
            if(sound.soundType == Sound.SoundType.music) sound.source.loop = true;
            sound.source.volume = sound.volume;
        }
    }

    static public void Play(string name) {
        Sound sound =Array.Find<Sound>(instance.sounds,x => x.soundName==name);
        sound.source.Play();
    }

    public void UpdateMusicVolume(float value){
        foreach (Sound sound in sounds){
            if(sound.soundType == Sound.SoundType.music)
                sound.source.volume = value;
        }
    }

    public void UpdateSoundVolume(float value){
        foreach (Sound sound in sounds){
            if(sound.soundType == Sound.SoundType.sound)
            sound.source.volume = value;
        }
    }

    private void Start() {
        Play("Main");
    }
}
