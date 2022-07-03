using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameManager;

public class ButtonClick : MonoBehaviour
{
    private int count = 0;
    public GameObject guiTextLink;
    public InputField inputField;

    public void onClick1()
    {
        GameManager_Class gm = new GameManager_Class();

        gm.AddNewPlayer(inputField.text, count);

        guiTextLink.GetComponent<Text>().text = gm.toString();
    }

}
