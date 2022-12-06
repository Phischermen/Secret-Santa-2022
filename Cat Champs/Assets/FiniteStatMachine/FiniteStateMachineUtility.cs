using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public static class FiniteStateMachineUtility
    {
        public static bool LoadSceneIfNotLoaded(int buildIdx)
        {
            if (!SceneManager.GetSceneByBuildIndex(buildIdx).isLoaded)
            {
                SceneManager.LoadScene(buildIdx, LoadSceneMode.Additive);
                return true;
            }
            return false;
        }

        public static bool UnloadIfLoaded(int buildIndex)
        {
            if (SceneManager.GetSceneByBuildIndex(buildIndex).isLoaded)
            {
                SceneManager.UnloadSceneAsync(buildIndex);
                return true;
            }

            return false;
        }
    }
}