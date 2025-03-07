using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace palmesneo_village
{
    /// <summary>
    /// Manages the building system, including placement, validation, and visualization
    /// </summary>
    public class BuildingSystem
    {
        private GameLocation gameLocation;
        private BuildingPreview buildingPreview;
        private BuildingItem currentBuildingItem;
        private Direction currentDirection = Direction.Down;
        private string[,] currentGroundPattern;
        private bool isPlacementValid = false;

        public BuildingSystem(GameLocation gameLocation)
        {
            this.gameLocation = gameLocation;

            buildingPreview = new BuildingPreview();
            buildingPreview.Depth = 100;
            buildingPreview.IsVisible = false;
        }

        public BuildingPreview Preview => buildingPreview;

        public void SetCurrentBuildingItem(BuildingItem item)
        {
            currentBuildingItem = item;

            if (item != null)
            {
                currentDirection = Direction.Down;
                buildingPreview.Texture = item.DirectionIcon[currentDirection];
                currentGroundPattern = item.GroundPattern;
                buildingPreview.IsVisible = true;
            }
            else
            {
                buildingPreview.IsVisible = false;
            }
        }

        public void RotateBuildingPreview()
        {
            if (currentBuildingItem != null && currentBuildingItem.IsRotatable)
            {
                currentDirection = currentDirection.Next();
                buildingPreview.Texture = currentBuildingItem.DirectionIcon[currentDirection];
                currentGroundPattern = Calc.RotateMatrix(currentBuildingItem.GroundPattern, currentDirection);
            }
        }

        public void UpdatePreview(Vector2 mouseTilePosition)
        {
            if (currentBuildingItem != null)
            {
                buildingPreview.LocalPosition = gameLocation.MapToWorld(mouseTilePosition);
                isPlacementValid = ValidatePlacement(mouseTilePosition);
            }
        }

        public bool TryPlaceBuilding(Vector2 position)
        {
            if (currentBuildingItem == null || !isPlacementValid)
                return false;

            bool success = gameLocation.TryBuild(
                (int)position.X,
                (int)position.Y,
                currentBuildingItem,
                currentDirection,
                currentGroundPattern);

            return success;
        }

        public void RenderPreview(Vector2 mouseTilePosition)
        {
            if (currentBuildingItem == null || currentGroundPattern == null)
                return;

            foreach (var checkTile in GetTilesCoveredByPattern(mouseTilePosition, currentGroundPattern))
            {
                Color color = Color.YellowGreen * 0.5f;

                int offsetX = (int)checkTile.X - (int)mouseTilePosition.X;
                int offsetY = (int)checkTile.Y - (int)mouseTilePosition.Y;

                if (offsetX < 0 || offsetY < 0 ||
                    offsetX >= currentGroundPattern.GetLength(0) ||
                    offsetY >= currentGroundPattern.GetLength(1))
                    continue;

                string groundPatternId = currentGroundPattern[offsetX, offsetY];
                if (!gameLocation.CheckGroundPattern((int)checkTile.X, (int)checkTile.Y, groundPatternId))
                {
                    color = Color.OrangeRed * 0.5f;
                }

                RenderManager.Rect(checkTile * Engine.TILE_SIZE, new Vector2(Engine.TILE_SIZE), color);
                RenderManager.HollowRect(checkTile * Engine.TILE_SIZE, new Vector2(Engine.TILE_SIZE), color);
            }
        }

        private bool ValidatePlacement(Vector2 position)
        {
            if (currentBuildingItem == null || currentGroundPattern == null)
                return false;

            int tileX = (int)position.X;
            int tileY = (int)position.Y;

            foreach (var checkTile in GetTilesCoveredByPattern(position, currentGroundPattern))
            {
                int offsetX = (int)checkTile.X - tileX;
                int offsetY = (int)checkTile.Y - tileY;

                if (offsetX < 0 || offsetY < 0 ||
                    offsetX >= currentGroundPattern.GetLength(0) ||
                    offsetY >= currentGroundPattern.GetLength(1))
                    continue;

                string groundPatternId = currentGroundPattern[offsetX, offsetY];
                if (!gameLocation.CheckGroundPattern((int)checkTile.X, (int)checkTile.Y, groundPatternId))
                {
                    return false;
                }
            }

            return true;
        }

        private IEnumerable<Vector2> GetTilesCoveredByPattern(Vector2 position, string[,] pattern)
        {
            int tileX = (int)position.X;
            int tileY = (int)position.Y;

            int widthInTiles = pattern.GetLength(0);
            int heightInTiles = pattern.GetLength(1);

            for (int i = 0; i < widthInTiles; i++)
            {
                for (int j = 0; j < heightInTiles; j++)
                {
                    yield return new Vector2(tileX + i, tileY + j);
                }
            }
        }
    }
}