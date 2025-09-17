using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml.Serialization;

public class ExperienceManager : MonoBehaviour
{
    [SerializeField] AnimationCurve experienceCurve;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI experienceText;
    [SerializeField] Image experienceFill;
    [SerializeField] int levelCap;


    int currentLevel, totalExperience;
    int previousLevelsExp, nextLevelExp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddExperience(5);
        }
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
        experienceText.text = start + " exp / " + end + " exp";
        experienceFill.fillAmount = (float)start / (float)end;
    }
}
