using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : HealthManager
{
    private void Start()
    {
        HealthDepleted += OnHealthDepleted;
    }

    private void OnHealthDepleted(Actor obj)
    {
        Debug.Log("Player is dead");
        // todo: game over
    }
}
