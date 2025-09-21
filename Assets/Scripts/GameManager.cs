using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerController player;
    public UIManager UIManager;
    public LevelManager LevelManager;

    //State
    public IGameState currentState;

    // Stats
    public int TotalEnemiesKilled = 0;
    public int CurrentLevelIndex = 0;
    public bool isPlay = true;

    private void Awake()
    {
        setupAppSettings();
        Instance = this;
    }

    private void Start()
    {
        player = PlayerController.instance;
        LoadGame();
        SwitchState(new MainMenuState());
    }

    public void playLevel()
    {
        LevelManager.play(CurrentLevelIndex);
    }


    public void startGame()
    {
        isPlay = true;
    }
    
    public void resetButtonClick()
    {
        isPlay = true;
        player.resetPlayer();
    }

    public void nextLevelButtonClick()
    {
        isPlay = true;
        CurrentLevelIndex++;
        SaveGame();
    }

    public void resetProgress()
    {
        CurrentLevelIndex = 0;
        TotalEnemiesKilled = 0;
        player.resetPlayer();
        SaveGame();
    }

    public void alienKilled(AlienController alien)
    {
        LevelManager.aliens.Remove(alien);
        TotalEnemiesKilled++;
    }


    private void Update()
    {
        currentState?.UpdateState(this);
    }

    public void SwitchState(IGameState newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }


    public void LoadGame()
    {
        // use playerPrefs to load level and killCount and recordKillCount
        CurrentLevelIndex = PlayerPrefs.GetInt("Level", 0);
        TotalEnemiesKilled = PlayerPrefs.GetInt("KillCount", 0);
        
        // use playerPrefs to load player level and xp
        player.level = PlayerPrefs.GetInt("PlayerLevel", 0);
        player.exp = PlayerPrefs.GetInt("PlayerXp", 0);
        player.health = PlayerPrefs.GetFloat("PlayerHp", player.startingHealth);
        player.updatePlayerLevel();
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("Level", CurrentLevelIndex);
        PlayerPrefs.SetInt("PlayerLevel", player.level);
        PlayerPrefs.SetInt("PlayerXp", player.exp);
        PlayerPrefs.SetFloat("PlayerHp", player.health);
        PlayerPrefs.SetInt("KillCount", TotalEnemiesKilled);
    }

    public void setupAppSettings()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
}