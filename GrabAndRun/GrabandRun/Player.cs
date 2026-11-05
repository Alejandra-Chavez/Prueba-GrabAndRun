using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GrabAndRun
{
    public class Player
    {
        // =====================================================================
        //  AJUSTES DE LOS ATLAS (spritesheets)
        //
        //  DetectarAutomatico = true:
        //     El juego busca solo cada frame de la imagen (los dibujos separados por
        //     espacio transparente). No hace falta contar nada. Sirve aunque los
        //     frames tengan distinto tamano o haya varias filas.
        //
        //  DetectarAutomatico = false:
        //     Usa los numeros de abajo. Columnas = cuadros a lo ancho,
        //     Filas = cuadros a lo alto, Frames = cuadros que tienen dibujo.
        //     Usalo si tus frames se tocan entre si (sin espacio transparente).
        // =====================================================================
        private const bool DetectarAutomatico = true;

        // Correr.png
        private const int CorrerColumnas = 6, CorrerFilas = 1, CorrerFrames = 6;
        private const float CorrerTiempo = 0.08f;   // segundos por cuadro

        // Saltar.png  (animacion mientras SUBE: al terminar se congela en el ultimo frame)
        private const int SaltoColumnas = 4, SaltoFilas = 1, SaltoFrames = 4;
        private const float SaltoTiempo = 0.10f;

        // Caer.png  (animacion mientras CAE: al terminar se congela en el ultimo frame hasta tocar el piso)
        private const bool UsarAnimacionCaida = false;  // false = el salto se congela en su ultimo frame hasta tocar el piso (no usa fall.png). true = al bajar cambia a fall.png
        private const int CaidaColumnas = 2, CaidaFilas = 1, CaidaFrames = 2;
        private const float CaidaTiempo = 0.10f;

        // Ataque.png
        private const int GolpeColumnas = 5, GolpeFilas = 1, GolpeFrames = 5;
        private const float GolpeTiempo = 0.07f;

        // =====================================================================
        //  AJUSTES DE MOVIMIENTO
        // =====================================================================
        private const float Escala = 1f;            // agranda o achica al personaje
        private const float Velocidad = 300f;       // pixeles por segundo
        private const float FuerzaSalto = 650f;
        private const float Gravedad = 1600f;
        // =====================================================================

        private enum Estado { Quieto, Correr, Saltar, Caer, Golpear }

        private Animation _quieto, _correr, _saltar, _golpear;
        private Animation _actual;
        private Estado _estado = Estado.Quieto;

        private float _y;                // posicion de los pies (eje Y de pantalla)
        private float _velY;
        private bool _enSuelo = true;
        private bool _mirandoDerecha = true;
        private bool _golpeando = false;

        public float WorldX { get; private set; }   // posicion en el mundo (la usa el fondo)

        public void Load(ContentManager content)
        {
            // idle.png es una sola imagen (1 columna, 1 fila, 1 cuadro)
            _quieto  = Animation.DesdeGrilla(content.Load<Texture2D>("arana"), 1, 1, 1, 1f, true);
            _correr  = Crear(content.Load<Texture2D>("Correr"),    CorrerColumnas, CorrerFilas, CorrerFrames, CorrerTiempo, true);
            _saltar  = Crear(content.Load<Texture2D>("Saltar"),   SaltoColumnas,  SaltoFilas,  SaltoFrames,  SaltoTiempo,  false);
            _golpear = Crear(content.Load<Texture2D>("Ataque"), GolpeColumnas,  GolpeFilas,  GolpeFrames,  GolpeTiempo,  false);

        }

        private static Animation Crear(Texture2D textura, int columnas, int filas, int frames, float tiempo, bool loop)
        {
            return DetectarAutomatico
                ? Animation.Automatica(textura, tiempo, loop)
                : Animation.DesdeGrilla(textura, columnas, filas, frames, tiempo, loop);
        }

        public void Reiniciar(float alturaSuelo)
        {
            WorldX = 0f;
            _y = alturaSuelo;
            _velY = 0f;
            _enSuelo = true;
            _golpeando = false;
            _mirandoDerecha = true;
            CambiarEstado(Estado.Quieto, true);
        }

        public void Update(GameTime gameTime, KeyboardState teclado, KeyboardState anterior, float alturaSuelo)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            bool izquierda = teclado.IsKeyDown(Keys.Left)  || teclado.IsKeyDown(Keys.A);
            bool derecha   = teclado.IsKeyDown(Keys.Right) || teclado.IsKeyDown(Keys.D);

            bool saltoPresionado = Presionada(teclado, anterior, Keys.Space);
            bool golpePresionado = Presionada(teclado, anterior, Keys.Q);


            // --- Movimiento horizontal (bloqueado mientras golpea) ---
            float direccion = 0f;
            if (!_golpeando)
            {
                if (izquierda) direccion -= 2f;
                if (derecha)   direccion += 2f;
            }
            WorldX += direccion * Velocidad * dt;
            if (direccion != 0f) _mirandoDerecha = direccion > 0f;

            // --- Salto ---
            if (saltoPresionado && _enSuelo)
            {
                _velY = -FuerzaSalto;
                _enSuelo = false;
            }

            // --- Golpe ---
            if (golpePresionado && !_golpeando)
            {
                _golpeando = true;
                CambiarEstado(Estado.Golpear, true);
            }

            // --- Gravedad ---
            _velY += Gravedad * dt;
            _y += _velY * dt;
            if (_y >= alturaSuelo)
            {
                _y = alturaSuelo;
                _velY = 0f;
                _enSuelo = true;
            }

            // --- Elegir animacion ---
            if (_golpeando && _golpear.Finished)
                _golpeando = false;

            if (!_golpeando)
            {
                if (!_enSuelo)
                {
                    // velY < 0: subiendo -> salto. velY > 0: bajando -> caida
                    bool cayendo = UsarAnimacionCaida && _velY > 0f;
                    CambiarEstado(cayendo ? Estado.Caer : Estado.Saltar);
                }
                else if (direccion != 0f) CambiarEstado(Estado.Correr);
                else                      CambiarEstado(Estado.Quieto);
            }

            _actual.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, float pantallaX)
        {
            _actual.Draw(spriteBatch, new Vector2(pantallaX, _y), _mirandoDerecha, Escala);
        }

        private void CambiarEstado(Estado nuevo, bool forzar = false)
        {
            if (nuevo == _estado && !forzar) return;

            _estado = nuevo;
            switch (nuevo)
            {
                case Estado.Quieto:  _actual = _quieto;  break;
                case Estado.Correr:  _actual = _correr;  break;
                case Estado.Saltar:  _actual = _saltar;  break;
                case Estado.Golpear: _actual = _golpear; break;
            }
            _actual.Reset();
        }

        private static bool Presionada(KeyboardState actual, KeyboardState anterior, Keys tecla)
        {
            return actual.IsKeyDown(tecla) && anterior.IsKeyUp(tecla);
        }
    }
}