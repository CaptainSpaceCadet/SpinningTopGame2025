using FMOD;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class GameManager : MonoBehaviour
{
    public System.Action OnLevelEnd;
    
    public static GameManager instance;
    
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
    
    // SoundEmitter
    [SerializeField] private StudioEventEmitter winEmitter;
    [SerializeField] private StudioEventEmitter loseEmitter;
    
    private void Awake()
    {
        SetEmitters();
        
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
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
        loseEmitter.Play();
        
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
        winEmitter.Play();
        
        lifeContainer.SetActive(false);
        balloonContainer.SetActive(false);
        
        lossScreen.SetActive(false);
        winScreen.SetActive(true);
        
        OnLevelEnd?.Invoke();
        
        Debug.Log("Game Won");
    }

    /// <summary>
    /// Set FMOD Studio Emitter for the game manager.
    /// </summary>
    public void SetEmitters()
    {
        winEmitter = gameObject.AddComponent<StudioEventEmitter>();
        EventReference winEvent = new EventReference();
        winEvent.Path = "event:/GameClear";
        winEvent.Guid = GUID.Parse("{5ddf43d3-d831-49c6-a9f4-8f3724c879a6}");
        winEmitter.EventReference = winEvent;
        
        loseEmitter = gameObject.AddComponent<StudioEventEmitter>();
        EventReference loseEvent = new EventReference();
        loseEvent.Path = "event:/GameOver";
        loseEvent.Guid = GUID.Parse("{9d2a53d8-5e5c-42ff-b7a0-d78c481b8c1d}");
        loseEmitter.EventReference = loseEvent;
    }
}
