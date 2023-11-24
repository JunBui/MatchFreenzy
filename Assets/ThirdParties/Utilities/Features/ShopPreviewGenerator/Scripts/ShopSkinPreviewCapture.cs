#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Modules.ThirdParties.EditorButton;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MainGame.Features.ShopPreviewGenerator
{
    public class ShopSkinPreviewCapture : MonoBehaviour
    {
        public MeshRenderer meshRenderer;
        public Object ResourceFolder;
        public Object SaveFolder;
        public RenderTexture renderTexture;
        public List<Texture2D> textures;

        private string ResourcePath;
        private string SavePath;


        private void Start()
        {
        }

        private void CacheTextures()
        {
            ResourcePath = AssetDatabase.GetAssetPath(ResourceFolder).Replace("Assets/Resources/", "");
            SavePath = AssetDatabase.GetAssetPath(SaveFolder);

            textures = Resources.LoadAll<Texture2D>(ResourcePath).ToList();
            textures.Sort((obj1, obj2) => string.Compare(obj1.name, obj2.name));
        }

        [EditorButton]
        private void QuickTest()
        {
            // SaveImageToFile("AAA");
            Debug.LogError(AssetDatabase.GetAssetPath(ResourceFolder).Replace("Assets/Resources/", ""));
            Debug.LogError(AssetDatabase.GetAssetPath(SaveFolder).Replace("Assets", ""));
        }

        [EditorButton]
        private void Capture()
        {
            StartCoroutine(IECapture());
        }

        private IEnumerator IECapture()
        {
#if UNITY_EDITOR
            CacheTextures();
            // yield return new EditorWaitForSeconds(0.5f);

            for (int i = 0; i < textures.Count; i++)
            {
                meshRenderer.materials[0].mainTexture = textures[i];
                meshRenderer.materials[1].mainTexture = textures[i];

                yield return new WaitForEndOfFrame();
                SaveImageToFile(textures[i].name);
            }
#endif
        }

        private void SaveImageToFile(string fileName)
        {
            RenderTexture rt = renderTexture;
            var oldRT = RenderTexture.active;

            var tex = new Texture2D(rt.width, rt.height);
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();

            var path = string.Format(Application.dataPath.Replace("/Assets", "") + "/{0}/{1}.png", SavePath, fileName);
            File.WriteAllBytes(path, tex.EncodeToJPG());
            RenderTexture.active = oldRT;

            Debug.Log("Save image to path: " + path);
        }
    }
}
#endif