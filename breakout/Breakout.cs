using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace breakout
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Breakout : Game
    {
        // Default fields
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Window size
        int WinWidth = 640;
        int WinHeight = 480;

        // Initializing GameObject, texture and keyboard state variables
        GameObject paddle, ball;
        Level level1;
        Texture2D generalTexture;
        SpriteFont font;
        KeyboardState keyboardState;

        // Game constants
        // Paddle
        const int paddleLength = 90;
        const int paddleHeight = 10;
        const float paddleSpeed = 7.0f;

        // Ball
        const int ballSize = 13;
        const float speedComponent = 7.0f;

        // Bricks
        const int brickLength = 40;
        const int brickHeight = 20;
        const int maxBrickStack = 5;
        const float maxBounceAngle = 55 * MathHelper.Pi / 180;
        //List<GameObject> bricks = new List<GameObject>();
        //List<Texture2D> brickTextures = new List<Texture2D>();
        string[] gfxFileNames = {
            "brick_blue",
            "brick_yellow",
            "brick_green",
            "brick_red",
            "brick_purple"
        };

        // Score
        int score = 0;
        const int ScorePenalty = 750;
        const int ScoreGainBrick = 100;




        /// <summary>
        /// Constructor
        /// </summary>
        public Breakout()
        {
            // Default
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Set window size
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

            // Create a general texture
            generalTexture = new Texture2D(this.GraphicsDevice, 1, 1);
            generalTexture.SetData(new[] { Color.White });

            // Loading
            font = Content.Load<SpriteFont>("Score");
            GenerateGeneralGameObjects();

            // Load level
            level1 = new Level();
            level1.LoadTextureFiles(gfxFileNames, Content);
            for (int i = 0; i < maxBrickStack * brickHeight; i += brickHeight)
            {
                for (int j = 0; j < GraphicsDevice.Viewport.Width; j += brickLength)
                {
                    level1.ObjectPositions.Add
                        (
                            new Vector2(j, i)
                        );
                }
            }
            level1.GenerateObjects();
            //GenerateBricks();
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

            // Drawing the constant GameObjects
            paddle.Draw(spriteBatch, new Color(242, 244, 255));
            ball.Draw(spriteBatch, new Color(242, 244, 255));

            // Draw the score
            spriteBatch.DrawString(
                font,
                score.ToString(),
                new Vector2(
                    20,
                    GraphicsDevice.Viewport.Height - 50
                ),
                Color.White
            );

            // Draw the level
            level1.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }


        /// <summary>
        /// Generates the constant GameObjects - the paddle and ball in this particular program
        /// </summary>
        private void GenerateGeneralGameObjects()
        {
            // Paddle at the bottom-center
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

            // Ball just above the paddle with an initial velocity
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
        }


        /// <summary>
        /// Checks the ball's collisions with the walls, paddles, and bricks and when it goes off-screen
        /// </summary>
        private void CheckBallCollisions()
        {
            // Ball collisions with the window walls
            if (ball.Rectangle.Right >= GraphicsDevice.Viewport.Width || ball.Rectangle.Left <= 0)
                ball.Velocity.X *= -1;
            if (ball.Rectangle.Top <= 0)
                ball.Velocity.Y *= -1;

            // Ball collsion with the paddle
            // Angled bounce depending on relative distance to paddle center
            if (ball.Rectangle.Intersects(paddle.Rectangle))
            {
                var relativeIntersectY = paddle.Rectangle.Center.X - ball.Rectangle.Center.X;
                var normalizedRelativeIntersectionY = (double)relativeIntersectY / (paddleLength / 2);
                var bounceAngle = normalizedRelativeIntersectionY * maxBounceAngle + MathHelper.PiOver2;
                ball.Velocity.X = Speed * (float)Math.Cos(bounceAngle);
                ball.Velocity.Y = Speed * (float)-Math.Sin(bounceAngle);
            }
                

            // Reset ball on out of bounds
            if (ball.Position.Y > GraphicsDevice.Viewport.Height + 100)
            {
                SetToStartPosition();
                score -= ScorePenalty;
            }

            // Check all bricks for collision with the ball
            foreach (var obj in level1.Objects)
            {
                if (ball.Rectangle.Intersects(obj.BoundingBox))
                {
                    ball.Velocity.Y *= -1;
                    obj.Disable(spriteBatch);
                    score += ScoreGainBrick;
                }
            }
        }


        /// <summary>
        /// Moves the paddle according to player input
        /// Controls: LeftArrow to go left
        ///           RightArrow to go right
        /// </summary>
        /// <param name="state"></param>
        private void ControlPaddle(KeyboardState state)
        {
            if (state.IsKeyDown(Keys.Left))
                paddle.Position.X -= paddleSpeed;
            if (state.IsKeyDown(Keys.Right))
                paddle.Position.X += paddleSpeed;
        }


        /// <summary>
        /// Ensures the paddle doesn't go off the screen
        /// </summary>
        private void CheckPaddleWallCollision()
        {
            if (paddle.Rectangle.Left <= 0)
                paddle.Position.X = 0;
            if (paddle.Rectangle.Right >= GraphicsDevice.Viewport.Width)
                paddle.Position.X = GraphicsDevice.Viewport.Width - paddleLength;
        }


        /// <summary>
        /// Sets the ball and paddle in their initial positions when the ball goes out of bounds
        /// </summary>
        private void SetToStartPosition()
        {
            ball.Position = new Vector2(ScreenCenterHorizontal(ballSize), paddle.Rectangle.Top - ballSize - 10);
            ball.Velocity = new Vector2(speedComponent, -speedComponent);
            paddle.Position = new Vector2(ScreenCenterHorizontal(paddleLength), GraphicsDevice.Viewport.Height - paddleHeight);
        }


        /// <summary>
        /// Returns the x position an object of a certain width should be placed to appear in the horizontal center
        /// </summary>
        /// <param name="objWidth">width</param>
        /// <returns></returns>
        private float ScreenCenterHorizontal(float objWidth)
        {
            return (GraphicsDevice.Viewport.Width - objWidth) / 2;
        }


        /// <summary>
        /// Returns the y position an object of a certain height should be placed to appear in the vertical center
        /// </summary>
        /// <param name="objHeight">height</param>
        /// <returns></returns>
        private float ScreenCenterVertical(float objHeight)
        {
            return (GraphicsDevice.Viewport.Height - objHeight) / 2;
        }

        /// <summary>
        /// Returns the hypotenuse where the legs are both equal to speedComponent
        /// </summary>
        private float Speed
        {
            get
            {
                return (float)Math.Sqrt(2 * speedComponent * speedComponent);
            }
        }
    }
}
