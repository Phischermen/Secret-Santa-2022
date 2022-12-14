using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpgrades
{
    void InitializeUpgrades();
    List<Upgrade> GetUpgrades();
}

public abstract class Upgrade
{
    public string name;
    public string description;
    public abstract void UpgradeChosen();
}
