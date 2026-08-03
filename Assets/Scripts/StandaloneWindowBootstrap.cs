using UnityEngine;

public static class StandaloneWindowBootstrap
{
    private const int DefaultWidth = 1280;
    private const int DefaultHeight = 720;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ApplyWindowMode()
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        if (Screen.fullScreenMode != FullScreenMode.Windowed)
        {
            Screen.SetResolution(
                DefaultWidth,
                DefaultHeight,
                FullScreenMode.Windowed);
        }
#endif
    }
}
