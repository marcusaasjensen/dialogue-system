using System.IO;
using UnityEditor;
using UnityEngine;

namespace Utility
{
    public abstract class EasyScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    InitializeInstance();

                return _instance;
            }
        }

        private static void InitializeInstance()
        {
            var tmpInstance = CreateInstance<T>() as EasyScriptableSingleton<T>;
            
            if (tmpInstance != null)
                _instance = Resources.Load<T>($"{tmpInstance.ResourcesPath}/{tmpInstance.FileName}");
            
            if (_instance != null) return;
            
            _instance = CreateInstance<T>();
            tmpInstance = _instance as EasyScriptableSingleton<T>;
            
            if (tmpInstance != null)
            {
                tmpInstance.Initialize();

                var fileResourcePath = $"{tmpInstance.ResourcesPath}/{tmpInstance.FileName}";
                var pathToResources = $"{tmpInstance.PathToResources}/{tmpInstance.ResourcesPath}";

                if (!AssetDatabase.IsValidFolder($"{pathToResources}"))
                {
                    Directory.CreateDirectory($"{pathToResources}");
                }

                var assetPath = $"{tmpInstance.PathToResources}/{fileResourcePath}.asset";

                AssetDatabase.CreateAsset(_instance, assetPath);
            }

            AssetDatabase.SaveAssets();
        }

        protected abstract string PathToResources { get; }
        protected abstract string ResourcesPath { get; }
        protected abstract string FileName { get; }

        protected void SaveRuntimeData()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        protected abstract void Initialize();
    }
}
