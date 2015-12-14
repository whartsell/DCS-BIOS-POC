using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gauge_POC
{
     class GaugeFace
    {
        protected Texture2D textureImage;
        protected Vector2 position;

        public GaugeFace(Texture2D textureImage, Vector2 position)
        {
            this.textureImage = textureImage;
            this.position = position;
        }


        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textureImage, position, Color.White);
        }
    }
}
