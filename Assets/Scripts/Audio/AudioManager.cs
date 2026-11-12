using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// Plays the game's sounds and music (GDD "Sound"). It lives on the RunManager prefab, so it survives scene loads and
// the music can fade from one screen to the next. Callers only name what happened: AudioManager.Play(Sfx.Buy).
// Nothing in the rules depends on it: without it (or without a clip) the game is just silent.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public SoundLibrarySO library;
    // How many sound effects can sound at once.
    public int voices = 10;
    public float musicFadeSeconds = 1.2f;

    private const string MusicVolumeKey = "musicVolume";
    private const string SfxVolumeKey = "sfxVolume";
    // The same sound again within this time is skipped (a multi-hit lands in one frame and would just be louder).
    private const float RepeatGap = 0.06f;

    private readonly List<AudioSource> sfxSources = new List<AudioSource>();
    private int nextVoice;
    private readonly Dictionary<Sfx, float> lastPlayed = new Dictionary<Sfx, float>();

    // Two music sources, so one track fades out while the next fades in.
    private AudioSource musicA, musicB;
    private MusicTrack currentTrack = MusicTrack.None;
    private float currentTrackVolume;
    private Coroutine musicFade;

    public static float MusicVolume {
        get { return PlayerPrefs.GetFloat(MusicVolumeKey, 0.8f); }
        set { PlayerPrefs.SetFloat(MusicVolumeKey, Mathf.Clamp01(value)); if(Instance != null) Instance.ApplyMusicVolume(); }
    }
    public static float SfxVolume {
        get { return PlayerPrefs.GetFloat(SfxVolumeKey, 0.8f); }
        set { PlayerPrefs.SetFloat(SfxVolumeKey, Mathf.Clamp01(value)); }
    }

    void Awake(){
        if(Instance != null && Instance != this){ Destroy(this); return; }
        Instance = this;
        for(int i = 0; i < voices; i++){
            sfxSources.Add(NewSource(false));
        }
        musicA = NewSource(true);
        musicB = NewSource(true);
    }

    void OnDestroy(){
        if(Instance == this) Instance = null;
    }

    private AudioSource NewSource(bool loop){
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = loop;
        source.spatialBlend = 0f;
        return source;
    }

    // Every interactable button clicks, wherever it is and however it was made (copied cards included): a click is
    // checked against what the pointer is over instead of wiring each button.
    void Update(){
        if(Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame || EventSystem.current == null) return;
        PointerEventData pointer = new PointerEventData(EventSystem.current){ position = Mouse.current.position.ReadValue() };
        List<RaycastResult> hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, hits);
        if(hits.Count == 0) return;
        Selectable selectable = hits[0].gameObject.GetComponentInParent<Selectable>();
        if(selectable != null && selectable.IsInteractable()) Play(Sfx.UIClick);
    }

    public static void Play(Sfx id){
        if(Instance != null) Instance.PlaySfx(id);
    }

    private void PlaySfx(Sfx id){
        if(library == null) return;
        SoundEntry entry = library.Find(id);
        if(entry == null || entry.clips == null || entry.clips.Length == 0) return;
        float last;
        if(lastPlayed.TryGetValue(id, out last) && Time.unscaledTime - last < RepeatGap) return;
        lastPlayed[id] = Time.unscaledTime;
        AudioClip clip = entry.clips[Random.Range(0, entry.clips.Length)];
        if(clip == null) return;
        AudioSource source = sfxSources[nextVoice];
        nextVoice = (nextVoice + 1) % sfxSources.Count;
        source.pitch = 1f + Random.Range(-entry.pitchJitter, entry.pitchJitter);
        source.PlayOneShot(clip, entry.volume * SfxVolume);
    }

    // Fades to the screen's track; the same track keeps playing (a battle after a battle doesn't restart it).
    public static void PlayMusic(MusicTrack track){
        if(Instance != null) Instance.SwitchMusic(track);
    }

    private void SwitchMusic(MusicTrack track){
        if(track == currentTrack && musicA.isPlaying) return;
        currentTrack = track;
        MusicEntry entry = library != null ? library.Find(track) : null;
        // The new track goes on the free source; the old one fades out on the other.
        AudioSource from = musicA, to = musicB;
        musicA = to; musicB = from;
        to.Stop();
        to.clip = entry != null ? entry.clip : null;
        currentTrackVolume = entry != null ? entry.volume : 0f;
        if(to.clip != null){
            to.volume = 0f;
            to.Play();
        }
        if(musicFade != null) StopCoroutine(musicFade);
        musicFade = StartCoroutine(Crossfade(from, to));
    }

    // Stops the music (e.g. under the victory or defeat jingle); the next PlayMusic starts it again.
    public static void StopMusic(){
        if(Instance != null) Instance.SwitchMusic(MusicTrack.None);
    }

    private IEnumerator Crossfade(AudioSource from, AudioSource to){
        float fromStart = from.volume;
        for(float t = 0f; t < musicFadeSeconds; t += Time.unscaledDeltaTime){
            float k = t / musicFadeSeconds;
            from.volume = Mathf.Lerp(fromStart, 0f, k);
            to.volume = Mathf.Lerp(0f, currentTrackVolume * MusicVolume, k);
            yield return null;
        }
        from.Stop();
        to.volume = currentTrackVolume * MusicVolume;
        musicFade = null;
    }

    private void ApplyMusicVolume(){
        if(musicFade == null && musicA != null) musicA.volume = currentTrackVolume * MusicVolume;
    }
}
