using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Ion.Gameplay;
using Ion.Gameplay.State;
using Ion.Presentation.Motion;
using Ion.Projection;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Ion.Tests
{
    /// <summary>
    /// The lens seam (up/lens): neutral by default (no lens: a 70° view, 50° snapshots, placing as before) and each hook
    /// doing what it says once a lens kit or a place guard uses it. The IonGrade film branch is checked by compiling the
    /// graded shaders for WebGL2. Edit mode never calls Awake, so the player rig is woken by hand.
    /// </summary>
    public sealed class LensSeamTests
    {
        static readonly Vector3 FarAway = new Vector3(-3000f, 0f, 3000f);
        const float Frame = 1f / 60f;

        readonly HashSet<GameObject> _rootsBefore = new HashSet<GameObject>();
        Func<Vector3, int> _zoneOfBefore;

        [SetUp]
        public void SetUp()
        {
            _rootsBefore.Clear();
            foreach (GameObject go in SceneManager.GetActiveScene().GetRootGameObjects()) _rootsBefore.Add(go);
            _zoneOfBefore = ZoneInfo.ZoneOfProvider;
            PhotoHolder.PlaceAllowedInZone = null;
            PhotoHolder.PlaceRefusedPrompt = null;
        }

        [TearDown]
        public void TearDown()
        {
            PhotoHolder.PlaceAllowedInZone = null;
            PhotoHolder.PlaceRefusedPrompt = null;
            ZoneInfo.ZoneOfProvider = _zoneOfBefore;
            foreach (GameObject go in SceneManager.GetActiveScene().GetRootGameObjects())
                if (!_rootsBefore.Contains(go)) UnityEngine.Object.DestroyImmediate(go);
        }

        // ------------------------------------------------------------------ rig

        static void Wake(MonoBehaviour behaviour)
        {
            MethodInfo awake = behaviour.GetType().GetMethod("Awake", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.IsNotNull(awake, behaviour.GetType().Name + " has no Awake");
            awake.Invoke(behaviour, null);
        }

        /// <summary>The real player rig (PlayerFactory), with the components the lens seam touches woken.</summary>
        static FirstPersonController MakePlayer()
        {
            FirstPersonController fpc = PlayerFactory.Create(FarAway, 0f);
            Wake(fpc);
            Wake(fpc.GetComponent<InstantCamera>());
            Wake(fpc.GetComponent<PhotoHolder>());
            return fpc;
        }

        /// <summary>Runs the controller's per-frame view feel (bob, dip, FOV springs, lens view) for <paramref name="seconds"/>.</summary>
        static void StepView(FirstPersonController fpc, float seconds)
        {
            MethodInfo step = typeof(FirstPersonController).GetMethod("UpdateViewEffects", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(step, "FirstPersonController.UpdateViewEffects");
            for (float t = 0f; t < seconds; t += Frame) step.Invoke(fpc, new object[] { Frame });
        }

        static ProjectionSystem EnsureProjection()
        {
            ProjectionSystem ps = ProjectionSystem.Instance;
            if (ps == null) ps = new GameObject("LensSeamProjection").AddComponent<ProjectionSystem>();
            return ps;
        }

        static void RewindTo(ProjectionSystem ps, int count)
        {
            while (ps != null && ps.PlacementCount > count && ps.CanRewind) ps.Rewind();
        }

        // ------------------------------------------------------------------ neutral defaults

        [Test]
        public void NoLens_ViewFovIs70_AndTheCameraKeepsTodaysShape()
        {
            FirstPersonController fpc = MakePlayer();
            Assert.AreEqual(70f, fpc.BaseFieldOfView);
            Assert.AreEqual(70f, fpc.ViewFov, "with no lens set, ViewFov = 70");
            Assert.IsFalse(fpc.HasLensView);
            Assert.AreEqual(1f, fpc.LookScale, "mouse look is unscaled");

            fpc.ResetViewEffects();
            StepView(fpc, 1f);
            fpc.ClearLensView(); // nothing to clear
            Assert.IsFalse(fpc.HasLensView);
            Assert.AreEqual(70f, fpc.ViewFov);
            Assert.AreEqual(70f, fpc.Camera.fieldOfView);

            InstantCamera cam = fpc.GetComponent<InstantCamera>();
            Assert.IsNull(cam.Lens);
            Assert.AreEqual(InstantCamera.CaptureFovY, cam.FovY);
            Assert.AreEqual(50f, cam.FovY);
            Assert.AreEqual(0f, cam.Aperture);
            Assert.AreEqual(0, cam.FilmStock);
            Assert.IsFalse(cam.ClaimsSelectionInput);
            Assert.IsNull(PhotoHolder.PlaceAllowedInZone);
            Assert.IsTrue(PhotoHolder.IsPlaceAllowed(0));
        }

        // ------------------------------------------------------------------ lens view

        [Test]
        public void LensView_SpringsIn_ScalesLook_AndReturnsExactlyToTheBase()
        {
            FirstPersonController fpc = MakePlayer();
            fpc.SetLensView(30f);
            Assert.IsTrue(fpc.HasLensView);
            if (!Feel.ReducedMotion)
            {
                Assert.AreEqual(70f, fpc.ViewFov, 1e-4f, "the lens view springs from the base, no jump");
                StepView(fpc, 0.1f);
                Assert.Less(fpc.ViewFov, 69f, "springing toward the lens");
                Assert.Greater(fpc.ViewFov, 30f, "not there yet");
            }
            StepView(fpc, 3f);
            Assert.AreEqual(30f, fpc.ViewFov, 0.05f);
            Assert.AreEqual(fpc.ViewFov, fpc.Camera.fieldOfView, 1e-3f, "the camera shows the lens view");
            float expected = Mathf.Tan(fpc.ViewFov * 0.5f * Mathf.Deg2Rad) / Mathf.Tan(35f * Mathf.Deg2Rad);
            Assert.AreEqual(expected, fpc.LookScale, 1e-4f, "look scales by tan(view / 2) / tan(base / 2)");

            fpc.ResetViewEffects();
            Assert.AreEqual(30f, fpc.ViewFov, "a reset lands the lens view on its target");

            fpc.ClearLensView();
            StepView(fpc, 3f);
            Assert.IsFalse(fpc.HasLensView, "home again: no lens view left");
            Assert.AreEqual(70f, fpc.ViewFov);
            Assert.AreEqual(1f, fpc.LookScale);
            Assert.AreEqual(70f, fpc.Camera.fieldOfView);

            fpc.SetLensView(500f);
            fpc.ResetViewEffects();
            Assert.AreEqual(FirstPersonController.MaxViewFov, fpc.ViewFov, "clamped");
            fpc.SetLensView(float.NaN);
            Assert.AreEqual(FirstPersonController.MaxViewFov, fpc.ViewFov, "NaN is ignored");
            fpc.ClearLensView();
            fpc.ResetViewEffects();
            Assert.IsFalse(fpc.HasLensView);
            Assert.AreEqual(70f, fpc.ViewFov);
            Assert.AreEqual(70f, fpc.Camera.fieldOfView);
        }

        // ------------------------------------------------------------------ instant camera

        [Test]
        public void Lens_SetAndClear_DrivesFovY_AndRaisesItsEvents()
        {
            var go = new GameObject("LensSeamCamera");
            InstantCamera cam = go.AddComponent<InstantCamera>();
            int lensChanged = 0, changed = 0;
            cam.LensChanged += () => lensChanged++;
            cam.Changed += () => changed++;

            cam.SetLens("85mm", 17.4f);
            Assert.AreEqual("85mm", cam.Lens);
            Assert.AreEqual(17.4f, cam.FovY);
            Assert.AreEqual(1, lensChanged);
            Assert.AreEqual(1, changed);
            cam.SetLens("85mm", 17.4f);
            Assert.AreEqual(1, lensChanged, "no event without a change");

            cam.SetLens("odd", 500f);
            Assert.AreEqual(InstantCamera.MaxLensFovY, cam.FovY);
            cam.SetLens("odd", 0f);
            Assert.AreEqual(InstantCamera.MinLensFovY, cam.FovY);
            cam.SetLens("odd", float.NaN);
            Assert.AreEqual(InstantCamera.CaptureFovY, cam.FovY);
            cam.ClearLens();
            Assert.IsNull(cam.Lens);
            Assert.AreEqual(InstantCamera.CaptureFovY, cam.FovY);

            cam.Aperture = 2.8f;
            Assert.AreEqual(2.8f, cam.Aperture);
            cam.Aperture = -1f;
            Assert.AreEqual(0f, cam.Aperture);
            cam.Aperture = float.NaN;
            Assert.AreEqual(0f, cam.Aperture);
            cam.FilmStock = 3;
            Assert.AreEqual(3, cam.FilmStock);
            cam.FilmStock = -2;
            Assert.AreEqual(0, cam.FilmStock);

            // A failing subscriber is logged and never stops the lens change.
            cam.LensChanged += () => throw new InvalidOperationException("lens kit bug");
            LogAssert.Expect(LogType.Exception, new Regex("lens kit bug"));
            cam.SetLens("50mm", 29.1f);
            Assert.AreEqual(29.1f, cam.FovY);
        }

        [Test]
        public void Capture_UsesTheMountedLens_AndBeforeCaptureComesFirst()
        {
            if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null)
                Assert.Ignore("Capture renders a preview; run with -nographics (BUILD.md) to exercise it in edit mode.");
            FirstPersonController fpc = MakePlayer();
            EnsureProjection();
            InstantCamera cam = fpc.GetComponent<InstantCamera>();
            PhotoInventory inventory = fpc.GetComponent<PhotoInventory>();
            cam.SetUnlockedSilently(true);
            cam.Film = 3;

            int filmSeen = -1, photosSeen = -1, before = 0;
            cam.BeforeCapture += () =>
            {
                before++;
                filmSeen = cam.Film;
                photosSeen = inventory.Count;
            };

            PhotoData plain = cam.TryCapture();
            Assert.IsNotNull(plain);
            Assert.AreEqual(InstantCamera.CaptureFovY, plain.FovY, "no lens: today's 50° photo shape");
            Assert.AreEqual(1, before);
            Assert.AreEqual(3, filmSeen, "before the film is spent");
            Assert.AreEqual(0, photosSeen, "before the photo exists");

            cam.SetLens("85mm", 17.4f);
            var viewfinder = UnityEngine.Object.FindFirstObjectByType<ViewfinderFrame>(FindObjectsInactive.Include);
            Assert.IsNotNull(viewfinder);
            Assert.AreEqual(17.4f, viewfinder.FovY, "the viewfinder frames the mounted lens");
            PhotoData tele = cam.TryCapture();
            Assert.IsNotNull(tele);
            Assert.AreEqual(17.4f, tele.FovY, 1e-4f, "snapshots use the mounted lens");
            Assert.AreEqual(InstantCamera.CaptureAspect, tele.Aspect, 1e-5f);

            cam.ClearLens();
            Assert.AreEqual(InstantCamera.CaptureFovY, viewfinder.FovY);
            cam.Film = 0;
            Assert.IsNull(cam.TryCapture(), "out of film");
            Assert.AreEqual(2, before, "no BeforeCapture without a capture");
        }

        [Test]
        public void CameraMode_ClaimsTheSelectionKeys_OnlyWhenALensKitAsks()
        {
            FirstPersonController fpc = MakePlayer();
            PhotoHolder holder = fpc.GetComponent<PhotoHolder>();
            InstantCamera cam = fpc.GetComponent<InstantCamera>();
            Assert.IsFalse(holder.CameraClaimsInput);

            cam.SetUnlockedSilently(true);
            cam.SetCameraMode(true);
            Assert.IsTrue(cam.IsCameraMode);
            Assert.IsFalse(holder.CameraClaimsInput, "by default camera mode keeps today's controls");

            cam.ClaimsSelectionInput = true;
            Assert.IsTrue(holder.CameraClaimsInput, "a lens kit owns the wheel, digits and Q/E while the camera is out");
            cam.SetCameraMode(false);
            Assert.IsFalse(holder.CameraClaimsInput, "with the camera away the photo keys are the holder's again");
        }

        // ------------------------------------------------------------------ place guard

        [Test]
        public void PlaceGuard_Null_PlacesExactlyAsBefore()
        {
            FirstPersonController fpc = MakePlayer();
            PhotoHolder holder = fpc.GetComponent<PhotoHolder>();
            PhotoInventory inventory = fpc.GetComponent<PhotoInventory>();
            ProjectionSystem ps = EnsureProjection();
            int before = ps.PlacementCount;
            try
            {
                var photo = new PhotoData { FovY = 30f, Aspect = 4f / 3f, Label = "free" };
                inventory.Add(photo);
                int placed = 0;
                holder.PlacedPhoto += p => placed++;
                Assert.IsTrue(holder.Raise());

                Assert.IsNull(PhotoHolder.PlaceAllowedInZone);
                Assert.IsTrue(holder.Place(), "no guard: placing works as before");
                Assert.AreEqual(before + 1, ps.PlacementCount);
                Assert.IsFalse(holder.IsRaised);
                Assert.IsFalse(inventory.Contains(photo), "the placed photo is consumed");
                Assert.AreEqual(1, placed);
            }
            finally { RewindTo(ps, before); }
        }

        [Test]
        public void PlaceGuard_ReturningFalse_RefusesPlace_AndThePhotoStaysHeld()
        {
            FirstPersonController fpc = MakePlayer();
            PhotoHolder holder = fpc.GetComponent<PhotoHolder>();
            PhotoInventory inventory = fpc.GetComponent<PhotoInventory>();
            ProjectionSystem ps = EnsureProjection();
            int before = ps.PlacementCount;
            try
            {
                var photo = new PhotoData { FovY = 30f, Aspect = 4f / 3f, Label = "guarded" };
                inventory.Add(photo);
                int placed = 0;
                holder.PlacedPhoto += p => placed++;
                Assert.IsTrue(holder.Raise());

                ZoneInfo.ZoneOfProvider = p => 7;
                var asked = new List<int>();
                var prompted = new List<int>();
                PhotoHolder.PlaceAllowedInZone = zone =>
                {
                    asked.Add(zone);
                    return false;
                };
                PhotoHolder.PlaceRefusedPrompt = zone =>
                {
                    prompted.Add(zone);
                    return "This room is protected";
                };

                Assert.IsFalse(holder.Place(), "the guard refuses Place");
                Assert.IsFalse(holder.PlaceWithPress(), "and the player's press");
                Assert.IsFalse(holder.IsPlacing, "no press-in started");
                Assert.IsTrue(holder.IsRaised, "the photo stays held up");
                Assert.AreSame(photo, holder.RaisedPhoto);
                Assert.IsTrue(inventory.Contains(photo), "nothing consumed");
                Assert.AreEqual(before, ps.PlacementCount, "nothing placed");
                Assert.AreEqual(0, placed);
                CollectionAssert.AreEqual(new[] { 7, 7 }, asked, "asked with the zone the player stands in");
                CollectionAssert.AreEqual(new[] { 7, 7 }, prompted, "the refusal shows the prompt line");

                // A guard that allows this zone: the same held photo places as before.
                PhotoHolder.PlaceAllowedInZone = zone => zone == 7;
                Assert.IsTrue(holder.Place());
                Assert.AreEqual(before + 1, ps.PlacementCount);
                Assert.IsFalse(inventory.Contains(photo));
                Assert.AreEqual(1, placed);
            }
            finally { RewindTo(ps, before); }
        }

        [Test]
        public void PlaceGuard_EveryGuardMustAgree_AndAThrowingGuardIsIgnored()
        {
            PhotoHolder.PlaceAllowedInZone = zone => true;
            PhotoHolder.PlaceAllowedInZone += zone => zone != 3;
            Assert.IsTrue(PhotoHolder.IsPlaceAllowed(2));
            Assert.IsFalse(PhotoHolder.IsPlaceAllowed(3), "one refusing guard is enough");

            PhotoHolder.PlaceAllowedInZone = zone => throw new InvalidOperationException("guard bug");
            LogAssert.Expect(LogType.Exception, new Regex("guard bug"));
            Assert.IsTrue(PhotoHolder.IsPlaceAllowed(3), "a broken guard never blocks the game");

            PhotoHolder.PlaceAllowedInZone = null;
            Assert.IsTrue(PhotoHolder.IsPlaceAllowed(3));
        }

        // ------------------------------------------------------------------ film branch (shaders)

        static readonly string[] GradedShaders = { "Ion/FlatToon", "Ion/Backdrop", "Ion/GradientSky" };

        [Test]
        public void FilmBranch_CompilesForWebGL2_InEveryGradedShader()
        {
            foreach (string name in GradedShaders)
            {
                Shader shader = Shader.Find(name);
                Assert.IsNotNull(shader, name);
                ShaderData data = ShaderUtil.GetShaderData(shader);
                int passes = 0;
                var filmPasses = new List<string>();
                var errors = new List<string>();
                for (int s = 0; s < data.SubshaderCount; s++)
                {
                    ShaderData.Subshader sub = data.GetSubshader(s);
                    for (int p = 0; p < sub.PassCount; p++)
                    {
                        ShaderData.Pass pass = sub.GetPass(p);
                        if (!pass.HasShaderStage(ShaderType.Vertex)) continue;
                        // GLES3x (WebGL2) compiles every stage of the pass under the Vertex type.
                        ShaderData.VariantCompileInfo info = pass.CompileVariant(ShaderType.Vertex, new string[0],
                            ShaderCompilerPlatform.GLES3x, BuildTarget.WebGL);
                        passes++;
                        if (!info.Success) errors.Add(pass.Name + ": compile failed");
                        foreach (ShaderMessage m in info.Messages)
                            if (m.severity == ShaderCompilerMessageSeverity.Error)
                                errors.Add(pass.Name + ": " + m.message + " (" + m.file + ":" + m.line + ")");
                        if (UsesFilmGlobals(info)) filmPasses.Add(pass.Name);
                    }
                }
                Debug.Log("[LensSeam] " + name + " GLES3x (WebGL2): " + passes + " passes compiled, " + errors.Count +
                          " errors, film globals in: " + string.Join(", ", filmPasses));
                Assert.IsEmpty(errors, name + " must compile for WebGL2");
                Assert.Greater(passes, 0, name);
                Assert.IsNotEmpty(filmPasses, name + ": the IonGrade film branch is compiled in");
            }
        }

        static bool UsesFilmGlobals(ShaderData.VariantCompileInfo info)
        {
            if (info.ConstantBuffers != null)
                foreach (ShaderData.ConstantBufferInfo cb in info.ConstantBuffers)
                    if (cb.Fields != null)
                        foreach (ShaderData.ConstantInfo c in cb.Fields)
                            if (c.Name == "_IonFilmP") return true;
            // GLES keeps plain uniforms in the GLSL text.
            return info.ShaderData != null && Encoding.ASCII.GetString(info.ShaderData).Contains("_IonFilmP");
        }
    }
}
