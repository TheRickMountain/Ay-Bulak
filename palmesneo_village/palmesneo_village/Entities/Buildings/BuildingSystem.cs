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
            buildingPreview.Texture = currentBuildingItem.DirectionIcon[currentDirection];

            int buildingHeightInPixels = currentBuildingItem.Height * Engine.TILE_SIZE;
            int buildingTextureHeight = currentBuildingItem.DirectionIcon[currentDirection].Height;

            buildingPreview.Offset = new Vector2(0, buildingTextureHeight - buildingHeightInPixels);
        }

        public bool TryPlaceBuilding(Vector2 position)
        {
            if (currentBuildingItem == null) return false;

            Vector2[,] tiles = GetTilesCoveredByPattern(position, currentGroundPattern);

            if (ValidatePlacement(tiles, currentGroundPattern) == false) return false;

            gameLocation.Build(currentBuildingItem, tiles, currentDirection);

            return true;
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

            RenderGroundPattern(gameLocation.MouseTile, currentGroundPattern);
        }

        private void RenderGroundPattern(Vector2 position, string[,] pattern)
        {
            if (currentBuildingItem == null)
                return;

            foreach (var checkTile in GetTilesCoveredByPattern(position, pattern))
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

        private bool ValidatePlacement(Vector2[,] tiles, string[,] pattern)
        {
            if (currentBuildingItem == null)
                return false;

            int tileX = (int)tiles[0, 0].X;
            int tileY = (int)tiles[0, 0].Y;

            foreach (var checkTile in tiles)
            {
                int offsetX = (int)checkTile.X - tileX;
                int offsetY = (int)checkTile.Y - tileY;

                if (offsetX < 0 || offsetY < 0 ||
                    offsetX >= pattern.GetLength(0) ||
                    offsetY >= pattern.GetLength(1))
                    continue;

                string groundPatternId = pattern[offsetX, offsetY];
                if (!gameLocation.CheckGroundPattern((int)checkTile.X, (int)checkTile.Y, groundPatternId))
                {
                    return false;
                }
            }

            return true;
        }

        private Vector2[,] GetTilesCoveredByPattern(Vector2 position, string[,] pattern)
        {
            int tileX = (int)position.X;
            int tileY = (int)position.Y;

            int widthInTiles = pattern.GetLength(0);
            int heightInTiles = pattern.GetLength(1);

            Vector2[,] tiles = new Vector2[widthInTiles, heightInTiles];

            for (int i = 0; i < widthInTiles; i++)
            {
                for (int j = 0; j < heightInTiles; j++)
                {
                    tiles[i, j] = new Vector2(tileX + i, tileY + j);
                }
            }

            return tiles;
        }
    }
}