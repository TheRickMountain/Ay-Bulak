using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public enum BuildingCategory
    {
        None,
        Wall,
        Floor,
        Hut,
        Food,
        Production,
        Storage,
        Agriculture,
        Metallurgy,
        Mechanisms,
        Mining,
        Medicine
    }

    public enum BuildingType
    {
        None,
        AutoCrafter,
        ManualCrafter,
        Storage,
        BuildFarmPlot,
        DestructFarmPlot,
        DestructFloor,
        Wall,
        Floor,
        Plant,
        Deposit,
        FishTrap,
        BuildIrrigationCanal,
        DestructIrrigationCanal,
        Mine
    }

    public struct Ingredient
    {
        public Item Item { get; set; }
        public int Weight { get; set; }
    }

    public class BuildingTemplate
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public BuildingType Type { get; set; }
        public BuildingCategory Category { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsRotatable { get; set; }
        public string[,] GroundPattern { get; set; }
        public int ConstructionTime { get; set; }
        public string IdleTexture { get; set; }

        public bool RequiresConstruction { get; set; }
        public List<Ingredient> Ingredients { get; set; }

        [JsonIgnore]
        public string Path { get; set; }

        [JsonIgnore]
        public Dictionary<string, List<MTexture>> TexturesDict { get; private set; }

        [JsonIgnore]
        public Dictionary<Direction, MTexture> PreviewsDict { get; private set; }

        [JsonIgnore]
        public Vector2 TextureOffset { get; private set; }

        [JsonIgnore]
        public Vector2 TextureLocalPosition { get; private set; }

        public void Initialize(MTileset floorTileset, MTileset wallTileset)
        {
            if (Ingredients == null)
            {
                Ingredients = new List<Ingredient>();
            }

            TexturesDict = new Dictionary<string, List<MTexture>>();
            PreviewsDict = new Dictionary<Direction, MTexture>();

            switch (Type)
            {
                case BuildingType.Plant:
                    {
                        //MTexture texture = ResourcesManager.GetTexture(Path, "sprite");

                        //ExtractTextures(texture, PlantData.SpringTexture, "spring");
                        //ExtractTextures(texture, PlantData.SummerTexture, "summer");
                        //ExtractTextures(texture, PlantData.AutumnTexture, "autumn");
                        //ExtractTextures(texture, PlantData.WinterTexture, "winter");

                        //PreviewsDict.Add(Direction.Down, TexturesDict["spring"][TexturesDict["spring"].Count - 1]);
                    }
                    break;
                case BuildingType.AutoCrafter:
                case BuildingType.ManualCrafter:
                    {
                        //MTexture texture = ResourcesManager.GetTexture(Path, "sprite");

                        //if (IsRotatable)
                        //{

                        //}
                        //else
                        //{

                        //    ExtractTextures(texture, CrafterData.IdleTexture, "idle");
                        //    ExtractTextures(texture, CrafterData.ProcessTexture, "process");

                        //    PreviewsDict.Add(Direction.Down, TexturesDict["idle"][0]);
                        //}
                    }
                    break;
                case BuildingType.Floor:
                    {
                        //TexturesDict.Add("idle", new List<MTexture>() { floorTileset[FloorData.FloorId * 16] });

                        //PreviewsDict.Add(Direction.Down, TexturesDict["idle"][0]);
                    }
                    break;
                case BuildingType.Wall:
                    {
                        //TexturesDict.Add("idle", new List<MTexture>() { wallTileset[WallData.WallId * 16] });

                        //PreviewsDict.Add(Direction.Down, TexturesDict["idle"][0]);
                    }
                    break;
                default:
                    {
                        MTexture texture = ResourcesManager.GetTexture(Path, "sprite");

                        if (IsRotatable)
                        {
                        }
                        else
                        {
                            ExtractTextures(texture, IdleTexture, "idle");

                            PreviewsDict.Add(Direction.Down, TexturesDict["idle"][0]);
                        }
                    }
                    break;
            }

            int buildingWidthInPixels = Width * Engine.TILE_SIZE;
            int buildingHeightInPixels = Height * Engine.TILE_SIZE;

            int imageHeight = PreviewsDict[Direction.Down].Height;

            TextureOffset = new Vector2(0, imageHeight / 2 - buildingHeightInPixels / 2);

            TextureLocalPosition = new Vector2(buildingWidthInPixels / 2, buildingHeightInPixels / 2);
        }

        private void ExtractTextures(MTexture texture, string rawCutInfo, string name)
        {
            int[] cutInfo = Array.ConvertAll(rawCutInfo.Split(','), int.Parse);

            int x = cutInfo[0];
            int y = cutInfo[1];
            int width = cutInfo[2];
            int height = cutInfo[3];
            int frames = cutInfo[4];

            List<MTexture> frameTextureList = new List<MTexture>();

            for (int i = 0; i < frames; i++)
            {
                int subX = x + (i * width);

                MTexture subTexture = new MTexture(texture, new Rectangle(subX, y, width, height));

                frameTextureList.Add(subTexture);
            }

            TexturesDict.Add(name, frameTextureList);
        }
    }
}
