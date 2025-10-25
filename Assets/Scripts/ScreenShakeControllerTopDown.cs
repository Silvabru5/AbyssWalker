using Unity.Cinemachine;
using UnityEngine;

public class ScreenShakeControllerTopDown : MonoBehaviour
{
    public static ScreenShakeControllerTopDown instance;
    private CinemachineImpulseSource source;
    private GameObject player;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        source = player.GetComponent<CinemachineImpulseSource>();
    }
    private void Update()
    {
        
    }


    public void StartShake()
    {
        source.GenerateImpulseWithForce(0.2f);
    }
}
