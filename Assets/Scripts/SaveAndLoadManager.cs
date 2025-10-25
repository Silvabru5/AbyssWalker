using System;
using System.IO;
using System.IO.Enumeration;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor.Overlays;
using UnityEngine;

// container for character specific data
[Serializable]
public class CharacterData
{
    public int experience;
    public int skillPointsDamage;
    public int skillPointsDefense;
    public int skillPointsHealth;
    public int skillPointsCritChance;
    public int skillPointsCritDamage;
    public int highestBossDefeated;

    // these are used in game but not saved to the file
    public int level;
    public int skillPointsUnspent;

    // define constants to ensure save data validity
    private const int MAX_EXPERIENCE = 99999;
    private const int MAX_SKILL_POINTS_DAMAGE = 99;
    private const int MAX_SKILL_POINTS_DEFENSE = 99;
    private const int MAX_SKILL_POINTS_HEALTH = 99;
    private const int MAX_SKILL_POINTS_CRIT_CHANCE = 99;
    private const int MAX_SKILL_POINTS_CRIT_DAMAGE = 99;
    private const int MAX_BOSS_LEVEL_DEFEATED = 2;

    // used to initialize all values, if any value is read in that is invalid it is replaced with these defaults
    public void LoadDefaultValues()
    {
        experience = 0;
        skillPointsDamage = 0;
        skillPointsDefense = 0;
        skillPointsHealth = 0;
        skillPointsCritChance = 0;
        skillPointsCritDamage = 0;
        highestBossDefeated = 0;
        level = 0;
        skillPointsUnspent = 0;
    }

    // create a key/value pair to save all character stat names and values and save the string encrypted
    public string GenerateEncryptedSaveString()
    {
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"Experience={experience}," +
            $"SkillPointsDamage={skillPointsDamage}," +
            $"SkillPointsDefense={skillPointsDefense}," +
            $"SkillPointsDefense={skillPointsHealth}," +
            $"SkillPointsCritChance={skillPointsCritChance}," +
            $"SkillPointsCritDamage={skillPointsCritDamage}," +
            $"HighestBossDefeated={highestBossDefeated}"));
    }

    // parse the load game player data read in and apply any valid values found
    public void ParseAndValidateEncryptedSaveFileString(string playerData)
    {
        // if no data found, leave at defaults
        if (string.IsNullOrEmpty(playerData)) { return; }

        // decrypt the encoded string
        playerData = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(playerData));

        // split the string by key/value pairs
        string[] stats = playerData.Split(',');

        foreach (string attribute in stats)
        {
            // if the value is blank, ignore and load defaults
            if (string.IsNullOrWhiteSpace(attribute))
                continue;

            // split each key/value into key and value
            string[] attributeKeyValuePair = attribute.Split("=");

            string key = attributeKeyValuePair[0].Trim();

            // if value isn't an int, ignore it
            if (!int.TryParse(attributeKeyValuePair[1], out int value))
                continue;

            if (key == "Experience" && value > 0 && value <= MAX_EXPERIENCE) experience = value;
            else if (key == "SkillPointsDamage" && value > 0 && value <= MAX_SKILL_POINTS_DAMAGE) skillPointsDamage = value;
            else if (key == "SkillPointsDefense" && value > 0 && value <= MAX_SKILL_POINTS_DEFENSE) skillPointsDefense = value;
            else if (key == "SkillPointsHealth" && value > 0 && value <= MAX_SKILL_POINTS_HEALTH) skillPointsDefense = value;
            else if (key == "SkillPointsCritChance" && value > 0 && value <= MAX_SKILL_POINTS_CRIT_CHANCE) skillPointsCritChance = value;
            else if (key == "SkillPointsCritDamage" && value > 0 && value <= MAX_SKILL_POINTS_CRIT_DAMAGE) skillPointsCritDamage = value;
            else if (key == "HighestBossDefeated" && value > 0 && value <= MAX_BOSS_LEVEL_DEFEATED) highestBossDefeated = value;
        }
    }
}

// container for all data to save
[Serializable]
public class SaveData
{
    public int currentCharacter = 0;
    public CharacterData warrior = new CharacterData();
    public CharacterData mage = new CharacterData();
}

public class SaveAndLoadManager : MonoBehaviour
{
    // constant for validity checking
    private const int MAX_LAST_PLAYED_CHARACTER = 1;

    // make this class a singleton - we only ever want one version running
    public static SaveAndLoadManager instance { get; private set; }

    // save data manager
    private SaveData saveData = new SaveData();

    // hold the path to the datafile
    private string filename;

    // create the instance if this is the first, otherwise destroy this attempt, then define the filename to save the data to
    private void Awake()
    {
        // if this class isn't already in memory, 
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        filename = Path.Combine(Application.persistentDataPath, "save.dat");

        LoadData();
    }

    // save the data to a file in binary format
    public void SaveGame()
    {
        try
        {
            using (FileStream fileStream = new FileStream(filename, FileMode.Create))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(0); // current character

                    // write out class data based in encrypted binary
                    binaryWriter.Write(saveData.warrior.GenerateEncryptedSaveString());
                    binaryWriter.Write(saveData.mage.GenerateEncryptedSaveString());
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("ERROR: failed to write to save file (" + filename + ")" + e.Message);
        }
    }

    // load he save data from a binary file
    public void LoadData()
    {
        // load the default values, then overwrite if the values found in the datafile are valid 
        saveData.warrior.LoadDefaultValues();
        saveData.mage.LoadDefaultValues();
        saveData.currentCharacter = 0;

        // if the file doesn't exist, make a new one
        if (!File.Exists(filename))
        {
            Debug.Log("No save file found, creating one.");
            SaveGame();
            return;
        }

        try
        {
            using (FileStream fileStream = new FileStream(filename, FileMode.Open))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    // read in last played character and update if valid
                    int currentCharacter = binaryReader.ReadInt32();
                    if (currentCharacter >= 0 && currentCharacter <= MAX_LAST_PLAYED_CHARACTER)
                        saveData.currentCharacter = currentCharacter;

                    // read in class specific data, and apply it if valid, apply defaults if not
                    string warriorData = binaryReader.ReadString();
                    if (warriorData != null) saveData.warrior.ParseAndValidateEncryptedSaveFileString(warriorData);

                    string mageData = binaryReader.ReadString();
                    if (mageData != null) saveData.mage.ParseAndValidateEncryptedSaveFileString(mageData);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("ERROR: failed to load save data file (" + filename + "), using default values" + e.Message);
        }
    }
}
