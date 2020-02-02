using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class EndScreenManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject canvas;

    public delegate void StartGame();
    public static event StartGame GameStartEvent;
    public float fadeSpeed = 0.1f;
    private bool doFade = false;
    void Start()
    {
        
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
                string currentSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(currentSceneName);
            }
        }
    }

    public void startGame() {
        Debug.Log("clicking restartgame");
        doFade = true;
    }

    public void quitGame() {
        Application.Quit();
    }
}
