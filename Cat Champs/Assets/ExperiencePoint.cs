using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ExperiencePoint : MonoBehaviour
{
    private bool _triggered;
    public int points;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (_triggered) return;
        if (col.CompareTag("Player"))
        {
            var player = col.GetComponent<PlayerActor>();
            player.CollectExperience(points);
            transform.DOMove(col.transform.position, 0.1f).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
    }
}
