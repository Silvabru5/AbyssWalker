using UnityEngine;

/*
 * Author: Adrian Agius
 * File: GlobalHelper.cs
 * Description: This script is designed to help label items we use for collectibles. Ensures we do not interact with the same one twice if used.
 */
public static class GlobalHelper
{
    // Creating global helper so that we can access ID's for the interactable objects. 
    public static string GenerateUniqueID(GameObject obj)
    {
        // returns a string that gives the objects name and x + y positions as Unique ID: ex: Switch_3_4
        return $"{obj.name}_{obj.transform.position.x}_{obj.transform.position.y} ";
    }
}
