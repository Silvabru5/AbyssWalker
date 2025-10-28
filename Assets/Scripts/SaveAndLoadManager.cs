using System;
using System.IO;
//using System.IO.Enumeration;
//using System.Runtime.Serialization.Formatters.Binary;
//using UnityEditor.Overlays;
using UnityEngine;
//using UnityEngine.InputSystem;
//using static UnityEngine.Rendering.DebugUI;

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
    public int skillPointsUnspent;

    // these are used in game but not saved to the file
    private int level;

    // define constants to ensure save data validity
    private const int MAX_EXPERIENCE = 99999;
    private const int MAX_SKILL_POINTS_DAMAGE = 99;
    private const int MAX_SKILL_POINTS_DEFENSE = 99;
    private const int MAX_SKILL_POINTS_HEALTH = 99;
    private const int MAX_SKILL_POINTS_CRIT_CHANCE = 99;
    private const int MAX_SKILL_POINTS_CRIT_DAMAGE = 99;
    private const int MAX_BOSS_LEVEL_DEFEATED = 2;
    private const int MAX_SKILL_POINTS_UNSPENT = 50;

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
        skillPointsUnspent = 0;
    }

    // create a key/value pair to save all character stat names and values and save the string encrypted
    public string GenerateEncryptedSaveString()
    {
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(
            $"Experience={experience}," +
            $"SkillPointsDamage={skillPointsDamage}," +
            $"SkillPointsDefense={skillPointsDefense}," +
            $"SkillPointsHealth={skillPointsHealth}," +
            $"SkillPointsCritChance={skillPointsCritChance}," +
            $"SkillPointsCritDamage={skillPointsCritDamage}," +
            $"HighestBossDefeated={highestBossDefeated}," +
            $"SkillPointsUnspent={skillPointsUnspent}"));
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

            if (key == "Experience" && value >= 0 && value <= MAX_EXPERIENCE) experience = value;
            else if (key == "SkillPointsDamage" && value >= 0 && value <= MAX_SKILL_POINTS_DAMAGE) skillPointsDamage = value;
            else if (key == "SkillPointsDefense" && value >= 0 && value <= MAX_SKILL_POINTS_DEFENSE) skillPointsDefense = value;
            else if (key == "SkillPointsHealth" && value >= 0 && value <= MAX_SKILL_POINTS_HEALTH) skillPointsHealth = value;
            else if (key == "SkillPointsCritChance" && value >= 0 && value <= MAX_SKILL_POINTS_CRIT_CHANCE) skillPointsCritChance = value;
            else if (key == "SkillPointsCritDamage" && value >= 0 && value <= MAX_SKILL_POINTS_CRIT_DAMAGE) skillPointsCritDamage = value;
            else if (key == "HighestBossDefeated" && value >= 0 && value <= MAX_BOSS_LEVEL_DEFEATED) highestBossDefeated = value;
            else if (key == "SkillPointsUnspent" && value >= 0 && value <= MAX_SKILL_POINTS_UNSPENT) skillPointsUnspent = value;
        }
    }
}

// container for all data to save
[Serializable]
public class SaveData
{
    //public int currentCharacter = 0;
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
    public SaveData saveData = new SaveData();

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

     //   LoadData();
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
                    // update player data
                    switch (PlayerPrefs.GetInt("SelectedCharacter",0))
                    {
                        case 0: UpdateCharacterData(saveData.warrior); break;
                        //case 1: UpdateCharacterData(saveData.mage); break;
                        default: Debug.Log("Error: character index out of range in SaveGame"); break;
                    }

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

    // grab the character data from the experience manager and statmanager
    public void UpdateCharacterData(CharacterData characterData)
    {
        characterData.experience = ExperienceManager.instance.GetExperience();
        characterData.skillPointsDamage = StatManager.instance.GetDamageLevel();
        characterData.skillPointsDefense = StatManager.instance.GetDefenseLevel();
        characterData.skillPointsCritChance = StatManager.instance.GetCritChanceLevel();
        characterData.skillPointsCritDamage = StatManager.instance.GetCritDamageLevel();
        characterData.skillPointsHealth = StatManager.instance.GetHealthLevel();
        characterData.highestBossDefeated = 0; // not using this yet
        characterData.skillPointsUnspent = StatManager.instance.GetSkillPoints(); // - characterData.skillPointsDamage - characterData.skillPointsDefense - characterData.skillPointsCritChance - characterData.skillPointsCritDamage - characterData.skillPointsHealth;
    }

