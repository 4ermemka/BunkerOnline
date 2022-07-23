using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Attribute : MonoBehaviour
{
    private int id = 0;
    [SerializeField] private Sprite icon;
    [SerializeField] private Color color;

    public int GetId() 
    {
        return id;
    }

    public void SetId(int newId) 
    {
        id = newId;
    }

    void Start()
    {
        icon = GetComponent<Image>().sprite;
        color = GetComponent<Image>().color;
    }

    void Update()
    {
        
    }

    public void UpdateIcon(Sprite newIcon)
    {
       icon = newIcon;
    }

    public void UpdateColor(Color newCol) 
    {
        color = newCol;
    }
}
