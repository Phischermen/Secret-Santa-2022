using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    private GameplayState _gameplayState;
    public List<TextMeshProUGUI> upgradeTexts;
    public List<Button> upgradeButtons;

    public void Initialize(GameplayState gameplayState)
    {
        _gameplayState = gameplayState;
        gameObject.SetActive(false);
    }

    private List<Upgrade> _upgrades;
    public void ShowUpgradeUI(List<Upgrade> upgrades)
    {
        _upgrades = upgrades;
        gameObject.SetActive(true);
        // Get the minimum number of upgrades to show
        int min = Mathf.Min(upgrades.Count, upgradeTexts.Count);
        // Loop through the minimum number of upgrades, setting the text
        for (int i = 0; i < min; i++)
        {
            upgradeTexts[i].text = $"{upgrades[i].name}\n{upgrades[i].description}";
            upgradeButtons[i].interactable = true;
        }
        // Loop through the remaining upgrade texts, setting them to empty
        for (int i = min; i < upgradeTexts.Count; i++)
        {
            upgradeTexts[i].text = "";
            upgradeButtons[i].interactable = false;
        }
    }

    public void UpgradeChosen(int idx)
    {
        _gameplayState.UpgradeChosen(_upgrades[idx]);
    }
    
    public void HideUpgradeUI()
    {
        gameObject.SetActive(false);
    }
}
