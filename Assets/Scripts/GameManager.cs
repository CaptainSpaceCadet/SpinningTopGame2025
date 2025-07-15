using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Instances and events
    public static GameManager instance;
    public System.Action OnLevelStart;
    public System.Action OnLevelEnd;
    
    // Fields shown in the inspector
    [Header("Gameplay Settings")]
    [SerializeField] private int totalLives = 3;
    [SerializeField] private int totalBalloons = 3;
    
    [Header("Resource UI Elements")]
    [SerializeField] private GameObject heartContainer;
    [SerializeField] private Sprite heartFull;
    [SerializeField] private Sprite heartEmpty;
    
    [SerializeField] private GameObject balloonContainer;
    [SerializeField] private Sprite balloonFull;
    [SerializeField] private Sprite balloonEmpty;
    
    [Header("Menu Screens")]
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject lossScreen;
    [SerializeField] private Image[] lossScreenBalloonImages;
    
    // Private members
    private int currentLives = 3;
    private int currentBalloons = 0;
    
    private Image[] heartImages;
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
        heartImages = heartContainer.GetComponentsInChildren<Image>();
        balloonImages = balloonContainer.GetComponentsInChildren<Image>();
    }
    
    // Resource functions
    public void DecreaseLives()
    {
        currentLives--;
        if (currentLives <= 0) GameOver();
        RenderLives(currentLives);
    }

    public void IncreaseBalloons()
    {
        currentBalloons++;
        if (currentBalloons >= totalBalloons) GameWon();
        RenderBalloons(currentBalloons, balloonImages);
    }
    
    // UI functions
    private void RenderLives(int lives)
    {
        Debug.Log(lives);
        if (lives > totalLives) return;
        for (int i = 0; i < lives; i++)
        {
            heartImages[i].sprite = heartFull;
        }
        for (int i = lives; i < totalLives; i++)
        {
            heartImages[i].sprite = heartEmpty;
        }
    }
    
    private void RenderBalloons(int balloons, Image[] balloonSprites)
    {
        if (balloons > totalLives) return;
        for (var i = 0; i < balloons; i++)
        {
            balloonSprites[i].sprite = balloonFull;
        }
        for (var i = balloons; i < totalLives; i++)
        {
            balloonSprites[i].sprite = balloonEmpty;
        }
    }

    // Event functions
    private void GameStarted()
    {
        lossScreen.SetActive(false);
        winScreen.SetActive(false);
        
        heartContainer.SetActive(true);
        balloonContainer.SetActive(true);
        
        // resources
        currentLives = totalLives;
        RenderLives(currentLives);
        currentBalloons = 0;
        RenderBalloons(currentBalloons, balloonImages);
    } 

    private void GameEnded()
    {
        heartContainer.SetActive(false);
        balloonContainer.SetActive(false);
    }

    public void GameOver()
    {
        GameEnded();
        lossScreen.SetActive(true);
        winScreen.SetActive(false);
        
        RenderBalloons(currentBalloons, lossScreenBalloonImages);
        
        OnLevelEnd?.Invoke();
        
        Debug.Log("Game Over");
    }

    public void GameWon()
    {
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
}
