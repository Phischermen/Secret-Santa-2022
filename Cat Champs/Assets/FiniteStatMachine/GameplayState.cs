using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static FiniteStateMachine;
using Random = UnityEngine.Random;

public class GameplayState : State
{
    private int _arenaSceneIdx;
    private int _character;
    
    public GameplayState(int arenaSceneIdx, int character)
    {
        _arenaSceneIdx = arenaSceneIdx;
        _character = character;
    }
    
    private GameOverUI _gameOverUI;
    private UpgradeUI _upgradeUI;
    protected override void BeginState()
    {
        LoadSceneIfNotLoaded(_arenaSceneIdx).Then(() =>
        {
            _arena = Object.FindObjectOfType<Arena>();
            LoadSceneIfNotLoaded(Scenum.PlayerUI).Then(() =>
            {
                LoadSceneIfNotLoaded(Scenum.GameScene).Then(SetupGameScene);
            });
        });
        LoadSceneIfNotLoaded(Scenum.GameoverScene).Then(() =>
        {
            _gameOverUI = Object.FindObjectOfType<GameOverUI>();
            _gameOverUI.Initialize(this);
        });
        LoadSceneIfNotLoaded(Scenum.UpgradeScene).Then(() =>
        {
            _upgradeUI = Object.FindObjectOfType<UpgradeUI>();
            _upgradeUI.Initialize(this);
        });
    }

    protected override void EndState()
    {
        UnloadSceneIfLoaded(Scenum.GameScene);
        UnloadSceneIfLoaded(_arenaSceneIdx);
        UnloadSceneIfLoaded(Scenum.PlayerUI);
        UnloadSceneIfLoaded(Scenum.GameoverScene);
        UnloadSceneIfLoaded(Scenum.UpgradeScene);
        _playerActor = null;
        _arena = null;
        Object.Destroy(_playerGameObject);
        foreach (var gameObject in GameObject.FindGameObjectsWithTag("DestroyOnReplay"))
        {
            Object.Destroy(gameObject);
        }
    }

    private bool _paused;
    private bool _gameOver;
    public override State DoState()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            return new MenuState();
        }

        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            PlayerActorOnLevelUp(_playerActor);
        }

        if (!_paused && !_gameOver && _arena)
        {
            _arena.ControlSpawning();
            _arena.UpdatePathGrid();
        }
        return nextState;
    }

    private void SetupGameScene()
    {
        _playerGameObject = Object.Instantiate(Object.FindObjectOfType<PlayerResourceProvider>().characters[_character], Vector3.zero, quaternion.identity);
        _playerActor = _playerGameObject.GetComponent<PlayerActor>();
        _playerActor.health.HealthDepleted += PlayerActorOnDeath;
        _playerActor.LevelUp += PlayerActorOnLevelUp;
        
        _arena.pathfindingTarget = _playerActor.transform;
        _arena.UpdatePathGrid();

        Object.FindObjectOfType<PlayerUI>().Initialize(_playerActor);
    }

    private static Arena _arena;
    public static Arena GetArena()
    {
        return _arena;
    }

    private static PlayerActor _playerActor;
    private static GameObject _playerGameObject;

    public static PlayerActor GetPlayer()
    {
        return _playerActor;
    }

    private void PlayerActorOnDeath(Actor obj)
    {
        _gameOverUI.Show();
        Object.Destroy(_playerActor);
    }
    
    private void PlayerActorOnLevelUp(PlayerActor player)
    {
        var upgrades = ((PlayerAttackController)player.attackController).GetUpgradesFromAllAttacks();
        // Pick four random upgrades to show player.
        var randomUpgrades = new List<Upgrade>();
        for (var i = 0; i < 4; i++)
        {
            var randomIndex = Random.Range(0, upgrades.Count);
            randomUpgrades.Add(upgrades[randomIndex]);
            upgrades.RemoveAt(randomIndex);
            if (upgrades.Count == 0) break;
        }
        _upgradeUI.ShowUpgradeUI(randomUpgrades);
        _paused = true;
        Time.timeScale = 0;
    }

    public void Replay()
    {
        // Reload this state
        nextState = new GameplayState(_arenaSceneIdx, _character);
    }

    public void GoToMainMenu()
    {
        nextState = new MenuState();
    }

    public void UpgradeChosen(Upgrade upgrade)
    {
        _paused = false;
        Time.timeScale = 1;
        upgrade.UpgradeChosen();
        _upgradeUI.HideUpgradeUI();
    }
}
