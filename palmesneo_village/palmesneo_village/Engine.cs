using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;
using Penumbra;
using System;
using System.IO;
using System.Reflection;
using System.Runtime;

namespace palmesneo_village
{
    // I love Ermek <3
    public class Engine : Game
    {
        public string Title = "Palmesneo village";
        public GameVersion Version = new GameVersion(0, 0, 0);

        public const int TILE_SIZE = 16;

        public static Engine Instance { get; private set; }
        public static GraphicsDeviceManager Graphics { get; private set; }
        public static Commands Commands { get; private set; }

        public static Color ClearColor;

        public static float DeltaTime { get; private set; }
        public static float GameDeltaTime { get; private set; }
        public static float TimeRate { get; set; } = 1.0f;

        public static float GlobalUIScale { get; set; } = 2.0f;

        public static bool IsMouseOnUI { get; set; } = false;

        public static bool DebugRender { get; set; } = false;

        public static Camera Camera { get; private set; }

        // content directory
#if !CONSOLE
        private static string AssemblyDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
#endif

        public static string ContentDirectory
        {
#if PS4
            get { return Path.Combine("/app0/", Instance.Content.RootDirectory); }
#elif NSWITCH
            get { return Path.Combine("rom:/", Instance.Content.RootDirectory); }
#elif XBOXONE
            get { return Instance.Content.RootDirectory; }
#else
            get { return Path.Combine(AssemblyDirectory, Instance.Content.RootDirectory); }
#endif
        }

        public static int ViewportWidth { get; private set; }
        public static int ViewportHeight { get; private set; }

        public static PenumbraComponent Penumbra { get; private set; }

        public Action<int, int> ScreenSizeChanged { get; set; }

        public static MTileset ItemsIcons { get; private set; }
        public static MTileset FloorPathTileset { get; private set; }

        public static ItemsDatabase ItemsDatabase { get; private set; }

        private static bool resizing;

        private Scene currentScene;
        private Scene nextScene;

        public Engine(int windowWidth, int windowHeight, bool isFullscreen,
            Scene mainScene, bool debugRender)
        {
            Instance = this;

            ViewportWidth = windowWidth;
            ViewportHeight = windowHeight;

            CurrentScene = mainScene;

            DebugRender = debugRender;

            ClearColor = Color.Black;

            Graphics = new GraphicsDeviceManager(this);
            Graphics.DeviceReset += OnGraphicsDeviceReset;
            Graphics.DeviceCreated += OnGraphicsDeviceCreated;
            Graphics.SynchronizeWithVerticalRetrace = true;
            Graphics.PreferMultiSampling = false;
            Graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Graphics.PreferredBackBufferFormat = SurfaceFormat.Color;
            Graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;

#if PS4 || XBOXONE
            Graphics.PreferredBackBufferWidth = 1920;
            Graphics.PreferredBackBufferHeight = 1080;
#elif NSWITCH
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
#else
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnClientSizeChanged;

            if (isFullscreen)
            {
                Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                Graphics.PreferredBackBufferWidth = windowWidth;
                Graphics.PreferredBackBufferHeight = windowHeight;
            }

            Graphics.IsFullScreen = isFullscreen;
            Graphics.ApplyChanges();
#endif

            Content.RootDirectory = @"Content";

            IsMouseVisible = false;
            IsFixedTimeStep = false;
            Window.Title = $"{Title} {Version}";

            Penumbra = new PenumbraComponent(this);

            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
        }

        private void OnGraphicsDeviceCreated(object sender, EventArgs e)
        {
            Console.WriteLine("Graphics created");

            UpdateView();
        }

        private void OnGraphicsDeviceReset(object sender, EventArgs e)
        {
            // Вызывается при переходе в полноэкранный или оконный режимы
            Console.WriteLine("Graphics reset");

            UpdateView();
        }

#if !CONSOLE
        protected virtual void OnClientSizeChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Client size changed");
            if (Window.ClientBounds.Width > 0 && Window.ClientBounds.Height > 0 && !resizing)
            {
                resizing = true;

                Graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                Graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                UpdateView();

                resizing = false;
            }
        }
#endif

