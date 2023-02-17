using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleDirector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClickStart()
    {
        SceneManager.LoadScene("0_GameStart");
    }
    public void OnClickTutorial()
    {
        SceneManager.LoadScene("How to Play");
    }
    public void OnClickTutorialExit()
    {
        SceneManager.LoadScene("TitleScene");
    }
    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif
    }
}
