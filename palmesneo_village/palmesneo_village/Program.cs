using palmesneo_village;

DevScene devScene = new DevScene();
GameplayScene gameplayScene = new GameplayScene();
using var game = new Engine(1280, 720, false, gameplayScene, false);
#if DEBUG
game.Run();
#elif RELEASE
game.RunWithLogging();
#endif