using System.Collections.Generic;
using UnityEngine;

namespace Ion.Gameplay
{
    /// <summary>
    /// Something the player uses with E, beyond photo pickups and switches (which <see cref="PlayerInteractor"/>
    /// handles itself): an exhibit, a terminal, an arcade cabinet. <see cref="PlayerInteractor"/> focuses the nearest
    /// usable in reach and roughly in view, shows "[E] <see cref="UsePrompt"/>" and calls <see cref="Use"/> on E.
    /// Register through <see cref="UsableRegistry"/>; <see cref="UsableBehaviour"/> does it on enable.
    /// </summary>
    public interface IUsable
    {
        /// <summary>World point the player has to be near and looking at.</summary>
        Vector3 FocusPoint { get; }

        /// <summary>Reach in metres from the eye to <see cref="FocusPoint"/>.</summary>
        float UseRange { get; }

        /// <summary>The verb after the key glyph, e.g. "play", "read", "open".</summary>
        string UsePrompt { get; }

        /// <summary>False hides the prompt and ignores E.</summary>
        bool CanUse { get; }

        void Use();
    }

    /// <summary>The usables in the scene (enabled ones only; photo templates register once pasted).</summary>
    public static class UsableRegistry
    {
        static readonly List<IUsable> s_items = new List<IUsable>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics() => s_items.Clear();

        public static IReadOnlyList<IUsable> Items => s_items;

        public static void Register(IUsable usable)
        {
            if (usable != null && !s_items.Contains(usable)) s_items.Add(usable);
        }

        public static void Unregister(IUsable usable) => s_items.Remove(usable);
    }

    /// <summary>
    /// Base class for usables: serialized focus offset, range and prompt (so photo copies keep them) and registry
    /// membership while enabled. Subclasses implement <see cref="Use"/>.
    /// </summary>
    public abstract class UsableBehaviour : MonoBehaviour, IUsable
    {
        [SerializeField] Vector3 _focusOffset;
        [SerializeField] float _range = 2.25f;
        [SerializeField] string _prompt = "use";

        /// <summary>Focus point in local space.</summary>
        public Vector3 FocusOffset { get => _focusOffset; set => _focusOffset = value; }

        public Vector3 FocusPoint => transform.TransformPoint(_focusOffset);

        public float UseRange { get => _range; set => _range = value; }

        public string UsePrompt { get => _prompt; set => _prompt = value; }

        public virtual bool CanUse => isActiveAndEnabled;

        public abstract void Use();

        protected virtual void OnEnable() => UsableRegistry.Register(this);

        protected virtual void OnDisable() => UsableRegistry.Unregister(this);
    }
}
