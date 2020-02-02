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
        GameStartEvent += this.startTutorial;
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

    private void startTutorial() {
        Subtitle_Manager.Instance.SendDialouge(Color.white, " ", "Hello! This is [ROBOTNAME].", 2);
        Subtitle_Manager.Instance.SendDialouge(Color.white, " ", "They like to fix things.", 2);
        Subtitle_Manager.Instance.SendDialouge(Color.white, " ", "Say, that windmill over there looks to be in bad shape.", 3);
        Subtitle_Manager.Instance.SendDialouge(Color.white, " ", "Press [BUTTON] to interact with the windmill.", 4);
        Subtitle_Manager.Instance.SendDialouge(Color.white, " ", "Why not head over?", 2);
        Subtitle_Manager.Instance.SendDialouge(Color.white, " ", "Use the [W][↑], [A][←], [S][↓], and [D][→] keys to move around.", 5);
        Subtitle_Manager.Instance.SendDialouge(Color.white, " ", "Clean and repair items on more tiles to clear the dust.", 3);
        Subtitle_Manager.Instance.SendDialouge(Color.white, " ", "As you progress, you’ll also gain access to saplings and sprinkler systems that can help keep the dust at bay.", 5);
    }
}
