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
        UnloadSceneIfLoaded(Scenum.PlayerUI);
        UnloadSceneIfLoaded(Scenum.GameoverScene);
        UnloadSceneIfLoaded(Scenum.UpgradeScene);
        _playerActor = null;
        _arena = null;
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
        if (!_paused && !_gameOver && _arena) _arena.ControlSpawning();
        return nextState;
    }

    private void SetupGameScene()
    {
        _playerActor = GameObject.FindWithTag("Player").GetComponent<PlayerActor>();
        _playerActor.health.HealthDepleted += PlayerActorOnDeath;
        _playerActor.LevelUp += PlayerActorOnLevelUp;

        Object.FindObjectOfType<PlayerUI>().Initialize(_playerActor);
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
        _gameOverUI.Show();
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
