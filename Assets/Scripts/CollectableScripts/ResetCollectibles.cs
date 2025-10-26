using UnityEngine;

public class ResetCollectibles : MonoBehaviour
{

    //this  resets collectible counts on scene start
    void Start()
    {
        if (CollectibleManager.Instance != null)
        {
            CollectibleManager.Instance.ResetText();
        }
    }
}
