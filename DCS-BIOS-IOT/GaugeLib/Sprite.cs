using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace net.willshouse.dcs.gaugelib

{
    public abstract class Sprite
    {
        protected Texture2D textureImage;
        protected Vector2 position;

        public Sprite ( Texture2D textureImage,Vector2 position)
        {
            this.textureImage = textureImage;
            this.position = position;
        }

        public virtual void Update(int value)
        {
            
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
