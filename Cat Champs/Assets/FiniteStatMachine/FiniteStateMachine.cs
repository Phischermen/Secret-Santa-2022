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
        Debug.Log("FiniteStateMachine initialized.");
        
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
                CurrentState = new GameplayState(Scenum.Arena, 0);
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
            // SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
            // SceneManager.sceneUnloaded += SceneManagerOnSceneUnloaded;
            nextState = this;
            BeginState();
        }

        public void EndStateInternal()
        {
            EndState();
            // SceneManager.sceneLoaded -= SceneManagerOnSceneLoaded;
            // SceneManager.sceneUnloaded -= SceneManagerOnSceneUnloaded;
        }

        protected SceneLoadHandle LoadSceneIfNotLoaded(int buildIdx)
        {
            if (!SceneManager.GetSceneByBuildIndex(buildIdx).isLoaded)
            {
                SceneManager.LoadScene(buildIdx, LoadSceneMode.Additive);
            }
            return new SceneLoadHandle(buildIdx, true);
        }

        protected SceneLoadHandle UnloadSceneIfLoaded(int buildIdx)
        {
            if (SceneManager.GetSceneByBuildIndex(buildIdx).isLoaded)
            {
                SceneManager.UnloadSceneAsync(buildIdx);
            }

            return new SceneLoadHandle(buildIdx, false);
        }
        protected class SceneLoadHandle
        {
            private readonly int _buildIdx;
            private readonly bool _loadHandle;
            public SceneLoadHandle(int buildIdx, bool loadHandle)
            {
                _buildIdx = buildIdx;
                _loadHandle = loadHandle;
            }

            private Action _action;
            public void Then(Action action)
            {
                if (SceneManager.GetSceneByBuildIndex(_buildIdx).isLoaded)
                {
                    action.Invoke();
                } else
                {
                    _action = action;
                    if (_loadHandle) ListenToLoad(action);
                    else ListenToUnload(action);
                }
            }

            private void ListenToLoad(Action action)
            {
                SceneManager.sceneLoaded += SceneLoaded;
            }
            
            void SceneLoaded(Scene scene, LoadSceneMode _)
            {
                if (scene.buildIndex == _buildIdx)
                {
                    _action.Invoke();
                    SceneManager.sceneLoaded -= SceneLoaded;
                }
            }

            private void ListenToUnload(Action action)
            {
                SceneManager.sceneUnloaded += SceneUnloaded;
            }
            
            void SceneUnloaded(Scene scene)
            {
                if (scene.buildIndex == _buildIdx)
                {
                    _action.Invoke();
                    SceneManager.sceneUnloaded -= SceneUnloaded;
                }
            }
        }

        protected abstract void BeginState();
        protected abstract void EndState();
        public abstract State DoState();
    }
}