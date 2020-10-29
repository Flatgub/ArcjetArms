using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXHelper : MonoBehaviour
{
    private static FXHelper instance;
    public static FXHelper Instance
    {
        get
        {
            if (instance == null)
            {
                Initialize();
            }
            return instance;
        }
    }

    private static ProjectileTracer tracerPrefab = null;
    private static Dictionary<string, AudioClip> singleSoundDatabase;
    private static Dictionary<string, AudioGroup> soundGroupDatabase;
    private static int numAudioSources = 4;
    private static List<AudioSource> audioSources;
    private static int audioSourceToUse = 0;

    public static void Initialize()
    {
        if (instance == null)
        {
            GameObject obj = new GameObject("FXHelper");
            instance = obj.AddComponent<FXHelper>();
            DontDestroyOnLoad(instance);
        }
    }

    public void Awake()
    {
        LoadAssets();

        audioSources = new List<AudioSource>();
        for (int i = 0; i < numAudioSources; i++)
        {
            AudioSource source = new GameObject("AudioSource" + i).AddComponent<AudioSource>();
            source.gameObject.transform.SetParent(transform);
            audioSources.Add(source);
        }
    }

    private void LoadAssets()
    {
        tracerPrefab = Resources.Load<ProjectileTracer>("Prefabs/BulletTracer");

        singleSoundDatabase = new Dictionary<string, AudioClip>();
        foreach (AudioClip clip in Resources.LoadAll<AudioClip>("Sounds"))
        {
            singleSoundDatabase.Add(clip.name, clip);
        }

        soundGroupDatabase = new Dictionary<string, AudioGroup>();
        foreach (AudioGroup group in Resources.LoadAll<AudioGroup>("Sounds"))
        {
            soundGroupDatabase.Add(group.name, group);
        }
    }

    public static void FireTracerBetween(Entity from, Entity to)
    {

        HexGrid grid = GameplayContext.Grid;
        Vector3 fromPos = grid.GetWorldPosition(from.Position);
        Vector3 toPos = grid.GetWorldPosition(to.Position);
        toPos.z = -1;

        ProjectileTracer tracer = Instantiate(tracerPrefab, fromPos, Quaternion.identity);
        tracer.GoTo(toPos);
    }

    public static void PlaySound(string soundName)
    {
        //attempt to use an audiogoup first
        if (soundGroupDatabase.TryGetValue(soundName, out AudioGroup group))
        {
            AudioSource source = audioSources[audioSourceToUse];
            source.clip = group.RandomClip();
            source.Play();
            audioSourceToUse = (audioSourceToUse + 1) % numAudioSources;
        } //if no audiogroup exists, try a single sound
        else if (singleSoundDatabase.TryGetValue(soundName, out AudioClip sound))
        {
            AudioSource source = audioSources[audioSourceToUse];
            source.clip = sound;
            source.Play();
            audioSourceToUse = (audioSourceToUse + 1) % numAudioSources;
        }
        else
        {
            Debug.LogError("No known sound or audiogroup: " + soundName);
        }
    }

    public static void PlaySound(string soundName, float delay)
    {
        LeanTween.delayedCall(delay, () => { FXHelper.PlaySound(soundName); });
    }
}
