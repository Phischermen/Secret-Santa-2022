using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FiniteStateMachine : MonoBehaviour
{
    public static FiniteStateMachine GetInst() => _inst;

    public static State CurrentState
    {
        get => _currentState;
        private set
        {
            if (value != _currentState)
            {
                // Transition states.
                _currentState?.EndStateInternal();
                value.BeginStateInternal();
                _currentState = value;
            }
        }
    }

    private static FiniteStateMachine _inst;
    private static State _currentState;

    [RuntimeInitializeOnLoadMethod]
    private static void OnRuntimeInitialize()
    {
        var go = new GameObject("FiniteStateMachine");
        _inst = go.AddComponent<FiniteStateMachine>();
        DontDestroyOnLoad(go);
        
#if UNITY_EDITOR
        var continueWithLoad = !EditorPrefs.GetBool("disableFiniteStateMachine");
#else        
        var continueWithLoad = true;
#endif
        if (continueWithLoad)
        {
            // Allow us to load and unload scenes to our hearts content
            // NOTE: LoadSceneMode single unloads whatever scene was loaded in editor.
            SceneManager.LoadScene("EmptyScene", LoadSceneMode.Single);
            SceneManager.sceneLoaded += (arg0, scene) =>
            {
                if (arg0.name != "EmptyScene") return;
                SceneManager.SetActiveScene(SceneManager.GetSceneByName("EmptyScene"));
                CurrentState = new GameplayState();
            };
        }
    }

    private void Update()
    {
        if (_currentState != null)
        {
            // Process current state.
            CurrentState = _currentState.DoState();
        }
    }

    private void OnDestroy()
    {
        _currentState?.EndStateInternal();
    }

    public abstract class State
    {
        protected State nextState;

        public void BeginStateInternal()
        {
            SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
            SceneManager.sceneUnloaded += SceneManagerOnSceneUnloaded;
            nextState = this;
            BeginState();
        }

        public void EndStateInternal()
        {
            EndState();
            SceneManager.sceneLoaded -= SceneManagerOnSceneLoaded;
            SceneManager.sceneUnloaded -= SceneManagerOnSceneUnloaded;
        }

        protected abstract void BeginState();
        protected abstract void EndState();
        public abstract State DoState();
        protected abstract void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode mode);
        protected abstract void SceneManagerOnSceneUnloaded(Scene scene);
    }
}