        protected override void Initialize()
        {
            base.Initialize();

            MInput.Initialize();
            InputBindings.Initialize();
            Commands = new Commands();

            Camera = new Camera(ViewportWidth, ViewportHeight);

            Penumbra.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            ResourcesManager.LoadContent(GraphicsDevice, Content, ContentDirectory);

            LocalizationManager.Initialize(GameCulture.Tatar);

            RenderManager.Initialize(GraphicsDevice);

            ItemsDatabase = JObject.Parse(File.ReadAllText(Path.Combine(ContentDirectory, "Items", "ItemsDatabase.json")))
                .ToObject<ItemsDatabase>();

            ItemsIcons = new MTileset(ResourcesManager.GetTexture("Items", "items_icons"), 16, 16);
            FloorPathTileset = new MTileset(ResourcesManager.GetTexture("Tilesets", "floor_path_tileset"), 16, 16);

            ItemsDatabase.Initialize(ItemsIcons);
        }

        protected override void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            GameDeltaTime = DeltaTime * TimeRate;

            IsMouseOnUI = false;

            MInput.Update();

            if (Keyboard.GetState().IsKeyDown(Keys.F11))
            {
                if (Graphics.IsFullScreen)
                {
                    SetWindowed(1280, 720);
                }
                else
                {
                    SetFullscreen();
                }
            }

            // Update current scene
            if (currentScene != null)
            {
                currentScene.Update();
            }

            //Debug Console
            if (Commands.Open)
            {
                Commands.UpdateOpen();
            }
            else if (Commands.Enabled)
            {
                Commands.UpdateClosed();
            }

            // Changing scenes
            if (currentScene != nextScene)
            {
                Scene lastScene = currentScene;

                if (currentScene != null)
                {
                    currentScene.End();
                }

                currentScene = nextScene;
                OnSceneTransition(lastScene, nextScene);

                if (currentScene != null)
                {
                    currentScene.Begin();
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(ClearColor);

            #region World render

            Penumbra.Transform = Camera.Matrix;
            Penumbra.BeginDraw();

            RenderManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap,
                DepthStencilState.None, RasterizerState.CullNone, null, Camera.Matrix);

            currentScene?.Render();

            RenderManager.SpriteBatch.End();

            Penumbra.Draw(gameTime);

            #endregion

            #region World debug render

            if (DebugRender)
            {
                RenderManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap,
                DepthStencilState.None, RasterizerState.CullNone, null, Camera.Matrix);

                currentScene?.DebugRender();

                RenderManager.SpriteBatch.End();
            }

            #endregion

            #region UI render

            RenderManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap,
                DepthStencilState.None, RasterizerState.CullNone);

            currentScene?.RenderUI();

            if (DebugRender)
            {
                currentScene?.DebugRenderUI();
            }

            if (Commands.Open)
            {
                Commands.Render();
            }

            RenderManager.SpriteBatch.End();

            #endregion

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, ExitingEventArgs args)
        {
            base.OnExiting(sender, args);
            MInput.Shutdown();
        }

        public void RunWithLogging()
        {
            try
            {
                Run();
            }
            catch (Exception e)
            {
                ErrorLog.Write(e);
#if DEBUG
                ErrorLog.Open();
#endif
            }
        }

        #region scene

        /// <summary>
        /// Called after a Scene ends, before the next Scene begins
        /// </summary>
        protected virtual void OnSceneTransition(Scene from, Scene to)
        {
            Penumbra.AmbientColor = Color.White;
            Penumbra.Lights.Clear();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// The currently active Scene. Note that if set, the Scene will not actually change until the end of the Update
        /// </summary>
        public static Scene CurrentScene
        {
            get { return Instance.currentScene; }
            set { Instance.nextScene = value; }
        }

        #endregion

        #region screen

        public static void SetWindowed(int width, int height)
        {
#if !CONSOLE
            if (width > 0 && height > 0)
            {
                resizing = true;
                Graphics.PreferredBackBufferWidth = width;
                Graphics.PreferredBackBufferHeight = height;
                Graphics.IsFullScreen = false;
                Graphics.ApplyChanges();
                Console.WriteLine("WINDOW-" + width + "x" + height);
                resizing = false;
            }
#endif
        }

        public static void SetFullscreen()
        {
#if !CONSOLE
            resizing = true;
            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Graphics.IsFullScreen = true;
            Graphics.ApplyChanges();
            Console.WriteLine("FULLSCREEN");
            resizing = false;
#endif
        }

        private void UpdateView()
        {
            ViewportWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            ViewportHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            ScreenSizeChanged?.Invoke(ViewportWidth, ViewportHeight);
        }

        #endregion
    }
}