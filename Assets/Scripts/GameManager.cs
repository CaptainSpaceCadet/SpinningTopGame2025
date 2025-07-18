using FMOD;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class GameManager : MonoBehaviour
{
    // Instances and events
    public static GameManager instance;
    public System.Action OnLevelStart;
    public System.Action OnLevelEnd;
    
    // Fields shown in the inspector
    [Header("Gameplay Settings")]
    [SerializeField] private int totalBalloons = 3;
    
    [Header("Resource UI Elements")]
    [SerializeField] private GameObject balloonContainer;
    [SerializeField] private Sprite balloonFull;
    [SerializeField] private Sprite balloonEmpty;
    
    [Header("Menu Screens")]
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject lossScreen;
    [SerializeField] private Image[] lossScreenBalloonImages;
        
    [Header("Win/Loss Sound Emitters")]
    [SerializeField] private StudioEventEmitter winEmitter;
    [SerializeField] private StudioEventEmitter loseEmitter;
    
    [Header("Next Level")]
    [SerializeField] private string nextScene;
    
    // Private members
    private int currentBalloons = 0;
    private Image[] balloonImages;
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
    }

    private void Start()
    {
        balloonImages = balloonContainer.GetComponentsInChildren<Image>();
    }
    
    // Resource functions
    public void IncreaseBalloons()
    {
        currentBalloons++;
        if (currentBalloons >= totalBalloons) GameWon();
        RenderBalloons(currentBalloons, balloonImages);
    }
    
    // UI functions
    private void RenderBalloons(int balloons, Image[] balloonSprites)
    {
        if (balloons > totalBalloons) return;
        for (var i = 0; i < balloons; i++)
        {
            balloonSprites[i].sprite = balloonFull;
        }
        for (var i = balloons; i < totalBalloons; i++)
        {
            balloonSprites[i].sprite = balloonEmpty;
        }
    }

    // Event functions
    private void GameStarted()
    {   
        lossScreen.SetActive(false);
        winScreen.SetActive(false);
        
        balloonContainer.SetActive(true);
        
        // resources
        currentBalloons = 0;
        RenderBalloons(currentBalloons, balloonImages);
    } 

    private void GameEnded()
    {
        balloonContainer.SetActive(false);
    }

    public void GameOver()
    {
        loseEmitter.Play();
        
        GameEnded();
        lossScreen.SetActive(true);
        winScreen.SetActive(false);
        
        RenderBalloons(currentBalloons, lossScreenBalloonImages);
        
        OnLevelEnd?.Invoke();
        
        Debug.Log("Game Over");
    }

    public void GameWon()
    {
        winEmitter.Play();

        GameEnded();
        lossScreen.SetActive(false);
        winScreen.SetActive(true);
        
        OnLevelEnd?.Invoke();
        
        Debug.Log("Game Won");
    }

    public void ResetLevel()
    {
        GameStarted();
        OnLevelStart?.Invoke();
    }

    public void ProgressLevel()
    {
        SceneManager.LoadScene(nextScene);
    }
}
