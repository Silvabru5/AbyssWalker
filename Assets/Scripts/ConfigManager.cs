using SuperTiled2Unity;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    // Singleton instance
    public static ConfigManager Instance { get; private set; }

    private Dictionary<string, string> config = new Dictionary<string, string>();
    private string filename;
    private List<string> validKeys = new List<string> { "musicVolume", "effectsVolume", "playerName" };

    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // persist across scenes

        filename = Path.Combine(Application.persistentDataPath, "config.ini");

        // Initialize default values
        foreach (string key in validKeys)
            if (!config.ContainsKey(key))
                config[key] = "100"; // default value
        LoadConfigFile();
    }

    // Save config dictionary to file
    public void SaveConfigFile()
    {
        // create a list of key=value pairs for the dictionary and write it out
        List<string> keyValueList = new List<string>();
        foreach (string key in validKeys)
            if (config.ContainsKey(key))
                keyValueList.Add(key + "=" + config[key]);
        File.WriteAllLines(filename, keyValueList);
    }

    // Load config dictionary from file
    void LoadConfigFile()
    {
        // if no file found, there is nothing to read, so no further actions necessary
        if (!File.Exists(filename)) return;

        string[] lines = File.ReadAllLines(filename);
        foreach (string line in lines)
        {
            // if not in the expected format, reject the line key=value
            if (!Regex.IsMatch(line, @"^\s*[A-Za-z]+\s*=\s*[0-9]+\s*$")) continue;

            // split key and value and trim leading/trailing whitespace
            string[] parts = line.Split('=', 2);
            string key = parts[0].Trim();
            string value = parts[1].Trim();

            // only overwrite the default value when a valid key and value are found
            if (validKeys.Contains(key) && Regex.IsMatch(config[key], @"^\d+$") && config[key].ToInt() >= 0 && config[key].ToInt() <= 100)
                config[key] = value;
        }
    }

    // getter for pause -> sound menu
    public int GetInt(string key, int defaultValue = 0)
    {
        if (config.ContainsKey(key) && int.TryParse(config[key], out int value))
            return value;
        return defaultValue;
    }

    // setter for pause -> sound menu
    public void SetInt(string key, int value)
    {
        if (validKeys.Contains(key))
            config[key] = value.ToString();
    }
}
