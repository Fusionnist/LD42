using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

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
using System;

namespace LD42
{
    public class Game1 : Game
    {
        GameState gameState;

        CursorManager cursorManager;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        EntBuilder42 ebuilder;
        NotTechnicallyATileset ts;
        Inventory inven;
        Player player;

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
            windowDivider = 4;
            vdims = new Point(224, 160);
            wdims = new Point((int)(vdims.X * windowDivider), (int)(vdims.Y * windowDivider));

            graphics.PreferredBackBufferWidth = wdims.X;
            graphics.PreferredBackBufferHeight = wdims.Y;
            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            gameState = GameState.Menu;
            currentUInb = 0;
            IsMouseVisible = true;
            //VALUES

            //UTILITY
            ebuilder = new EntBuilder42();

            scenes = new SceneCollection();
            scenes.scenes.Add(new Scene(
                new RenderTarget2D(GraphicsDevice, vdims.X, vdims.Y),
                new Rectangle(0,0,vdims.X, vdims.Y),
                new Rectangle(0,0,wdims.X, wdims.Y),
                "main"
                ));

            scenes.scenes.Add(new Scene(
                new RenderTarget2D(GraphicsDevice, vdims.X, vdims.Y),
                new Rectangle(0, 0, vdims.X, vdims.Y),
                new Rectangle(0, 0, vdims.X, vdims.Y),
                "UI"
                ));

            scenes.scenes.Add(new Scene(
               new RenderTarget2D(GraphicsDevice, vdims.X, vdims.Y),
               new Rectangle(0, 0, vdims.X, vdims.Y),
               new Rectangle(0, 0, vdims.X, vdims.Y),
               "game"
               ));

            cursorManager = new CursorManager();
            KeyManager[] keyManagers = new KeyManager[] { };
            ipp = InputProfile.GetLetterProfile();

            base.Initialize();
        }
        protected override void LoadContent()
        { 
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //LOAD XML
            ElementCollection.ReadDocument(XDocument.Load("Content/XML/Items.xml"));
            ElementCollection.ReadDocument(XDocument.Load("Content/XML/MiscEntities.xml"));
            ElementCollection.ReadDocument(XDocument.Load("Content/XML/Pickups.xml"));
            ElementCollection.ReadDocument(XDocument.Load("Content/XML/Spritesheets.xml"));

            SpriteSheetCollection.LoadSheet(ElementCollection.GetSpritesheetRef("placeholderSheet"), Content);

            RecipeBook.ReadDocument(XDocument.Load("Content/XML/Recipes.xml"));
            //LOAD TEXTURES

            //LOAD SOUND

            //LOAD ENTITIES

            //LOAD UR MOM
            SetupUISystems();

            //END - SETUP THE GAME!
            SetupGame();
        }
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        //SETUP
        void SetupGame()
        {
            //CREATE GROUPS
            EntityCollection.Flush();
            EntityCollection.CreateGroup("slot", "slots");
            EntityCollection.CreateGroup("item", "items");
            EntityCollection.CreateGroup("pickup", "pickups");

            SetupUISystems();
            ts = new NotTechnicallyATileset(new Texture2D[] { Content.Load<Texture2D>("yesnpressed"), Content.Load<Texture2D>("Placeholder/placeholder1") }, vdims, ebuilder, Content);
            player = new Player
                (
                new DrawerCollection(new List<TextureDrawer>() { new TextureDrawer(Content.Load<Texture2D>("yesnpressed"), new HitboxCollection[] { new HitboxCollection(new FRectangle[][] { new FRectangle[] { new FRectangle(0, 0, 50, 20) } }, "collision") }) }, "texes"), 
                new Vector2(2 * vdims.X / 7, 3 * vdims.Y / 7), 
                new List<Property>()
                );

            inven = new Inventory(Content);
            for (int x = 0; x < 7; x++)
            {
                inven.AddItem(Assembler.GetEnt(ElementCollection.GetEntRef("placeholderItem"), new Vector2(0, 0), Content, ebuilder));
            }
        }
        protected void SetupUISystems()
        {
            uis = new UISystem[]
            {
                new UISystem(new List<Button>()
                {
                    new Button("startGame", new Rectangle(vdims.X / 5, vdims.Y / 5, vdims.X / 3, vdims.Y / 3), new TextureDrawer(Content.Load<Texture2D>("yesnpressed"), new TextureFrame(new Rectangle(vdims.X / 5, vdims.Y / 5, vdims.X / 3, vdims.Y / 3), new Point(vdims.X / 10, vdims.Y / 20))))
                }),
                new UISystem(new List<Button>()),
                new UISystem(new List<Button>()
                {
                    new Button("returnToMenu", new Rectangle(vdims.X / 5, vdims.Y / 5, vdims.X / 2, vdims.Y / 4), new TextureDrawer(Content.Load<Texture2D>("yesnpressed"), new TextureFrame(new Rectangle(vdims.X / 5, vdims.Y / 5, vdims.X / 2, vdims.Y / 4), new Point(vdims.X / 10, vdims.Y / 20))))
                })
            };

        }

