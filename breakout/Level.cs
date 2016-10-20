using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace breakout
{
    class Level
    {
        public List<Texture2D> ObjectTextures = new List<Texture2D>();
        public List<Vector2> ObjectPositions = new List<Vector2>();
        public List<GameObject> Objects = new List<GameObject>();


        /// <summary>
        /// Constructor
        /// </summary>
        public Level()
        {

        }


        /// <summary>
        /// Load the files into the ObjectTextures list
        /// </summary>
        /// <param name="gfxFileNames">List of filenames to be loaded from the Content/graphics folder</param>
        /// <param name="content">ContentManager instance</param>
        public void LoadTextureFiles(string[] gfxFileNames, ContentManager content)
        {
            foreach (var fileName in gfxFileNames)
                this.ObjectTextures.Add(content.Load<Texture2D>("graphics/" + fileName));
        }


        /// <summary>
        /// Generates the level's GameObjects
        /// </summary>
        public void GenerateObjects()
        {
            if (ObjectTextures.Any() && ObjectPositions.Any())
            {
                int index = 0;
                foreach (var pos in ObjectPositions)
                {
                    this.Objects.Add
                        (
                            new GameObject
                            (
                                this.ObjectTextures[index],
                                pos
                            )
                        );
                    if (index == ObjectTextures.Count - 1)
                        index = 0;
                    else
                        index++;
                }
            }
        }


        /// <summary>
        /// Draws the level's GameObjects
        /// </summary>
        /// <param name="spriteBatch">A SoriteBatch instance</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var obj in Objects)
                spriteBatch.Draw(obj.Texture, obj.BoundingBox, Color.White);
        }

    }
}
