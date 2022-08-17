using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct BunkerIcon
{
    public string name;
    public Sprite sprite;

    public BunkerIcon(string _name, Sprite _sprite)
    {
        name = _name;
        sprite = _sprite;
    }
}

public static class IconsList
{
    public static List<BunkerIcon> AllIcons = new List<BunkerIcon>();

    public static Sprite GetIcon(string name)
    {
        return AllIcons.Find(x => x.name == name+"White").sprite;
    }
}

public class IconsListScript : MonoBehaviour
{
    public void Awake()
    {
        List<Sprite> sprites = Resources.LoadAll("Images", typeof(Sprite)).Cast<Sprite>().ToList();
        Debug.Log(sprites.Count);
        foreach(Sprite s in sprites) 
        {
            IconsList.AllIcons.Add(new BunkerIcon(s.name, s));    
        }
    }
}
