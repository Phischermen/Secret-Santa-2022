using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    PlayerActor _playerActor;
    HealthManager _healthManager;

    public TextMeshProUGUI uiText;
    public void Initialize(PlayerActor playerActor)
    {
        _playerActor = playerActor;
        _healthManager = _playerActor.health;
        _healthManager.OnDamageTaken += (_,_) => UpdateUI();
        UpdateUI();
    }

    private void UpdateUI()
    {
        uiText.text = $"<color=red>Health : {_healthManager.CurrentHealth}</color>";
    }
}
