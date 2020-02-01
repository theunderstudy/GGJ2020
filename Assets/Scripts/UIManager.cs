using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class UIManager : Singleton<UIManager>
{
    public CanvasGroup NightFadeGroup;
    public ImageButton NewDayButton;
    public Image EnergyFill;
    private PlayerController Player;
    private void Start()
    {
        Player = PlayerController.Instance;
    }

    public void NightFadeOut(float fadeTime)
    {
        NightFadeGroup.DOFade(1.0f,fadeTime).SetEase(Ease.OutQuad);
    }  
    
    
    public void NightFadeIn(float fadeTime)
    {
        NightFadeGroup.DOFade(0.0f, fadeTime).SetEase(Ease.InQuad);

    }


    public void ShowNewDayButton(bool show)
    {
        NewDayButton.gameObject.SetActive(show);
    }

    public void HighlightButton(int buttonIndex)
    {

    }

    private void Update()
    {
        EnergyFill.fillAmount =(float) Player.Energy / Player.MaxEnergy;
    }
}
