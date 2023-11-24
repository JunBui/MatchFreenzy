#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Modules.ThirdParties.EditorButton;
using UnityEditor;
using UnityEngine;

namespace MainGame.Features.ShopPreviewGenerator
{
    public class ShopSkinPreviewCaptureModels : MonoBehaviour
    {
        public Transform itemParent;
        public RenderTexture renderTexture;
        public Object SaveFolder;

        private List<GameObject> items;

        private string ResourcePath;
        private string SavePath;


        private void Start()
        {
        }

        private void CacheItems()
        {
            SavePath = AssetDatabase.GetAssetPath(SaveFolder);
            items = new List<GameObject>();
            foreach (Transform child in itemParent)
            {
                items.Add(child.gameObject);
            }
        }


        [EditorButton]
        private void Capture()
        {
            StartCoroutine(IECapture());
        }

        private IEnumerator IECapture()
        {
#if UNITY_EDITOR

            CacheItems();
            DisableAllItems();
            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < items.Count; i++)
            {
                items[i].SetActive(true);
                yield return new WaitForEndOfFrame();
                SaveImageToFile(items[i].name);
                DisableAllItems();
            }
#endif
        }

        private void DisableAllItems()
        {
            foreach (var item in items)
            {
                item.SetActive(false);
            }
        }

        private void SaveImageToFile(string fileName)
        {
            RenderTexture rt = renderTexture;
            var oldRT = RenderTexture.active;

            // var tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
            // RenderTexture.active = rt;
            // tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0, false);
            // tex.Apply();
            //
            var path = string.Format(Application.dataPath.Replace("/Assets", "") + "/{0}/{1}.png", SavePath, fileName);
            // File.WriteAllBytes(path, tex.EncodeToJPG());
            // RenderTexture.active = oldRT;

            CaptureScreenshot.CaptureTransparentScreenshot(Camera.main, rt.width, rt.height, path);
            Debug.Log("Save image to path: " + path);
        }

        public static class CaptureScreenshot
        {
            public static void CaptureTransparentScreenshot(Camera cam, int width, int height, string screengrabfile_path)
            {
                // This is slower, but seems more reliable.
                var bak_cam_targetTexture = cam.targetTexture;
                var bak_cam_clearFlags = cam.clearFlags;
                var bak_RenderTexture_active = RenderTexture.active;

                var tex_white = new Texture2D(width, height, TextureFormat.ARGB32, false);
                var tex_black = new Texture2D(width, height, TextureFormat.ARGB32, false);
                var tex_transparent = new Texture2D(width, height, TextureFormat.ARGB32, false);
                // Must use 24-bit depth buffer to be able to fill background.
                var render_texture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
                var grab_area = new Rect(0, 0, width, height);

                RenderTexture.active = render_texture;
                cam.targetTexture = render_texture;
                cam.clearFlags = CameraClearFlags.SolidColor;

                cam.backgroundColor = Color.black;
                cam.Render();
                tex_black.ReadPixels(grab_area, 0, 0);
                tex_black.Apply();

                cam.backgroundColor = Color.white;
                cam.Render();
                tex_white.ReadPixels(grab_area, 0, 0);
                tex_white.Apply();

                // Create Alpha from the difference between black and white camera renders
                for (int y = 0; y < tex_transparent.height; ++y)
                {
                    for (int x = 0; x < tex_transparent.width; ++x)
                    {
                        float alpha = tex_white.GetPixel(x, y).r - tex_black.GetPixel(x, y).r;
                        alpha = 1.0f - alpha;
                        Color color;
                        if (alpha == 0)
                        {
                            color = Color.clear;
                        }
                        else
                        {
                            color = tex_black.GetPixel(x, y) / alpha;
                        }

                        color.a = alpha;
                        tex_transparent.SetPixel(x, y, color);
                    }
                }

                // Encode the resulting output texture to a byte array then write to the file
                byte[] pngShot = ImageConversion.EncodeToPNG(tex_transparent);
                File.WriteAllBytes(screengrabfile_path, pngShot);

                cam.clearFlags = bak_cam_clearFlags;
                cam.targetTexture = bak_cam_targetTexture;
                RenderTexture.active = bak_RenderTexture_active;
                RenderTexture.ReleaseTemporary(render_texture);

                Texture2D.Destroy(tex_black);
                Texture2D.Destroy(tex_white);
                Texture2D.Destroy(tex_transparent);
            }

            public static void SimpleCaptureTransparentScreenshot(Camera cam, int width, int height, string screengrabfile_path)
            {
                // Depending on your render pipeline, this may not work.
                var bak_cam_targetTexture = cam.targetTexture;
                var bak_cam_clearFlags = cam.clearFlags;
                var bak_RenderTexture_active = RenderTexture.active;

                var tex_transparent = new Texture2D(width, height, TextureFormat.ARGB32, false);
                // Must use 24-bit depth buffer to be able to fill background.
                var render_texture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
                var grab_area = new Rect(0, 0, width, height);

                RenderTexture.active = render_texture;
                cam.targetTexture = render_texture;
                cam.clearFlags = CameraClearFlags.SolidColor;

                // Simple: use a clear background
                cam.backgroundColor = Color.clear;
                cam.Render();
                tex_transparent.ReadPixels(grab_area, 0, 0);
                tex_transparent.Apply();

                // Encode the resulting output texture to a byte array then write to the file
                byte[] pngShot = ImageConversion.EncodeToPNG(tex_transparent);
                File.WriteAllBytes(screengrabfile_path, pngShot);

                cam.clearFlags = bak_cam_clearFlags;
                cam.targetTexture = bak_cam_targetTexture;
                RenderTexture.active = bak_RenderTexture_active;
                RenderTexture.ReleaseTemporary(render_texture);

                Texture2D.Destroy(tex_transparent);
            }
        }
    }
}
#endif