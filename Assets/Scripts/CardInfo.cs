using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardInfo : MonoBehaviour
{
    [SerializeField] Text categoryDisplay;
    [SerializeField] Text attributeNameDisplay;
    [SerializeField] Text descriptionDisplay;
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
