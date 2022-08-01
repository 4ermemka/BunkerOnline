using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI categoryDisplay;
    [SerializeField] TextMeshProUGUI attributeNameDisplay;
    [SerializeField] TextMeshProUGUI descriptionDisplay;
    [SerializeField] Image iconDisplay;
    [SerializeField] Color iconColor;

    public void SetInfo(string attributeName, string category, string description, Sprite icon, Color color) 
    {
        this.attributeNameDisplay.text = attributeName; 
        this.categoryDisplay.text = category; 
        this.descriptionDisplay.text = description; 
        this.iconDisplay.sprite = icon; 
        this.iconDisplay.color = color;
    }
}
