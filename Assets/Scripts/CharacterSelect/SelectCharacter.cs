using UnityEngine;

public class SelectCharacter : MonoBehaviour
{
    public GameObject character1;
    public GameObject character2;

    public GameObject characterTitle1;
    public GameObject characterTitle2;

    public GameObject characterDescription1;
    public GameObject characterDescription2;

    public GameObject characterIcon1;
    public GameObject characterIcon2;







    private int number = 0;

    void Start()
    {
        ShowCharacter();
    }

    public void ChangeCharacter()
    {
        Debug.Log("Button pressed!");  // test line
        number = 1 - number; // toggle between 0 and 1
        ShowCharacter();
    }

    private void ShowCharacter()
    {
        character1.SetActive(number == 0);
        character2.SetActive(number == 1);

        characterDescription1.SetActive(number == 0);
        characterDescription2.SetActive(number == 1);

        characterTitle1.SetActive(number == 0);
        characterTitle2.SetActive(number == 1);

        characterIcon1.SetActive(number == 0);
        characterIcon2.SetActive(number == 1);





    }
}
