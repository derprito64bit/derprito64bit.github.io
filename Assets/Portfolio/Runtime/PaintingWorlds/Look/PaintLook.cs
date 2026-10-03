using System;
using UnityEngine;

namespace Ion.Portfolio
{
    /// <summary>Per-world painted-look parameters (G-C2 defines the fields).</summary>
    [Serializable]
    public sealed class PaintParams
    {
        public string Name;
    }

    /// <summary>
    /// The Painting Worlds' painted look over a world and its diorama. Contract stub seeded by M-F: <see cref="Apply"/>
    /// changes nothing until G-C2 implements it.
    /// </summary>
    public static class PaintLook
    {
        public static void Apply(Transform root, Transform diorama, PaintParams look) { }
    }
}
