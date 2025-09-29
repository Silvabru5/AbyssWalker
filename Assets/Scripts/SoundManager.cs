using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.EventSystems.EventTrigger;

// SoundManager.cs
// Created by: Carey Cunningham
// Purpose: To handle all sound effects and background music within the game
//          This script stays persistent so it can be called from any scene
// Usage: load in the Title scene
//        from other scenes:
//           SoundManager.PlaySound(SoundTypeEffects.BAT_ATTACK);
//           SoundManager.PlayBackgroundMusic(SoundTypeBackground.CEMETERY_BOSS);

// used this enum to define the different sound effect types
public enum SoundTypeEffects
{
    // Player Characters
    WARRIOR_ATTACK, WARRIOR_SPECIAL_ATTACK, WARRIOR_DASH_PORTAL, WARRIOR_TAKES_DAMAGE, WARRIOR_DEATH,
    MAGE_ATTACK, MAGE_SPECIAL_ATTACK, MAGE_DASH_PORTAL, MAGE_TAKES_DAMAGE, MAGE_DEATH,
    // Enemies - Normal
    BAT_ATTACK, BAT_TAKES_DAMAGE, BAT_DEATH,
    BUFF_ZOMBIE_ATTACK, BUFF_ZOMBIE_TAKES_DAMAGE, BUFF_ZOMBIE_DEATH,
    GHOUL_ATTACK, GHOUL_TAKES_DAMAGE, GHOUL_DEATH,
    SKELETON_ATTACK, SKELETON_TAKES_DAMAGE, SKELETON_DEATH,
    SPIDER_ATTACK, SPIDER_TAKES_DAMAGE, SPIDER_DEATH,
    ZOMBIE_ATTACK, ZOMBIE_TAKES_DAMAGE, ZOMBIE_DEATH,
    // Enemies - Bosses
    NECROMANCER_TAKES_DAMAGE, NECROMANCER_DEATH,
    NECROMANCER_VATS_TAKES_DAMAGE, NECROMANCER_VATS_DESTROYED,
    NECROMANCER_MINION_ATTACK, NECROMANCER_MINION_TAKES_DAMAGE, NECROMANCER_MINION_DEATH,
    NECROMANCER_PUFF_SMOKE_SPAWN,
    NECROMANCER_SKULLS_ATTACK, NECROMANCER_SKULLS_TAKES_DAMAGE, NECROMANCER_SKULLS_DEATH,
    VAMPIRE_ATTACK, VAMPIRE_SPECIAL_ATTACK, VAMPIRE_TAKES_DAMAGE, VAMPIRE_DEATH,
    VAMPIRE_BLOOD_RAIN_SPAWN,
    // Events
    TITLE_SCREEN_LOAD,
    CEMETERY_LOAD, CEMETERY_BOSS_LOAD,
    CAVE_LOAD, CAVE_BOSS_LOAD,
    LEVEL_UP,
    SKILL_UP, SKILL_RESET,
    //  Objects
    CEMETERY_TOTEM_TAKES_DAMAGE, CEMETERY_TOTEM_DESTROY, CEMETERY_BOSS_DOOR_BLOCKED, CEMETERY_DOOR_UNBLOCKED, CEMETERY_DOOR_ENTER,
    CAVE_TOTEM_TAKES_DAMAGE, CAVE_TOTEM_DESTROY, CAVE_BOSS_DOOR_BLOCKED, CAVE_DOOR_UNBLOCKED, CAVE_DOOR_ENTER,
    SKILL_UP_VENDOR_INTERACTION
}

// used this enum to define the different background sound types
public enum SoundTypeBackground
{
    MAIN_MENU,
    HOME_BASE,
    CEMETERY,
    CEMETERY_BOSS,
    CAVE,
    CAVE_BOSS
}

// create a class with 2 variables to allow for the sound effects to have differering amounts of sound options per sound type
[System.Serializable]
public class SoundEffectEntry
{
    public SoundTypeEffects type;
    public AudioClip[] clips;   // multiple variations per enum
}

// create a class with 2 variables to allow for the sound effects to have differering amounts of sound options per sound type
[System.Serializable]
public class BackgroundSoundEntry
{
    public SoundTypeBackground type;
    public AudioClip[] clips;   // multiple variations per enum
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;

    [Header("Background Music")]
    [SerializeField] private BackgroundSoundEntry[] soundListBackground; // drop down populated by the enum above
    [SerializeField] private AudioSource backgroundMusicSource;      // used this enum to define the different sound types

    [Header("Sound Effects")]
    [SerializeField] private SoundEffectEntry[] soundEffects;  // drop down populated by the enum above 
    [SerializeField] private AudioSource soundEffectsSource;   // used this enum to define the different sound types


    // create a persistant instance that is active whichever scene is active
    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance == null) instance = this;
        else Destroy(gameObject); // prevent duplicates

        // play title screen background music
        PlayBackgroundMusic(SoundTypeBackground.MAIN_MENU);

        // play the load game sound effect
        PlaySound(SoundTypeEffects.TITLE_SCREEN_LOAD);
    }


    // sound effects
    public static void PlaySound(SoundTypeEffects sound, float volume = 0.35f)
    {
        // if the sound is to be a takes damage, attack sounds only play some of the time
        if (Regex.IsMatch(sound.ToString(), "(_TAKES_DAMAGE|_ATTACK)$") & Random.Range(0, 2) == 0) return;

        // look through the sound effect lists for any categories that match the one passed in and play a random one from the list
        foreach (var entry in instance.soundEffects)
            if (entry.type == sound && entry.clips.Length > 0) // if any match and have audio clips in them proceed, otherwise do nothing
            {
                instance.soundEffectsSource.PlayOneShot(entry.clips[Random.Range(0, entry.clips.Length)], volume);
                return;
            }
    }

    // play a sound and wait for completion
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
        // look through the sound effect lists for any categories that match the one passed in and play a random one from the list
        foreach (var entry in instance.soundListBackground) // look through background sounds
            if (entry.type == sound && entry.clips.Length > 0)  // if any match and have audio clips in them proceed, otherwise do nothing
            {
                // select a random matching sound file then play it
                instance.backgroundMusicSource.clip = entry.clips[Random.Range(0, entry.clips.Length)];
                instance.backgroundMusicSource.loop = true;
                instance.backgroundMusicSource.Play();
                return;
            }
    }
}
