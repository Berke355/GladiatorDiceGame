using UnityEngine;
using System;
using System.Collections.Generic;

// Every sound effect the game plays. Assets store these as numbers: always add new ones at the end.
public enum Sfx
{
    UIClick,
    DieRoll,
    // A hit that cost the enemy HP / the player HP.
    HitEnemy,
    HitPlayer,
    // Block or Fortify absorbed (some of) a hit.
    Blocked,
    GainBlock,
    Heal,
    Death,
    YourTurn,
    EnemyTurn,
    Victory,
    Defeat,
    ItemTrigger,
    Buy,
    Upgrade,
    Reroll,
    Enchant,
    Lock,
    Swap,
    RunComplete
}

// The background music of each screen. Stored as numbers: append only.
public enum MusicTrack
{
    None,
    RunStart,
    Market,
    Battle,
    Boss
}

// One sound effect: one of its clips is picked at random each time, with a little pitch variation so repeated hits
// don't sound like a machine gun.
[Serializable]
public class SoundEntry
{
    public Sfx id;
    public AudioClip[] clips;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0f, 0.3f)] public float pitchJitter = 0.05f;
}

[Serializable]
public class MusicEntry
{
    public MusicTrack track;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 0.5f;
}

// Which clip plays for which sound (GDD "Sound"). Design data only; the player's volume settings live in PlayerPrefs.
[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Dice/Sound Library")]
public class SoundLibrarySO : ScriptableObject
{
    public List<SoundEntry> sounds = new List<SoundEntry>();
    public List<MusicEntry> music = new List<MusicEntry>();

    public SoundEntry Find(Sfx id){
        foreach(SoundEntry entry in sounds){
            if(entry.id == id) return entry;
        }
        return null;
    }

    public MusicEntry Find(MusicTrack track){
        foreach(MusicEntry entry in music){
            if(entry.track == track) return entry;
        }
        return null;
    }
}
