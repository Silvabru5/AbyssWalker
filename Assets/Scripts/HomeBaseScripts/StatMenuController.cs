using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

// This scriptcontrols the stat upgrade menu in the home base
public class StatMenuController : MonoBehaviour
{

    //various ui references and their buttons
    [Header("UI References")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private TMP_Text skillPointsText;
    [SerializeField] private TMP_Text critChanceText;
    [SerializeField] private TMP_Text critDamageText;
    [SerializeField] private TMP_Text defenseText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private Button damageButton;
    [SerializeField] private Button critChanceButton;
    [SerializeField] private Button critDamageButton;
    [SerializeField] private Button healthButton;
    [SerializeField] private Button defenseButton;
    private PlayerHealth playerHealth;

    // Track if the menu is open, defaulted to not open
    private bool isOpen = false;

    private void Start()
    {
       // Initially hide the menu
        menuPanel.SetActive(false);

        //add listeners to buttons to call upgrade functions
        damageButton.onClick.AddListener(OnDamageUpgrade);
        critChanceButton.onClick.AddListener(OnCritChanceUpgrade);
        critDamageButton.onClick.AddListener(OnCritDamageUpgrade);
        healthButton.onClick.AddListener(OnHealthUpgrade);
        defenseButton.onClick.AddListener(OnDefenseUpgrade);
    }

    private void OnDamageUpgrade() //function called when damage upgrade button is pressed
    {
        StatManager.instance.UpgradeDamage(); //call stat manager to upgrade damage
        UpdateUI(); //update the ui to reflect changes
    }

    private void OnCritChanceUpgrade() //function called when crit chance upgrade button is pressed
    {
        StatManager.instance.UpgradeCritChance(); //call stat manager to upgrade crit chance
        UpdateUI(); //update the ui to reflect changes
    }

    private void OnCritDamageUpgrade() //function called when crit damage upgrade button is pressed
    {
        StatManager.instance.UpgradeCritDamage(); //call stat manager to upgrade crit damage
        UpdateUI(); //update the ui to reflect changes
    }

    private void OnHealthUpgrade() //function called when health upgrade button is pressed
    {
        GameObject player = GameObject.FindWithTag("Player"); //find player in scene
        playerHealth = player.GetComponent<PlayerHealth>(); //get player health component
        StatManager.instance.UpgradeHealth(); //call stat manager to upgrade health
        playerHealth.UpdateHealthFromStats(); //update player health based on new stats
        UpdateUI(); //update the ui to reflect changes
    }

    private void OnDefenseUpgrade() //function called when defense upgrade button is pressed
    {
        StatManager.instance.UpgradeDefense(); //call stat manager to upgrade defense
        UpdateUI(); //update the ui to reflect changes
    }

    private void Update()
    {
        //Toggle stats menu when player presses Tab 
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMenu();
          
        }
    }

    private void ToggleMenu() //function to open/close stats menu
    {

        //1. Toggle menu visibility
        isOpen = !isOpen;
        // 2. Show/hide menu panel
        menuPanel.SetActive(isOpen);
        // 3. Show/hide cursor and lock state based on menu state and scene
        if (isOpen && SceneManager.GetActiveScene().buildIndex == 2)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            isOpen = false;
            menuPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }
        // 4. Pause/unpause game time based on menu state
        Time.timeScale = isOpen ? 0 : 1; 
        // 5. Update UI elements to reflect current stats
        UpdateUI();
    }

    private void UpdateUI() //function to update the stat menu ui elements
    {
        skillPointsText.text = $"Skill Points: {StatManager.instance.GetSkillPoints()}";
        critChanceText.text = $"Level: {StatManager.instance.GetCritChanceLevel()}";
        critDamageText.text = $"Level: {StatManager.instance.GetCritDamageLevel()}";
        defenseText.text = $"Level: {StatManager.instance.GetDefenseLevel()}";
        healthText.text = $"Level: {StatManager.instance.GetHealthLevel()}";
        damageText.text = $"Level: {StatManager.instance.GetDamageLevel()}";

    }
}
