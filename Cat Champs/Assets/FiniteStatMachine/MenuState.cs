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
        FiniteStateMachineUtility.LoadSceneIfNotLoaded(Scenum.MenuScene);
    }

    protected override void EndState()
    {
        FiniteStateMachineUtility.UnloadIfLoaded(Scenum.MenuScene);
    }
    
    public override FiniteStateMachine.State DoState()
    {
        return nextState;
    }

    protected override void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.buildIndex)
        {
            case Scenum.MenuScene:
                foreach (var button in Object.FindObjectsOfType<Button>())
                {
                    switch (button.name)
                    {
                        case "PlayButton":
                            button.onClick.AddListener(() => nextState = new GameplayState());
                            break;
                    }
                }
                break;
        }
    }

    protected override void SceneManagerOnSceneUnloaded(Scene scene)
    {
        
    }
}
