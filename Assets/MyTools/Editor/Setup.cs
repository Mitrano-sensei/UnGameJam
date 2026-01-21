using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using static System.IO.Directory;
using static System.IO.Path;
using static UnityEditor.AssetDatabase;
using System.IO;

namespace MyTools
{
    public static class Setup
    {
        #region Folders And Imports 
        [MenuItem("Tools/Setup/Folders And Imports/Create Default Folders")]
        public static void CreateDefaultFolders()
        {
            Folders.CreateDefault("_Project", "Animation", "Art", "Materials", "Prefabs", "Scripts/ScriptableObjects", "Scripts/UI", "Scenes", "Sounds");
            Refresh();
        }

        [MenuItem("Tools/Setup/Folders And Imports/Import My Favorite Assets")]
        public static void ImportMyFavoriteAssets()
        {
            Assets.ImportAsset(
                "DOTween HOTween v2.unitypackage",
                "Demigiant/Editor ExtensionsAnimation"
                );
        }

        [MenuItem("Tools/Setup/Folders And Imports/Install Netcode for GameObjects")]
        public static void InstallNetcodeForGameObjects()
        {
            Packages.InstallPackages(new[] {
                "com.unity.multiplayer.tools",
                "com.unity.netcode.gameobjects"
            });
        }

        [MenuItem("Tools/Setup/Folders And Imports/Install Unity AI Navigation")]
        public static void InstallUnityAINavigation()
        {
            Packages.InstallPackages(new[] {
                "com.unity.ai.navigation"
            });
        }

        [MenuItem("Tools/Setup/Folders And Imports/Install My Favorite Open Source")]
        public static void InstallOpenSource()
        {
            Packages.InstallPackages(new[] {
                "git+https://github.com/KyleBanks/scene-ref-attribute",
                "git+https://github.com/mackysoft/Unity-SerializeReferenceExtensions.git",
                "git+https://github.com/madsbangh/EasyButtons.git",
                "git+https://github.com/KyleBanks/scene-ref-attribute.git",
                "git+https://github.com/Mitrano-sensei/Finite-State-Machine.git",
                "git+https://github.com/Mitrano-sensei/Unity-Utilities.git"
                // "git+https://github.com/starikcetin/Eflatun.SceneReference.git#3.1.1" // No longer maintained, should be used with caution
            });
        }

        #endregion

        #region Scene

        [MenuItem("Tools/Setup/Scene/Init Scene")]
        public static void InitScene()
        {
            // Verify the number of gameobjects in the scene
            var gameObjects = Object.FindObjectsOfType<GameObject>();
            if (gameObjects.Length != 1)
            {
                if (gameObjects.Length == 0)
                {
                    Debug.Log("Camera not found, creating a new one :)");
                    Scenes.CreateCamera();
                }
                else if (!EditorUtility.DisplayDialog("Are you sure ?", "The Scene is not empty", "Yes", "No"))
                {
                    return;
                }
            }

            string[] items = new string[] { "Managers", "Setup", "Environment", "Systems" };

            Scenes.CreateSeparator();
            foreach (var item in items)
            {
                var go = new GameObject(item);
                Scenes.CreateSeparator();

                if (item == "Setup")
                {
                    var camera = Camera.main;
                    camera.transform.SetParent(go.transform);
                    camera.transform.position = new(0, 0, -10);
                }
            }
        }

        [MenuItem("Tools/Setup/Scene/Create Separators")]
        public static void CreateSeparators()
        {
            Scenes.CreateSeparator();
        }

        [MenuItem("Tools/Setup/Debug/Graphics Card")]
        public static void DebugGraphicsCard()
        {
            SomeDebug.LogGraphicCard();
        }

        #endregion

        #region Helpers
        static class Folders
        {
            public static void CreateDefault(string root, params string[] folders)
            {
                var fullpath = Path.Combine(Application.dataPath, root);
                if (!Exists(fullpath))
                {
                    CreateDirectory(fullpath);
                }
                foreach (var folder in folders)
                {
                    CreateSubFolders(fullpath, folder);
                }
            }

            private static void CreateSubFolders(string rootPath, string folderHierarchy)
            {
                var folders = folderHierarchy.Split('/');
                var currentPath = rootPath;
                foreach (var folder in folders)
                {
                    currentPath = Path.Combine(currentPath, folder);
                    if (!Directory.Exists(currentPath))
                    {
                        Directory.CreateDirectory(currentPath);
                    }
                }
            }
        }

        static class Packages
        {
            static AddRequest Request;
            static Queue<string> PackagesToInstall = new();

            public static void InstallPackages(string[] packages)
            {
                foreach (var package in packages)
                {
                    PackagesToInstall.Enqueue(package);
                }

                // Start the installation of the first package
                if (PackagesToInstall.Count > 0)
                {
                    Request = Client.Add(PackagesToInstall.Dequeue());
                    EditorApplication.update += Progress;
                }
            }

            static async void Progress()
            {
                if (Request.IsCompleted)
                {
                    if (Request.Status == StatusCode.Success)
                        Debug.Log("Installed: " + Request.Result.packageId);
                    else if (Request.Status >= StatusCode.Failure)
                        Debug.Log(Request.Error.message);

                    EditorApplication.update -= Progress;

                    // If there are more packages to install, start the next one
                    if (PackagesToInstall.Count > 0)
                    {
                        // Add delay before next package install
                        await Task.Delay(1000);
                        Request = Client.Add(PackagesToInstall.Dequeue());
                        EditorApplication.update += Progress;
                    }
                }
            }
        }

        static class Assets
        {
            public static void ImportAsset(string asset, string subfolder,
                string rootFolder = "C:/Users/Elyas Tigre/AppData/Roaming/Unity/Asset Store-5.x")
            {
                ImportPackage(Combine(rootFolder, subfolder, asset), false);
            }
        }

        static class Scenes
        {
            public static void CreateSeparator()
            {
                new GameObject("--------------------");
            }

            public static void CreateCamera()
            {
                var camera = new GameObject("Main Camera");
                camera.AddComponent<Camera>();
                camera.AddComponent<AudioListener>();
                camera.tag = "MainCamera";
            }
        }

        #endregion

        #region Debug
        static class SomeDebug
        {
            public static void LogGraphicCard()
            {
                Debug.Log(SystemInfo.graphicsDeviceName);
            }
        }
        #endregion
    }
}