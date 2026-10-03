using Ion.Levels.Arch;
using Ion.Levels.Props;
using Ion.Presentation;
using UnityEngine;
using B = Ion.Levels.Props.PropBuild;

namespace Ion.Portfolio
{
    /// <summary>The Manor's exhibit pieces (moved from ManorKit.cs): gilt picture frames, the arcade cabinet and
    /// placeholder paintings. The candles and decor half of the class lives in Look/ManorKit.cs.</summary>
    public static partial class ManorKit
    {
        /// <summary>
        /// A gilt painting: the gallery frame (PropKit) holding <paramref name="image"/>, inside a brass outer frame.
        /// <paramref name="center"/> is the image centre on the wall face; <paramref name="facing"/> points into the room.
        /// </summary>
        public static GameObject Painting(Transform p, Vector3 center, Dir facing, Vector2 imageSize, Texture2D image)
        {
            GameObject frame = PropKit.PictureFrame(p, center, facing, imageSize, image, FrameStyle.Gallery);
            Transform g = B.Frame(p, "Gilt", center, B.Yaw(facing));
            float hw = imageSize.x * 0.5f + 0.22f, hh = imageSize.y * 0.5f + 0.22f;
            const float t = 0.09f, z0 = 0f, z1 = 0.08f;
            B.Box(g, -hw - t, hh, z0, hw + t, hh + t, z1, Mat.Brass, Soft);
            B.Box(g, -hw - t, -hh - t, z0, hw + t, -hh, z1, Mat.Brass, Soft);
            B.Box(g, -hw - t, -hh, z0, -hw, hh, z1, Mat.Brass, Soft);
            B.Box(g, hw, -hh, z0, hw + t, hh, z1, Mat.Brass, Soft);
            B.Exempt(g);
            return frame;
        }

        /// <summary>
        /// An arcade cabinet facing <paramref name="facing"/>: a walnut body with graphite sides, a lit marquee, a
        /// screen and a control deck.
        /// </summary>
        public static GameObject ArcadeCabinet(Transform p, Vector3 basePos, Dir facing)
        {
            Transform g = B.Frame(p, "Arcade cabinet", basePos, B.Yaw(facing));
            const float w = 0.375f;
            B.Box(g, -w, 0f, -0.375f, w, 1.75f, 0.25f, Mat.Walnut, ArchFlags.Solid);           // body
            B.Box(g, -w - 0.04f, 0f, -0.4f, -w, 1.85f, 0.3f, Mat.Graphite, Soft);             // side panels
            B.Box(g, w, 0f, -0.4f, w + 0.04f, 1.85f, 0.3f, Mat.Graphite, Soft);
            B.Box(g, -w, 1.6f, 0.25f, w, 1.85f, 0.32f, Mat.Warm, Soft);                       // marquee
            B.Box(g, -0.3f, 1.05f, 0.25f, 0.3f, 1.5f, 0.27f, Mat.Graphite, Soft);             // screen bezel
            B.Box(g, -w, 0.9f, 0.25f, w, 0.98f, 0.55f, Mat.Graphite, Soft);                    // control deck
            B.Prism(g, new Vector3(-0.18f, 0.98f, 0.42f), 0.03f, 0.08f, 6, Mat.Graphite, Soft); // stick
            foreach (float x in new[] { 0.05f, 0.15f, 0.25f })
                B.Prism(g, new Vector3(x, 0.98f, 0.42f), 0.03f, 0.02f, 8, Mat.TextileRed, Soft);
            B.Exempt(g);
            // The lit marquee and the attract screen are prints (unlit, so they glow in the candlelight).
            Print(p, g, "Arcade marquee", new Vector3(0f, 1.725f, 0.321f), new Vector2(0.7f, 0.2f), ArcadeMarquee());
            Print(p, g, "Arcade screen", new Vector3(0f, 1.275f, 0.271f), new Vector2(0.52f, 0.39f), ArcadeScreen());
            return g.gameObject;
        }

        static void Print(Transform p, Transform g, string name, Vector3 local, Vector2 size, Texture2D image)
        {
            PrintCard card = PrintBuilder.Build(p, name, g.localPosition + g.localRotation * local, g.localRotation, size,
                                                0f, false, false, true);
            card.Image = image;
        }

        static Texture2D s_marquee, s_screen;

        /// <summary>"ARCADE" in a 5x7 pixel font, burgundy on a warm lit panel with brass rules.</summary>
        static Texture2D ArcadeMarquee()
        {
            if (s_marquee != null) return s_marquee;
            const int w = 96, h = 28, scale = 2;
            var px = new Color32[w * h];
            Color top = new Color(1f, 0.88f, 0.66f), bottom = new Color(0.98f, 0.72f, 0.42f);
            for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                Color c = Color.Lerp(bottom, top, y / (float)(h - 1));
                if (y < 2 || y >= h - 2) c = new Color(0.77f, 0.6f, 0.27f);
                px[y * w + x] = c;
            }
            string[] glyphs = { Glyph.A, Glyph.R, Glyph.C, Glyph.A, Glyph.D, Glyph.E };
            int textW = (glyphs.Length * 6 - 1) * scale, x0 = (w - textW) / 2, y0 = (h - 7 * scale) / 2;
            var ink = new Color32(0x6B, 0x1E, 0x2A, 0xFF);
            for (int i = 0; i < glyphs.Length; i++)
            for (int row = 0; row < 7; row++)
            for (int col = 0; col < 5; col++)
            {
                if (glyphs[i][row * 5 + col] != '#') continue;
                for (int sy = 0; sy < scale; sy++)
                for (int sx = 0; sx < scale; sx++)
                    px[(y0 + (6 - row) * scale + sy) * w + x0 + (i * 6 + col) * scale + sx] = ink;
            }
            return s_marquee = PixelTexture("Arcade marquee", w, h, px);
        }

