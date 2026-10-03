using System;
using Ion.Gameplay;
using UnityEngine;

namespace Ion.Portfolio
{
    /// <summary>
    /// E reads an exhibit (M-D009): range 4.5 m, prompt 'read'; Use opens the DOM wall label with the exhibit's content
    /// and then calls onRead. Contract stub seeded by M-F: <see cref="Attach"/> adds nothing and returns null until
    /// M-C5 implements it.
    /// </summary>
    public sealed class ExhibitUsable : UsableBehaviour
    {
        [SerializeField] ExhibitRef _exhibit;

        public ExhibitRef Exhibit => _exhibit;

        public override bool CanUse => false;

        public override void Use() { }

        /// <summary>Makes <paramref name="host"/> readable with E, focused at <paramref name="focus"/> (host-local).</summary>
        public static ExhibitUsable Attach(Transform host, Vector3 focus, ExhibitRef exhibit, float range = 4.5f,
                                           Action onRead = null) => null;
    }
}
