using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    PlayerActor _playerActor;
    HealthManager _healthManager;

    public TextMeshProUGUI xpText;
    public TextMeshProUGUI healthText;
    
    public Image xpBar;
    public Image healthBar;
    public void Initialize(PlayerActor playerActor)
    {
        _playerActor = playerActor;
        _healthManager = _playerActor.health;
        _healthManager.OnDamageTaken += (_,_) => UpdateUI();
        _playerActor.XpChanged += (_) => UpdateUI();
        // Set everything to the initial values
        xpText.text = "0";
        healthText.text = "100";
        xpBar.fillAmount = 0;
        healthBar.fillAmount = 1;
    }

    private void UpdateUI()
    {
        xpText.text = _playerActor.experience.ToString();
        healthText.text = _healthManager.CurrentHealth.ToString();
        xpBar.fillAmount = (float)_playerActor.experience / _playerActor.GetExperienceToNextLevel();
        healthBar.fillAmount = (float)_healthManager.CurrentHealth / _healthManager.MaxHealth;
    }
}
