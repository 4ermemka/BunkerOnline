using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.EventSystems;

[Serializable]
public class PlayerInfo : MonoBehaviour, IPointerClickHandler
{
    private User user;
    private GameManager gm;
    [SerializeField] private KickConfirm kickPanelPref;
    [SerializeField] private CanvasGroup selectedCircle;
    [SerializeField] private Image avatar;
    [SerializeField] private CircleLayoutGroup attributePanel;
    [SerializeField] private TextMeshProUGUI nicknameText;
    [SerializeField] private TextMeshProUGUI votes;

    private List<Attribute> attributesList;

    private string Nickname = "Nickname";

    public User GetUser()
    {
        return user;
    }

    public void SetUser(User us)
    {
        this.user = us;
    }

    public void SetNickname (string Nickname)
    {
        this.Nickname = Nickname;
    }

    public void SetCards(string[] cards) {
    }

    public void Start() 
    {
        gm = FindObjectOfType<GameManager>();
        attributesList = attributePanel.GetComponentsInChildren<Attribute>().ToList();
        DeselectPlayer();
    }

    public void Update()
    {
        nicknameText.text = Nickname;
        if(gm.currentStage == CurrentStage.Voting) votes.GetComponent<CanvasGroup>().alpha = 1;
        else votes.GetComponent<CanvasGroup>().alpha = 0;
        
        votes.text = user.votesFor.ToString();
        votes.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = user.votesFor.ToString();
    }

    public void SelectPlayer()
    {
        LeanTween.alphaCanvas(selectedCircle, 1f, 0.3f);
    }

    public void DeselectPlayer()
    {
        LeanTween.alphaCanvas(selectedCircle, 0f, 0.3f);
    }

    public void AddAttribute(Attribute attribute)
    {
        attribute.gameObject.GetComponent<CanvasGroup>().alpha=0;
        attribute.transform.SetParent(attributePanel.transform);
        attribute.transform.localScale = new Vector3(1,1,1);
        attribute.transform.localPosition = new Vector3(0,0,20);
        LeanTween.alphaCanvas(attribute.gameObject.GetComponent<CanvasGroup>(),1, 0.8f);
    }

    public Attribute FindAttribute(int id)
    {
        Attribute attribute = attributesList.Find(x => x.GetComponent<Attribute>().GetId() == id);
        return attribute;
    }

    public void DeleteAttribute(int id) 
    {
        Attribute deletedAttribute = FindAttribute(id);
        if (deletedAttribute!=null) Destroy(deletedAttribute.gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        KickConfirm panel = FindObjectOfType<KickConfirm>();
        if(panel == null && gm.IsMyVoteTurn() && gm.user.id != this.user.id)
        {
            panel = Instantiate(kickPanelPref) as KickConfirm;
            panel.transform.SetParent(FindObjectOfType<Canvas>().transform);
            panel.SetUser(user);
        }
    }
}
