using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GrabAndRun
{
    public class GameWorld
    {
        // =====================================================
        //  AJUSTES
        // =====================================================
        private const float VelocidadFondo = 0.5f;  // 1 = se mueve igual que el jugador, 0.5 = mas lento (efecto profundidad)
        private const float AlturaSuelo = 120f;     // pixeles desde abajo de la pantalla hasta donde pisa el personaje
        private const bool DibujarSuelo = true;     // poner false si tu fondo ya trae el piso dibujado
        // =====================================================

        private readonly Texture2D _fondo;
        private readonly Texture2D _sand;
        private readonly Texture2D _sandSurface;
        private readonly Player _jugador;
        private readonly int _ancho;
        private readonly int _alto;
        private readonly float _ySuelo;
        private KeyboardState _tecladoAnterior;

        public GameWorld(GraphicsDevice graphicsDevice, ContentManager content, int ancho, int alto)
        {
            _ancho = ancho;
            _alto = alto;
            _ySuelo = alto - AlturaSuelo;

            _fondo = content.Load<Texture2D>("fondo desierto");
            _sand = content.Load<Texture2D>("arena");
            _sandSurface = content.Load<Texture2D>("arena_superficie");

            _jugador = new Player();
            _jugador.Load(content);
            Reiniciar();
        }

        public void Reiniciar()
        {
            _jugador.Reiniciar(_ySuelo);
            _tecladoAnterior = Keyboard.GetState();
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState teclado = Keyboard.GetState();
            _jugador.Update(gameTime, teclado, _tecladoAnterior, _ySuelo);
            _tecladoAnterior = teclado;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DibujarFondo(spriteBatch);
        

        int tilesAcross =  _alto / (int)_ySuelo;
        int surfaceY =  _alto - ((int)_ySuelo * 2);
    if (DibujarSuelo)
        {
        // Desplazamiento para que el suelo se mueva con el jugador
        int desplazamiento = (int)(_jugador.WorldX % _sand.Width);
        if (desplazamiento < 0) desplazamiento += _sand.Width;

        for (int x = -desplazamiento; x < _ancho; x += _sand.Width)
        {
            // Capa superior (superficie)
            spriteBatch.Draw(_sandSurface, new Vector2(x, _ySuelo), Color.White);

            // Capas de arena hacia abajo hasta el borde de la pantalla
            for (float y = _ySuelo + _sandSurface.Height; y < _alto; y += _sand.Height)
            {
                spriteBatch.Draw(_sand, new Vector2(x, y), Color.White);
            }
        }
}

            // El jugador queda siempre en el centro y el mundo se mueve detras
            _jugador.Draw(spriteBatch, _ancho / 2f);
        }

        // Repite la imagen de fondo en bucle infinito
        private void DibujarFondo(SpriteBatch spriteBatch)
        {
            float escala = (float)_alto / _fondo.Height;
            int anchoTile = (int)(_fondo.Width * escala);

            int desplazamiento = (int)((_jugador.WorldX * VelocidadFondo) % anchoTile);
            if (desplazamiento < 0) desplazamiento += anchoTile;

            for (int x = -desplazamiento; x < _ancho; x += anchoTile)
            {
                // +1 de ancho para evitar lineas finas entre una imagen y otra
                spriteBatch.Draw(_fondo, new Rectangle(x, 0, anchoTile + 1, _alto), Color.White);
            }
        }
    }
}
