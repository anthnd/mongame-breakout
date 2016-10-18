using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace breakout
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Breakout : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int WinWidth = 640;
        int WinHeight = 480;

        KeyboardState keyboardState;

        GameObject leftWall, rightWall, paddle, ball;
        Texture2D generalTexture;
        SpriteFont font;

        int wallThickness = 0;

        int paddleLength = 90;
        int paddleHeight = 10;
        float paddleSpeed = 6.5f;

        int ballSize = 13;
        float speedComponent = 7.0f;

        int brickLength = 40;
        int brickHeight = 15;
        int maxBrickStack = 4;
        List<GameObject> bricks = new List<GameObject>();
        Color[] brickColors = {
            new Color(32, 148, 250), // blue
            new Color(255, 230, 32),  // yellow
            new Color(4, 222, 113),  // green
            new Color(255, 59, 48),  // red
            new Color(120, 122, 255) // purple
        };

        int score = 0;
        private const int ScorePenalty = 750;
        private const int ScoreGainBrick = 100;

        public Breakout()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = WinWidth;
            graphics.PreferredBackBufferHeight = WinHeight;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            generalTexture = new Texture2D(this.GraphicsDevice, 1, 1);
            generalTexture.SetData(new[] { Color.White });

            leftWall = new GameObject
                (
                    generalTexture,
                    Vector2.Zero,
                    wallThickness,
                    GraphicsDevice.Viewport.Height
                );

            rightWall = new GameObject
                (
                    generalTexture,
                    new Vector2
                    (
                        GraphicsDevice.Viewport.Width - wallThickness,
                        0
                    ),
                    wallThickness,
                    GraphicsDevice.Viewport.Height
                );

            paddle = new GameObject
                (
                    generalTexture,
                    new Vector2
                    (
                        ScreenCenterHorizontal(paddleLength),
                        GraphicsDevice.Viewport.Height - paddleHeight
                    ),
                    paddleLength,
                    paddleHeight
                );

            ball = new GameObject
                (
                    generalTexture,
                    new Vector2
                    (
                        ScreenCenterHorizontal(ballSize),
                        paddle.Position.Y - ballSize - 5
                    ),
                    new Vector2(speedComponent, -speedComponent),
                    ballSize,
                    ballSize
                );

            GenerateBricks();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Score");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            keyboardState = Keyboard.GetState();

            ball.Position += ball.Velocity;

            CheckBallCollisions();
            ControlPaddle(keyboardState);
            CheckPaddleWallCollision();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(30, 30, 38));

            spriteBatch.Begin();

            leftWall.Draw(spriteBatch, new Color(230, 230, 230));
            rightWall.Draw(spriteBatch, new Color(230, 230, 230));
            paddle.Draw(spriteBatch, new Color(242, 244, 255));
            ball.Draw(spriteBatch, new Color(242, 244, 255));

            int i = 0;
            foreach (var brick in bricks)
            {
                brick.Draw(spriteBatch, brickColors[i]);
                if (i != brickColors.Length - 1)
                    i++;
                else
                    i = 0;
            }

            spriteBatch.DrawString(
                font,
                score.ToString(),
                new Vector2(
                    20,
                    GraphicsDevice.Viewport.Height - 50
                ),
                Color.White
            );

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void GenerateBricks()
        {
            for (int i = 0; i < maxBrickStack * brickHeight; i += brickHeight)
            {
                for (int j = 0; j < GraphicsDevice.Viewport.Width; j += brickLength)
                {
                    System.Console.WriteLine(i + ", " + j);
                    bricks.Add
                    (
                        new GameObject
                        (
                            generalTexture,
                            new Vector2(j, i),
                            brickLength,
                            brickHeight
                        )
                    );
                }
            }
        }

        private void CheckBallCollisions()
        {
            if (ball.Rectangle.Right >= GraphicsDevice.Viewport.Width || ball.Rectangle.Left <= 0)
                ball.Velocity.X *= -1;
            if (ball.Rectangle.Top <= 0)
                ball.Velocity.Y *= -1;

            if (ball.Rectangle.Intersects(paddle.Rectangle))
                ball.Velocity.Y *= -1;

            if (ball.Position.Y > GraphicsDevice.Viewport.Height + 100)
            {
                SetToStartPosition();
                score -= ScorePenalty;
            }
                

            foreach (var brick in bricks)
            {
                if (ball.Rectangle.Intersects(brick.Rectangle))
                {
                    ball.Velocity.Y *= -1;
                    brick.Disable(spriteBatch);
                    score += ScoreGainBrick;
                }
                    
            }
        }

        private void ControlPaddle(KeyboardState state)
        {
            if (state.IsKeyDown(Keys.Left))
                paddle.Position.X -= paddleSpeed;
            if (state.IsKeyDown(Keys.Right))
                paddle.Position.X += paddleSpeed;
        }

        private void CheckPaddleWallCollision()
        {
            if (paddle.Rectangle.Left <= 0)
                paddle.Position.X = 0;
            if (paddle.Rectangle.Right >= GraphicsDevice.Viewport.Width)
                paddle.Position.X = GraphicsDevice.Viewport.Width - paddleLength;
        }

        private void SetToStartPosition()
        {
            ball.Position = new Vector2(ScreenCenterHorizontal(ballSize), paddle.Rectangle.Top - ballSize - 10);
            paddle.Position = new Vector2(ScreenCenterHorizontal(paddleLength), GraphicsDevice.Viewport.Height - paddleHeight);
        }

        private float ScreenCenterHorizontal(float objWidth)
        {
            return (GraphicsDevice.Viewport.Width - objWidth) / 2;
        }

        private float ScreenCenterVertical(float objHeight)
        {
            return (GraphicsDevice.Viewport.Height - objHeight) / 2;
        }
    }
}
