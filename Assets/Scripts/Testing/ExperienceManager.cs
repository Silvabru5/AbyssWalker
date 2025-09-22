using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml.Serialization;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager instance;

    [SerializeField] AnimationCurve experienceCurve;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI experienceText;
    [SerializeField] Image experienceFill;
    [SerializeField] int levelCap;


    int currentLevel, totalExperience;
    int previousLevelsExp, nextLevelExp;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateLevel();
    }

    // Update is called once per frame
    void Update()
    {
        CheckLevelUp();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddExperience(5);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            AddExperience(20);
        }
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void AddExperience(int amount)
    {
        totalExperience += amount;
        CheckLevelUp();
        if(currentLevel != levelCap)
        {
            UpdateUI();
        }
    }

    void CheckLevelUp()
    {
        if (totalExperience >= nextLevelExp && currentLevel != levelCap)
        {
            currentLevel++;
            UpdateLevel();
        }
    }

    void UpdateLevel()
    {
        previousLevelsExp = (int)experienceCurve.Evaluate(currentLevel);
            nextLevelExp = (int)experienceCurve.Evaluate(currentLevel + 1);
            UpdateUI();
    }

    void UpdateUI()
    {
        int start = totalExperience - previousLevelsExp;
        int end = nextLevelExp - previousLevelsExp;

        levelText.text = currentLevel.ToString();
        float percentage = (float)start / (float)end;
        experienceText.text = (percentage * 100f).ToString("F2") +" %";
        experienceFill.fillAmount = (float)start / (float)end;
        if(currentLevel == levelCap) {
            experienceText.text = "0.00 %";
            experienceFill.fillAmount = 0;
        }
    }
}