        /// <summary>An attract screen for the brick breaker: rows of bricks, a paddle, a ball and scanlines.</summary>
        static Texture2D ArcadeScreen()
        {
            if (s_screen != null) return s_screen;
            const int w = 64, h = 48;
            var px = new Color32[w * h];
            var bg = new Color32(0x0E, 0x0A, 0x0C, 0xFF);
            for (int i = 0; i < px.Length; i++) px[i] = bg;
            Color32[] rows =
            {
                new Color32(0x6B, 0x1E, 0x2A, 0xFF), new Color32(0x9B, 0x2C, 0x34, 0xFF), new Color32(0xC5, 0x9A, 0x45, 0xFF),
                new Color32(0xD6, 0xA4, 0x3B, 0xFF), new Color32(0x3E, 0x8C, 0x86, 0xFF),
            };
            for (int r = 0; r < rows.Length; r++)
            for (int b = 0; b < 8; b++)
            {
                if ((r == 3 && b == 5) || (r == 4 && (b == 2 || b == 6))) continue;   // a game in progress
                int bx = 4 + b * 7, by = h - 8 - r * 4;
                for (int y = by; y < by + 3; y++)
                for (int x = bx; x < bx + 6; x++) px[y * w + x] = rows[r];
            }
            var brass = new Color32(0xC5, 0x9A, 0x45, 0xFF);
            for (int x = 26; x < 38; x++) { px[4 * w + x] = brass; px[5 * w + x] = brass; }
            var ball = new Color32(0xFF, 0xD9, 0xA0, 0xFF);
            for (int y = 16; y < 18; y++) for (int x = 40; x < 42; x++) px[y * w + x] = ball;
            for (int y = 0; y < h; y += 2)
            for (int x = 0; x < w; x++)
            {
                Color32 c = px[y * w + x];
                px[y * w + x] = new Color32((byte)(c.r * 0.82f), (byte)(c.g * 0.82f), (byte)(c.b * 0.82f), 0xFF);
            }
            return s_screen = PixelTexture("Arcade screen", w, h, px);
        }

        static Texture2D PixelTexture(string name, int w, int h, Color32[] px)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false)
            {
                name = name,
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point,
            };
            tex.SetPixels32(px);
            tex.Apply(false, true);
            return tex;
        }

        /// <summary>5x7 pixel glyphs, rows top to bottom.</summary>
        static class Glyph
        {
            public const string A = ".###.#...##...#######...##...##...#";
            public const string R = "####.#...##...#####.#.#..#..#.#...#";
            public const string C = ".#####....#....#....#....#.....####";
            public const string D = "####.#...##...##...##...##...#####.";
            public const string E = "######....#....####.#....#....#####";
        }

        /// <summary>
        /// A placeholder "oil painting" for a project with no artwork yet: a vertical gradient from the accent colour,
        /// a soft glow and a vignette, deterministic per seed.
        /// </summary>
        public static Texture2D PlaceholderPainting(Color accent, int seed, int width = 192, int height = 144)
        {
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                name = "Placeholder painting " + seed,
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear,
            };
            var px = new Color32[width * height];
            Color top = Color.Lerp(accent, new Color(1f, 0.93f, 0.8f), 0.35f);
            Color bottom = accent * 0.45f;
            float ox = (seed * 37 % 100) / 100f, oy = (seed * 53 % 100) / 100f;
            float gx = 0.3f + 0.4f * ox, gy = 0.45f + 0.25f * oy;
            for (int y = 0; y < height; y++)
            {
                float v = y / (float)(height - 1);
                for (int x = 0; x < width; x++)
                {
                    float u = x / (float)(width - 1);
                    Color c = Color.Lerp(bottom, top, v);
                    float glow = Mathf.Exp(-((u - gx) * (u - gx) * 9f + (v - gy) * (v - gy) * 14f));
                    c = Color.Lerp(c, new Color(1f, 0.86f, 0.6f), glow * 0.45f);
                    float brush = Mathf.PerlinNoise(u * 18f + seed, v * 6f + seed * 0.5f) - 0.5f;
                    c *= 1f + brush * 0.12f;
                    float dx = u - 0.5f, dy = v - 0.5f;
                    c *= 1f - Mathf.Clamp01((dx * dx + dy * dy) * 1.6f) * 0.55f;
                    c.a = 1f;
                    px[y * width + x] = c;
                }
            }
            tex.SetPixels32(px);
            tex.Apply(false, true);
            return tex;
        }
    }
}
