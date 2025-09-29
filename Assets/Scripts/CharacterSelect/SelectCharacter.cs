using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectCharacter : MonoBehaviour
{


    //using enums to make readable and in case we  add characters later
    public enum CharacterType
    {
        Warrior,
        Mage
    }


    //Enum type that is defaulted to warrior / the character the player is currently selecting
    public CharacterType currentCharacter = CharacterType.Warrior;

    //array of characters to be in the scene
    [Header("Character Objects")]
    public GameObject[] characters;


    //below is for UI elements with specific text/images for each character
    [Header("Titles")]
    public GameObject[] titles; 

    [Header("Descriptions")]
    public GameObject[] descriptions;

    [Header("Icons")]
    public GameObject[] icons;

    void Start()
    {
        //on scene start show the character, defaulted to warrior
        ShowCharacter();
    }


    
    public void ChangeCharacter()
    {

        //if a player clicks on the swap button, move to next character
        //
        int nextIndex = ((int)currentCharacter + 1) % characters.Length;

        //update character using the enum
        currentCharacter = (CharacterType)nextIndex;

        //show new character on ui
        ShowCharacter();
    }


    private void ShowCharacter()
    {

        //cast enum value as an integer to loop thru array of characters
        int index = (int)currentCharacter;

        //loop thru all ui elements and only activate the selected character/index
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == index);
            titles[i].SetActive(i == index);
            descriptions[i].SetActive(i == index);
            icons[i].SetActive(i == index);
        }

    }


    //after selecting character, use player prefs to save what the player selected and load the homebase scene with the number representing character
    public void PlayGame()
    {
        PlayerPrefs.SetInt("SelectedCharacter", (int)currentCharacter);
        //PlayerPrefs.Save(); commented out for testing purposes

        SceneManager.LoadScene("HomeBase");
    }
}
