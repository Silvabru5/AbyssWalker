using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    public bool IsOpened { get; set; }
    public string ItemID { get; private set; }
    private Animator anim;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ItemID = GlobalHelper.GenerateUniqueID(gameObject);
        Debug.Log(ItemID);
        anim = GetComponent<Animator>();
    }

    public bool GetIsOpened()
    {
        return IsOpened;
    }
    public bool CanInteract()
    {
        return !IsOpened;
    }

    public void Interact()
    {
        if (!CanInteract()) return;
        CollectibleManager.Instance?.AddCollectible("Skull");
        Debug.Log($"collected item");
        TurnOnSwitch();
    }

    private void TurnOnSwitch()
    {
        SetOpened(true);
    }

    private void SetOpened(bool opened)
    {
        if (IsOpened == opened)
        {
            anim.SetBool("isOpen", opened);
        }
        else
        {
            gameObject.SetActive(false); //for collectibles, no anim needed and it object removes from scene
        }
    }
}
