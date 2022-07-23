using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region UserClass

[Serializable]
public class User : MonoBehaviour
{
    public bool isHost;
    public int id;
    public string name;
    public bool isReady;

    public User()
    {
        isHost = false;
        id = 0;
        name = "PLAYER";
        isReady = false;
    }

    public User(int id, string name)
    {
        this.isHost = false;
        this.id = id;
        this.name = name;
        isReady = false;
    }

    public User(int id, string name, bool host)
    {
        this.isHost = host;
        this.id = id;
        this.name = name;
        isReady = false;
    }

    public void SetName(string name)
    {
        this.name = name;
    }

    public void SetId(int id)
    {
        this.id = id;
    }

    public void ToggleHost(bool host)
    {
        this.isHost = host;
    }
}
#endregion
