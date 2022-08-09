using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct BunkerIcon
{
    public string name;
    public Sprite sprite;

    public BunkerIcon(string _name, string _sprite)
    {
        name = _name;
        sprite = Resources.Load<Sprite>(_sprite);
    }
}

public static class IconsList
{
    public static List<BunkerIcon> AllIcons = new List<BunkerIcon>();

    public static Sprite GetIcon(string name)
    {
        return AllIcons.Find(x => x.name == name).sprite;
    }
}

public class IconsListScript : MonoBehaviour
{
    public void Awake()
    {
        IconsList.AllIcons.Add(new BunkerIcon("8Ball", "/Images/8BallWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Aim", "/Images/AimWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Alco", "/Images/AlcoWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Anchor", "/Images/AnchorWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("At", "/Images/AtWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Atom", "/Images/AtomWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("BrokenHeart", "/Images/BrokenHeartWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Bullet", "/Images/BulletWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Butterfly", "/Images/ButterflyWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Camera", "/Images/CameraWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Chemical", "/Images/ChemicalWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Clapperboard", "/Images/ClapperboardWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Cog", "/Images/CogWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Compass", "/Images/CompassWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Controller", "/Images/ControllerWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Dice", "/Images/DiceWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Dollar", "/Images/DollarWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Drug", "/Images/DrugWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Female", "/Images/FemaleWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Globe", "/Images/GlobeWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Headphones", "/Images/HeadphonesWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Heart", "/Images/HeartWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("House", "/Images/HouseWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Illuminati", "/Images/IlluminatiWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Inst", "/Images/InstWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Lightning", "/Images/LightningWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Lock", "/Images/LockWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Male", "/Images/MaleWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Message", "/Images/MessageWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Note", "/Images/NoteWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Paw", "/Images/PawWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Pentagram", "/Images/PentagramWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Phone", "/Images/PhoneWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Plane", "/Images/PlaneWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Playboy", "/Images/PlayboyWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Plus", "/Images/PlusWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Recycle", "/Images/RecycleWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Rifle", "/Images/RifleWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Rocket", "/Images/RocketWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Search", "/Images/SearchWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Skull", "/Images/SkullWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("SoccerBall", "/Images/SoccerBallWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Spades", "/Images/SpadesWhite"));
        IconsList.AllIcons.Add(new BunkerIcon("Tools", "/Images/ToolsWhite"));
    }
}
