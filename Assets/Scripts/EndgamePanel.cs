using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndgamePanel : MonoBehaviour
{
    [SerializeField] GameObject Panel;
    [SerializeField] EndgameInfo endgameInfoPref;

    private List<EndgameInfo> panelsList;

    void Start()
    {
        GetComponent<CanvasGroup>().alpha = 0f;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        panelsList = new List<EndgameInfo>();
    }

    public void Appear()
    {
        LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 1f, 0.5f);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
    
    public void AddUserToEndgame(User user, DeckCardSerializable card)
    {
        EndgameInfo newPanel = panelsList.Find(x=>x.id == user.id);
        if(newPanel == null) 
        {
            newPanel = Instantiate(endgameInfoPref) as EndgameInfo;
            newPanel.transform.SetParent(Panel.transform);
            newPanel.transform.localScale = Vector3.one;

            newPanel.SetId(user.id);
            newPanel.SetName(user.Nickname);

            panelsList.Add(newPanel);
        }

        newPanel.AddAttribute(card);
    }
    
    public void OnClickExit()
    {
        MessageProcessing.Exit();
    }
}