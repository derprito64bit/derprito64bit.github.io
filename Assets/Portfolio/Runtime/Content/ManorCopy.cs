using System;
using System.Collections.Generic;
using System.Text;
using Ion.Levels.Rooms;
using UnityEngine;
using UnityEngine.UI;

namespace Ion.Portfolio
{
    /// <summary>
    /// Placeholder hygiene (M-D011) for every visible string in the Manor. Owner facts come only from the content
    /// JSON, where they read "[PLACEHOLDER: what goes here]" until owner-facts.md supplies them. The only other visible
    /// text is the fixed wayfinding copy below: room names, door labels and how-to hints, none of which states a fact
    /// about the owner. <see cref="IsSourced"/> is the placeholder guard's rule.
    /// </summary>
    public static class ManorCopy
    {
        public const string PlaceholderPrefix = "[PLACEHOLDER";

        /// <summary>Room names, door labels and instructions. Add a string only if it states no fact about the owner.</summary>
        public static readonly string[] Wayfinding =
        {
            "The Manor", "Grand Gallery", "Hall of Honours", "Camera Room", "Painting Wing", "Workshop", "Study",
            "[ GRAND GALLERY ]", "[ PLAY THE GAME ]", "[ FOYER ]", "[ ARCADE ]",
            "every project, framed", "photo puzzles", "portfolio",
            "Left: the Grand Gallery.  Right: the game.", "Every project, framed.",
            "Welcome. Left door: every project in the Grand Gallery. Right door: play the game.",
            "The arcade: press E at the cabinet to play.",
        };

        public static bool IsPlaceholder(string text) =>
            text != null && text.TrimStart().StartsWith(PlaceholderPrefix, StringComparison.Ordinal);

        /// <summary>
        /// True if <paramref name="text"/> may be shown: empty, wayfinding copy, the owner's handle in brackets
        /// (projects.json 'owner'), or text made only of "[PLACEHOLDER: ...]" parts, "[NN]" plaque numbers and
        /// punctuation (a placeholder, or a plaque or caption composed from placeholder content). Words outside the
        /// brackets fail, even after a placeholder.
        /// </summary>
        public static bool IsSourced(string text, string ownerHandle = null)
        {
            if (string.IsNullOrWhiteSpace(text)) return true;
            string t = text.Trim();
            if (Array.IndexOf(Wayfinding, t) >= 0) return true;
            if (!string.IsNullOrEmpty(ownerHandle) && t == "[" + ownerHandle + "]") return true;
            var rest = new StringBuilder();
            for (int i = 0; i < t.Length; i++)
            {
                if (t[i] != '[')
                {
                    rest.Append(t[i]);
                    continue;
                }
                int close = t.IndexOf(']', i);
                if (close < 0) return false;
                string inner = t.Substring(i, close - i + 1);
                if (!IsPlaceholder(inner) && !IsPlaqueNumber(inner)) return false;
                i = close;
            }
            foreach (char c in rest.ToString())
                if (char.IsLetterOrDigit(c)) return false;
            return true;
        }

        /// <summary>Every string shown in the world under <paramref name="root"/>: text labels and hint toasts.</summary>
        public static List<string> VisibleText(Transform root)
        {
            var list = new List<string>();
            if (root == null) return list;
            foreach (Text t in root.GetComponentsInChildren<Text>(true)) list.Add(t.text);
            foreach (TextMesh t in root.GetComponentsInChildren<TextMesh>(true)) list.Add(t.text);
            foreach (ZoneHint h in root.GetComponentsInChildren<ZoneHint>(true)) list.Add(h.Text);
            return list;
        }

        static bool IsPlaqueNumber(string bracketed)
        {
            if (bracketed.Length < 3) return false;
            for (int i = 1; i < bracketed.Length - 1; i++)
                if (!char.IsDigit(bracketed[i])) return false;
            return true;
        }
    }
}
