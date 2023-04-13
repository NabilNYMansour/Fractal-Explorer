using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
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
    public Material portalMat;

    public GameObject portalExit;
    public GameObject gameStartUI;

    public AudioSource source;
    public AudioClip themeSong;
    public AudioClip coinPickup;
    public AudioClip hitNoise;
    public AudioClip portalSpawn;
    public AudioClip hyperspace;


    private int totalCollected = 0;
    private int health = 100;
    private int maxCoins;
    private GameObject[] allCoins;
    private AudioSource themeSource;

    public static event Action gameStart;

    private void Awake() {
        scoreText.text = "";
        healthText.text = "";
        missionText.text = "Smash the yellow button down to start!";

        source.volume = 0.2f;

        Material[] materialsArray = new Material[2];
        materialsArray[0] = rayMarcherMat;
        materialsArray[1] = blackMat;
        shipScreen.materials = materialsArray;

        // Store all coin references
        allCoins = GameObject.FindGameObjectsWithTag("collectible");
        maxCoins = allCoins.Length;
        portalExit.SetActive(false);

    }

    private void Start() {
        themeSource = this.transform.GetComponent<AudioSource>();
    }

    // Subscribe and unsubscribe to event 
    private void OnEnable() {
        CoinManager.collectEvent += collectEvent;
        ButtonHandler.startGame += startEvent;
        AstroidController.damageEvent += onAstroidHit;
        ExitManager.gameEnd += endEvent;
    }

    private void OnDisable() {
        CoinManager.collectEvent -= collectEvent;
        ButtonHandler.startGame -= startEvent;
        AstroidController.damageEvent -= onAstroidHit;
        ExitManager.gameEnd -= endEvent;

    }

    private void FixedUpdate() {
        GameObject closestCoin = FindClosestCoin();
        // We have traversed all coins so point to the exit gate
        if (closestCoin == null){
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

        gameStartUI.SetActive(false);

        themeSource.clip = themeSong;
        themeSource.Play();
        themeSource.loop = true;

        // Emit game start to all listeners
        gameStart.Invoke();
    }

    void endEvent(){
        Material[] materialsArray = new Material[2];
        materialsArray[0] = rayMarcherMat;
        materialsArray[1] = portalMat;
        shipScreen.materials = materialsArray;
        source.volume = 0.5f;
        source.PlayOneShot(hyperspace);
        StartCoroutine(endGame());
    }

 
    void collectEvent(){
        totalCollected += 1;
        scoreText.text = totalCollected.ToString() + '/' + maxCoins;
        allCoins = GameObject.FindGameObjectsWithTag("collectible");
        if (totalCollected == maxCoins){
            initExitPortal();

        }
        else{
            source.PlayOneShot(coinPickup);
        }
    }

    void initExitPortal(){
        portalExit.SetActive(true);
        missionText.text = "ESCAPE NOW! Follow the yellow arrow to the exit!";
        source.PlayOneShot(portalSpawn);
    }

    void onAstroidHit(){
        health -= 10;
        if (health == 0){
            missionText.text = "You failed. Restarting level.";
            StartCoroutine(levelReset());
            
        }
        else{
            healthText.text = health.ToString() + '%';
            source.PlayOneShot(hitNoise);
        }
    }

    IEnumerator levelReset(){
        themeSource.Stop();
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    } 

       IEnumerator endGame(){
        missionText.text = "Thank you for playing! Restarting level now...";
        yield return new WaitForSeconds(8);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
