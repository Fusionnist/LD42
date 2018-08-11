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

using System.Xml.Linq;

namespace LD42
{
    public class Game1 : Game
    {
        GameState gameState;

        CursorManager cursorManager;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        EntityBuilder ebuilder;
        Tileset ts;

        UISystem[] uis;
        int currentUInb;

        

        Point vdims, wdims;
        double windowDivider;
        SceneCollection scenes;
        InputProfile ipp;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //GAME VALUES
            windowDivider = 0.5;
            vdims = new Point(224, 160);
            wdims = new Point((int)(1920 * windowDivider), (int)(1080 * windowDivider));

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
            ebuilder = new EntityBuilder();

            scenes = new SceneCollection();
            scenes.scenes.Add(new Scene(
                new RenderTarget2D(GraphicsDevice, vdims.X, vdims.Y),
                new Rectangle(0,0,vdims.X, vdims.Y),
                new Rectangle(0,0,wdims.X, wdims.Y),
                "main"
                ));

            cursorManager = new CursorManager();
            KeyManager[] keyManagers = new KeyManager[] { };
            ipp = new InputProfile(keyManagers);
        }

        //dick

        protected override void LoadContent()
        { 
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //LOAD XML
            ElementCollection.ReadDocument(XDocument.Load("Content/XML/Items.xml"));
            ElementCollection.ReadDocument(XDocument.Load("Content/XML/MiscEntities.xml"));
            ElementCollection.ReadDocument(XDocument.Load("Content/XML/Pickups.xml"));
            ElementCollection.ReadDocument(XDocument.Load("Content/XML/Spritesheets.xml"));
            //LOAD TEXTURES

            //LOAD SOUND

            //LOAD ENTITIES

            //LOAD UR MOM

            //END - SETUP THE GAME!
            SetupGame();
        }

        void SetupGame()
        {
            uis = new UISystem[]
            {
                new UISystem(new List<Button>()
                {
                    new Button("startGame", new Rectangle(vdims.X / 7, vdims.Y / 5, vdims.X / 5, vdims.Y / 10), new TextureDrawer(Content.Load<Texture2D>("yesnpressed"), new TextureFrame(new Rectangle(vdims.X / 7, vdims.Y / 5, vdims.X / 5, vdims.Y / 10), new Point(vdims.X / 10, vdims.Y / 20))))
                }),
                new UISystem(new List<Button>())
            };
            SetupUISystems();
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
            if (gameState == GameState.Game && ipp.JustPressed("p"))
            {
                gameState = GameState.Pause;
                currentUInb = 2;
            }
            if (gameState == GameState.Pause && ipp.JustPressed("p"))
            {
                gameState = GameState.Game;
                currentUInb = 1;
            }
            if (gameState == GameState.Pause && uis[currentUInb].IssuedCommand("returnToMenu"))
            {
                gameState = GameState.Menu;
                currentUInb = 0;
            }
        }

        protected void SetupUISystems()
        {
            uis = new UISystem[]
            {
                new UISystem(new List<Button>()
                {
                    new Button("startGame", new Rectangle(vdims.X / 5, vdims.Y / 5, vdims.X / 5, vdims.Y / 10), new TextureDrawer(Content.Load<Texture2D>("yesnpressed"), new TextureFrame(new Rectangle(vdims.X / 5, vdims.Y / 5, vdims.X / 5, vdims.Y / 10), new Point(vdims.X / 10, vdims.Y / 20))))
                }),
                new UISystem(new List<Button>()),
                new UISystem(new List<Button>()
                {
                    new Button("returnToMenu", new Rectangle(vdims.X / 5, vdims.Y / 5, vdims.X / 5, vdims.Y / 10), new TextureDrawer(Content.Load<Texture2D>("yesnpressed"), new TextureFrame(new Rectangle(vdims.X / 5, vdims.Y / 5, vdims.X / 5, vdims.Y / 10), new Point(vdims.X / 10, vdims.Y / 20))))
                })
            };

        }
    }
}
