using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour {
    //Main Menu functions

    public void ExitButton() {
        Application.Quit(); // closes the applicatoin when "ExitButton" is run
        Debug.Log("Game Closed");
    }

    public void StartGame() {
        SceneManager.LoadScene("DenGemteGaard"); // Switches the scene from the star scene "MainMenu" to the game scene "DenGemteGaard"
        Debug.Log("Game Started");
    }

    public void RestartGame() {
        SceneManager.LoadScene("MainMenu"); // Switches the scene from game scene "DenGemteGaard" to start scene "MainMenu"
        Debug.Log("Going back to main menus");
    }
}
