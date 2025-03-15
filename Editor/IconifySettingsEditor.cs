using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BP.Iconify
{
    [CustomEditor(typeof(IconifySettings))]
    public class IconifySettingsEditor : Editor
    {
        [SerializeField] private VisualTreeAsset treeAsset;
        private SerializedProperty optionsProperty;

        private void OnEnable()
        {
            optionsProperty = serializedObject.FindProperty("options");
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            treeAsset.CloneTree(root);

            var list = root.Q<ListView>();
            list.bindItem += BindItem;
            return root;
        }

        private void BindItem(VisualElement element, int index)
        {
            var itemProperty = optionsProperty.GetArrayElementAtIndex(index);
            var modeProperty = itemProperty.FindPropertyRelative("mode");
            var textureProperty = itemProperty.FindPropertyRelative("texture");
            var assetProperty = itemProperty.FindPropertyRelative("asset");
            var nameProperty = itemProperty.FindPropertyRelative("name");

            /*==== MODE ====*/
            var matchModeField = element.Q<EnumField>("ModeField");
            matchModeField.BindProperty(modeProperty);

            var assetField = element.Q<ObjectField>("AssetField");
            assetField.BindProperty(assetProperty);

            var nameField = element.Q<TextField>("NameField");
            nameField.BindProperty(nameProperty);

            void UpdateVisibility()
            {
                if ((FolderMatchMode)modeProperty.enumValueFlag == FolderMatchMode.Ref)
                {
                    nameField.style.display = DisplayStyle.None;
                    assetField.style.display = DisplayStyle.Flex;
                }
                else
                {
                    nameField.style.display = DisplayStyle.Flex;
                    assetField.style.display = DisplayStyle.None;
                }
            }

            matchModeField.TrackPropertyValue(modeProperty, _ => UpdateVisibility());
            UpdateVisibility();

            /*==== TEXTURE SELECTION ====*/
            var textureBox = element.Q("TextureBox");
            var textureSelector = textureBox.Q<ObjectField>("Select");
            textureSelector.BindProperty(textureProperty);

            var preview = textureBox.Q("Preview");
            preview.style.backgroundImage = textureProperty.objectReferenceValue as Texture2D;
            preview.TrackPropertyValue(textureProperty, (prop) =>
            {
                preview.style.backgroundImage = prop.objectReferenceValue as Texture2D;
            });
        }
    }
}