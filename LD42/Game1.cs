using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.FZT;
using MonoGame.FZT.Assets;
using MonoGame.FZT.Data;
using MonoGame.FZT.Drawing;
using MonoGame.FZT.Input;
using MonoGame.FZT.Physics;
using MonoGame.FZT.Sound;
using MonoGame.FZT.UI;
using MonoGame.FZT.XML;
using System.Collections.Generic;

namespace LD42
{
    public class Game1 : Game
    {
        CursorManager cursorManager;
        GameState gameState;
        GraphicsDeviceManager graphics;
        int currentUInb;
        SpriteBatch spriteBatch;
        UISystem[] uis;
        

        Point vdims, wdims;
        int windowDivider;
        SceneCollection scenes;
        InputProfile ipp;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //GAME VALUES
            windowDivider = 2;
            vdims = new Point(200, 200);
            wdims = new Point(1920 / windowDivider, 1080 / windowDivider);

            graphics.PreferredBackBufferWidth = wdims.X;
            graphics.PreferredBackBufferHeight = wdims.Y;
            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            gameState = GameState.Menu;
            currentUInb = 0;
            IsMouseVisible = true;

            base.Initialize();
            //VALUES

            //UTILITY
            scenes = new SceneCollection();
            scenes.scenes.Add(new Scene(
                new RenderTarget2D(GraphicsDevice, vdims.X, vdims.Y),
                new Rectangle(0,0,vdims.X, vdims.Y),
                new Rectangle(0,0,wdims.X, wdims.Y),
                "main"
                ));

            KeyManager[] keyManagers = new KeyManager[] { };
            ipp = new InputProfile(keyManagers);
        }

        //dick

        protected override void LoadContent()
        { 
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //LOAD XML

            //LOAD TEXTURES

            //LOAD SOUND

            //LOAD ENTITIES

            //LOAD UR MOM
            cursorManager = new CursorManager();

            //END - SETUP THE GAME!
            SetupGame();
        }

        void SetupGame()
        {
            uis = new UISystem[]
            {
                new UISystem(new List<Button>()
                {
                    new Button("startGame", new Rectangle(vdims.X / 3, vdims.Y / 5, vdims.X / 5, vdims.Y / 10), new TextureDrawer(Content.Load<Texture2D>("yesnpressed"), new TextureFrame(new Rectangle(vdims.X / 7, vdims.Y / 5, vdims.X / 5, vdims.Y / 10), new Point(vdims.X / 10, vdims.Y / 20))))
                }),
                new UISystem(new List<Button>())
            };
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            //GENERATE VALUES
            float es = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //UPDATE INPUT
            ipp.Update(Keyboard.GetState(), GamePad.GetState(PlayerIndex.One));
            cursorManager.Update();

            //END
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            uis[currentUInb].HandleMouseInput(cursorManager);

            HandleGameStateChanges();



            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //DRAW TO MAIN
            scenes.SelectScene("main");
            scenes.SetupScene(spriteBatch, GraphicsDevice);

            GraphicsDevice.Clear(Color.Red);
            uis[currentUInb].Draw(spriteBatch);

            spriteBatch.End();

            //DRAW TO SCREEN
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin();

            scenes.DrawScene(spriteBatch, "main");

            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void HandleGameStateChanges()
        {
            if (gameState == GameState.Menu && uis[currentUInb].IssuedCommand("startGame"))
            {
                gameState = GameState.Game;
                currentUInb = 1;
            }
        }
    }
}
