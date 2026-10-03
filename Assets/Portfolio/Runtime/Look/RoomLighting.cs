using Ion.Levels;
using UnityEngine;

namespace Ion.Portfolio
{
    /// <summary>
    /// Candle fixtures per room (M-D013): chandeliers, girandoles, plinth cups, door sconces and the few registered
    /// lights. Contract stub seeded by M-F: <see cref="Light"/> builds nothing yet. M-C4 owns it.
    /// </summary>
    public static class RoomLighting
    {
        /// <summary>Lights the room <paramref name="key"/> built under <paramref name="root"/>.</summary>
        public static void Light(Transform root, RoomContext ctx, string key) { }
    }
}
