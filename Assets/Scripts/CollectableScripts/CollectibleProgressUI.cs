using TMPro;
using UnityEngine;

public class CollectibleProgressUI : MonoBehaviour
{
    public string collectibleType = "Skull"; 
    public int requiredAmount = 5;

    private TextMeshProUGUI label;

    void Awake()
    {
        // auto-grab the TMP component on the same object
        label = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (CollectibleManager.Instance == null || label == null) return;

        int count = CollectibleManager.Instance.GetCount(collectibleType);
        label.text = $"Collectables gathered {count}/{requiredAmount}"
                   + (count >= requiredAmount ? " Boss Arena Open" : "");
    }
}
