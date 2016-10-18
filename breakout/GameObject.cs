using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace breakout
{
    class GameObject
    {
        public Texture2D Texture;
        public Vector2 Position;
        public Vector2 Velocity;
        public int Width;
        public int Height;
        public Boolean Disabled = false;

        /// <summary>
        /// Rectangle for GameObjects without an imported texture
        /// </summary>
        public Rectangle Rectangle
        {
            get
            {
                if (!Disabled)
                {
                    return new Rectangle
                        (
                            (int)Position.X,
                            (int)Position.Y,
                            Width,
                            Height
                        );
                }
                return new Rectangle();
            }
        }

        /// <summary>
        /// Rectangle for GameObjects with an imported texture
        /// </summary>
        public Rectangle BoundingBox
        {
            get
            {
                if (!Disabled)
                {
                    return new Rectangle
                        (
                            (int)Position.X,
                            (int)Position.Y,
                            Texture.Width,
                            Texture.Height
                        );
                }
                return new Rectangle();
            }
        }

        /// <summary>
        /// Create a new GameObject with an imported texture
        /// </summary>
        /// <param name="texture">Texture to use for the object. A Texture2D Object</param>
        /// <param name="position">Initial position. A Vector2 Object</param>
        public GameObject(Texture2D texture, Vector2 position)
        {
            this.Texture = texture;
            this.Position = position;
        }

        /// <summary>
        /// Create a new GameObject with an imported texture
        /// </summary>
        /// <param name="texture">Texture to use for the object. A Texture2D Object</param>
        /// <param name="position">Initial position. A Vector2 Object</param>
        /// <param name="velocity">Initial velocity. A Vector2 Object</param>
        public GameObject(Texture2D texture, Vector2 position, Vector2 velocity)
        {
            this.Texture = texture;
            this.Position = position;
            this.Velocity = velocity;
        }

        /// <summary>
        /// Create a new GameObject without an imported texture
        /// </summary>
        /// <param name="texture">Texture to use for the object. A Texture2D Object</param>
        /// <param name="position">Initial position. A Vector2 Object</param>
        /// <param name="width">Width of the new GameObject</param>
        /// <param name="height">Height of the new GameObject</param>
        public GameObject(Texture2D texture, Vector2 position, int width, int height)
        {
            this.Texture = texture;
            this.Position = position;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Create a new GameObject without an imported texture
        /// </summary>
        /// <param name="texture">Texture to use for the object. A Texture2D Object</param>
        /// <param name="position">Initial position. A Vector2 Object</param>
        /// <param name="velocity">Initial velocity. A Vector2 Object</param>
        /// <param name="width">Width of the new GameObject</param>
        /// <param name="height">Height of the new GameObject</param>
        public GameObject(Texture2D texture, Vector2 position, Vector2 velocity, int width, int height)
        {
            this.Texture = texture;
            this.Position = position;
            this.Velocity = velocity;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Faster way of drawing GameObjects
        /// </summary>
        /// <param name="spriteBatch">A SpriteBatch instance</param>
        /// <param name="color">Color for the GameObject to be rendered. Default is white.</param>
        public void Draw(SpriteBatch spriteBatch, Color? color = null)
        {
            Color c = color ?? Color.White;
            spriteBatch.Draw(Texture, Rectangle, c);
        }

        public void Disable(SpriteBatch spriteBatch)
        {
            Disabled = true;
            spriteBatch.Begin();
            spriteBatch.Draw(Texture, Rectangle, new Color(0, 0, 0, 0));
            spriteBatch.End();
        }

    }
}
