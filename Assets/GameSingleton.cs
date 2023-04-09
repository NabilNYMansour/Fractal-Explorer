using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameSingleton : MonoBehaviour
{
    public TMP_Text scoreText;
    public Renderer shipScreen;
    public Material rayMarcherMat;
    public Material blackMat;
    private int totalCollected;

    private void Awake() {
        scoreText.text = "Smash the button to start!";
        Material[] materialsArray = new Material[2];
        materialsArray[0] = rayMarcherMat;
        materialsArray[1] = blackMat;
        shipScreen.materials = materialsArray;
    }

    // Subscribe and unsubscribe to event 
    private void OnEnable() {
        CoinManager.collectEvent += collectEvent;
        ButtonHandler.startGame += startEvent;
    }

    private void OnDisable() {
        CoinManager.collectEvent += collectEvent;
        ButtonHandler.startGame += startEvent;

    }
    void startEvent(){
        scoreText.text = "0";
        Material[] materialsArray = new Material[1];
        materialsArray[0] = rayMarcherMat;
        
        shipScreen.materials = materialsArray;
    }

    void collectEvent(){
        Debug.Log("collected coin");
        totalCollected += 1;
        scoreText.text = totalCollected.ToString();
    }
}