        //UPDATE
        protected override void Update(GameTime gameTime)
        {
            //GENERATE VALUES
            float es = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //UPDATE INPUT
            ipp.Update(Keyboard.GetState(), GamePad.GetState(PlayerIndex.One));
            cursorManager.Update();
            //self-explanatory
            UpdateUIStuff();

            if (gameState == GameState.Game)
                UpdateGame(es);

            //END
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }
        protected void HandleGameStateChanges()
        {
            if (gameState == GameState.Menu && uis[currentUInb].IssuedCommand("startGame"))
            {
                gameState = GameState.Game;
                currentUInb = 1;
            }
            else if (gameState == GameState.Game && ipp.JustPressed("p"))
            {
                gameState = GameState.Pause;
                currentUInb = 2;
            }
            else if (gameState == GameState.Pause && ipp.JustPressed("p"))
            {
                gameState = GameState.Game;
                currentUInb = 1;
            }
            else if (gameState == GameState.Pause && uis[currentUInb].IssuedCommand("returnToMenu"))
            {
                gameState = GameState.Menu;
                currentUInb = 0;
            }
        }
        protected void UpdateUIStuff()
        {
            uis[currentUInb].HandleMouseInput(cursorManager, scenes.GetScene("main").ToVirtualPos(cursorManager.RawPos()));
            HandleGameStateChanges();
        }
        protected void UpdateGame(float es_)
        {
            Vector2 input = Vector2.Zero;
            if (ipp.JustPressed("w"))
                input.Y = -1;

            player.Input(input);
            player.Move();
            player.MultMov(es_);
            HandleCollisions();
            player.Update(es_);

            ts.Update(es_, player.pos.X - 64);

            inven.Update(es_);

            EntityCollection.RecycleAll();
        }
        protected void HandleCollisions()
        {
            foreach (var tile in EntityCollection.GetGroup("tiles"))
            {
                CollisionSolver.SolveEntTileCollision(player, tile);
                CollisionSolver.SecondPassCollision(player, tile);
            }
            foreach (var pickup in EntityCollection.GetGroup("pickups"))
            {
                bool x = false;
                foreach (FRectangle rect in player.MovHB())
                {
                    foreach (FRectangle rect2 in pickup.MovHB())
                    {
                        if (rect.Intersects(rect2))
                        {
                            if(pickup.exists)
                            x = true;
                        }
                    }
                }
                if (x)
                {
                    inven.AddItem(Assembler.GetEnt(ElementCollection.GetEntRef("placeholderItem"), new Vector2(0, 0), Content, ebuilder));
                    pickup.exists = false;
                }

            }
        }
        //DRAW
        protected override void Draw(GameTime gameTime)
        {
            switch (gameState)
            {
                case GameState.Menu:
                    DrawUI();
                    break;
                case GameState.Game:
                    DrawInventory();
                    DrawGame();
                    break;
                case GameState.Pause:
                    DrawGame();
                    DrawInventory();
                    DrawUI();
                    break;
            }

            //DRAW TO MAIN
            scenes.SelectScene("main");
            scenes.SetupScene(spriteBatch, GraphicsDevice);

            switch (gameState)
            {
                case GameState.Menu:
                    scenes.DrawScene(spriteBatch, "UI");
                    break;
                case GameState.Game:
                    scenes.DrawScene(spriteBatch, "game");
                    scenes.DrawScene(spriteBatch, "UI");
                    break;
                case GameState.Pause:
                    scenes.DrawScene(spriteBatch, "game");
                    scenes.DrawScene(spriteBatch, "UI");
                    break;
            }
            spriteBatch.End();

            //DRAW TO SCREEN
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(samplerState: SamplerState.PointWrap);

            scenes.DrawScene(spriteBatch, "main");

            spriteBatch.End();

            base.Draw(gameTime);
        }
        void DrawUI()
        {
            scenes.SelectScene("UI");
            scenes.SetupScene(spriteBatch, GraphicsDevice);

            uis[currentUInb].Draw(spriteBatch);

            spriteBatch.End();
        }
        void DrawGame()
        {
            scenes.SelectScene("game");
            scenes.CurrentScene.TranslateTo(new Vector2((float)Math.Round(player.pos.X) - 8, 0), false);
            scenes.SetupScene(spriteBatch, GraphicsDevice);
            //DRAW HERE
            ts.Draw(spriteBatch);
            player.Draw(spriteBatch);
            spriteBatch.End();
        }
        void DrawInventory()
        {
            scenes.SelectScene("UI");
            scenes.SetupScene(spriteBatch, GraphicsDevice);

            GraphicsDevice.Clear(Color.TransparentBlack);
            //draw slots
            inven.UpdateSlots();
            EntityCollection.DrawGroup("slots", spriteBatch);
            EntityCollection.DrawGroup("items", spriteBatch);

            spriteBatch.End();
        }
    }
}
