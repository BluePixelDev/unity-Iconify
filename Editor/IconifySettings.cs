using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BP.Iconify
{
    /// <summary>
    /// Holds settings for folder icon customization.
    /// </summary>
    public class IconifySettings : ScriptableObject
    {
        [SerializeField] private bool enableIcons = true;
        [SerializeField] private List<FolderOption> options = new();

        /// <summary>
        /// Determines if icons are enabled.
        /// </summary>
        public bool EnableIcons => enableIcons;

        /// <summary>
        /// Finds a matching FolderOption based on the folder asset or path.
        /// </summary>
        /// <param name="folderAsset">The folder asset.</param>
        /// <param name="folderPath">The path to the folder.</param>
        /// <returns>The matching FolderOption, or null if no match is found.</returns>
        public FolderOption FindMatchingOption(DefaultAsset folderAsset, string folderPath)
        {
            foreach (var option in options)
            {
                // Match by folder reference
                if (option.Mode == FolderMatchMode.Ref && option.Asset == folderAsset)
                {
                    return option;
                }

                if (option.MatchRegex == null) continue;

                // Match by folder name or path
                if (option.Mode == FolderMatchMode.Name)
                {
                    string relativePath = folderPath.StartsWith("Assets/")
                    ? folderPath["Assets/".Length..]
                    : folderPath;

                    if (option.Name.Contains("/") && option.MatchRegex.IsMatch(relativePath))
                    {
                        return option;
                    }
                    else
                    {
                        string simpleName = System.IO.Path.GetFileName(folderPath.TrimEnd('/', '\\'));
                        if (option.MatchRegex.IsMatch(simpleName))
                        {
                            return option;
                        }
                    }
                }
            }
            return null;
        }
    }
}