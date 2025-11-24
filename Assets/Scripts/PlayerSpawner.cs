using UnityEngine;
using Unity.Cinemachine;


//This script spawns the selected character based off of player prefs from character select screen 
public class PlayerSpawner : MonoBehaviour
{

    // References to character prefabs
    public GameObject warriorPrefab;
    public GameObject magePrefab;

    // Reference to the Cinemachine virtual camera
    public CinemachineCamera vCam;

    // Variable to hold selected character index
    private int selected = 0;
    void Start()
    {
        // Retrieve selected character index from PlayerPrefs 
        selected = PlayerPrefs.GetInt("SelectedCharacter", 0);

        GameObject player;

        if (selected == 0) // Warrior selected
        {
            player = Instantiate(warriorPrefab);
        }
        else if (selected == 1) // Mage selected
        {
            player = Instantiate(magePrefab);
        }
        else
        {
            player = Instantiate(warriorPrefab); // Default to warrior if invalid selection
        }

        // Set the Cinemachine virtual camera to follow and look at the spawned player
        vCam.Follow = player.transform; 
        vCam.LookAt = player.transform;
    }


}
