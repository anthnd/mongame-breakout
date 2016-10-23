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
        public List<Texture2D> ObjectTextures;
        public List<Vector2> ObjectPositions;
        public List<GameObject> Objects;
        public List<int> ObjectHeightsSorted;


        /// <summary>
        /// Constructor
        /// </summary>
        public Level()
        {
            ObjectTextures = new List<Texture2D>();
            ObjectPositions = new List<Vector2>();
            Objects = new List<GameObject>();
            ObjectHeightsSorted = new List<int>();
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
                                this.ObjectTextures[GetTextureIndex((int)pos.Y)],
                                pos
                            )
                        );
                    if (index == ObjectTextures.Count - 1)
                        index = 0;
                    else
                        index++;
                }
            }
            else
                Console.WriteLine("ObjectTextures or ObjectPositions are empty.");
        }


        /// <summary>
        /// Draws the level's GameObjects
        /// </summary>
        /// <param name="spriteBatch">A SoriteBatch instance</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (Objects.Any())
            {
                foreach (var obj in Objects)
                    spriteBatch.Draw(obj.Texture, obj.BoundingBox, Color.White);
            }
            else
                Console.WriteLine("No objects to draw");
        }


        public void SetObjectPositions(List<Vector2> posList)
        {
            posList.Sort((pos1, pos2) => pos1.Y.CompareTo(pos2.Y));
            ObjectPositions.AddRange(posList);
            foreach (var pos in ObjectPositions)
            {
                if (ObjectHeightsSorted.Contains((int)pos.Y) == false)
                    ObjectHeightsSorted.Add((int)pos.Y);
            }
        }


        public int GetTextureIndex(int objHeight)
        {
            if (ObjectPositions.Any() && ObjectHeightsSorted.Any())
                return ObjectHeightsSorted.IndexOf(objHeight) % ObjectTextures.Count;
            else
                return -1;
        }


        public void Disable(SpriteBatch spriteBatch)
        {
            Objects.ForEach(obj => obj.Disable(spriteBatch));
        }


        public void Enable(SpriteBatch spriteBatch)
        {
            Objects.ForEach(obj => obj.Enable(spriteBatch));
        }


        public void Complete()
        {
            
        }

    }
}
