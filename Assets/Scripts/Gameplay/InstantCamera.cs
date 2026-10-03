using System;
using Ion.Gameplay.State;
using Ion.Presentation;
using Ion.Projection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ion.Gameplay
{
    /// <summary>
    /// The instant camera. Once <see cref="Unlocked"/>, C toggles camera mode; in camera mode hold Shift (or
    /// RMB) to raise the viewfinder and press LMB to take a photo (costs one film). The photo goes to the
    /// inventory, and the shot is a rewindable change (R gives the film back).
    ///
    /// Lens seam: a lens kit may mount a lens (<see cref="SetLens"/>: its name and snapshot FOV), set an
    /// <see cref="Aperture"/> and a <see cref="FilmStock"/>, apply a look in <see cref="BeforeCapture"/>, and claim
    /// the wheel / digit / Q/E keys in camera mode (<see cref="ClaimsSelectionInput"/>). With none of that set the
    /// camera behaves exactly as before: <see cref="FovY"/> is <see cref="CaptureFovY"/>.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class InstantCamera : MonoBehaviour
    {
        public const float CaptureAspect = 4f / 3f;
        /// <summary>
        /// Vertical FOV of a snapshot: the same shape as the pre-made photos (50°, 4:3), so a raised snapshot
        /// fits inside its Polaroid frame like the others instead of filling the whole 70° view.
        /// </summary>
        public const float CaptureFovY = 50f;
        /// <summary>Snapshot preview width (about screen resolution for the raised frame).</summary>
        public const int CapturePreviewWidth = 1024;
        public const string SnapshotLabel = "Snapshot";
        /// <summary>Range a mounted lens's snapshot FOV is clamped to (degrees, vertical).</summary>
        public const float MinLensFovY = 1f, MaxLensFovY = 150f;

        FirstPersonController _fpc;
        PhotoInventory _inventory;
        ViewfinderFrame _viewfinder;

        bool _unlocked;
        int _film;
        string _lens;
        float _fovY = CaptureFovY;
        float _aperture;
        int _filmStock;

        /// <summary>Whether the player owns the camera (C does nothing until then).</summary>
        public bool Unlocked
        {
            get => _unlocked;
            set
            {
                if (value == _unlocked) return;
                _unlocked = value;
                if (!value) SetCameraMode(false);
                else if (Time.timeSinceLevelLoad > 1f) GameplayUI.Toast("Instant camera  " + UIUtil.Key("C") + " take it out");
                Changed?.Invoke();
            }
        }

        /// <summary>Sets <see cref="Unlocked"/> without the toast (rewind, checkpoint restore, the camera stand).</summary>
        public void SetUnlockedSilently(bool value)
        {
            if (value == _unlocked) return;
            _unlocked = value;
            if (!value) SetCameraMode(false);
            Changed?.Invoke();
        }

        /// <summary>Remaining shots.</summary>
        public int Film
        {
            get => _film;
            set
            {
                value = Mathf.Max(0, value);
                if (value == _film) return;
                _film = value;
                Changed?.Invoke();
            }
        }

        /// <summary>Camera is out (photo holding is disabled meanwhile).</summary>
        public bool IsCameraMode { get; private set; }

        /// <summary>Viewfinder is up (Shift / RMB held in camera mode).</summary>
        public bool IsRaised { get; private set; }

        /// <summary>Raised when Unlocked, Film, camera mode or raise state changes.</summary>
        public event Action Changed;

        /// <summary>Raised after a successful capture, with the new photo.</summary>
        public event Action<PhotoData> Captured;

        // ---------------------------------------------------------------- lens seam

        /// <summary>
        /// Name of the mounted lens, or null for the camera's own lens (<see cref="CaptureFovY"/>). Lenses belong to
        /// whoever mounts them (<see cref="SetLens"/>); the camera keeps only the name and the FOV.
        /// </summary>
        public string Lens => _lens;

        /// <summary>Vertical FOV (degrees) of the next snapshot and of the viewfinder frame: the mounted lens's, else <see cref="CaptureFovY"/>.</summary>
        public float FovY => _fovY;

        /// <summary>
        /// Aperture as an f-number, for capture-time effects drawn by a subscriber of <see cref="BeforeCapture"/> or
        /// <see cref="Captured"/> (depth of field). 0 = not set: a pinhole, everything sharp, as before.
        /// </summary>
        public float Aperture
        {
            get => _aperture;
            set
            {
                if (float.IsNaN(value) || value < 0f) value = 0f;
                if (value == _aperture) return;
                _aperture = value;
                Changed?.Invoke();
            }
        }

        /// <summary>
        /// Index of the loaded film stock: a look its owner applies (e.g. through the IonGrade film globals in
        /// <see cref="BeforeCapture"/>). 0 = the standard stock, no look, as before. (<see cref="Film"/> already counts
        /// the remaining shots, hence the name.)
        /// </summary>
        public int FilmStock
        {
            get => _filmStock;
            set
            {
                value = Mathf.Max(0, value);
                if (value == _filmStock) return;
                _filmStock = value;
                Changed?.Invoke();
            }
        }

        /// <summary>
        /// Set by a lens kit that drives the camera from the mouse wheel, the digit keys and Q/E while the camera is out
        /// (wheel = lens, Q/E = aperture). <see cref="PhotoHolder"/> then leaves the photo selection and roll alone in
        /// camera mode. False by default: camera mode keeps today's controls.
        /// </summary>
        public bool ClaimsSelectionInput { get; set; }

        /// <summary>Raised after <see cref="SetLens"/> / <see cref="ClearLens"/> changed the lens name or its FOV.</summary>
        public event Action LensChanged;

        /// <summary>
        /// Raised in <see cref="TryCapture"/> right before the snapshot is rendered (film is available, the view is
        /// levelled and its effects reset): the moment to apply a look the print should bake in.
        /// </summary>
        public event Action BeforeCapture;

        /// <summary>
        /// Mounts a lens: snapshots and the viewfinder frame use <paramref name="fovY"/> (vertical degrees, clamped to
        /// <see cref="MinLensFovY"/>..<see cref="MaxLensFovY"/>) from now on. <paramref name="lens"/> is any name (null = unnamed).
        /// </summary>
        public void SetLens(string lens, float fovY)
        {
            if (float.IsNaN(fovY)) fovY = CaptureFovY;
            fovY = Mathf.Clamp(fovY, MinLensFovY, MaxLensFovY);
            if (lens == _lens && fovY == _fovY) return;
            _lens = lens;
            _fovY = fovY;
            if (_viewfinder != null) _viewfinder.FovY = fovY;
            InvokeSafely(LensChanged);
            Changed?.Invoke();
        }

        /// <summary>Back to the camera's own lens: no name, <see cref="CaptureFovY"/>.</summary>
        public void ClearLens() => SetLens(null, CaptureFovY);

        static void InvokeSafely(Action handler)
        {
            if (handler == null) return;
            try { handler(); }
            catch (Exception e) { Debug.LogException(e); }
        }

        void Awake()
        {
            _fpc = GetComponent<FirstPersonController>();
            _inventory = GetComponent<PhotoInventory>();
        }

        void OnDisable()
        {
            SetRaised(false);
        }

        void OnDestroy()
        {
            if (_viewfinder != null) Destroy(_viewfinder.gameObject);
        }

        ViewfinderFrame Viewfinder
        {
            get
            {
                if (_viewfinder == null) _viewfinder = ViewfinderFrame.Create(CaptureAspect, _fovY);
                return _viewfinder;
            }
        }

        void Update()
        {
            var kb = Keyboard.current;

            if ((_fpc != null && (!_fpc.InputEnabled || _fpc.Frozen)) || kb == null)
            {
                SetRaised(false);
                return;
            }

            if (_unlocked && IonInput.Active && kb.cKey.wasPressedThisFrame)
                SetCameraMode(!IsCameraMode);

            if (!IsCameraMode)
            {
                SetRaised(false);
                return;
            }

            SetRaised(IonInput.RaiseHeld);

            if (IsRaised && IonInput.PrimaryPressedThisFrame)
                TryCapture();
        }

        /// <summary>Takes the camera out / puts it away.</summary>
        public void SetCameraMode(bool on)
        {
            if (on && !_unlocked) on = false;
            if (on == IsCameraMode) return;
            IsCameraMode = on;
            if (!on) SetRaised(false);
            Changed?.Invoke();
        }

        void SetRaised(bool raised)
        {
            if (raised == IsRaised) return;
            IsRaised = raised;
            if (raised || _viewfinder != null) Viewfinder.Visible = raised;
            Changed?.Invoke();
        }

        /// <summary>
        /// Takes a photo from the player camera right now (what LMB does with the viewfinder up):
        /// costs one film, adds the photo to the inventory. Returns null if out of film / not possible.
        /// </summary>
        public PhotoData TryCapture()
        {
            if (_film <= 0)
            {
                GameplayUI.Toast("Out of film");
                return null;
            }

            var ps = ProjectionSystem.Instance;
            var cam = _fpc != null ? _fpc.Camera : null;
            if (ps == null || cam == null) return null;

            // Same level-view assist as placing, so a snapshot and its paste line up.
            _fpc.SnapPitchLevel(PhotoHolder.LevelSnapDegrees);
            _fpc.ResetViewEffects(); // exact eye pose and base FOV (no head bob / FOV punch in the photo)
            InvokeSafely(BeforeCapture);
            var t = cam.transform;
            var photo = ps.Capture(new Pose(t.position, t.rotation), _fovY, CaptureAspect, SnapshotLabel, Mathf.Max(CapturePreviewWidth, ProjectionSystem.DefaultPreviewWidth));
            if (photo == null) return null;

            int filmBefore = _film;
            Film = _film - 1;
            Viewfinder.Flash();
            if (_inventory != null) _inventory.Add(photo);
            var history = WorldHistory.Instance;
            if (history != null)
            {
                history.Push(new CaptureChange
                {
                    Photo = photo,
                    InventoryIndex = _inventory != null ? _inventory.IndexOf(photo) : -1,
                    FilmBefore = filmBefore,
                    Inventory = _inventory,
                    Camera = this,
                    SafePose = WorldHistory.SafePoseNow(),
                }.At(WorldHistory.PoseNow()));
            }
            GameplayUI.PhotoPrinted(photo);
            GameplayUI.Toast(_film > 0
                ? "Photo added   " + UIUtil.Key("C") + " put the camera away"
                : "That was the last of the film   " + UIUtil.Key("R") + " gives it back");
            Captured?.Invoke(photo);
            return photo;
        }
    }
}
