using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StartScreenManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject canvas;
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void startGame() {
        Debug.Log("CLAEK BUOTOMN");
        canvas.SetActive(false);
    }

    public void quitGame() {
        Application.Quit();
    }
}
