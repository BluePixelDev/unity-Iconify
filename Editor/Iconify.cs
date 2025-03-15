using BP.Iconify;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Handles automatic folder icon customization in Unity's Project window.
/// </summary>
[InitializeOnLoad]
public static class Iconify
{
    private const string AssetPath = "Assets/IconifySettings.asset";

    // Accessor for settings with lazy initialization
    private static IconifySettings Settings => GetOrCreateSettings();
    private static IconifySettings settings;
    private static readonly Texture2D coverTexture;

    // Static constructor to initialize the icon handler
    static Iconify()
    {
        GetOrCreateSettings();
        EditorApplication.projectWindowItemOnGUI -= HandleItemGUI;
        EditorApplication.projectWindowItemOnGUI += HandleItemGUI;

        // Initialize cover texture for background
        coverTexture = new Texture2D(1, 1);
        coverTexture.SetPixel(0, 0, Color.white);
        coverTexture.Apply();
    }

    /// <summary>
    /// Loads or creates the Iconify settings asset.
    /// </summary>
    /// <returns>IconifySettings instance.</returns>
    public static IconifySettings GetOrCreateSettings()
    {
        if (settings != null) return settings;
        settings = AssetDatabase.LoadAssetAtPath<IconifySettings>(AssetPath);

        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<IconifySettings>();
            AssetDatabase.CreateAsset(settings, AssetPath);
        }

        return settings;
    }

    private static void HandleItemGUI(string guid, Rect selectionRect)
    {
        if (!Settings.EnableIcons) return;

        string path = AssetDatabase.GUIDToAssetPath(guid);
        var asset = AssetDatabase.LoadAssetAtPath(path, typeof(DefaultAsset)) as DefaultAsset;
        bool isFolder = AssetDatabase.IsValidFolder(path);
        if (!isFolder) return;

        var folderOption = Settings.FindMatchingOption(asset, path);

        if (folderOption == null) return;
        var folderIcon = folderOption.Texture;
        if (folderIcon == null) return;

        // Set icon rectangle
        Rect iconRect = new(selectionRect.x, selectionRect.y, selectionRect.height, selectionRect.height);

        // Set background color based on theme
        Color backgroundColor = EditorGUIUtility.isProSkin
         ? new Color32(56, 56, 56, 255)
         : new Color32(194, 194, 194, 255);

        bool isAssetsTab = false;

        /*==== ASSETS TAB ====*/
        // Adjust size for larger icons
        if (selectionRect.height > 16)
        {
            iconRect.width -= 14;
            iconRect.height -= 14;
            isAssetsTab = true;
        }

        // Update background color for assets tab
        if (selectionRect.x <= 14)
        {
            iconRect.x += 2;
            isAssetsTab = true;
        }

        // Update background color for assets tab
        if (isAssetsTab)
        {
            backgroundColor = EditorGUIUtility.isProSkin
          ? new Color32(51, 51, 51, 255)
          : new Color32(194, 194, 194, 255);
        }

        /*==== RENDERING ====*/
        // Cover Texture
        GUI.color = backgroundColor;
        GUI.DrawTexture(iconRect, coverTexture);
        GUI.color = Color.white;

        GUI.DrawTexture(iconRect, folderIcon, ScaleMode.ScaleAndCrop);
    }
}
