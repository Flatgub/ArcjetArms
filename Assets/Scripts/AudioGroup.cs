using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAudioGroup", menuName = "AudioGroup", order = 1)]
public class AudioGroup : ScriptableObject
{
    public AudioClip[] clips;

    public AudioClip RandomClip()
    {
        int index = Random.Range(0, clips.Length-1);
        return clips[index];
    }
}
