using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

public enum SoundTypeEffects
{
    GAME_LEVEL_ENTER,
    PLAYER_BARBARIAN_TAKES_DAMAGE,
    PLAYER_BARBARIAN_ATTACK,
    PLAYER_BARBARIAN_SPECIAL_ATTACK,
    PLAYER_BARBARIAN_DEATH,
    ENEMY_ATTACK_SPIDER,
    ENEMY_TAKES_DAMAGE_SPIDER,
    ENEMY_DEATH_SPIDER,
    ENEMY_ATTACK_SKELETON,
    ENEMY_TAKES_DAMAGE_SKELETON,
    ENEMY_DEATH_SKELETON,
    ENEMY_ATTACK_ZOMBIE,
    ENEMY_TAKES_DAMAGE_ZOMBIE,
    ENEMY_DEATH_ZOMBIE,
    BOSS_DOOR_OPENS,
    BOSS_DOOR_ENTER,
    BOSS_DOOR_BLOCKED
}

public enum SoundTypeBackground
{
    BACKGROUND_TITLE,
    BACKGROUND_GAME,
    BACKGROUND_BOSS
}

// make an array to represent the ENUM of sounds, for each sound, make an array of possible clips to play
[System.Serializable]
public class SoundEffectEntry
{
    public SoundTypeEffects type;
    public AudioClip[] clips;   // multiple variations per enum
}


[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;

    [Header("Background Music")]
    [SerializeField] private AudioClip[] soundListBackground;
    [SerializeField] private AudioSource backgroundMusicSource;

    [Header("Sound Effects")]
    [SerializeField] private SoundEffectEntry[] soundEffects;
    [SerializeField] private AudioSource soundEffectsSource;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance == null) instance = this;
        else Destroy(gameObject); // prevent duplicates

        // play title screen background music
        PlayBackgroundMusic(SoundTypeBackground.BACKGROUND_TITLE);
    }


    // sound effects
    public static void PlaySound(SoundTypeEffects sound, float volume = 0.35f)
    {
        // if the sound is to be a takes damage, attack sounds only play some of the time
        if (Regex.IsMatch(sound.ToString(), "(_TAKES_DAMAGE|_ATTACK)$") & Random.Range(0, 2) == 0) return;

        foreach (var entry in instance.soundEffects)
            if (entry.type == sound && entry.clips.Length > 0)
            {
                instance.soundEffectsSource.PlayOneShot(entry.clips[Random.Range(0, entry.clips.Length)], volume);
                return;
            }
    }

    public IEnumerator WaitForSound(AudioSource audioSource)
    {
        yield return new WaitWhile(() => audioSource.isPlaying);
    }

    public static void PlaySoundWaitForCompletion(SoundTypeEffects sound)
    {
        PlaySound(sound);
        instance.StartCoroutine(instance.WaitForSound(instance.soundEffectsSource));
    }


    // background music
    public static void PlayBackgroundMusic(SoundTypeBackground sound)
    {
        instance.backgroundMusicSource.Stop();
        instance.backgroundMusicSource.clip = instance.soundListBackground[(int)sound];
        instance.backgroundMusicSource.loop = true;
        instance.backgroundMusicSource.Play();
    }
}
