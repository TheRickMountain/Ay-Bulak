using palmesneo_village;

GameplayScene devScene = new GameplayScene();
using var game = new Engine(1280, 720, false, devScene, false);
game.RunWithLogging();