using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region UserClass

[Serializable]
public class User
{
    public bool isHost;
    public int id;
    public string Nickname;
    public bool isReady;
    public int votesFor;

    public User()
    {
        isHost = false;
        id = 0;
        Nickname = "PLAYER";
        isReady = false;

        votesFor = 0;
    }

    public User(int id, string Nickname)
    {
        this.isHost = false;
        this.id = id;
        this.Nickname = Nickname;
        isReady = false;
    }

    public User(int id, string Nickname, bool host)
    {
        this.isHost = host;
        this.id = id;
        this.Nickname = Nickname;
        isReady = false;
    }

    public void SetNickname(string Nickname)
    {
        this.Nickname = Nickname;
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
