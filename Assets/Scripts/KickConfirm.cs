using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KickConfirm : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI message;
    private User votingForUser;
    private GameManager gm;
    
    void Start()
    {
        GetComponent<CanvasGroup>().alpha = 0;
        GetComponent<CanvasGroup>().blocksRaycasts = false;

        gm = FindObjectOfType<GameManager>();
        message.text = "Проголосовать за игрока ";
    }

    public void Appear()
    {
        LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 1, 0.5f);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void Disapear()
    {
        LeanTween.alphaCanvas( GetComponent<CanvasGroup>(), 0, 0.5f);
         GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void SetUser(User user)
    {
        this.votingForUser = user;
        SetNickname(user.Nickname);
    }

    public void SetNickname(string nickname)
    {
        message.text += nickname + "?";
    }

    public void OnClickNo()
    {
        Disapear();
    }

    public void OnClickYes() 
    {
        gm.MyVoteFor(votingForUser);
        Disapear();;
    }
}
