﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class FarmLocation : GameLocation
    {

        public FarmLocation(string id, TimeOfDayManager timeOfDayManager) : base(id, 128, 128, true, timeOfDayManager)
        {
            GenerateGrassTiles();

            GenerateWaterTiles();

            GenerateForestTilesOnMapBorders();

            List<Point> buildableTiles = GetBuildableTiles();

            foreach(Point tile in buildableTiles)
            {
                int x = tile.X;
                int y = tile.Y;

                int generationType = Calc.Random.Range(0, 5);

                if (Calc.Random.Chance(0.10f))
                {
                    if (generationType == 0)
                    {
                        TreeItem birchTree = Engine.ItemsDatabase.GetItemByName<TreeItem>("birch_tree");

                        TryBuild(birchTree, x, y, Direction.Down);

                        TreeBuilding treeBuilding = GetBuilding(x, y) as TreeBuilding;

                        treeBuilding?.SetGrowthProgress(Calc.Random.Choose(0.35f, 0.5f, 0.75f, 1.0f), timeOfDayManager);
                    }
                    if (generationType == 1)
                    {
                        TreeItem birchTree = Engine.ItemsDatabase.GetItemByName<TreeItem>("spruce_tree");

                        TryBuild(birchTree, x, y, Direction.Down);

                        TreeBuilding treeBuilding = GetBuilding(x, y) as TreeBuilding;

                        treeBuilding?.SetGrowthProgress(Calc.Random.Choose(0.35f, 0.5f, 0.75f, 1.0f), timeOfDayManager);
                    }
                    else if (generationType == 2)
                    {
                        ResourceItem resourceItem = Engine.ItemsDatabase.GetItemByName<ResourceItem>("stone_resource");

                        TryBuild(resourceItem, x, y, Direction.Down);
                    }
                    else if (generationType == 3)
                    {
                        ResourceItem resourceItem = Engine.ItemsDatabase.GetItemByName<ResourceItem>("wood_resource");

                        TryBuild(resourceItem, x, y, Direction.Down);
                    }
                    else if (generationType == 4)
                    {
                        GrassItem grassItem = Engine.ItemsDatabase.GetItemByName<GrassItem>("blade_grass");

                        TryBuild(grassItem, x, y, Direction.Down);
                    }
                }
            }
        }

        private void GenerateGrassTiles()
        {
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    SetGroundTile(x, y, GroundTile.Grass);
                }
            }
        }

        private void GenerateWaterTiles()
        {
            foreach (Point tile in CalcGridShapes.GetFilledCirclePoints(new Point(MapWidth / 2, 0), 9.9f))
            {
                TrySetGroundTile(tile.X, tile.Y, GroundTile.Water);
            }
        }

        private void GenerateForestTilesOnMapBorders()
        {
            float[] radiusSet = { 2.5f, 3.5f, 4.2f, 4.4f, 4.5f };

            GenerateBorderLine(radiusSet, 0, MapWidth - 1, 0, true);
            GenerateBorderLine(radiusSet, 0, MapWidth - 1, MapHeight - 1, true);

            GenerateBorderLine(radiusSet, 0, MapHeight - 1, 0, false);
            GenerateBorderLine(radiusSet, 0, MapHeight - 1, MapWidth - 1, false);
        }

        private void GenerateBorderLine(float[] radiusSet, int startCoord, int endCoord, int fixedCoord, bool isHorizontal)
        {
            int previousRadiusCountdown = 0;

            for (int currentCoord = startCoord; currentCoord <= endCoord; currentCoord++)
            {
                if (previousRadiusCountdown == 0)
                {
                    float randomRadius = Calc.Random.Choose(radiusSet);

                    Point circleCenter;
                    if (isHorizontal)
                    {
                        circleCenter = new Point(currentCoord, fixedCoord);
                    }
                    else
                    {
                        circleCenter = new Point(fixedCoord, currentCoord);
                    }

                    PlaceForestCircle(circleCenter, randomRadius);

                    previousRadiusCountdown = (int)Math.Floor(randomRadius);
                }
                else
                {
                    previousRadiusCountdown--;
                }
            }
        }

        private void PlaceForestCircle(Point center, float radius)
        {
            foreach (Point tile in CalcGridShapes.GetFilledCirclePoints(center, radius))
            {
                if (GetGroundTile(tile.X, tile.Y) == GroundTile.Water) continue;

                TrySetAirTile(tile.X, tile.Y, AirTile.Forest);
            }

            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    if(GetAirTile(x, y - 2) == AirTile.Forest || 
                        GetAirTile(x, y - 1) == AirTile.Forest ||
                        GetAirTile(x, y) == AirTile.Forest)
                    {
                        SetGroundTopTile(x, y, GroundTopTile.ForestTrunk);
                    }
                }
            }
        }

        private List<Point> GetBuildableTiles()
        {
            List<Point> tiles = new List<Point>();

            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    GroundTile groundTile = GetGroundTile(x, y);
                    AirTile airTile = GetAirTile(x, y);
                    GroundTopTile groundTopTile = GetGroundTopTile(x, y);

                    if (groundTile == GroundTile.Water) continue;

                    if (airTile == AirTile.Forest) continue;

                    if(groundTopTile == GroundTopTile.ForestTrunk) continue;

                    tiles.Add(new Point(x, y));
                }
            }

            return tiles;
        }
    }
}
