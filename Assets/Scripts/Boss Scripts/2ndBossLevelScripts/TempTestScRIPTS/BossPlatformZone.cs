//using UnityEngine;

//public class BossPlatformZone : MonoBehaviour
//{
//    public bool playerInZone = false;

//    void OnTriggerEnter2D(Collider2D other)
//    {
//        if (other.GetComponent<PlayerSSBoss2>() != null)
//            playerInZone = true;
//    }

//    void OnTriggerExit2D(Collider2D other)
//    {
//        if (other.GetComponent<PlayerSSBoss2>() != null)
//            playerInZone = false;
//    }
//}
using UnityEngine;

public class BossPlatformZone : MonoBehaviour
{
    public bool playerInZone;
    public bool bossInZone;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerSSBoss2>() != null) playerInZone = true;
        if (other.GetComponent<BossControllerHybrid>() != null) bossInZone = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerSSBoss2>() != null) playerInZone = false;
        if (other.GetComponent<BossControllerHybrid>() != null) bossInZone = false;
    }
}
