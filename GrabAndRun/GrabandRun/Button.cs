using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrabAndRun
{
    public class Button
    {
        public Texture2D Texture;
        public Rectangle Bounds;
        public bool IsSelected;      // true si es el boton elegido con el teclado
        public Action OnClick;

        public Button(Texture2D texture, Vector2 center, Action onClick)
        {
            Texture = texture;
            OnClick = onClick;
            Bounds = new Rectangle(
                (int)(center.X - texture.Width / 2f),
                (int)(center.Y - texture.Height / 2f),
                texture.Width,
                texture.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Seleccionado: un poco mas grande y brillante. Normal: algo opaco.
            Color color = IsSelected ? Color.White : new Color(200, 200, 200);
            float scale = IsSelected ? 1.08f : 1f;

            Vector2 origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
            Vector2 position = new Vector2(Bounds.Center.X, Bounds.Center.Y);

            spriteBatch.Draw(Texture, position, null, color, 0f, origin, scale, SpriteEffects.None, 0f);
        }
    }
}
