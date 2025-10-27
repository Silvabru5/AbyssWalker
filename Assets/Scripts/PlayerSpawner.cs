using UnityEngine;
using Unity.Cinemachine;

public class PlayerSpawner : MonoBehaviour
{

    public GameObject warriorPrefab;
    public GameObject magePrefab;
    public CinemachineCamera vCam;
    private int selected = 0;
    void Start()
    {
        selected = PlayerPrefs.GetInt("SelectedCharacter", 0);

        GameObject player;

        if (selected == 0)
        {
            player = Instantiate(warriorPrefab);
        }
        else if (selected == 1)
        {
            player = Instantiate(magePrefab);
        }
        else
        {
            player = Instantiate(warriorPrefab); // Default to warrior if invalid selection
        }


        vCam.Follow = player.transform;
        vCam.LookAt = player.transform;
    }


}
