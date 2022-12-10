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
        foreach (var button in Object.FindObjectsOfType<Button>())
        {
            switch (button.name)
            {
                case "PlayButton":
                    button.onClick.AddListener(() => nextState = new GameplayState(Scenum.Arena, 0));
                    break;
            }
        }
    }
}
