using UnityEditor;

namespace Editor
{
    public static class BuildScript
    {
        [MenuItem("Tools/Build")]
        public static void Build()
        {
            // Generate some source files.
            ScenumGenerator.GenerateScenum();
            // Get scenes from build settings
            string[] scenes = new string[EditorBuildSettings.scenes.Length];
            for (int i = 0; i < scenes.Length; i++)
            {
                if (EditorBuildSettings.scenes[i].enabled)
                {
                    scenes[i] = EditorBuildSettings.scenes[i].path;
                }
            }

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions()
            {
                locationPathName = "Build/EE Unity Project.exe",
                scenes = scenes,
                options = BuildOptions.None,
                target = BuildTarget.StandaloneWindows64
            };
            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }
    }
}

