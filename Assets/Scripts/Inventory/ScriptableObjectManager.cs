using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AW
{
    public static class ScriptableObjectManager {

        public static void CreateAsset<T>() where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            if (Resources.Load(typeof (T).ToString()) == null)
            {
                string assetPath =
                    AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/" + typeof (T).ToString() + ".asset");
                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = asset;
            }
            else
            {
                Debug.Log(typeof(T).ToString() + "already created!");
            }
        }

        [MenuItem("Assets/Inventory/CreateInventory")]
        public static void CreateInventory()
        {
            
        }

        [MenuItem("Assets/Inventory/CreateWeapenList")]
        public static void CreateWeapenList()
        {
            CreateAsset<WeapenScriptableObject>();
        }

        [MenuItem("Assets/Inventory/CreateSpellItemList")]
        public static void CreateSpellItemList()
        {
            CreateAsset<SpellItemScriptableObject>();
        }
    }


}
