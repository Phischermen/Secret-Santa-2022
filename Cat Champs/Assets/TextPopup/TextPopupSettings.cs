using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Text Popup Settings", menuName = "TextPopupSettings")]
public class TextPopupSettings : ScriptableObject
{
    public GameObject textPopupPrefab;
    public Gradient damageGradient;
}
