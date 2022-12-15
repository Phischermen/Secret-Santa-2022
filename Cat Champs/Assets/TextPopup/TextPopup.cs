/*
 * TextPopup is used to display simple text via a TMP component. It can be conveniently instantiated anytime through
 * Create().
 */

using System.Collections;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TextPopup : MonoBehaviour
{
    private static TextPopupSettings _settings;

    private static TextPopupSettings Settings
    {
        get
        {
            return _settings ??= Resources.Load<TextPopupSettings>("TextPopupSettings");
        }
    }
    public AnimationCurve riseCurve;
    public AnimationCurve fadeCurve;
    public float duration;
    private TextMeshPro _text;

    private void Awake()
    {
        _text = gameObject.GetComponent<TextMeshPro>();
    }

    public static TextPopup CreateForDamage(int score, int maxScorePossible, Vector3 pos)
    {
        var color = Settings.damageGradient.Evaluate((float) score / (float) maxScorePossible);
        return Create(score.ToString(), color, pos);
    }

    public static TextPopup Create(string text, Color color, Vector3 pos)
    {
        var gameObject = Instantiate(Settings.textPopupPrefab, pos, Quaternion.identity);
        var textPopup = gameObject.GetComponent<TextPopup>();
        var textPopupText = textPopup._text;
        textPopupText.text = text;
        textPopupText.color = color;
        textPopup.transform.DOMoveY(1f, textPopup.duration).SetRelative(true).SetEase(textPopup.riseCurve);
        var destinationColor = textPopupText.color;
        destinationColor.a = 0;
        textPopupText.DOColor(destinationColor, textPopup.duration).SetEase(textPopup.fadeCurve).onComplete +=
            () => Destroy(textPopup.gameObject);
        return textPopup;
    }
}