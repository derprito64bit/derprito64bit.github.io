using Ion.Levels.Arch;
using Ion.Levels.Props;
using Ion.Presentation;
using Ion.Presentation.Quality;
using UnityEngine;
using B = Ion.Levels.Props.PropBuild;

namespace Ion.Portfolio
{
    /// <summary>
    /// The Manor's furnishings: candles, candelabras, chandeliers, candle sconces, carpet runners and wainscoting (the
    /// gilt frames and the arcade cabinet are the other half of this class, in Exhibits/ManorKit.Exhibits.cs).
    /// Everything is soft clutter (exempt from the architecture grid, merged by material at bake) built from PropBuild
    /// primitives, so it cuts, pastes and batches like the rest of the world. Candle glows register Ultra-tier local
    /// lights (LocalLights), which UltraFx enables near the camera.
    /// </summary>
    public static partial class ManorKit
    {
        const ArchFlags Soft = ArchFlags.Soft;
        static readonly Color CandleLight = new Color(1f, 0.72f, 0.42f);

        /// <summary>A pillar candle standing at <paramref name="basePos"/> (local to <paramref name="g"/>).</summary>
        static void Candle(Transform g, Vector3 basePos, float height)
        {
            B.Prism(g, basePos, 0.035f, height, 6, Mat.Paper, Soft);
            B.Prism(g, basePos + new Vector3(0f, height, 0f), 0.018f, 0.07f, 4, Mat.Warm, Soft);
        }

        /// <summary>A brass floor candelabra (1.5 m) with five candles on a cross arm.</summary>
        public static GameObject Candelabra(Transform p, Vector3 basePos, float yaw)
        {
            Transform g = B.Frame(p, "Candelabra", basePos, yaw);
            B.Prism(g, Vector3.zero, 0.2f, 0.05f, 8, Mat.Brass, Soft);
            B.Prism(g, new Vector3(0f, 0.05f, 0f), 0.04f, 1.25f, 6, Mat.Brass, Soft);
            B.Box(g, -0.4f, 1.28f, -0.025f, 0.4f, 1.32f, 0.025f, Mat.Brass, Soft);
            for (int i = -2; i <= 2; i++)
            {
                float x = i * 0.2f;
                float lift = 0.06f * (2 - Mathf.Abs(i));
                B.Prism(g, new Vector3(x, 1.32f, 0f), 0.04f, 0.03f + lift, 6, Mat.Brass, Soft);
                Candle(g, new Vector3(x, 1.35f + lift, 0f), 0.16f);
            }
            B.Exempt(g);
            LocalLights.Add(g, new Vector3(0f, 1.6f, 0f), CandleLight, 4.5f, 1.2f);
            return g.gameObject;
        }

        /// <summary>
        /// A brass chandelier hanging <paramref name="drop"/> below the ceiling point <paramref name="ceilingPos"/>:
        /// a chain, a hub and a ring of <paramref name="candles"/> candles (a second, smaller ring when there are 8+).
        /// </summary>
        public static GameObject Chandelier(Transform p, Vector3 ceilingPos, float drop, int candles = 8)
        {
            Transform g = B.Frame(p, "Chandelier", ceilingPos, 0f);
            float d = Mathf.Max(0.5f, drop);
            B.Box(g, -0.015f, -d + 0.2f, -0.015f, 0.015f, 0f, 0.015f, Mat.Brass, Soft);       // chain
            B.Prism(g, new Vector3(0f, -d - 0.05f, 0f), 0.12f, 0.25f, 8, Mat.Brass, Soft);      // hub
            Ring(g, -d, 0.75f, candles);
            if (candles >= 8) Ring(g, -d + 0.3f, 0.4f, candles / 2);
            B.Prism(g, new Vector3(0f, -d - 0.25f, 0f), 0.04f, 0.2f, 6, Mat.Brass, Soft);       // finial
            B.Exempt(g);
            LocalLights.Add(g, new Vector3(0f, -d + 0.1f, 0f), CandleLight, 9f, 1.8f);
            return g.gameObject;
        }

        static void Ring(Transform g, float y, float radius, int candles)
        {
            int n = Mathf.Max(3, candles);
            for (int i = 0; i < n; i++)
            {
                float a = i * Mathf.PI * 2f / n;
                var at = new Vector3(Mathf.Cos(a) * radius, y, Mathf.Sin(a) * radius);
                // An arm from the hub to the cup, then a cup and a candle.
                B.Beam(g, new Vector3(0f, y, 0f), at, 0.025f, Mat.Brass, Soft);
                B.Prism(g, at, 0.05f, 0.04f, 6, Mat.Brass, Soft);
                Candle(g, at + new Vector3(0f, 0.04f, 0f), 0.14f);
            }
        }

