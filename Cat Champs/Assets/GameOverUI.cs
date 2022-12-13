using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    GameplayState _gameplayState;
    public void Initialize(GameplayState gameplayState)
    {
        _gameplayState = gameplayState;
        gameObject.SetActive(false);
    }

    public void Replay()
    {
        _gameplayState?.Replay();
    }
    
    public void GoToMainMenu()
    {
        _gameplayState?.GoToMainMenu();
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
    }
}
