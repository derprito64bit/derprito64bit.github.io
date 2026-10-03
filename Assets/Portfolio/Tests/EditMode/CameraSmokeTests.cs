using Ion.Portfolio;
using NUnit.Framework;
using UnityEngine;

namespace Ion.Tests.EditMode
{
    /// <summary>M-F seed for M-C3: the CameraRoomKit contract compiles, and the stub furnishes nothing.</summary>
    public sealed class CameraSmokeTests
    {
        [Test]
        public void CameraRoomKit_FurnishesNothingYet()
        {
            var root = new GameObject("smoke camera room").transform;
            try
            {
                CameraRoomKit.Furnish(root, null);
                Assert.AreEqual(0, root.childCount);
            }
            finally
            {
                Object.DestroyImmediate(root.gameObject);
            }
        }
    }
}
