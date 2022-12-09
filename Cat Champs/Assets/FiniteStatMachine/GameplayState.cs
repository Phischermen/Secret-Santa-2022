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
        if (!FiniteStateMachineUtility.LoadSceneIfNotLoaded(Scenum.GameScene))
        {
            SetupGameScene();
        }
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
        switch (scene.buildIndex)
        {
            case Scenum.GameScene:
                SetupGameScene();
                break;
        }
    }

    private void SetupGameScene()
    {
        _playerActor = GameObject.FindWithTag("Player").GetComponent<PlayerActor>();
        _playerActor.health.HealthDepleted += PlayerActorOnDeath;
    }

    protected override void SceneManagerOnSceneUnloaded(Scene scene)
    {
        switch (scene.buildIndex)
        {
            case Scenum.GameScene:
                _playerActor = null;
                break;
        }
    }

    private static PlayerActor _playerActor;

    public static PlayerActor GetPlayer()
    {
        return _playerActor;
    }

    private void PlayerActorOnDeath(Actor obj)
    {
        Debug.Log("Player died");
    }
}
