using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameplayState : FiniteStateMachine.State
{

    protected override void BeginState()
    {
        FiniteStateMachineUtility.LoadSceneIfNotLoaded(Scenum.GameScene);
    }

    protected override void EndState()
    {
        FiniteStateMachineUtility.UnloadIfLoaded(Scenum.GameScene);
    }

    public override FiniteStateMachine.State DoState()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            return new MenuState();
        }
        return nextState;
    }

    protected override void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }

    protected override void SceneManagerOnSceneUnloaded(Scene scene)
    {
        
    }
}
