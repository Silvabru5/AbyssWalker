//using UnityEngine;
//using UnityEngine.SceneManagement;

//[RequireComponent(typeof(Collider2D))]
//public class ReturnToHomebase : MonoBehaviour, IInteractable
//{
//    [SerializeField] private string sceneName = "HomeBase"; // exact scene name in Build Settings
//    [SerializeField] private GameObject interactPrompt;     // small “Press E” UI (world-space or sprite)
//    [SerializeField] private KeyCode interactKey = KeyCode.E;

//    private bool playerInRange = false;

//    private void Reset()
//    {
//        // auto-config collider on add
//        var col = GetComponent<Collider2D>();
//        col.isTrigger = true;
//    }

//    private void OnEnable()
//    {
//        if (interactPrompt != null) interactPrompt.SetActive(false);
//        Debug.Log("[Portal] Enabled; waiting for player.");
//    }

//    private void Update()
//    {
//        if (playerInRange && Input.GetKeyDown(interactKey))
//        {
//            Debug.Log("[Portal] Interact key pressed. Loading scene: " + sceneName);
//            SceneManager.LoadScene(sceneName);
//            // SceneLoader.instance.LoadSpecificLevel(2);
//        }
//    }

//    //private void OnTriggerEnter2D(Collider2D other)
//    //{
//    //    if (!other.CompareTag("Player")) return;

//    //    playerInRange = true;
//    //    if (interactPrompt != null) interactPrompt.SetActive(true);
//    //    Debug.Log("[Portal] Player entered. Showing prompt.");
//    //}

//    //private void OnTriggerExit2D(Collider2D other)
//    //{
//    //    if (!other.CompareTag("Player")) return;

//    //    playerInRange = false;
//    //    if (interactPrompt != null) interactPrompt.SetActive(false);
//    //    Debug.Log("[Portal] Player exited. Hiding prompt.");
//    //}

//    public void Interact()
//    {
//        throw new System.NotImplementedException();
//    }

//    public bool CanInteract()
//    {
//        throw new System.NotImplementedException();
//    }
//}
