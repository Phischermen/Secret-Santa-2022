using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class RunParameterMenu : EditorWindow
{
    private const string MenuName = "RunParameters";
    private const string ItemDisableFiniteStateMachine = MenuName + "/" + nameof(disableFiniteStateMachine);

    public static bool disableFiniteStateMachine;

    static RunParameterMenu()
    {
        disableFiniteStateMachine = EditorPrefs.GetBool(nameof(disableFiniteStateMachine), false);
    }

    [MenuItem(ItemDisableFiniteStateMachine)]
    private static void ToggleDisableFiniteStateMachine()
    {
        disableFiniteStateMachine = !disableFiniteStateMachine;
        EditorPrefs.SetBool(nameof(disableFiniteStateMachine), disableFiniteStateMachine);
    }

    [MenuItem(ItemDisableFiniteStateMachine, true)]
    private static bool ToggleDisableFiniteStateMachineValidate()
    {
        Menu.SetChecked(ItemDisableFiniteStateMachine, disableFiniteStateMachine);
        return true;
    }
}