using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuState : FiniteStateMachine.State
{
    protected override void BeginState()
    {
        LoadSceneIfNotLoaded(Scenum.MenuScene).Then(SetupMainMenuScene);
    }

    protected override void EndState()
    {
        UnloadSceneIfLoaded(Scenum.MenuScene);
    }
    
    public override FiniteStateMachine.State DoState()
    {
        return nextState;
    }

    private void SetupMainMenuScene()
    {
        var playerSelection = GameObject.Find("PlayerSelection");
        foreach (var button in Object.FindObjectsOfType<Button>())
        {
            switch (button.name)
            {
                case "PlayButton":
                    button.onClick.AddListener(() =>
                    {
                        playerSelection.SetActive(true);
                        button.gameObject.SetActive(false);
                    });
                    break;
                case "Character1":
                    button.onClick.AddListener(() =>
                    {
                        playerSelection.SetActive(false);
                        nextState = new GameplayState(Scenum.Arena, 0);
                    });
                    break;
                case "Character2":
                    button.onClick.AddListener(() =>
                    {
                        playerSelection.SetActive(false);
                        nextState = new GameplayState(Scenum.Arena, 1);
                    });
                    break;
                case "Character3":
                    button.onClick.AddListener(() =>
                    {
                        playerSelection.SetActive(false);
                        nextState = new GameplayState(Scenum.Arena, 2);
                    });
                    break;
            }
        }
        playerSelection.gameObject.SetActive(false);
    }
}
