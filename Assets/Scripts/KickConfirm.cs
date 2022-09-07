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

        LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 1f, 0.3f);
        transform.localScale = Vector3.one;
        gm = FindObjectOfType<GameManager>();
        message.text = "Проголосовать за игрока ";
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
        Destroy(gameObject);
    }

    public void OnClickYes() 
    {
        gm.MyVoteFor(votingForUser);
        Destroy(gameObject);
    }
}
