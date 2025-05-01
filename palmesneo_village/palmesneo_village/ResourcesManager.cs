using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public static class ResourcesManager
    {
        private static Dictionary<string, MTexture> textures = new Dictionary<string, MTexture>();
        private static Dictionary<string, SoundEffect> soundEffects = new Dictionary<string, SoundEffect>();

        private static List<string> allFilesPaths = new List<string>();

        public static void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager, string contentDirectory)
        {
            DirectoryInfo rootDirectoryInfo = new DirectoryInfo(contentDirectory);

            SearchAllFiles(rootDirectoryInfo.FullName, rootDirectoryInfo);

            LoadAllFiles(graphicsDevice, contentManager);
        }

        public static MTexture GetTexture(params string[] path)
        {
            string finalPath = Path.Combine(path);

            if (!textures.ContainsKey(finalPath))
            {
                Debug.WriteLine($"Texture not found: {finalPath}");
                return null;
            }

            return textures[finalPath];
        }

        public static SoundEffect GetSoundEffect(params string[] path)
        {
            string finalPath = Path.Combine(path);

            if(!soundEffects.ContainsKey(finalPath))
            {
                Debug.WriteLine($"Sound effect not found: {finalPath}");
                return null;
            }

            return soundEffects[finalPath];
        }

        private static void SearchAllFiles(string rootDirectory, DirectoryInfo dir)
        {
            FileInfo[] files = dir.GetFiles("*.xnb");
            foreach (FileInfo file in files)
            {
                string relativeFilePath = Path.GetRelativePath(rootDirectory, file.FullName);
                string relativeFilePathWithoutExtension = relativeFilePath.Split(".xnb")[0];
                allFilesPaths.Add(relativeFilePathWithoutExtension);
            }

            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                SearchAllFiles(rootDirectory, subDir);
            }
        }

        private static void LoadAllFiles(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            Debug.WriteLine("Loading: textures, sound effects");
            foreach (var filePath in allFilesPaths)
            {
                object file = contentManager.Load<object>(filePath);

                if (file is Texture2D)
                {
                    textures.Add(filePath, new MTexture(file as Texture2D));
                }
                else if(file is SoundEffect)
                {
                    soundEffects.Add(filePath, file as SoundEffect);
                }
            }
        }
    }
}
