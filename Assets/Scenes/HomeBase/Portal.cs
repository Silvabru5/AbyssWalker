using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IInteractable
{
    [Header("Portal Settings")]
    [SerializeField] private string targetScene; // The name of the scene to load

    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        // load the target scene
        if (!string.IsNullOrEmpty(targetScene))
        {
            SceneManager.LoadScene(targetScene);
        }
        else
        {
            Debug.LogWarning("no scene bru");
        }
    }
}
