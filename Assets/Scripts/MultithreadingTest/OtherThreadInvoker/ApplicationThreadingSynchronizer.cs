using System.Threading;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultithreadingTest.OtherThreadInvoker
{
    #if UNITY_EDITOR
    [InitializeOnLoad]
    #endif
    public static class ApplicationThreadingSynchronizer
    {
        private static readonly CancellationTokenSource QuitSource;

        public static CancellationToken QuitToken { get; }

        public static SynchronizationContext UnityContext { get; private set; }

        static ApplicationThreadingSynchronizer()
        {
            QuitSource = new CancellationTokenSource();
            QuitToken = QuitSource.Token;
        }

        #if UNITY_EDITOR
        [InitializeOnLoadMethod]
        #endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void MainThreadInitialize()
        {
            UnityContext = SynchronizationContext.Current;
            Application.quitting += QuitSource.Cancel;
            #if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            #endif
        }

        #if UNITY_EDITOR
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                QuitSource.Cancel();
            }
        }
        #endif
    }
}