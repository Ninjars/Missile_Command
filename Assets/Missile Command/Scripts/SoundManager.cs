using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    private static SoundManager _instance;

    public static SoundManager Instance { get { return _instance; } }

    private List<AudioSource> audioSources;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
        audioSources = new List<AudioSource>();
    }

    private AudioSource createAudioSource() {
        return gameObject.AddComponent<AudioSource>();
    }

    private AudioSource getAvailableAudioSource() {
        foreach (var source in audioSources) {
            if (!source.isPlaying) {
                return source;
            }
        }

        var newSource = createAudioSource();
        audioSources.Add(newSource);
        return newSource;
    }

    public void play(AudioClip clip) {
        play(clip, 1);
    }

    public void play(AudioClip clip, float volume) {
        play(clip, volume, null);
    }

    public void play(AudioClip clip, float volume, float? pitch) {
        var source = getAvailableAudioSource();
        source.pitch = pitch.HasValue ? pitch.Value : UnityEngine.Random.Range(0.95f, 1.05f);
        source.PlayOneShot(clip);
    }
}
