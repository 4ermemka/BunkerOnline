using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameManagerClass;

public class ButtonClick : MonoBehaviour
{

    public GameObject guiTextLink;
    public InputField inputField;

    public void onClick1()
    {
        GameManager_Class gm = new GameManager_Class();

        gm.AddNewPlayer(0, inputField.text);

        guiTextLink.GetComponent<Text>().text = gm.toString();
    }

}
