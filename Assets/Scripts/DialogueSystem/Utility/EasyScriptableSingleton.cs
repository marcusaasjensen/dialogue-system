using System.IO;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace DialogueSystem.Utility
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
                {
                    InitializeInstance();
                }

                return _instance;
            }
        }

        private static void InitializeInstance()
        {
            var template = CreateInstance<T>() as EasyScriptableSingleton<T>;

            if (template != null)
            {
                _instance = Resources.Load<T>($"{template.ResourcesPath}/{template.FileName}");
            }

            if (_instance != null)
            {
                return;
            }
            
            _instance = CreateInstance<T>();
            #if UNITY_EDITOR
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
            #endif
        }
        
        protected void SaveRuntimeData()
        {
            // Only executes in editor mode
            #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            #endif
        }

        protected abstract void Initialize();
    }
}