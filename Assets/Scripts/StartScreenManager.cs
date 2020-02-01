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
    void Start()
    {
        GameStartEvent += () => { Debug.Log("Received gamestart event"); };
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void startGame() {
        canvas.SetActive(false);
        Debug.Log("Firing game start event");
        GameStartEvent();
    }

    public void quitGame() {
        Application.Quit();
    }
}
