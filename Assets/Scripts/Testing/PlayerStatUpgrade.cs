using Unity.VisualScripting;
using UnityEngine;

public class PlayerStatUpgrade : MonoBehaviour
{
    public static PlayerStatUpgrade instance;

    //Stat upgrades available for player - all floats to calculate percent increases
    private float damageIncrease;
    private float critChance;
    private float critDamage;
    private float healthAmount;
    private float defenseAmount;

    //Counters for skill point total
    private int skillPoints;
    private int damageLevel;
    private int critChanceLevel;
    private int critDamageLevel;
    private int defenseLevel;
    private int healthLevel;

    //Level Cap viewable within the Inspector
    [SerializeField] private int levelCap;

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
        if(damageLevel != levelCap)
        {
            skillPoints--;
            damageLevel++;
        }
        damageIncrease = 1f + (damageLevel * 0.15f);
    }

    public void UpgradeCritChance()
    {
        if (critChanceLevel != levelCap)
        {
            skillPoints--;
            critChanceLevel++;
        }
        critChance = Mathf.Min(critChanceLevel * 0.1f, 1f);
    }

    public void UpgradeCritDamage()
    {
        if(critDamageLevel != levelCap)
        {
            skillPoints--;
            critDamageLevel++;
        }
        critDamage = 1f + (critDamageLevel * 0.08f);
    } 
}
