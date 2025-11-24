using Unity.VisualScripting;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    public static StatManager instance;

    //Stat upgrades available for player - all floats to calculate percent increases
    private float damageIncrease = 1;
    private float critChance = 0;
    private float critDamage = 1.15f;
    private float healthAmount = 1;
    private float defenseAmount = 1;

    //Counters for skill point total
    [SerializeField] private int skillPoints;
    [SerializeField] private int damageLevel;
    [SerializeField] private int critChanceLevel;
    [SerializeField] private int critDamageLevel;
    [SerializeField] private int defenseLevel;
    [SerializeField] private int healthLevel;

    //Level Cap viewable within the Inspector
    [SerializeField] private int skillCap;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

    }

    //On levelup call this function
    public void AddSkillPoint()
    {
        skillPoints++;
        SaveAndLoadManager.instance.SaveGame();

    }

    //Getters for Stats
    public float GetDamageIncrease()
    {
        return damageIncrease;
    }
    public float GetCritChance()
    {
        return critChance;
    }

    public float GetCritDamage()
    {
        return critDamage;
    }

    public float GetHealthAmount()
    {
        return healthAmount;
    }

    public float GetDefenseAmount()
    {
        return defenseAmount;
    }

    public int GetDamageLevel()
    {
        return damageLevel;
    }

    public int GetCritChanceLevel()
    {
        return critChanceLevel;
    }
    public int GetCritDamageLevel()
    {
        return critDamageLevel;
    }
    public int GetDefenseLevel()
    {
        return defenseLevel;
    }
    public int GetHealthLevel()
    {
        return healthLevel;
    }
    //Spend Skill Points
    public void UpgradeDamage()
    {
        if (damageLevel != skillCap && skillPoints >= 1)
        {
            skillPoints--;
            SetDamageLevel(++damageLevel);
            SaveAndLoadManager.instance.SaveGame();
        }
    }

    public void UpgradeCritChance()
    {
        if (critChanceLevel != skillCap && skillPoints >= 1)
        {
            skillPoints--;
            SetCritChanceLevel(++critChanceLevel);
            SaveAndLoadManager.instance.SaveGame();
        }
    }

    public void UpgradeCritDamage()
    {
        if (critDamageLevel != skillCap && skillPoints >= 1)
        {
            skillPoints--;
            SetCritDamageLevel(++critDamageLevel);
            SaveAndLoadManager.instance.SaveGame();
        }
    }

    public void UpgradeHealth()
    {
        if (healthLevel != skillCap && skillPoints >= 1)
        {
            skillPoints--;
            SetHealthLevel(++healthLevel);
            SaveAndLoadManager.instance.SaveGame();
        }
    }

    public void UpgradeDefense()
    {
        if (defenseLevel != skillCap && skillPoints >= 1)
        {
            skillPoints--;
            SetDefenseLevel(++defenseLevel);
            SaveAndLoadManager.instance.SaveGame();
        }
    }

    //Tristan Addition - Getter for skill points to update UI
    public int GetSkillPoints()
    {
        return skillPoints;
    }

    public void SetDamageLevel(int _damageLevel)
    {
        damageLevel = _damageLevel;
        damageIncrease = 1f + (damageLevel * 0.15f);
    }

    public void SetCritChanceLevel(int _critChanceLevel)
    {
        critChanceLevel = _critChanceLevel;
        critChance = Mathf.Min(critChanceLevel * (1f/skillCap), 1f);
    }
    public void SetCritDamageLevel(int _critDamageLevel)
    {
        critDamageLevel = _critDamageLevel;
        critDamage = 1f + (critDamageLevel * 0.25f); // 25% chance to hit
    }
    public void SetDefenseLevel(int _defenseLevel)
    {
        defenseLevel = _defenseLevel;
        // Calculate damage multiplier (1 = full damage, 0.1 = 90% reduced)
        float maxReduction = 0.9f; // max 90% reduction
        defenseAmount = 1f - (defenseLevel / (float)skillCap) * maxReduction;

        // Ensure damage never completely blocked
        defenseAmount = Mathf.Clamp(defenseAmount, 0.1f, 1f);
    }
    public void SetHealthLevel(int _healthLevel)
    {
        healthLevel = _healthLevel;
        healthAmount = 1f + (healthLevel * 0.1f); //10% health increase per level
    }
    public void SetSkillPoints(int _skillPoints)
    {
        skillPoints = _skillPoints;

        //// adjust for skill points automatically assigned from exp curve in this setter
        //skillPoints -= damageLevel;
        //skillPoints -= critChanceLevel;
        //skillPoints -= critDamageLevel;
        //skillPoints -= defenseLevel;
        //skillPoints -= healthLevel;
    }
}
