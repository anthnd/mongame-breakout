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
        // Default fields
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Window size
        int WinWidth = 640;
        int WinHeight = 480;

        // Initializing GameObject, texture and keyboard state variables
        GameObject paddle, ball;
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
        List<GameObject> bricks = new List<GameObject>();
        List<Texture2D> brickTextures = new List<Texture2D>();
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
            LoadTextureFiles();
            GenerateGeneralGameObjects();
            GenerateBricks();
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
            paddle.Follow(ball, "h");
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

            // Draw the generated bricks
            foreach (var brick in bricks)
                brick.DrawTextured(spriteBatch);

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

            spriteBatch.End();

            base.Draw(gameTime);
        }


        /// <summary>
        /// Generates the constant GameObjects - the paddle and ball in this particular program
        /// </summary>
        private void GenerateGeneralGameObjects()
        {
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
        }


        /// <summary>
        /// Load the files in the graphics files according to the supplmented filenames in gfxFileNames
        /// </summary>
        private void LoadTextureFiles()
        {
            foreach (var fileName in gfxFileNames)
            {
                brickTextures.Add(Content.Load<Texture2D>("graphics/" + fileName));
            }
        }


        /// <summary>
        /// Populates the bricks List with brick GameObjects
        /// </summary>
        private void GenerateBricks()
        {
            int index = 0;
            for (int i = 0; i < maxBrickStack * brickHeight; i += brickHeight)
            {
                for (int j = 0; j < GraphicsDevice.Viewport.Width; j += brickLength)
                {
                    
                    bricks.Add
                    (
                        new GameObject
                        (
                            brickTextures[index],
                            new Vector2(j, i)
                        )
                    );
                    if (index == brickTextures.Count - 1)
                        index = 0;
                    else
                        index++;
                }
            }
        }


        /// <summary>
        /// Checks the ball's collisions with the walls, paddles, and bricks and when it goes off-screen
        /// </summary>
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
                if (ball.Rectangle.Intersects(brick.BoundingBox))
                {
                    ball.Velocity.Y *= -1;
                    brick.Disable(spriteBatch);
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
    }
}
