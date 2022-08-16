using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotatingHitbox : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public AnimationCurve curve;
    [SerializeField] private float rotateFactor = 5;
    [SerializeField] private float scaleFactor = 1.1f;
    [SerializeField] private bool changeScale = false;
    private GameObject hitbox;
    
    private bool rotating = false;
    void Start()
    {
        hitbox = gameObject.transform.GetChild(0).gameObject;
    }

    void Update()
    {
        if(rotating) UpdateRotation(Camera.allCameras[0].ScreenToWorldPoint(Input.mousePosition));
    }

    public void UpdateRotation(Vector3 pos)
    {
        float rX, rY;
        pos.z = 0;
        Vector3 rectPos = hitbox.transform.position;
        rY = rectPos.x - pos.x;
        rX = pos.y - rectPos.y;
        //Debug.Log("C:" + pos.x + "," + pos.y + " P:" + rectPos.x + "," + rectPos.y);
        Vector3 newRot = new Vector3(rX,rY,0);
        float radius = Mathf.Max(hitbox.GetComponent<RectTransform>().rect.height, hitbox.GetComponent<RectTransform>().rect.width)/2;
        rotateFactor = radius/140;
        hitbox.transform.rotation = Quaternion.Euler(newRot/rotateFactor);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rotating = true;
        if(changeScale)LeanTween.scale(hitbox, new Vector3(scaleFactor,scaleFactor,scaleFactor),0.5f).setEase(curve);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rotating = false;
        hitbox.transform.rotation = Quaternion.Euler(Vector3.zero);
        if(changeScale)LeanTween.scale(hitbox, new Vector3(1,1,1),0.5f).setEase(curve);
    }
}
