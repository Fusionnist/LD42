using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.FZT;
using MonoGame.FZT.Data;
using MonoGame.FZT.Drawing;
using MonoGame.FZT.Input;
using MonoGame.FZT.UI;
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

        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            gameState = GameState.Menu;
            currentUInb = 0;

            base.Initialize();
        }

        //dick

        protected override void LoadContent()
        { 
            spriteBatch = new SpriteBatch(GraphicsDevice);
            cursorManager = new CursorManager();
            uis = new UISystem[]
            {
                new UISystem(new List<Button>()
                {
                    new Button("startGame", new Rectangle(0, 0, 800, 400), new TextureDrawer(Content.Load<Texture2D>("yesnpressed"), new TextureFrame(new Rectangle(0, 0, 800, 400), new Point(400, 200))), new TextureDrawer(Content.Load<Texture2D>("yesnpressed")))
                }),
                new UISystem()
            };
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            cursorManager.Update();

            uis[currentUInb].HandleMouseInput(cursorManager);

            HandleGameStateChanges();



            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            uis[currentUInb].Draw(spriteBatch);

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
