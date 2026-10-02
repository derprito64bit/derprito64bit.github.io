using System.Collections;
using Ion.Gameplay;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Ion.Tests.PlayMode
{
    /// <summary>The usable seam: a usable in reach and in view is focused, prompted and used with E.</summary>
    public sealed class UsableTests : IonPlayTestBase
    {
        sealed class Counter : UsableBehaviour
        {
            public int Uses;
            public override void Use() => Uses++;
        }

        [UnityTest]
        public IEnumerator Usable_InReachAndView_IsFocusedAndUsed()
        {
            yield return GoTo("gallery");
            Transform eye = Player.Camera.transform;
            var go = new GameObject("Test usable");
            go.transform.position = eye.position + eye.forward * 1.5f;
            var usable = go.AddComponent<Counter>();
            usable.UsePrompt = "test";
            yield return null;
            yield return null;

            var interactor = Player.GetComponent<PlayerInteractor>();
            Assert.AreSame(usable, interactor.FocusedUsable, "the usable in front of the player is focused");
            Assert.IsTrue(interactor.UseFocused());
            Assert.AreEqual(1, usable.Uses);

            go.transform.position = eye.position + eye.forward * 6f;
            yield return null;
            yield return null;
            Assert.IsNull(interactor.FocusedUsable, "out of reach");

            go.transform.position = eye.position - eye.forward * 1.5f + Vector3.up * 0.1f;
            yield return null;
            yield return null;
            Assert.IsNull(interactor.FocusedUsable, "behind the player");
            Object.Destroy(go);
        }
    }
}
