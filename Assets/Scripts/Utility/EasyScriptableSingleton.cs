using System.IO;
using UnityEditor;
using UnityEngine;

namespace Utility
{
    public abstract class EasyScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        protected abstract string PathToResources { get; }
        protected abstract string ResourcesPath { get; }
        protected abstract string FileName { get; }
     
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
            var template = CreateInstance<T>() as EasyScriptableSingleton<T>;
            
            if (template != null)
                _instance = Resources.Load<T>($"{template.ResourcesPath}/{template.FileName}");
            
            if (_instance != null) return;
            
            _instance = CreateInstance<T>();
            template = _instance as EasyScriptableSingleton<T>;
            
            if (template != null)
            {
                template.Initialize();

                var fileResourcePath = $"{template.ResourcesPath}/{template.FileName}";
                var pathToResources = $"{template.PathToResources}/{template.ResourcesPath}";

                if (!AssetDatabase.IsValidFolder($"{pathToResources}"))
                {
                    Directory.CreateDirectory($"{pathToResources}");
                }

                var assetPath = $"{template.PathToResources}/{fileResourcePath}.asset";

                AssetDatabase.CreateAsset(_instance, assetPath);
            }

            AssetDatabase.SaveAssets();
        }
        
        protected void SaveRuntimeData()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        protected abstract void Initialize();
    }
}
