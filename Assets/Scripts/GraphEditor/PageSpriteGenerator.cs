using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class PageSpriteGenerator : MonoBehaviour
{
    private const string SpritePath = "/Art/GeneratedSprite/";
    private const string PrefabsTextPath = "/Prefabs/TextPagePrefabs";
    private const string PrefabsImagePath = "/Prefabs/ImagePrefabs";
    
    
    
    private const string ScenePath = "/Scenes/RenderScene.unity";
    private const int ResolutionWidth = 558 * 2;
    private const int ResolutionHeight = 860 * 2;

#if UNITY_EDITOR

    [MenuItem("Tools/Generate")]
    private static void GenerateAll()
    {
        if(!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        string lastScene = EditorSceneManager.GetSceneManagerSetup()[0].path;
        EditorSceneManager.OpenScene(Application.dataPath + ScenePath);

        var files = new DirectoryInfo($"{Application.dataPath}/Data/PageDatas").GetFiles();

        foreach (var file in files)
        {
            if(file.Name.EndsWith(".meta"))
                continue;
            // var data = AssetDatabase.LoadAssetAtPath<PageData>($"Assets/Data/PageDatas/{file.Name}");
            // data.CreateOrUpdate();
        }
        
        GenerateTextSprite();
        GenerateImageSprite();

        EditorSceneManager.OpenScene(lastScene);
    }
    
    
    [ContextMenu("Generate text page sprites")]
    public static void GenerateTextSprite() => Generate(PrefabsTextPath);
    
    [ContextMenu("Generate image page sprites")]
    public static void GenerateImageSprite() => Generate(PrefabsImagePath);

    private static void Generate(string prefabPath)
    {
        var info = new DirectoryInfo(Application.dataPath + prefabPath);
        var fileInfo = info.GetFiles();
        
        foreach (var file in fileInfo)
        {
            string name = file.Name;
            if (name.EndsWith(".meta"))
                continue;
            GenerateFromFileName(name, prefabPath);        
        }
    }

    private static void GenerateFromFileName(string name, string assetPath)
    {
        var asset = AssetDatabase.LoadAssetAtPath("Assets/" + assetPath + "/" + name, typeof(GameObject));
        var go = (GameObject)PrefabUtility.InstantiatePrefab(asset, FindObjectOfType<Canvas>().transform);

        var camera = Camera.main;

        var renderTexture = new RenderTexture(ResolutionWidth, ResolutionHeight, 24);
        camera.targetTexture = renderTexture;
        var screenShot = new Texture2D(ResolutionWidth, ResolutionHeight, TextureFormat.ARGB32, false);
        screenShot.alphaIsTransparency = true;
        camera.Render();
        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(new Rect(0, 0, ResolutionWidth, ResolutionHeight), 0, 0);
        camera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        DestroyImmediate(renderTexture);
        byte[] bytes = screenShot.EncodeToPNG();
        string path = Application.dataPath + SpritePath + name[0..name.LastIndexOf('.')] + ".png";
        File.WriteAllBytes(path, bytes);
        
        AssetDatabase.ImportAsset($"Assets{SpritePath}{name[0..name.LastIndexOf('.')]}.png");
        AssetDatabase.Refresh();
        var newSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets" + SpritePath + name[0..name.LastIndexOf('.')]+".png");
        
        
        // asset.GetComponent<Page>().SpritePage = (Sprite)newSprite;
        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();
        
        DestroyImmediate(go);
    }
    
#endif    
}