using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    // I love Ermek <3
    public class Engine : Game
    {
        public string Title = "Palmesneo village";
        public Version Version = new Version(0, 0);

        public static Engine Instance { get; private set; }
        public static GraphicsDeviceManager Graphics { get; private set; }

        public static Color ClearColor { get; private set; }

        public static float DeltaTime { get; private set; }
        public static float GameDeltaTime { get; private set; }
        public static float TimeRate { get; set; } = 1.0f;

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

        public Action<int, int> ScreenSizeChanged { get; set; }

        private static bool resizing;

        public static int ViewportWidth { get; private set; }
        public static int ViewportHeight { get; private set; }

        public Engine(int windowWidth, int windowHeight, bool isFullscreen)
        {
            Instance = this;

            ViewportWidth = windowWidth;
            ViewportHeight = windowHeight;

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

            // TODO: initialize here
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            // Load content here
        }

        protected override void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            GameDeltaTime = DeltaTime * TimeRate;

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

            // TODO: update here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(ClearColor);

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, ExitingEventArgs args)
        {
            base.OnExiting(sender, args);
        }

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
