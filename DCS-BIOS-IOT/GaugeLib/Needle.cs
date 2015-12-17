using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace net.willshouse.dcs.gaugelib
{
    public class Needle: Sprite

    {
        private float rads;
        public Needle(Texture2D textureImage, Vector2 position )
                    :base(textureImage, position)
        {

        }

        public override void Update(int value)
        {
            rads = (float)(2f * Math.PI * (value / 65535f));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, textureImage.Width, textureImage.Height);
            Vector2 origin = new Vector2(textureImage.Width/2, textureImage.Height-35);
            spriteBatch.Draw(textureImage, position, sourceRectangle, Color.White, rads, origin, 0.9f, SpriteEffects.None, 1);
            //spriteBatch.Draw(textureImage, position, Color.White);
            //spriteBatch.DrawString(font, "value: " + value, new Vector2(50, 25), Color.White);
            //spriteBatch.DrawString(font, "radians: " + rads.ToString("N6"), new Vector2(50, 50), Color.White);
        }
    }
}