        /// <summary>A two-candle brass sconce on a wall face (<paramref name="outward"/> points into the room).</summary>
        public static GameObject CandleSconce(Transform p, Vector3 wallFace, Dir outward)
        {
            Transform g = B.Frame(p, "Candle sconce", wallFace, B.Yaw(outward));
            B.Box(g, -0.1f, -0.25f, 0f, 0.1f, 0.15f, 0.03f, Mat.Brass, Soft);                  // back plate
            B.Box(g, -0.2f, -0.13f, 0.03f, 0.2f, -0.1f, 0.2f, Mat.Brass, Soft);                // arm
            foreach (float x in new[] { -0.17f, 0.17f })
            {
                B.Prism(g, new Vector3(x, -0.1f, 0.17f), 0.045f, 0.03f, 6, Mat.Brass, Soft);
                Candle(g, new Vector3(x, -0.07f, 0.17f), 0.15f);
            }
            B.Exempt(g);
            LocalLights.Add(g, new Vector3(0f, 0.1f, 0.35f), CandleLight, 3.5f, 1f);
            return g.gameObject;
        }

        /// <summary>
        /// A red carpet runner on the floor: TextileRed field, a Mustard inner border and brass edge strips.
        /// Walk-through (no collider), 0.03 high.
        /// </summary>
        public static GameObject CarpetRunner(Transform p, RectXZ r)
        {
            Transform g = B.Frame(p, "Carpet", Vector3.zero, 0f);
            const float h = 0.03f, edge = 0.06f, border = 0.12f;
            B.Box(g, r.X0, 0f, r.Z0, r.X1, h, r.Z1, Mat.TextileRed, Soft);
            bool alongZ = r.Depth >= r.Width;
            if (alongZ)
            {
                foreach (float x in new[] { r.X0, r.X1 - edge })
                    B.Box(g, x, 0f, r.Z0, x + edge, h + 0.005f, r.Z1, Mat.Brass, Soft);
                foreach (float x in new[] { r.X0 + edge + border, r.X1 - edge - border - 0.05f })
                    B.Box(g, x, 0f, r.Z0 + 0.3f, x + 0.05f, h + 0.004f, r.Z1 - 0.3f, Mat.Mustard, Soft);
            }
            else
            {
                foreach (float z in new[] { r.Z0, r.Z1 - edge })
                    B.Box(g, r.X0, 0f, z, r.X1, h + 0.005f, z + edge, Mat.Brass, Soft);
                foreach (float z in new[] { r.Z0 + edge + border, r.Z1 - edge - border - 0.05f })
                    B.Box(g, r.X0 + 0.3f, 0f, z, r.X1 - 0.3f, h + 0.004f, z + 0.05f, Mat.Mustard, Soft);
            }
            B.Exempt(g);
            return g.gameObject;
        }

        /// <summary>
        /// Walnut wainscoting along a wall face from <paramref name="from"/> to <paramref name="to"/> (on the face,
        /// floor level): framed panels up to <paramref name="height"/> with a brass dado rail.
        /// </summary>
        public static GameObject Wainscot(Transform p, Vector3 from, Vector3 to, Dir outward, float height = 1.25f)
        {
            Transform g = B.Frame(p, "Wainscot", Vector3.zero, 0f);
            Vector3 o = B.Vec(outward);
            Vector3 along = (to - from);
            float len = along.magnitude;
            if (len < 0.1f) return g.gameObject;
            along /= len;
            int panels = Mathf.Max(1, Mathf.RoundToInt(len / 1.5f));
            float step = len / panels;
            // Base board, the panel field and a brass rail.
            AddSlab(g, from, along, o, 0f, len, 0.05f, 0f, height, Mat.Walnut);
            AddSlab(g, from, along, o, 0f, len, 0.075f, height, height + 0.06f, Mat.Brass);
            for (int i = 0; i < panels; i++)
            {
                float a0 = i * step + 0.12f, a1 = (i + 1) * step - 0.12f;
                AddSlab(g, from, along, o, a0, a1, 0.065f, 0.62f, height - 0.12f, Mat.Walnut);
            }
            B.Exempt(g);
            return g.gameObject;
        }

        // A box along a wall face: from a0 to a1 along the wall, out from the face to 'depth', y0 to y1.
        static void AddSlab(Transform g, Vector3 origin, Vector3 along, Vector3 outward, float a0, float a1, float depth,
                            float y0, float y1, Mat m)
        {
            Vector3 p0 = origin + along * a0;
            Vector3 p1 = origin + along * a1 + outward * depth;
            var min = new Vector3(Mathf.Min(p0.x, p1.x), y0, Mathf.Min(p0.z, p1.z));
            var max = new Vector3(Mathf.Max(p0.x, p1.x), y1, Mathf.Max(p0.z, p1.z));
            if (max.x - min.x < 0.01f) max.x = min.x + 0.01f;
            if (max.z - min.z < 0.01f) max.z = min.z + 0.01f;
            B.Box(g, min, max, m, Soft);
        }
    }
}
