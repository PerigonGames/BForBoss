using System.Collections.Generic;
using System.IO;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

public class MaterialGenerationAssetPostProcessor : AssetPostprocessor
{
    private class MaterialGenerationAsset
    {
        public string BaseMapAssetPath;
        public string NormalMapAssetPath;
        public string MaskMapAssetPath;
    }
    
    private const string NORMAL_MAP_PREFIX = "N_";
    private const string BASE_MAP_PREFIX = "B_";
    private const string MASK_MAP_PREFIX = "M_";
    private readonly string[] TEXTURE_FILE_EXTENSIONS = new string[2]{".tga", ".png"};

    private static readonly int BASE_MAP_PROPERTY_ID = Shader.PropertyToID("_BaseColorMap");
    private static readonly int NORMAL_MAP_PROPERTY_ID = Shader.PropertyToID("_NormalMap");
    private static readonly int MASK_MAP_PROPERTY_ID = Shader.PropertyToID("_MaskMap");
    
    private static Dictionary<string, MaterialGenerationAsset> _materialGenerationAssets = new Dictionary<string, MaterialGenerationAsset>();
    private static string _directory = string.Empty;

    private void OnPreprocessTexture()
    {
        string extension = string.Empty;
        foreach (string fileExtension in TEXTURE_FILE_EXTENSIONS)
        {
            if (assetPath.EndsWith(fileExtension))
            {
                extension = fileExtension;
            }
        }
        
        if (extension == string.Empty)
        {
            return;
        }
        
        int startingIndex = assetPath.LastIndexOf('/') + 1;
        string prefix = assetPath.Substring(startingIndex, 2);
        string materialNameWithExtension = assetPath.Substring(startingIndex + prefix.Length);
        string materialName = materialNameWithExtension.Remove(materialNameWithExtension.Length - extension.Length);
        _directory = assetPath.Substring(0, startingIndex);

        if (_materialGenerationAssets.ContainsKey(materialName))
        {
            MaterialGenerationAsset materialGenerationAsset = _materialGenerationAssets[materialName];
            switch (prefix)
            {
                case BASE_MAP_PREFIX when materialGenerationAsset.BaseMapAssetPath == null:
                {
                    materialGenerationAsset.BaseMapAssetPath = assetPath;
                    return;
                }
                case NORMAL_MAP_PREFIX when materialGenerationAsset.NormalMapAssetPath == null:
                {
                    materialGenerationAsset.NormalMapAssetPath = assetPath;
                    return;
                }
                case MASK_MAP_PREFIX when materialGenerationAsset.MaskMapAssetPath == null:
                {
                    materialGenerationAsset.MaskMapAssetPath = assetPath;
                    return;
                }
            }
        }
        
        MaterialGenerationAsset newMaterialGenerationAsset = new MaterialGenerationAsset();
        switch (prefix)
        {
            case BASE_MAP_PREFIX:
            {
                newMaterialGenerationAsset.BaseMapAssetPath = assetPath;
                break;
            }
            case NORMAL_MAP_PREFIX:
            {
                newMaterialGenerationAsset.NormalMapAssetPath = assetPath;
                break;
            }
            case MASK_MAP_PREFIX:
            {
                newMaterialGenerationAsset.MaskMapAssetPath = assetPath;
                break;
            }
        }
        _materialGenerationAssets.Add(materialName, newMaterialGenerationAsset);
    }

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        if (importedAssets.IsNullOrEmpty() || _materialGenerationAssets.Count == 0)
        {
            return;
        }
        
        foreach (var materialGenerationPair in _materialGenerationAssets)
        {
            Material material = new Material(Shader.Find("HDRP/Lit"));
            string materialName = materialGenerationPair.Key;
            MaterialGenerationAsset asset = materialGenerationPair.Value;
            string materialPath = $"{_directory}{materialName}.mat";

            //Don't recreate material if it already exists in the current Directory
            if (File.Exists(materialPath))
            {
                continue;
            }

            if (asset.BaseMapAssetPath != null)
            {
                Texture2D baseMap = AssetDatabase.LoadAssetAtPath<Texture2D>(asset.BaseMapAssetPath);
                material.SetTexture(BASE_MAP_PROPERTY_ID, baseMap);
            }
            else
            {
                // Don't create material if there is no associated base map
                continue;
            }
            
            if (asset.NormalMapAssetPath != null)
            {
                Texture2D normalMap = AssetDatabase.LoadAssetAtPath<Texture2D>(asset.NormalMapAssetPath);
                material.SetTexture(NORMAL_MAP_PROPERTY_ID, normalMap);
            }
            
            if (asset.MaskMapAssetPath != null)
            {
                Texture2D maskMap = AssetDatabase.LoadAssetAtPath<Texture2D>(asset.MaskMapAssetPath);
                material.SetTexture(MASK_MAP_PROPERTY_ID, maskMap);
            }
            
            AssetDatabase.CreateAsset(material, materialPath);
        }
        
        _materialGenerationAssets = new Dictionary<string, MaterialGenerationAsset>();
        AssetDatabase.Refresh();
    }
}