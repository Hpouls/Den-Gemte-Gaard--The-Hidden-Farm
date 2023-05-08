using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour {
    public GameObject PauseMenu;
    public GameObject InGameButtonCanvas;
    public TextMeshProUGUI FortsaetText;
    public TextMeshProUGUI AfslutText;
    public TextMeshProUGUI PauseText;

    //Pause Menu functions
    public void PauseGame() {
        PauseMenu.SetActive(true);
        InGameButtonCanvas.SetActive(false);
        Time.timeScale = 0;
        Debug.Log("Game Paused");
    }

    public void ContinueGame() {
        PauseMenu.SetActive(false);
        InGameButtonCanvas.SetActive(true);
        Time.timeScale = 1;
        Debug.Log("Game Continued");
    }

    public void EndGame(){
        SceneManager.LoadScene("MainMenu");
        InGameButtonCanvas.SetActive(true);
        Time.timeScale = 1;
        Debug.Log("Game ended, going back to main menu");
    }

}
