using UnityEditor;

// TAUXR studio default: every newly imported image generates mipmaps.
// In VR, textures without mipmaps shimmer and alias when seen from a distance or at an angle.
// Only applies the first time an image is imported - settings you change later in the Inspector are kept.
public class TAUXRTextureImportDefaults : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        if (!assetImporter.importSettingsMissing) return;
        if (!assetPath.StartsWith("Assets/")) return;

        TextureImporter textureImporter = (TextureImporter)assetImporter;
        textureImporter.mipmapEnabled = true;
    }
}
