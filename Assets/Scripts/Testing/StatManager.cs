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
    private float defenseAmount = 0;

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

    //Spend Skill Points
    public void UpgradeDamage()
    {
        if(damageLevel != skillCap)
        {
            skillPoints--;
            damageLevel++;
        }
        damageIncrease = 1f + (damageLevel * 0.15f);
    }

    public void UpgradeCritChance()
    {
        if (critChanceLevel != skillCap)
        {
            skillPoints--;
            critChanceLevel++;
        }
        critChance = Mathf.Min(critChanceLevel * 0.25f, 1f);
    }

    public void UpgradeCritDamage()
    {
        if(critDamageLevel != skillCap)
        {
            skillPoints--;
            critDamageLevel++;
        }
        critDamage = 1f + (critDamageLevel * 0.25f); // 25% chance to hit
    }

    public void UpgradeHealth()
    {
        if (healthLevel != skillCap)
        {
            skillPoints--;
            healthLevel++;
        }

        healthAmount = 1f + (healthLevel * 0.1f); //10% health increase per level
    }

    public void UpgradeDefense()
    {
        if(defenseLevel != skillCap)
        {
            skillPoints--;
            defenseLevel++;
        }

        defenseAmount = 1f - (defenseLevel * 0.15f);
        defenseAmount = Mathf.Max(defenseAmount, 0.1f);
    }
}
