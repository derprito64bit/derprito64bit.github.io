#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using Ion.Gameplay;
using Ion.Levels;
using UnityEngine;

namespace Ion.Portfolio
{
    /// <summary>
    /// The Grand Gallery's arcade cabinet: E ("play") opens the project demo at <see cref="Url"/> (a page of the
    /// site, relative to /play/) in the arcade overlay. Serialized fields only, so a photo copy works too.
    /// </summary>
    public sealed class ArcadeMachine : UsableBehaviour
    {
        [SerializeField] string _url = "../arcade/demo/";
        [SerializeField] string _title = "Arcade";

        public string Url { get => _url; set => _url = value; }
        public string Title { get => _title; set => _title = value; }

        public int Plays { get; private set; }

        public override bool CanUse => base.CanUse && !string.IsNullOrEmpty(_url) && !ArcadeOverlay.IsOpen;

        public override void Use()
        {
            Plays++;
            ArcadeOverlay.Open(_url, _title);
        }
    }

    /// <summary>
    /// Opens a demo in a full-screen arcade overlay on the web page (an iframe in a cabinet bezel). The game releases
    /// the pointer and stops capturing the keyboard while it is open; closing it (button, Esc or a click outside)
    /// leaves the game paused until the visitor clicks back in. In the Editor it only shows a toast.
    /// </summary>
    public static class ArcadeOverlay
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] static extern void IonArcadeOpen(string url, string title);
        [DllImport("__Internal")] static extern int IonArcadeIsOpen();
#endif

        public static bool IsOpen
        {
            get
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                return IonArcadeIsOpen() != 0;
#else
                return false;
#endif
            }
        }

        public static void Open(string url, string title)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLInput.captureAllKeyboardInput = false;
            IonArcadeOpen(url ?? string.Empty, title ?? string.Empty);
            ArcadeWatcher.Ensure();
#else
            RoomContext.Toast("The arcade runs in the browser build: " + title + " (" + url + ")", 4f);
#endif
        }
    }

    /// <summary>Gives the keyboard back to the game once the overlay closes.</summary>
    sealed class ArcadeWatcher : MonoBehaviour
    {
        static ArcadeWatcher s_instance;
        bool _wasOpen;

        public static void Ensure()
        {
            if (s_instance != null) return;
            var go = new GameObject("Ion Arcade Watcher");
            DontDestroyOnLoad(go);
            s_instance = go.AddComponent<ArcadeWatcher>();
        }

        void Update()
        {
            bool open = ArcadeOverlay.IsOpen;
#if UNITY_WEBGL && !UNITY_EDITOR
            if (_wasOpen && !open) WebGLInput.captureAllKeyboardInput = true;
#endif
            _wasOpen = open;
        }
    }
}
