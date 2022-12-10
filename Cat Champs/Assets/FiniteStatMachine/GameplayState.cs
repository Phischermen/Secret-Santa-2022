using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static FiniteStateMachine;

public class GameplayState : State
{
    private int _arenaSceneIdx;
    private int _character;
    
    public GameplayState(int arenaSceneIdx, int character)
    {
        _arenaSceneIdx = arenaSceneIdx;
        _character = character;
    }
    protected override void BeginState()
    {
        LoadSceneIfNotLoaded(_arenaSceneIdx).Then(() =>
        {
            _arena = Object.FindObjectOfType<Arena>();
            LoadSceneIfNotLoaded(Scenum.GameScene).Then(SetupGameScene);
        });
    }

    protected override void EndState()
    {
        UnloadSceneIfLoaded(Scenum.GameScene);
        _playerActor = null;
    }

    public override State DoState()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            return new MenuState();
        }
        if (_arena) _arena.ControlSpawning();
        return nextState;
    }

    private void SetupGameScene()
    {
        _playerActor = GameObject.FindWithTag("Player").GetComponent<PlayerActor>();
        _playerActor.health.HealthDepleted += PlayerActorOnDeath;
    }

    private static Arena _arena;
    public static Arena GetArena()
    {
        return _arena;
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
