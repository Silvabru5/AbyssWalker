using System.Collections;
using Unity.VisualScripting;
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


[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    [SerializeField] private AudioClip[] soundListBackground;
    [SerializeField] private AudioClip[] soundListEffects;
    [SerializeField] private AudioSource backgroundMusicSource;
    [SerializeField] private AudioSource soundEffectsSource;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance == null) instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        instance.backgroundMusicSource.clip = instance.soundListBackground[1];
        instance.backgroundMusicSource.Play();
    }

    public static void PlaySound(SoundTypeEffects sound, float volume = 0.35f)
    {
        // PlayOneShot - play a clip one time with settings set in the AudioSource
        instance.soundEffectsSource.PlayOneShot(instance.soundListEffects[(int)sound], volume);
    }

    public IEnumerator WaitForSound(AudioSource audioSource)
    {
        yield return new WaitWhile(() => audioSource.isPlaying);
        Debug.Log("Sound has finished playing!");
    }

    // play a sound aand wait until complete
    public static void PlaySoundWaitForCompletion(SoundTypeEffects sound, float volume = 0.35f)
    {
        PlaySound(sound, volume);
        instance.StartCoroutine(instance.WaitForSound(instance.soundEffectsSource)); // wait for the audio clip to complete before continuing
    }

    public static void PlayBackgroundMusic(SoundTypeBackground sound, float volume = .25f)
    {
        instance.backgroundMusicSource.Stop();
        instance.backgroundMusicSource.clip = instance.soundListBackground[(int)sound];
        instance.backgroundMusicSource.loop = true;
        instance.backgroundMusicSource.volume = volume;
        instance.backgroundMusicSource.Play();
    }

    public AudioSource getSoundEffectsSource() { return soundEffectsSource; }

    public static void OnAwake()
    {
        PlayBackgroundMusic(SoundTypeBackground.BACKGROUND_TITLE);
    }
    
}