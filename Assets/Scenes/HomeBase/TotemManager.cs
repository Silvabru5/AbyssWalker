using UnityEngine;

public class TotemManager : MonoBehaviour
{
    public int totemsToDestroy = 2;
    private int destroyedCount = 0;

    public GameObject bossPortal; 

    private void OnEnable()
    {
        Totem.OnTotemDestroyed += HandleTotemDestroyed;
    }

    private void OnDisable()
    {
        Totem.OnTotemDestroyed -= HandleTotemDestroyed;
    }

    private void HandleTotemDestroyed()
    {
        destroyedCount++;

        if (destroyedCount >= totemsToDestroy)
        {
            ActivatePortal();
        }
    }

    private void ActivatePortal()
    {
        if (bossPortal != null)
        {
            bossPortal.SetActive(true);
        }
    }
}
