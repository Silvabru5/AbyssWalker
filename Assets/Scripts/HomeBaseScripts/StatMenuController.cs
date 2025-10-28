using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatMenuController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private TMP_Text skillPointsText;
    [SerializeField] private Button damageButton;
    [SerializeField] private Button critChanceButton;
    [SerializeField] private Button critDamageButton;
    [SerializeField] private Button healthButton;
    [SerializeField] private Button defenseButton;
    private PlayerHealth playerHealth;

    private bool isOpen = false;

    private void Start()
    {
       
        menuPanel.SetActive(false);

        //1. add button listeners
        damageButton.onClick.AddListener(OnDamageUpgrade);
        critChanceButton.onClick.AddListener(OnCritChanceUpgrade);
        critDamageButton.onClick.AddListener(OnCritDamageUpgrade);
        healthButton.onClick.AddListener(OnHealthUpgrade);
        defenseButton.onClick.AddListener(OnDefenseUpgrade);
    }

    private void OnDamageUpgrade()
    {
        StatManager.instance.UpgradeDamage();
        UpdateUI();
    }

    private void OnCritChanceUpgrade()
    {
        StatManager.instance.UpgradeCritChance();
        UpdateUI();
    }

    private void OnCritDamageUpgrade()
    {
        StatManager.instance.UpgradeCritDamage();
        UpdateUI();
    }

    private void OnHealthUpgrade()
    {
        GameObject player = GameObject.FindWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        StatManager.instance.UpgradeHealth();
        playerHealth.UpdateHealthFromStats();
        UpdateUI();
    }

    private void OnDefenseUpgrade()
    {
        StatManager.instance.UpgradeDefense();
        UpdateUI();
    }

    private void Update()
    {
        //2. Toggle menu when player presses Tab 
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        isOpen = !isOpen;
        menuPanel.SetActive(isOpen);

        Time.timeScale = isOpen ? 0 : 1; //3. Freeze/unfreeze game
        UpdateUI();
    }

    private void UpdateUI()
    {
        skillPointsText.text = $"Skill Points: {StatManager.instance.GetSkillPoints()}";
    }
}
