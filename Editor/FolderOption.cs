using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace BP.Iconify
{
    public enum FolderMatchMode
    {
        Ref,
        Name
    }

    [Serializable]
    public class FolderOption : ISerializationCallbackReceiver
    {
        [SerializeField] private FolderMatchMode mode;
        [SerializeField] private string name;
        [SerializeField] private DefaultAsset asset;
        [SerializeField] private Texture2D texture;

        private Regex regexCache;
        private string previousMatch;
        private FolderMatchMode previousMode;

        /// <summary>Gets the matching mode.</summary>
        public FolderMatchMode Mode => mode;
        /// <summary>Gets the folder name pattern.</summary>
        public string Name => name;
        /// <summary>Gets the folder asset reference.</summary>
        public DefaultAsset Asset => asset;
        /// <summary>Gets the folder icon texture.</summary>
        public Texture2D Texture => texture;
        /// <summary>Gets the compiled regex for name matching.</summary>
        public Regex MatchRegex => regexCache ??= CompileRegex();

        public void OnAfterDeserialize() => UpdateCache();
        public void OnBeforeSerialize() => UpdateCache();

        /// <summary>
        /// Updates the regex cache if the name or mode has changed.
        /// </summary>
        private void UpdateCache()
        {
            if (previousMatch == name && previousMode == mode) return;
            regexCache = CompileRegex();
            previousMatch = name;
            previousMode = mode;
        }

        /// <summary>
        /// Compiles the regex from the name pattern.
        /// </summary>
        private Regex CompileRegex()
        {
            string regexPattern = WildcardToRegex(name);
            regexCache = new Regex(regexPattern, RegexOptions.IgnoreCase);
            return regexCache;
        }

        /// <summary>
        /// Converts a wildcard pattern to a regex pattern.
        /// </summary>
        /// <param name="pattern">The wildcard pattern (e.g., "*/Joe/**/Woa").</param>
        /// <returns>A regex string that matches the pattern.</returns>
        private string WildcardToRegex(string pattern)
        {
            string regex = Regex.Escape(pattern)
                .Replace(@"\*\*", ".*")            // ** -> match any character sequence (including directory separators)
                .Replace(@"\*", "[^/\\]*")         // *  -> match any character sequence except '/' or '\'
                .Replace(@"\?", ".")                // ?  -> match any single character
                .Replace(@"\|", "|");              // |  -> OR operator
            return pattern.Contains("|") ? $"^(?:{regex})$" : $"^{regex}$";
        }
    }
}
