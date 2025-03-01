using palmesneo_village;

DevScene devScene = new DevScene();
using var game = new Engine(1280, 720, false, devScene, false);
game.RunWithLogging();