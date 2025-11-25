using UnityEngine;
using UnityEngine.SceneManagement;

/*
author: Tristan Ung
file: Portal.cs
description: This script handles the functionality of a portal
*/
public class Portal : MonoBehaviour, IInteractable
{
    [Header("Portal Settings")]
    [SerializeField] private int targetScene; // The name of the scene to load

    // Check if the portal can be interacted with
    public bool CanInteract()
    {
        return true;
    }

    // Handle interaction with the portal
    public void Interact()
    {
        if (!CanInteract()) return;
        SceneLoader.instance.LoadSpecificLevel(targetScene);
    }
}
