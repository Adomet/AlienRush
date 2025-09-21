using UnityEngine;

public interface IGameState
{
    void EnterState(GameManager gameManager);
    void UpdateState(GameManager gameManager);
    void ExitState(GameManager gameManager);
}

public class MainMenuState : IGameState
{
    public void EnterState(GameManager gameManager)
    {
        GameManager.Instance.UIManager.ShowUIOfState(this);
    }

    public void UpdateState(GameManager gameManager)
    {
        if (gameManager.isPlay)
        {
            gameManager.SwitchState(new PlayingState());
        }
    }

    public void ExitState(GameManager gameManager)
    {
    }
}

public class PlayingState : IGameState
{
    public void EnterState(GameManager gameManager)
    {
        GameManager.Instance.UIManager.ShowUIOfState(this);

        gameManager.playLevel();
    }

    public void UpdateState(GameManager gameManager)
    {
        if (PlayerController.instance.isDead)
        {
            gameManager.SwitchState(new DeathState());
        }
        else if (gameManager.LevelManager.isLevelFinished)
        {
            gameManager.SwitchState(new NextLevel());
        }
        
        GameManager.Instance.UIManager.updateUI();
    }

    public void ExitState(GameManager gameManager)
    {
    }
}

public class DeathState : IGameState
{
    public void EnterState(GameManager gameManager)
    {
        GameManager.Instance.UIManager.ShowUIOfState(this);
        gameManager.isPlay = false;
    }

    public void UpdateState(GameManager gameManager)
    {
        if (gameManager.isPlay)
        {
            gameManager.SwitchState(new PlayingState());
        }
    }

    public void ExitState(GameManager gameManager)
    {
        gameManager.resetProgress();
    }
}

public class NextLevel : IGameState 
{
    public void EnterState(GameManager gameManager)
    {
        Debug.Log(GameManager.Instance.CurrentLevelIndex);
        if (GameManager.Instance.CurrentLevelIndex >= 2)
        {
            gameManager.SwitchState(new GameWon());
            return;       
        }
        
        
        GameManager.Instance.UIManager.ShowUIOfState(this);
        gameManager.isPlay = false;
        PlayerController.instance.resetForNextLevel();
    }

    public void UpdateState(GameManager gameManager)
    {
        if (gameManager.isPlay)
        {
            gameManager.SwitchState(new PlayingState());
        }
    }

    public void ExitState(GameManager gameManager)
    {
    }
}

public class GameWon : IGameState // GAME WON
{
    public void EnterState(GameManager gameManager)
    {
        GameManager.Instance.UIManager.ShowUIOfState(this);
        gameManager.isPlay = false;
    }

    public void UpdateState(GameManager gameManager)
    {
        if (gameManager.isPlay)
        {
            gameManager.SwitchState(new PlayingState());
        }
    }

    public void ExitState(GameManager gameManager)
    {
        gameManager.resetProgress();
    }
}