    // load he save data from a binary file
    public void LoadData()
    {
        // load the default values, then overwrite if the values found in the datafile are valid 
        saveData.warrior.LoadDefaultValues();
        saveData.mage.LoadDefaultValues();

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
                    //// read in last played character and update if valid
                    //int currentCharacter = binaryReader.ReadInt32();
                    //if (currentCharacter >= 0 && currentCharacter <= MAX_LAST_PLAYED_CHARACTER)
                    //    saveData.currentCharacter = currentCharacter;

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

        // update the experience and stat manager
        switch (PlayerPrefs.GetInt("SelectedCharacter",0))
        {
            case 0: LoadCharacterData(saveData.warrior); break;
            case 1: LoadCharacterData(saveData.mage); break;
            default: Debug.Log("Error: character index out of range in SaveGame"); break;
        }
    }

    // populate the experience manager and statmanager with save game data
    public void LoadCharacterData(CharacterData characterData)
    {
        //ExperienceManager.instance.AddExperience(characterData.experience);
        //for (int i = 0; i < characterData.skillPointsDamage; i++) { StatManager.instance.UpgradeDamage(); }
        //for (int i = 0; i < characterData.skillPointsDefense; i++) { StatManager.instance.UpgradeDefense(); }
        //for (int i = 0; i < characterData.skillPointsHealth; i++) { StatManager.instance.UpgradeHealth(); }
        //for (int i = 0; i < characterData.skillPointsCritChance; i++) { StatManager.instance.UpgradeCritChance(); }
        //for (int i = 0; i < characterData.skillPointsCritDamage; i++) { StatManager.instance.UpgradeCritDamage(); }

        //Debug.Log($"Loading: CritChance:{characterData.skillPointsCritChance}, {StatManager.instance.GetCritChanceLevel()}, {saveData.warrior.skillPointsCritChance}");

        //ExperienceManager.instance.SetExperience(characterData.experience);
        //StatManager.instance.SetDamageLevel(characterData.skillPointsDamage);
        //StatManager.instance.SetDefenseLevel(characterData.skillPointsDefense);
        //StatManager.instance.SetHealthLevel(characterData.skillPointsHealth);
        //StatManager.instance.SetCritChanceLevel(characterData.skillPointsCritChance);
        //StatManager.instance.SetCritDamageLevel(characterData.skillPointsCritDamage);
        //StatManager.instance.SetSkillPoints(characterData.skillPointsUnspent);

        ExperienceManager.instance.SetExperience(characterData.experience);
        for (int i = 0; i < characterData.skillPointsDamage; i++) { StatManager.instance.SetDamageLevel(StatManager.instance.GetDamageLevel()+1); StatManager.instance.SetSkillPoints(StatManager.instance.GetSkillPoints() - 1); }
        for (int i = 0; i < characterData.skillPointsCritDamage; i++) { StatManager.instance.SetCritDamageLevel(StatManager.instance.GetCritDamageLevel() + 1); StatManager.instance.SetSkillPoints(StatManager.instance.GetSkillPoints() - 1); }
        for (int i = 0; i < characterData.skillPointsCritChance; i++) { StatManager.instance.SetCritChanceLevel(StatManager.instance.GetCritChanceLevel() + 1); StatManager.instance.SetSkillPoints(StatManager.instance.GetSkillPoints() - 1); }
        for (int i = 0; i < characterData.skillPointsHealth; i++) { StatManager.instance.SetHealthLevel(StatManager.instance.GetHealthLevel() + 1); StatManager.instance.SetSkillPoints(StatManager.instance.GetSkillPoints() - 1); }
        for (int i = 0; i < characterData.skillPointsDefense; i++) { StatManager.instance.SetDefenseLevel(StatManager.instance.GetDefenseLevel() + 1); StatManager.instance.SetSkillPoints(StatManager.instance.GetSkillPoints() - 1); }

        StatManager.instance.GetHealthAmount(); // update player health
    }
}
