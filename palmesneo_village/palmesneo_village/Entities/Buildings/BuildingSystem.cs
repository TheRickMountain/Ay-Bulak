using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace palmesneo_village
{
    /// <summary>
    /// Manages the building system, including placement, validation, and visualization
    /// </summary>
    public class BuildingSystem : Entity
    {
        private GameLocation gameLocation;
        private ImageEntity buildingPreview;
        private BuildingItem currentBuildingItem;
        private Direction currentDirection = Direction.Down;
        private string[,] currentGroundPattern;

        public BuildingSystem()
        {
            buildingPreview = new ImageEntity();
            buildingPreview.SelfColor = Color.White * 0.5f;
            buildingPreview.Depth = 100;
            buildingPreview.IsVisible = false;
            AddChild(buildingPreview);
        }

        public void SetGameLocation(GameLocation gameLocation)
        {
            this.gameLocation = gameLocation;
        }

        public void SetCurrentBuildingItem(BuildingItem item)
        {
            currentBuildingItem = item;

            if (item != null)
            {
                currentDirection = Direction.Down;
                currentGroundPattern = currentBuildingItem.GroundPattern;

                UpdatePreviewTexture();

                buildingPreview.IsVisible = true;
            }
            else
            {
                buildingPreview.IsVisible = false;
            }
        }

        public void RotateBuildingPreview()
        {
            if (currentBuildingItem == null || currentBuildingItem.IsRotatable == false) return;

            currentDirection = currentDirection.Next();
            currentGroundPattern = Calc.RotateMatrix(currentBuildingItem.GroundPattern, currentDirection);

            UpdatePreviewTexture();
        }

        private void UpdatePreviewTexture()
        {
            buildingPreview.Texture = currentBuildingItem.DirectionTexture[currentDirection];

            int buildingHeightInPixels = currentBuildingItem.Height * Engine.TILE_SIZE;
            int buildingTextureHeight = currentBuildingItem.DirectionTexture[currentDirection].Height;

            buildingPreview.Offset = new Vector2(0, buildingTextureHeight - buildingHeightInPixels);
        }

        public bool TryPlaceBuilding(Vector2 position)
        {
            if (currentBuildingItem == null) return false;

            return gameLocation.TryBuild(
                currentBuildingItem, 
                (int)position.X, 
                (int)position.Y, 
                currentDirection);
        }

        public override void Update()
        {
            base.Update();

            if (currentBuildingItem == null) return;

            buildingPreview.LocalPosition = gameLocation.MapToWorld(gameLocation.MouseTile);
        }

        public override void Render()
        {
            base.Render();

            if(currentBuildingItem == null) return;

            RenderGroundPattern(gameLocation.MouseTile, currentGroundPattern);
        }

        private void RenderGroundPattern(Vector2 position, string[,] pattern)
        {
            if (currentBuildingItem == null)
                return;

            foreach (var checkTile in Calc.GetVector2DArray(position, pattern.GetLength(0), pattern.GetLength(1)))
            {
                Color color = Color.YellowGreen * 0.5f;

                int offsetX = (int)checkTile.X - (int)position.X;
                int offsetY = (int)checkTile.Y - (int)position.Y;

                if (offsetX < 0 || offsetY < 0 ||
                    offsetX >= pattern.GetLength(0) ||
                    offsetY >= pattern.GetLength(1))
                    continue;

                string groundPatternId = pattern[offsetX, offsetY];
                if (!gameLocation.CheckGroundPattern((int)checkTile.X, (int)checkTile.Y, groundPatternId))
                {
                    color = Color.OrangeRed * 0.5f;
                }

                RenderManager.Rect(checkTile * Engine.TILE_SIZE, new Vector2(Engine.TILE_SIZE), color);
                RenderManager.HollowRect(checkTile * Engine.TILE_SIZE, new Vector2(Engine.TILE_SIZE), color);
            }
        }
    }
}