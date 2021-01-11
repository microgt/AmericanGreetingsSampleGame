using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    Transform player;
    int score;
    AudioSource aSource;
    public AudioClip scoreSFX;
    float defaultPitchtimer = 2;
    float pitchTimer;
    public Text scoreText;
    public Image healthBar;
    public Image hurtGraphic;
    bool isGameOver = false;
    public GameObject startScreen;
    public Transform[] spawnners;
    int currentSpawnner;
    float defaultSpawntimer = 1;
    float spawntimer;
    public Text gameOverScreen;
    public GameObject enemy;
    void Awake()
    {
        if(instance == null){
            instance = this;
        }else{
            Destroy(this);
        }
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        aSource = gameObject.AddComponent<AudioSource>();
        aSource.playOnAwake = false;
        scoreText.text = "";
        healthBar.fillAmount = 1;
        spawntimer = defaultSpawntimer;
    }

    void Update(){

        if(Input.GetKeyDown(KeyCode.Escape)){
            Application.Quit();
        }

        pitchTimer -= Time.deltaTime;
        if(pitchTimer <= 0){
            aSource.pitch = 1;
        }
        if(startScreen.activeSelf && Input.anyKeyDown){
            startScreen.SetActive(false);
                foreach(Transform t in spawnners){
                    t.gameObject.SetActive(true);
            }
        }

        if(isGameOver) return;
        spawntimer -= Time.deltaTime;
        if(spawntimer <= 0){
            Instantiate(enemy, spawnners[Random.Range(0,spawnners.Length)].position, Quaternion.identity);
            spawntimer = defaultSpawntimer;
        }
    }
    public Transform getPlayer(){
        return player;
    }
    public void increaseScore(int increment){
        score += increment;
        playSFX(scoreSFX);
        scoreText.text = "Score: " + score;
        scoreText.GetComponent<Animator>().Play("Scored", 0);
    }

    void playSFX(AudioClip clip){
        aSource.clip = clip;
        aSource.pitch += 0.1f;
        pitchTimer = defaultPitchtimer;
        aSource.Play();
    }
    public void updateHealthbar(float newHealth){
        if(isGameOver) return;
        healthBar.fillAmount = newHealth;
        hurtGraphic.GetComponent<Animator>().Play("hurt", 0);
    }
    public void gameOver(){
        isGameOver = true;
        foreach(Transform t in spawnners){
            t.gameObject.SetActive(false);
        }
        scoreText.text = "";
        float savedScore = PlayerPrefs.GetFloat("high");
        if(savedScore < score){
            PlayerPrefs.SetFloat("high", score);
            gameOverScreen.text = "Game Over \n New High Score! \n" + score + "\n Restarting in \n 3 Seconds";
        }else{
            gameOverScreen.text = "Game Over! \n" + score + "\n Restarting in \n 3 Seconds";
        }

        StartCoroutine(resetGame());
    }
    IEnumerator resetGame(){
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public bool getGameOver(){
        return isGameOver;
    }
}
