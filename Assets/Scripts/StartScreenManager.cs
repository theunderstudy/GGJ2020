using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StartScreenManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject canvas;

    public delegate void StartGame();
    public static event StartGame GameStartEvent;
    public float fadeSpeed = 0.1f;
    private bool doFade = false;
    void Start()
    {
        GameStartEvent += () => { Debug.Log("Received gamestart event"); };
    }
    // Update is called once per frame
    void Update()
    {
        if (doFade) {
            CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
            canvasGroup.alpha -= fadeSpeed;
            if (canvasGroup.alpha == 0.0f) {
                canvas.SetActive(false);
                // reset to full opacity in case we get recreated
                canvasGroup.alpha = 1.0f;
                Debug.Log("Firing game start event");
                doFade = false;
                GameStartEvent();
            }
        }
    }

    public void startGame() {
        Debug.Log("clicking startgame");
        doFade = true;
    }

    public void quitGame() {
        Application.Quit();
    }
}
