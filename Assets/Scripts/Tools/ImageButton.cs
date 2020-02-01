using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public abstract class ImageButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{

    public Image DisplayImage;
    public void OnPointerDown(PointerEventData eventData)
    {
        ButtonPushed();
    }
    protected bool bPushed = false;

    protected abstract void ButtonPushed();

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }
}
