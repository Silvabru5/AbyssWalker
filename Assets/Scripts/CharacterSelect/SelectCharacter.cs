using UnityEngine;

public class SelectCharacter : MonoBehaviour
{
    public GameObject character1;
    public GameObject character2;

    private int number = 0;

    void Start()
    {
        ShowCharacter();
    }

    public void ChangeCharacter()
    {
        number = 1 - number; // toggle between 0 and 1
        ShowCharacter();
    }

    private void ShowCharacter()
    {
        character1.SetActive(number == 0);
        character2.SetActive(number == 1);
    }
}
