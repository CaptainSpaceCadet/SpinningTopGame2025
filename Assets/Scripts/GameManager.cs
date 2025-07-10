using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public System.Action OnLevelEnd;
    
    private List<IResettable> resettables = new List<IResettable>();
    
    [SerializeField] private int totalLives = 3;
    [SerializeField] private int totalBalloons = 3;
    
    private int currentLives = 3;
    private int currentBalloons = 0;

    [SerializeField] private GameObject lifeContainer;
    [SerializeField] private Image[] heartSprites; // could get this from above gameObject
    [SerializeField] private Sprite heartFull;
    [SerializeField] private Sprite heartEmpty;
    
    [SerializeField] private GameObject balloonContainer;
    [SerializeField] private Image[] balloonSprites; // could get this from above gameobject
    [SerializeField] private Sprite balloonFull;
    [SerializeField] private Sprite balloonEmpty;
    
    // Win, Loss
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject lossScreen;
    [SerializeField] private Image[] lossBalloonSprites;
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
    }

    public void Register(IResettable resettable)
    {
        if (!resettables.Contains(resettable))
        {
            resettables.Add(resettable);
            resettable.ResetState();
        }
    }

    public void ResetLevel()
    {
        foreach (var obj in resettables)
        {
            obj.ResetState();
        }
    }
    
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
        RenderBalloons(currentBalloons, balloonSprites);
    }
    
    private void RenderLives(int lives)
    {
        Debug.Log(lives);
        if (lives > totalLives) return;
        for (int i = 0; i < lives; i++)
        {
            heartSprites[i].sprite = heartFull;
        }
        for (int i = lives; i < totalLives; i++)
        {
            heartSprites[i].sprite = heartEmpty;
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

    public void GameOver()
    {
        lifeContainer.SetActive(false);
        balloonContainer.SetActive(false);
        
        lossScreen.SetActive(true);
        winScreen.SetActive(false);
        
        RenderBalloons(currentBalloons, lossBalloonSprites);
        
        OnLevelEnd?.Invoke();
        
        Debug.Log("Game Over");
    }

    public void GameWon()
    {
        lifeContainer.SetActive(false);
        balloonContainer.SetActive(false);
        
        lossScreen.SetActive(false);
        winScreen.SetActive(true);
        
        OnLevelEnd?.Invoke();
        
        Debug.Log("Game Won");
    }
}
