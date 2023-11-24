using System.Collections;
using System.Collections.Generic;
using MainGame.Features.ShopPreviewGenerator;
using Modules.ThirdParties.EditorButton;
using UnityEditor;
using UnityEngine;

namespace ThirdParties.Utilities.Features.ShopPreviewGenerator.Scripts
{
    public class ShopPreviewCapture : MonoBehaviour
    {
        public Transform itemParent;
        public Object SaveFolder;
        public Vector2 imageSize;

        private List<GameObject> items;
        private string ResourcePath;
        private string SavePath;


        private void CacheItems()
        {
            SavePath = AssetDatabase.GetAssetPath(SaveFolder);
            items = new List<GameObject>();
            foreach (Transform child in itemParent)
            {
                items.Add(child.gameObject);
            }
        }


        [ContextMenu("Start Capture")]
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
            var path = string.Format(Application.dataPath.Replace("/Assets", "") + "/{0}/{1}.png", SavePath, fileName);
            CaptureScreenshot.CaptureTransparentScreenshot(Camera.main, (int) imageSize.x, (int) imageSize.y, path);
            Debug.Log("Save image to path: " + path);
        }
    }
}