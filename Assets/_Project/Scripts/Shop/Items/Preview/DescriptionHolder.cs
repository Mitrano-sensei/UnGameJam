using TMPro;
using UnityEngine;

public class DescriptionHolder : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    
    public void SetDescription(string description) => descriptionText.text = $"Description: \n{description}";
}