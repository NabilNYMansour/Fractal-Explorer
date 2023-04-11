using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameSingleton : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text missionText;
    public TMP_Text healthText;
    public Renderer shipScreen;
    public Transform arrowNav;
    public Transform ship;
    public Material rayMarcherMat;
    public Material blackMat;
    public GameObject portalExit;
    private int totalCollected = 0;
    private int health = 100;
    private int maxCoins;
    private GameObject[] allCoins;

    private void Awake() {
        scoreText.text = "";
        healthText.text = "";
        missionText.text = "Smash the yellow button down to start!";

        Material[] materialsArray = new Material[2];
        materialsArray[0] = rayMarcherMat;
        materialsArray[1] = blackMat;
        shipScreen.materials = materialsArray;

        // Store all coin references
        allCoins = GameObject.FindGameObjectsWithTag("collectible");
        maxCoins = allCoins.Length;
        portalExit.SetActive(false);

    }

    // Subscribe and unsubscribe to event 
    private void OnEnable() {
        CoinManager.collectEvent += collectEvent;
        ButtonHandler.startGame += startEvent;
        AstroidController.damageEvent += onAstroidHit;
    }

    private void OnDisable() {
        CoinManager.collectEvent -= collectEvent;
        ButtonHandler.startGame -= startEvent;
        AstroidController.damageEvent -= onAstroidHit;

    }

    private void FixedUpdate() {
        GameObject closestCoin = FindClosestCoin();
        // We have traversed all coins so point to the exit gate
        if (closestCoin == null){
            arrowNav.gameObject.SetActive(false);
            arrowNav.rotation = Quaternion.Slerp(arrowNav.rotation, Quaternion.LookRotation(portalExit.transform.position - arrowNav.position), 3f * Time.deltaTime);
        }
        else{
            if (!arrowNav.gameObject.activeSelf){
                arrowNav.gameObject.SetActive(true);
            }
            arrowNav.rotation = Quaternion.Slerp(arrowNav.rotation, Quaternion.LookRotation(closestCoin.transform.position-arrowNav.position), 3f * Time.deltaTime);
        }
     }

    // From unity docs
    public GameObject FindClosestCoin(){
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = ship.position;

        foreach (GameObject coin in allCoins)
        {
            if (coin){
                Vector3 diff = coin.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = coin;
                    distance = curDistance;
                }
            }

        }
        return closest;
    }

    void startEvent(){
        scoreText.text = "0";
        healthText.text = "100%";
        missionText.text = "Collect all coins and avoid the astroids!";
        Material[] materialsArray = new Material[1];
        materialsArray[0] = rayMarcherMat;
        
        shipScreen.materials = materialsArray;

        // Init start arrow
        
    }

    void collectEvent(){
        Debug.Log("collected coin");
        totalCollected += 1;
        scoreText.text = totalCollected.ToString() + '/' + maxCoins;
        allCoins = GameObject.FindGameObjectsWithTag("collectible");
        if (totalCollected == maxCoins){
            initExitPortal();
        }
    }

    void initExitPortal(){
        portalExit.SetActive(true);
        missionText.text = "Follow the yellow arrow to the exit!";
    }

    void onAstroidHit(){
        health -= 10;
        healthText.text = health.ToString() + '%';
    }
}
