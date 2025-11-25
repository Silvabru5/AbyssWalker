using UnityEngine;
/*
 * Author: Adrian Agius
 * File: OpenDoor.cs
 * Description: this script is attached to switches we wanted interactable to open gates in level 2 in 
 * order to have animations and allow for checks if players in range. 
 * 
 */
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
