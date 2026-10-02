using Ion.Levels.Arch;
using Ion.Presentation;
using UnityEngine;

namespace Ion.Portfolio
{
    /// <summary>
    /// The Manor's look: a royal home after dark. Marble floors, cream plaster, walnut panelling, red carpets and
    /// brass, lit by candles. One mood (Candlelight) and one architecture style (Manor), both fork-only.
    /// </summary>
    public static class ManorLook
    {
        static ZoneMood s_candlelight;
        static bool s_made;

        /// <summary>Dim, warm and close: a low amber "sun" from above, deep plum shade, a warm haze.</summary>
        public static ZoneMood Candlelight
        {
            get
            {
                if (!s_made)
                {
                    s_candlelight = ZoneMood.Make("Candlelight", "#1A1216", "#3A2620", "#FFB46A", "#3E2732",
                                                  new Vector3(0.22f, 0.92f, -0.30f), 0.82f, 0.024f, 12f);
                    s_candlelight.ShadeValue = 0.4f;
                    s_candlelight.AmbientSky = Dim(s_candlelight.AmbientSky, 0.62f);
                    s_candlelight.AmbientEquator = Dim(s_candlelight.AmbientEquator, 0.66f);
                    s_candlelight.AmbientGround = Dim(s_candlelight.AmbientGround, 0.6f);
                    s_made = true;
                }
                return s_candlelight;
            }
        }

        /// <summary>Marble checker floors, smooth plaster walls, walnut trims and beams, red panels, brass metal.</summary>
        public static ArchStyle Style => new ArchStyle
        {
            Floor = new Surf(Mat.Limestone, Pat.Checker),
            Wall = new Surf(Mat.Plaster, Pat.Ashlar),
            Trim = new Surf(Mat.Walnut, Pat.Boards),
            Base = new Surf(Mat.Concrete, Pat.Courses),
            Accent = new Surf(Mat.TextileRed, Pat.None),
            Ceiling = new Surf(Mat.Plaster, Pat.Coffer),
            Wood = new Surf(Mat.Walnut, Pat.Boards),
            Metal = Mat.Brass,
            Bracket = new Surf(Mat.Walnut, Pat.Boards),
            Inlay = Mat.Brass,
            Panel = Mat.TextileRed,
        };

        static Color Dim(Color c, float k) => new Color(c.r * k, c.g * k, c.b * k, c.a);
    }
}
