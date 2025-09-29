using UnityEngine;
using Unity.Cinemachine;

public class PlayerSpawner : MonoBehaviour
{

    public GameObject warriorPrefab;
    public GameObject magePrefab;
    public CinemachineCamera vCam;
    void Start()
    {
        int selected = PlayerPrefs.GetInt("SelectedCharacter", 0);

        GameObject player;

        if (selected == 0)
        {
            player = Instantiate(warriorPrefab);
        }
        else
        {
            player = Instantiate(magePrefab);
        }


        vCam.Follow = player.transform;
        vCam.LookAt = player.transform;
    }


}
