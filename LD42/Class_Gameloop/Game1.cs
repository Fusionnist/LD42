using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

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

        Timer easeIn, easeOut;
        CursorManager cursorManager;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        EntBuilder42 ebuilder;
        NotTechnicallyATileset ts;
        Inventory inven;
        Entity player;
        Timer dist;
        FontDrawer fdrawer;
        int score;
        TextureDrawer scoretex;
        UISystem[] uis;
        int currentUInb;

        Point vdims, wdims;
        double windowDivider;
        SceneCollection scenes;
        InputProfile ipp;
        float blackness;
        bool fading, goingToMenu;

        public Game1()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

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
            dist = new Timer(0.2f);

            easeOut = new Timer(5f);
            easeIn = new Timer(3f);

            gameState = GameState.Menu;
            currentUInb = 0;
            score = 0;
            IsMouseVisible = true;
            blackness = 0.5f;
            fading = true;
            goingToMenu = false;
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

            scenes.scenes.Add(new Scene(
              new RenderTarget2D(GraphicsDevice, vdims.X, vdims.Y),
              new Rectangle(0, 0, vdims.X, vdims.Y),
              new Rectangle(0, 0, vdims.X, vdims.Y),
              "inven"
              ));

            cursorManager = new CursorManager();
            KeyManager[] keyManagers = new KeyManager[] { };
            ipp = InputProfile.GetLetterProfile();
            fdrawer = new FontDrawer();

            List<TextureDrawer> chars = new List<TextureDrawer>();
            string white = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789?!:;.-_@";
            for (int i = 0; i < white.Length; i++)
                chars.Add(new TextureDrawer(Content.Load<Texture2D>("Placeholder/font_white"), new TextureFrame(new Rectangle(i * 6, 0, 5, 11), Point.Zero), null, white[i].ToString()));
            white = "abcdefghijklmnopqrstuvwxyz";
            for (int i = 0; i < white.Length; i++)
                chars.Add(new TextureDrawer(Content.Load<Texture2D>("Placeholder/font_white"), new TextureFrame(new Rectangle(i * 6, 13, 5, 11), Point.Zero), null, white[i].ToString()));
            //real font
            List<TextureDrawer> char2s = new List<TextureDrawer>();
            white = "01234";
            for (int i = 0; i < white.Length; i++)
                char2s.Add(new TextureDrawer(Content.Load<Texture2D>("UI/score"), new TextureFrame(new Rectangle(i * 16, 16, 16, 16), Point.Zero), null, white[i].ToString()));
            white = "56789";
            for (int i = 0; i < white.Length; i++)
                char2s.Add(new TextureDrawer(Content.Load<Texture2D>("UI/score"), new TextureFrame(new Rectangle(i * 16, 32, 16, 16), Point.Zero), null, white[i].ToString()));


            fdrawer.fonts = new List<DrawerCollection> { new DrawerCollection(chars, "whitefont"), new DrawerCollection(char2s, "realfont") };

            ipp.AddArrowInput();

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

            SpriteSheetCollection.ReadDocument(XDocument.Load("Content/XML/Spritesheets.xml"), Content);

            SoundManager.AddEffect(Content.Load<SoundEffect>("SFX/hit1"), "hit1");
            SoundManager.AddEffect(Content.Load<SoundEffect>("SFX/hit2"), "hit2");
            SoundManager.AddEffect(Content.Load<SoundEffect>("SFX/hit3"), "hit3");
            SoundManager.AddEffect(Content.Load<SoundEffect>("SFX/hit4"), "hit4");
            SoundManager.AddEffect(Content.Load<SoundEffect>("SFX/hit5"), "hit5");
            SoundManager.AddEffect(Content.Load<SoundEffect>("SFX/hit6"), "hit6");

            SoundManager.AddSong(Content.Load<Song>("Music/dungeonrun"), "gamesong");
            SoundManager.AddSong(Content.Load<Song>("Music/menusong"), "menusong");
            SoundManager.PlaySong("menusong");

            RecipeBook.ReadDocument(XDocument.Load("Content/XML/Recipes.xml"));
            //LOAD TEXTURES
            ParticleSystem.AcquireTxture(SpriteSheetCollection.GetTex("appear", "slotappears", "slot"));
            ParticleSystem.AcquireTxture(SpriteSheetCollection.GetTex("stars", "miscsheet1", "particle"));
            scoretex = SpriteSheetCollection.GetTex("score", "score", "button");
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
            EntityCollection.CreateGroup("enemy", "enemies");
            EntityCollection.CreateGroup("player", "players");

            SetupUISystems();
            ts = new NotTechnicallyATileset(vdims, ebuilder, Content);
            player = Assembler.GetEnt(ElementCollection.GetEntRef("player"), new Vector2(64, 65), Content, ebuilder);

            inven = new Inventory(Content);
            for (int x = 0; x < 3; x++)
            {
                inven.AddItem(Assembler.GetEnt(ElementCollection.GetEntRef("heart"), new Vector2(0, 0), Content, ebuilder));
            }
        }
        protected void SetupUISystems()
        {
            uis = new UISystem[]
            {
                new UISystem(new List<Button>()
                {
                    new Button("null", new Rectangle(0, 0, 224, 160), new TextureDrawer(Content.Load<Texture2D>("Placeholder/actualtitle"))),
                    new Button("startGame", new Rectangle(50, 140, 60, 20), SpriteSheetCollection.GetTex("play","playpressed","button"), SpriteSheetCollection.GetTex("pressed","playpressed","button"), SpriteSheetCollection.GetTex("pressed","playpressed","button")),
                    new Button("quit", new Rectangle(130, 140, 60, 20),  SpriteSheetCollection.GetTex("exitbutton","miscsheet1","button"), SpriteSheetCollection.GetTex("pressed","playpressed","button"), SpriteSheetCollection.GetTex("pressed","playpressed","button")),
                }),
                new UISystem(new List<Button>()),
                new UISystem(new List<Button>()
                {
                    new Button("null", new Rectangle(0, 0, 224, 160), new TextureDrawer(Content.Load<Texture2D>("Placeholder/pause"))),
                    new Button("returnToMenu", new Rectangle(120, 100, 60, 20),  SpriteSheetCollection.GetTex("menu","menuresume","button"), SpriteSheetCollection.GetTex("pressed","playpressed","button"), SpriteSheetCollection.GetTex("pressed","playpressed","button")),
                    new Button("retry", new Rectangle(40, 100, 60, 20),  SpriteSheetCollection.GetTex("retry","retry","button"), SpriteSheetCollection.GetTex("pressed","playpressed","button"), SpriteSheetCollection.GetTex("pressed","playpressed","button"))
                }),
                new UISystem(new List<Button>()
                {
                    new Button("null", new Rectangle(0, 0, 224, 160), new TextureDrawer(Content.Load<Texture2D>("Placeholder/dead"))),
                    new Button("returnToMenu", new Rectangle(120, 100, 60, 20), SpriteSheetCollection.GetTex("menu","menuresume","button"), SpriteSheetCollection.GetTex("pressed","playpressed","button"), SpriteSheetCollection.GetTex("pressed","playpressed","button")),
                    new Button("retry", new Rectangle(40, 100, 60, 20), SpriteSheetCollection.GetTex("retry","retry","button"), SpriteSheetCollection.GetTex("pressed","playpressed","button"), SpriteSheetCollection.GetTex("pressed","playpressed","button")),
                }),
                new UISystem(new List<Button>()
                {
                    new Button("null", new Rectangle(0, 0, 224, 160), new TextureDrawer(Content.Load<Texture2D>("UI/Image1"))),
                    new Button("startGame", new Rectangle(168, 137, 44, 15), SpriteSheetCollection.GetTex("play","playpressed","button"), SpriteSheetCollection.GetTex("pressed","playpressed","button"), SpriteSheetCollection.GetTex("pressed","playpressed","button")),
                }),
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

            if (gameState == GameState.Game || gameState == GameState.TransitionG || gameState == GameState.Dead || gameState == GameState.TransitionD)
                UpdateGame(es);

            if (gameState == GameState.TransitionG || gameState == GameState.TransitionM || gameState == GameState.TransitionP || gameState == GameState.TransitionD)
                ChangeAlpha(es);

            //END
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }
        protected void HandleGameStateChanges()
        {
            if (gameState == GameState.Menu && uis[currentUInb].IssuedCommand("startGame"))
            {
                gameState = GameState.Tutorial;
                currentUInb = 4;
                SoundManager.PlaySong("gamesong");
            }
            if (gameState == GameState.Tutorial && uis[currentUInb].IssuedCommand("startGame"))
            {
                gameState = GameState.TransitionM;
                easeIn.Reset();
            }
            else if (gameState == GameState.Menu && uis[currentUInb].IssuedCommand("quit"))
            {
                Exit();
            }
            else if (inven.PlayerDead() && gameState == GameState.Game)
            {
                gameState = GameState.Dead;
                currentUInb = 3;
                player.isDestroyed = true;
                score = inven.GetScore();
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
                gameState = GameState.TransitionP;
                SoundManager.PlaySong("menusong");
                goingToMenu = true;
            }
            else if (gameState == GameState.Pause && uis[currentUInb].IssuedCommand("retry"))
            {
                gameState = GameState.TransitionP;
                goingToMenu = false;
                ParticleSystem.Flush();
            }
            else if (gameState == GameState.Dead && uis[currentUInb].IssuedCommand("returnToMenu"))
            {
                gameState = GameState.TransitionD;
                SoundManager.PlaySong("menusong");
                goingToMenu = true;
            }
            else if (gameState == GameState.Dead && uis[currentUInb].IssuedCommand("retry"))
            {
                gameState = GameState.TransitionD;
                goingToMenu = false;
                ParticleSystem.Flush();
            }
        }
        protected void UpdateUIStuff()
        {
            if (gameState == GameState.Pause || gameState == GameState.Tutorial || gameState == GameState.Menu || gameState == GameState.Dead)
                uis[currentUInb].HandleMouseInput(cursorManager, scenes.GetScene("main").ToVirtualPos(cursorManager.RawPos()));
            HandleGameStateChanges();
        }
        protected void UpdateGame(float es_)
        {
            dist.Update(es_);
            if(gameState == GameState.Dead && dist.Complete())
            {
                inven.LoseItem();
                dist.Reset();
            }
            easeIn.Update(es_);
            if (easeIn.Complete()) { easeIn.Stop(); }
            if (gameState != GameState.TransitionG)
            {
                Vector2 input = Vector2.Zero;
                if (gameState == GameState.Game)
                {
                    if (ipp.Pressed("w") || ipp.Pressed("up"))
                        input.Y = -1;

                    if (ipp.Pressed("s") || ipp.Pressed("down"))
                        input.Y = 1;

                    if (ipp.JustPressed("d") || ipp.JustPressed("right"))
                        input.X = 1;
                }

                player.Input(input);
                player.Move();
                EntityCollection.MoveGroup("enemies");

                player.MultMov(es_);
                EntityCollection.MultMovGroup("enemies", es_);

                HandleCollisions();

                player.Update(es_);
                EntityCollection.UpdateGroup("enemies", es_);
                EntityCollection.UpdateGroup("pickups", es_);
            }
            ParticleSystem.UpdateAll(es_);

            ts.Update(es_, player.pos.X - 32);

            inven.Update(es_);

            EntityCollection.RecycleAll();
        }
        protected void HandleCollisions()
        {
            player.touchWallD = false;
            if (player.pos.Y + player.mov.Y + 32 > 92)
            {
                player.vel.Y = 0;
                player.mov.Y -= (player.pos.Y + player.mov.Y + 32 - 92);
                player.touchWallD = true;
            }

            foreach (var tile in EntityCollection.GetGroup("tiles"))
            {
                foreach (Entity enemy in EntityCollection.GetGroup("enemies"))
                {
                    CollisionSolver.SolveEntTileCollision(enemy, tile);
                }
            }
           
            foreach (var tile in EntityCollection.GetGroup("tiles"))
            {
                foreach (Entity enemy in EntityCollection.GetGroup("enemies"))
                {
                    CollisionSolver.SecondPassCollision(enemy, tile);
                }
            }

            foreach (var tile in EntityCollection.GetGroup("slots"))
            {
                if (tile.PredictIntersect(player, new Vector2(player.pos.X + player.mov.X - 33, 0)))
                {
                    if(player.vel.Y < 0) { player.vel.Y = 0; }
                }

            }


            foreach (Entity en in EntityCollection.GetGroup("enemies"))
            {
                if(!en.isDestroyed && player.PredictIntersect(en))
                {
                    if(en.pos.Y - player.pos.Y > 16 && player.vel.Y > 0)
                    {
                        player.React("headJump");
                        en.React("headJump");
                        ParticleSystem.CreateInstance(en.pos + new Vector2(8, -16), "stars");
                        SoundManager.PlayEffect("hit2");
                    }
                    else if(!player.invin)
                    {
                        player.ToggleInvin();
                        inven.LoseHP();
                        SoundManager.PlayEffect("hit4");
                    }                
                }
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
                    inven.AddItem(pickup.SubEntities()[0]);
                    pickup.exists = false;
                }

            }
        }
        protected void ChangeAlpha(float es_)
        {
            if (fading)
            {
                blackness += es_ * .75f;
                if (blackness >= 1)
                {
                    blackness = 1;
                    fading = false;
                    if (gameState == GameState.TransitionM)
                    { gameState = GameState.TransitionG; currentUInb = 1; }
                    else if (goingToMenu)
                    { gameState = GameState.TransitionM; currentUInb = 0; }
                    else
                    { gameState = GameState.TransitionG; currentUInb = 1; SetupGame(); easeIn.Reset(); }
                }
            }
            else
            {
                blackness -= es_ * .75f;
                if (blackness <= 0)
                {
                    blackness = 0;
                    fading = true;
                    if (gameState == GameState.TransitionG)
                    { gameState = GameState.Game; currentUInb = 1; }
                    else if (gameState == GameState.TransitionM)
                    { gameState = GameState.Menu; currentUInb = 0; SetupGame(); }
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
                case GameState.Tutorial:
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
                case GameState.TransitionM:
                    DrawUI();
                    break;
                case GameState.TransitionG:
                    DrawInventory();
                    DrawGame();
                    break;
                case GameState.TransitionP:
                    DrawGame();
                    DrawInventory();
                    break;
                case GameState.TransitionD:
                    DrawInventory();
                    DrawGame();
                    DrawUI();
                    break;
                case GameState.Dead:
                    DrawInventory();
                    DrawGame();
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
                case GameState.Tutorial:
                    scenes.DrawScene(spriteBatch, "UI");
                    break;
                case GameState.Game:
                    scenes.DrawScene(spriteBatch, "game");
                    scenes.DrawScene(spriteBatch, "inven");
                    break;
                case GameState.Pause:
                    scenes.DrawScene(spriteBatch, "game");
                    scenes.DrawScene(spriteBatch, "inven");
                    scenes.DrawScene(spriteBatch, "UI");
                    break;
                case GameState.TransitionM:
                    scenes.DrawScene(spriteBatch, "UI");
                    break;
                case GameState.TransitionG:
                    scenes.DrawScene(spriteBatch, "game");
                    scenes.DrawScene(spriteBatch, "inven");
                    break;
                case GameState.TransitionP:
                    scenes.DrawScene(spriteBatch, "game");
                    scenes.DrawScene(spriteBatch, "inven");
                    scenes.DrawScene(spriteBatch, "UI");
                    break;
                case GameState.TransitionD:
                    scenes.DrawScene(spriteBatch, "game");
                    scenes.DrawScene(spriteBatch, "inven");
                    scenes.DrawScene(spriteBatch, "UI");
                    break;
                case GameState.Dead:
                    scenes.DrawScene(spriteBatch, "game");
                    scenes.DrawScene(spriteBatch, "inven");
                    scenes.DrawScene(spriteBatch, "UI");
                    break;
            }
            spriteBatch.End();

            spriteBatch.Begin();
            if (gameState == GameState.TransitionG || gameState == GameState.TransitionM || gameState == GameState.TransitionP || gameState == GameState.TransitionD)
                spriteBatch.Draw(Content.Load<Texture2D>("Placeholder/black"), new Rectangle(0, 0, wdims.X, wdims.Y), new Color(Color.Black, blackness));
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

            if (gameState == GameState.Dead)
            {
                fdrawer.DrawText("realfont", score.ToString(), new Rectangle(144, 53, 224, 160), spriteBatch);
                scoretex.Draw(spriteBatch, new Vector2(64, 53));
            }

            spriteBatch.End();
        }
        void DrawGame()
        {
            scenes.SelectScene("game");
            scenes.CurrentScene.TranslateTo(new Vector2((float)Math.Round(player.pos.ToPoint().ToVector2().X, 1) - 32 + easeIn.timer*64, 0), false);
            scenes.SetupScene(spriteBatch, GraphicsDevice);
            //DRAW HERE
            ts.Draw(spriteBatch);
            EntityCollection.DrawGroup("enemies", spriteBatch);
            ParticleSystem.DrawGroup("stars", spriteBatch);
            player.Draw(spriteBatch);
            spriteBatch.End();
        }
        void DrawInventory()
        {
            scenes.SelectScene("inven");
            scenes.SetupScene(spriteBatch, GraphicsDevice);

            GraphicsDevice.Clear(Color.TransparentBlack);
            //draw slots
            inven.UpdateSlots();
            EntityCollection.DrawGroup("slots", spriteBatch);
            EntityCollection.DrawGroup("items", spriteBatch);

            ParticleSystem.DrawGroup("appear", spriteBatch);

            spriteBatch.End();
        }
    }
}
