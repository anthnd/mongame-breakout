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
        // TODO: Refactor, Refactor, REFACTOR!!
        // TODO: Fix ball-brick collisions. Right now it only reflects the ball's motion in the vertical axis.
        //       If it hits the side of a brick, it won't reflect horizontally (still vertically).


        // Default fields
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Window size
        int WinWidth = 640;
        int WinHeight = 480;

        // Initializing GameObject, texture and keyboard state variables
        GameObject paddle, ball;
        Level level1, level2, level3;
        Texture2D generalTexture;
        SpriteFont font;
        KeyboardState keyboardState;

        // Game constants
        // Paddle
        const int paddleLength = 100;
        const int paddleHeight = 13;
        const float paddleSpeed = 8.0f;

        // Ball
        const int ballSize = 17;
        const float speedComponent = 7.0f;

        // Bricks
        const int brickLength = 64;
        const int brickHeight = 24;
        const int maxBrickStack = 5;
        const float maxBounceAngle = 40 * MathHelper.Pi / 180;
        string[] gfxFileNames = {
            "brick_red",
            "brick_orange",
            "brick_yellow",
            "brick_green",
            "brick_lightblue",
            "brick_blue",
            "brick_purple"
        };

        // Score
        int score = 0;
        const int ScorePenalty = 750;
        const int ScoreGainBrick = 100;

        // Game state
        const int Level1 = 0;
        const int Level2 = 1;
        const int Level3 = 2;
        const int GameOver = 3;
        public int GameState = Level1;
        Level ActiveLevel;




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
            LoadLevel1();
            ActiveLevel = level1;
            LoadLevel2();
            LoadLevel3();
            level2.Disable(spriteBatch);
            level3.Disable(spriteBatch);
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
            ProgressLevels();
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
            DrawString( score.ToString(), new Vector2(20, GraphicsDevice.Viewport.Height - 50), spriteBatch  );

            // Draw the level
            level1.Draw(spriteBatch);
            level2.Draw(spriteBatch);
            level3.Draw(spriteBatch);

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


        private void LoadLevel1()
        {
            // Load level
            level1 = new Level();
            level1.LoadTextureFiles(gfxFileNames, Content);
            var posList = new List<Vector2>();
            for (int i = brickHeight; i < maxBrickStack * brickHeight; i += brickHeight)
            {
                for (int j = brickLength; j < GraphicsDevice.Viewport.Width - brickLength; j += brickLength)
                {
                    posList.Add
                        (
                            new Vector2(j, i)
                        );
                }
            }
            level1.SetObjectPositions(posList);
            level1.GenerateObjects();
        }


        private void LoadLevel2()
        {
            level2 = new Level();
            level2.LoadTextureFiles(gfxFileNames, Content);
            var posList = new List<Vector2>()
            {
                ToPos(0, 0), ToPos(1, 0), ToPos(2, 0), ToPos(3, 0), ToPos(4, 0), ToPos(5, 0), ToPos(6, 0), ToPos(7, 0), ToPos(8, 0), ToPos(9, 0),
                ToPos(0, 1),                                                                                                         ToPos(9, 1),                                                        ToPos(9, 1),
                ToPos(0, 2),              ToPos(2, 2), ToPos(3, 2), ToPos(4, 2), ToPos(5, 2), ToPos(6, 2), ToPos(7, 2),              ToPos(9, 2),
                ToPos(0, 3),              ToPos(2, 3),                                                     ToPos(7, 3),              ToPos(9, 3),                                                                                                           ToPos(9, 3),
                ToPos(0, 4),              ToPos(2, 4), ToPos(3, 4), ToPos(4, 4), ToPos(5, 4), ToPos(6, 4), ToPos(7, 4),              ToPos(9, 4),                                                                                                                                   ToPos(9, 4),
                ToPos(0, 5),                                                                                                         ToPos(9, 5),                                                        ToPos(9, 5),
                ToPos(0, 6), ToPos(1, 6), ToPos(2, 6), ToPos(3, 6), ToPos(4, 6), ToPos(5, 6), ToPos(6, 6), ToPos(7, 6), ToPos(8, 6), ToPos(9, 6)
            };
            level2.SetObjectPositions(posList);
            level2.GenerateObjects();
        }

        private void LoadLevel3()
        {
            level3 = new Level();
            level3.LoadTextureFiles(gfxFileNames, Content);
            var posList = new List<Vector2>()
            {
                ToPos(0, 0),              ToPos(2, 0),              ToPos(4, 0),              ToPos(6, 0),              ToPos(8, 0),
                             ToPos(1, 1),              ToPos(3, 1),              ToPos(5, 1),              ToPos(7, 1),              ToPos(9, 1),
                ToPos(0, 2),              ToPos(2, 2),              ToPos(4, 2),              ToPos(6, 2),              ToPos(8, 2),
                             ToPos(1, 3),              ToPos(3, 3),              ToPos(5, 3),              ToPos(7, 3),              ToPos(9, 3),
                ToPos(0, 4),              ToPos(2, 4),              ToPos(4, 4),              ToPos(6, 4),              ToPos(8, 4),
                             ToPos(1, 5),              ToPos(3, 5),              ToPos(5, 5),              ToPos(7, 5),              ToPos(9, 5),
                ToPos(0, 6),              ToPos(2, 6),              ToPos(4, 6),              ToPos(6, 6),              ToPos(8, 6)
            };
            level3.SetObjectPositions(posList);
            level3.GenerateObjects();
        }


        private void ProgressLevels()
        {
            switch(GameState)
            {
                case 0:
                    if (GameObject.IsDisabledList(level1.Objects))
                    {
                        ActiveLevel = level2;
                        GameState = Level2;
                        level1.Disable(spriteBatch);
                        level2.Enable(spriteBatch);
                        SetToStartPosition();
                    }
                    break;
                case 1:
                    if (GameObject.IsDisabledList(level2.Objects))
                    {
                        ActiveLevel = level3;
                        GameState = Level3;
                        level2.Disable(spriteBatch);
                        level3.Enable(spriteBatch);
                        SetToStartPosition();
                    }
                    break;
                case 2:
                    if (GameObject.IsDisabledList(level3.Objects))
                    {
                        level3.Disable(spriteBatch);
                        GameState = GameOver;
                    }
                    break;
                case 3:
                    DrawStringGlobal(
                        "It's overrrrr!!1one",
                        new Vector2( (GraphicsDevice.Viewport.Width-100)/2, 100 ),
                        spriteBatch
                        );
                    paddle.Follow(ball, "h");
                    break;
                default:
                    break;
            }
        }



        /// <summary>
        /// Checks the ball's collisions with the walls, paddles, and bricks and when it goes off-screen
        /// </summary>
        private void CheckBallCollisions()
        {
            // Ball collisions with the window walls
            if (ball.Rectangle.Right >= GraphicsDevice.Viewport.Width)
            {
                ball.Position.X = GraphicsDevice.Viewport.Width - ballSize;
                ball.Velocity.X *= -1;
            } 
            else 
            {
                if (ball.Position.X <= 0)
                {
                    ball.Position.X = 0;
                    ball.Velocity.X *= -1;
                }

            }
            if (ball.Rectangle.Top <= 0)
            {
                ball.Position.Y = 0;
                ball.Velocity.Y *= -1;
            }

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
            foreach (var obj in ActiveLevel.Objects)
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


        public void DrawStringGlobal(string s, Vector2 pos, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString( font, s, pos, Color.White );
            spriteBatch.End();
        }


        public void DrawString(string s, Vector2 pos, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString( font, s, pos, Color.White );
        }


        private Vector2 ToPos(int x, int y)
        {
            return new Vector2(x * brickLength, y * brickHeight);
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
