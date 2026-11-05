using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrabAndRun
{
    // Animacion basada en un atlas / spritesheet.
    // Cada frame es un RECTANGULO distinto dentro de la imagen. En pantalla se muestra
    // un solo rectangulo a la vez y se va pasando al siguiente en orden
    // (izquierda a derecha, y de arriba hacia abajo).
    public class Animation
    {
        private const int AlphaMinimo = 10;    // pixel con alpha mayor a esto = tiene dibujo
        private const int TamanoMinimo = 4;    // ignora manchitas sueltas menores a esto (px)

        private readonly List<Rectangle> _frames;

        public Texture2D Texture { get; }
        public int FrameCount => _frames.Count;
        public float FrameTime { get; }   // segundos que dura cada frame
        public bool Loop { get; }
        public bool Finished { get; private set; }

        private int _frame;
        private float _timer;

        public Animation(Texture2D texture, List<Rectangle> frames, float frameTime, bool loop)
        {
            Texture = texture;
            _frames = frames.Count > 0
                ? frames
                : new List<Rectangle> { new Rectangle(0, 0, texture.Width, texture.Height) };
            FrameTime = frameTime;
            Loop = loop;
        }

        // ---------- Opcion A: la imagen es una grilla de cuadros iguales ----------
        public static Animation DesdeGrilla(Texture2D texture, int columnas, int filas, int frames,
                                            float frameTime, bool loop)
        {
            int fw = texture.Width / columnas;
            int fh = texture.Height / filas;
            int total = Math.Min(frames, columnas * filas);

            var lista = new List<Rectangle>();
            for (int i = 0; i < total; i++)
                lista.Add(new Rectangle((i % columnas) * fw, (i / columnas) * fh, fw, fh));

            return new Animation(texture, lista, frameTime, loop);
        }

        // ---------- Opcion B: detecta solos los frames ----------
        // Busca los dibujos separados por espacio transparente. Sirve aunque los frames
        // tengan distinto tamano o la imagen tenga varias filas.
        public static Animation Automatica(Texture2D texture, float frameTime, bool loop)
        {
            return new Animation(texture, DetectarFrames(texture), frameTime, loop);
        }

        private static List<Rectangle> DetectarFrames(Texture2D tex)
        {
            int w = tex.Width, h = tex.Height;
            Color[] px = new Color[w * h];
            tex.GetData(px);

            var resultado = new List<Rectangle>();

            // 1) Separar en filas (bandas horizontales con dibujo)
            foreach (var (y0, y1) in Rangos(h, y =>
            {
                for (int x = 0; x < w; x++)
                    if (px[y * w + x].A > AlphaMinimo) return true;
                return false;
            }))
            {
                // 2) Dentro de cada fila, separar en columnas (frames de izquierda a derecha)
                foreach (var (x0, x1) in Rangos(w, x =>
                {
                    for (int y = y0; y <= y1; y++)
                        if (px[y * w + x].A > AlphaMinimo) return true;
                    return false;
                }))
                {
                    int ancho = x1 - x0 + 1;
                    int alto = y1 - y0 + 1;
                    if (ancho >= TamanoMinimo && alto >= TamanoMinimo)
                        resultado.Add(new Rectangle(x0, y0, ancho, alto));
                }
            }

            return resultado;
        }

        // Devuelve los tramos consecutivos (inicio, fin) donde tieneContenido es true
        private static List<(int, int)> Rangos(int largo, Func<int, bool> tieneContenido)
        {
            var rangos = new List<(int, int)>();
            int inicio = -1;
            for (int i = 0; i < largo; i++)
            {
                if (tieneContenido(i))
                {
                    if (inicio < 0) inicio = i;
                }
                else if (inicio >= 0)
                {
                    rangos.Add((inicio, i - 1));
                    inicio = -1;
                }
            }
            if (inicio >= 0) rangos.Add((inicio, largo - 1));
            return rangos;
        }

        // ---------- Animacion ----------
        public void Reset()
        {
            _frame = 0;
            _timer = 0f;
            Finished = false;
        }

        public void Update(GameTime gameTime)
        {
            if (Finished || (FrameCount <= 1 && Loop)) return;

            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (_timer >= FrameTime)
            {
                _timer -= FrameTime;
                _frame++;

                if (_frame >= FrameCount)
                {
                    if (Loop)
                    {
                        _frame = 0;
                    }
                    else
                    {
                        _frame = FrameCount - 1;
                        Finished = true;
                        break;
                    }
                }
            }
        }

        // 'feet' es el punto donde apoya el personaje (centro-abajo del frame)
        public void Draw(SpriteBatch spriteBatch, Vector2 feet, bool facingRight, float scale)
        {
            Rectangle source = _frames[_frame];
            Vector2 origin = new Vector2(source.Width / 2f, source.Height);
            SpriteEffects effect = facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(Texture, feet, source, Color.White, 0f, origin, scale, effect, 0f);
        }
    }
}