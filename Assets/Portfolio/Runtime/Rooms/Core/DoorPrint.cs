using Ion.Levels;
using Ion.Levels.Arch;
using UnityEngine;

namespace Ion.Portfolio
{
    /// <summary>
    /// The Manor signature (M-D025): a 1.6 x 1.2 m instant print beside a door, showing the target room from its
    /// arrival pose. Contract stub seeded by M-F: <see cref="Hang"/> hangs nothing and returns null. M-C2 owns it.
    /// </summary>
    public static class DoorPrint
    {
        /// <summary>Hangs <paramref name="shot"/>'s print centred on the wall face at <paramref name="centre"/>.</summary>
        public static GameObject Hang(Transform p, Vector3 centre, Dir facing, DioramaShot shot) => null;
    }
}
