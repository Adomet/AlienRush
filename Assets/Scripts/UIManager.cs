using System;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public const float UI_UPDATE_INTERVAL = 0.5f;
    
    public TextMeshProUGUI levelText, timeText, killCountText; // play
    public TextMeshProUGUI menulevelText,menukillCountText;
    public TextMeshProUGUI deathlevelText, deathkillCountText;
    public TextMeshProUGUI nextlevelText, nextkillCountText;
    public TextMeshProUGUI wonlevelText, wonkillCountText;

    public Image xpBar;

    
    [Header("UI Panels")] public RectTransform mainMenuPanel;
    public RectTransform playPanel;
    public RectTransform NextLevelPanel;
    public RectTransform DeathPanel;
    public RectTransform GameWonPanel;

    private Dictionary<System.Type, RectTransform> stateUIPanels;

    void Awake()
    {
        stateUIPanels = new Dictionary<System.Type, RectTransform>()
        {
            { typeof(MainMenuState), mainMenuPanel },
            { typeof(PlayingState), playPanel },
            { typeof(NextLevel), NextLevelPanel },
            { typeof(DeathState), DeathPanel },
            { typeof(GameWon), GameWonPanel }

        };
    }


    public void ShowUIOfState(IGameState state)
    {
        foreach (var panel in stateUIPanels.Values)
        {
            if (panel)
                panel.gameObject.SetActive(false);
        }

        if (stateUIPanels.TryGetValue(state.GetType(), out RectTransform statePanel))
        {
            statePanel.gameObject.SetActive(true);

            if (statePanel.gameObject.TryGetComponent(out CanvasGroup canvasGroup))
            {
                canvasGroup.alpha = 0;
                canvasGroup.DOFade(1, 1f).SetEase(Ease.OutBack);
            }
        }
        else
        {
            Debug.LogWarning($"UIManager: {state.GetType()} none found in stateUIPanels!");
        }
        
        updateUI();
    }

    private float lastUpdateTime = 0;
    public void updateUI()
    {
        if (Time.time - lastUpdateTime < UI_UPDATE_INTERVAL && lastUpdateTime != 0)
            return;
       
        
        lastUpdateTime = Time.time;
        int level = GameManager.Instance.CurrentLevelIndex + 1;
        int time = GameManager.Instance.LevelManager.timeRemaining;
        int killCount = GameManager.Instance.TotalEnemiesKilled;
        
        levelText.text = "Lv." + level;
        timeText.text = sec2MinSec(time);
        killCountText.text = "" + killCount;
        
        menulevelText.text = "Level: " + level;
        menukillCountText.text = "Kills: " + killCount;
        
        deathlevelText.text = "Level: " + level;
        deathkillCountText.text = "Kills: " + killCount;
        
        nextlevelText.text = "Level: " + level;
        nextkillCountText.text = "Kills: " + killCount;
        
        wonlevelText.text = "Level: " + level;
        wonkillCountText.text = "Kills: " + killCount;
        
        var retio = PlayerController.instance.exp / (float)PlayerController.instance.maxExp;
        xpBar.DOFillAmount(retio, 0.2f).SetEase(Ease.Linear);

    }
    
    public static string sec2MinSec(int totalSeconds)
    {
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return $"{minutes:D2}:{seconds:D2}";
    }
}