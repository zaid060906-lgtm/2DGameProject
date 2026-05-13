using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuseMenu : MonoBehaviour
{
   public GameObject Contenr;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Contenr.SetActive(true);
            Time.timeScale = 0f;       
        }
    }
    public void Resume()
    {
        Contenr.SetActive(false);
        Time.timeScale = 1f;
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void Puse()
    {
        Contenr.SetActive(true);
        Time.timeScale = 0f;
    }
}
