using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [SerializeField] private GameObject doorSwitch;
    private Animator anim;
    Collider2D col;
    private bool doorOpened;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        CheckForDoor();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForDoor();
        if(doorOpened)
        {
            col.enabled = false;
            anim.SetBool("isOpen", doorOpened);
                
        }
    }

    private void CheckForDoor()
    {
        doorOpened = doorSwitch.GetComponent<Item>().GetIsOpened();
    }
}
