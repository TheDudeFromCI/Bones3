using Bones3Rebuilt;

using UnityEditor;

using UnityEditorInternal;

using UnityEngine;

namespace WraithavenGames.Bones3
{
    public class TextureArrayGenerator : EditorWindow
    {
        [MenuItem("Assets/Create/Texture Array from Textures")]
        static void Init()
        {
            TextureArrayGenerator window = EditorWindow.GetWindow<TextureArrayGenerator>();
            window.Show();
        }

        private TextureArrayGeneratorPropertyHolder tempObject;
        private SerializedProperty textures;
        private SerializedProperty linearColorSpace;

        private ReorderableList textureList;

        private Material texturePreview;
        private ObjectPicker objectPicker;

        void OnEnable()
        {
            tempObject = ScriptableObject.CreateInstance<TextureArrayGeneratorPropertyHolder>();
            SerializedObject propertyHolder = new SerializedObject(tempObject);

            textures = propertyHolder.FindProperty("textures");
            linearColorSpace = propertyHolder.FindProperty("linearColorSpace");

            textureList = new ReorderableList(propertyHolder, textures, true, true, true, true)
            {
                elementHeight = 68,
            };

            textureList.drawHeaderCallback += DrawTexturesHeader;
            textureList.onRemoveCallback += RemoveTexture;
            textureList.onAddCallback += AddTexture;
            textureList.drawElementCallback += DrawTextures;

            texturePreview = new Material(Shader.Find("Hidden/Bones3/Texture Preview"));
            objectPicker = new ObjectPicker();
        }

        void OnDisable()
        {
            Object.DestroyImmediate(tempObject);
        }

        void DrawTexturesHeader(Rect rect) =>
            EditorGUI.LabelField(rect, "Texture List", GUI.skin.GetStyle("BoldLabel"));

        void AddTexture(ReorderableList list) =>
            objectPicker.ShowPickerWindow<Texture2D>("textures", null, false);

        void RemoveTexture(ReorderableList list)
        {
            if (textures.GetArrayElementAtIndex(list.index).objectReferenceValue != null)
                textures.DeleteArrayElementAtIndex(list.index);

            textures.DeleteArrayElementAtIndex(list.index);
        }

        void DrawTextures(Rect rect, int index, bool active, bool focused)
        {
            var tex = (Texture2D) textures.GetArrayElementAtIndex(index).objectReferenceValue;
            var r = new Rect(rect.x, rect.y + 2, 64, 64);
            EditorGUI.DrawPreviewTexture(r, tex, texturePreview);
        }

        void OnGUI()
        {
            var newTexture = objectPicker.PickerObject("textures");
            if (newTexture != null)
            {
                int index = textures.arraySize;
                textures.InsertArrayElementAtIndex(index);
                textures.GetArrayElementAtIndex(index).objectReferenceValue = newTexture;
            }

            EditorGUILayout.Space();
            ValidateTextureList();

            textureList.DoLayoutList();
            bool warnings = ShowWarnings();

            if (!warnings)
            {
                EditorGUILayout.Space();
                if (GUILayout.Button("Build Array"))
                {
                    BuildTexture();
                    Close();
                }
            }
        }

        void BuildTexture()
        {
            string path = "Assets";
            foreach (Object obj in UnityEditor.Selection.GetFiltered(typeof(Object), UnityEditor.SelectionMode.Assets))
            {
                path = UnityEditor.AssetDatabase.GetAssetPath(obj);

                if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
                {
                    path = System.IO.Path.GetDirectoryName(path);
                    break;
                }
            }

            var firstTex = textures.GetArrayElementAtIndex(0).objectReferenceValue as Texture2D;
            int width = firstTex.width;
            int height = firstTex.height;
            int depth = textures.arraySize;
            TextureFormat format = TextureFormat.RGBA32;
            FilterMode filter = firstTex.filterMode;
            int mipmapCount = firstTex.mipmapCount;
            bool linearColor = linearColorSpace.boolValue;

            var texture = new Texture2DArray(width, height, depth, format, mipmapCount, linearColor)
            {
                name = "Texture Array",
                filterMode = filter,
                anisoLevel = firstTex.anisoLevel,
            };

            AssetDatabase.CreateAsset(texture, path + "/Voxel Texture Atlas.asset");
            AssetDatabase.ImportAsset(UnityEditor.AssetDatabase.GetAssetPath(texture));

            for (int i = 0; i < textures.arraySize; i++)
            {
                var src = textures.GetArrayElementAtIndex(i).objectReferenceValue as Texture2D;
                CopyData(src, texture, i);
            }

            texture.Apply();
        }

        void CopyData(Texture2D src, Texture2DArray dst, int index)
        {
            for (int m = 0; m < src.mipmapCount; m++)
            {
                var pixels = src.GetPixels32(m);
                dst.SetPixels32(pixels, index, m);
            }
        }

        bool ShowWarnings()
        {
            if (textures.arraySize == 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Texture list cannot be empty!", MessageType.Warning);
                return true;
            }

            if (!CheckMatching(out string warning))
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(warning, MessageType.Warning);
                return true;
            }

            return false;
        }

        bool CheckMatching(out string warning)
        {
            warning = "";
            Texture2D first = textures.GetArrayElementAtIndex(0).objectReferenceValue as Texture2D;

            for (int i = 1; i < textures.arraySize; i++)
            {
                var next = textures.GetArrayElementAtIndex(i).objectReferenceValue as Texture2D;
                if (!CheckMatching(first, next, ref warning))
                    return false;
            }

            return true;
        }

        private bool IsCompressed(TextureFormat format)
        {
            switch (format)
            {
                case TextureFormat.RGBA32:
                case TextureFormat.RGB24:
                case TextureFormat.RG16:
                case TextureFormat.R8:
                    return true;

                default:
                    return false;
            }
        }

        bool CheckMatching(Texture2D a, Texture2D b, ref string warning)
        {
            if (a.width != b.width || a.height != b.height)
            {
                warning = "Textures must all be the same size!";
                return false;
            }

            if (!IsCompressed(a.format) || !IsCompressed(b.format))
            {
                warning = "Textures must uncompressed!!";
                return false;
            }

            if (a.filterMode != b.filterMode)
            {
                warning = "Textures must all use the same filter mode!";
                return false;
            }

            if (a.mipmapCount != b.mipmapCount)
            {
                warning = "Textures must all have the same mipmap count!";
                return false;
            }

            if (a.anisoLevel != b.anisoLevel)
            {
                warning = "Textures must all use the same anisotropic level!";
                return false;
            }

            return true;
        }

        bool MatchingFormat()
        {
            TextureFormat form = (textures.GetArrayElementAtIndex(0).objectReferenceValue as Texture2D).format;
            for (int i = 1; i < textures.arraySize; i++)
            {
                var form2 = (textures.GetArrayElementAtIndex(i).objectReferenceValue as Texture2D).format;

                if (form != form2)
                    return false;
            }

            return true;
        }

        void ValidateTextureList()
        {
            for (int i = 0; i < textures.arraySize; i++)
            {
                if (textures.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    textureList.index = i;
                    RemoveTexture(textureList);
                }
            }
        }

#pragma warning disable 169

        private class TextureArrayGeneratorPropertyHolder : ScriptableObject
        {
            [SerializeField]
            private Texture2D[] textures;

            [SerializeField]
            private bool linearColorSpace;
        }
    }
}
