using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Attribute : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected int id = 0;
    [SerializeField] protected string category;
    [SerializeField] protected string attributeName;
    [SerializeField] protected string description;
    [SerializeField] protected Sprite Sprite;
    [SerializeField] protected Color Color;

    [SerializeField] protected Image icon;

    [SerializeField] private CardInfo cardInfoPref;

    protected Canvas mainCanvas;
    private CardInfo currCardInfo;

    protected Camera cam;

    void Start()
    {
        cam = Camera.allCameras[0];
        mainCanvas = FindObjectOfType<Canvas>();
    }

    void Update()
    {
        UpdateIcon();
        UpdateColor();
        if(currCardInfo) 
        {
            Vector3 newPos = cam.ScreenToWorldPoint(Input.mousePosition);
            newPos.z = 0;
            UpdateInfoPanelPosition(cam.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    #region GetFields
    public int GetId() 
    {
        return id;
    }

    public Sprite GetIcon() 
    {
        return Sprite;
    }

    public Color GetColor() 
    {
        return Color;
    }

    public string GetDescription() 
    {
        return description;
    }

    public string GetCategory() 
    {
        return category;
    }

    public string GetAttributeName() 
    {
        return attributeName;
    }

    #endregion

    #region SetFields
    public void SetId(int newId) 
    {
        id = newId;
    }

    public void SetDescription(string descr)
    {
        description = descr;
    }

    public void SetCategory (string cat) 
    {
        category = cat;
    }

    public void SetAttributeName(string name) 
    {
        attributeName = name;
    }

    public void SetIcon(Sprite newIcon)
    {
        Sprite = newIcon;
    }

    public void SetColor(Color newCol) 
    {
        Color = newCol;
    }

    public void UpdateIcon() 
    {
        icon.sprite = Sprite;
    }

    public void UpdateColor() 
    {
        icon.color = Color;
    }

    #endregion

    #region PointerBehaviour
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!currCardInfo) 
        {
            currCardInfo = Instantiate(cardInfoPref) as CardInfo;
            currCardInfo.transform.SetParent(mainCanvas.transform);
            currCardInfo.transform.localScale = new Vector3(1,1,1);
            currCardInfo.SetInfo(attributeName, category, description, Sprite, Color);
        }

        Vector3 newPos = cam.ScreenToWorldPoint(eventData.position);
        UpdateInfoPanelPosition(newPos);
    }

    public void UpdateInfoPanelPosition(Vector3 pos)
    {
        if(currCardInfo)
        {
            float offsetX = currCardInfo.GetComponent<RectTransform>().rect.width/2;
            float offsetY = currCardInfo.GetComponent<RectTransform>().rect.height/2;

            Vector3 offset = new Vector3(offsetX, offsetY, 0);
            Vector3 screen = cam.ScreenToWorldPoint(new Vector3(Screen.width-offsetX*2, Screen.height-offsetY*2,0));
            pos.z = 0;

            if(pos.x>screen.x) offset.x *=-1;
            if(pos.y>screen.y) offset.y *=-1;

            currCardInfo.gameObject.transform.position = pos;
            currCardInfo.gameObject.transform.localPosition += offset;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(currCardInfo) Destroy(currCardInfo.gameObject);
    }
    #endregion
}