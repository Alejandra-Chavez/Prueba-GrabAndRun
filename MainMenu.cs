using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GrabAndRun
{
    public class MainMenu
    {
        private Texture2D _fondo;
        private Texture2D _titulo;
        private readonly List<Button> _botones = new List<Button>();
        private int _indiceSeleccionado = 0;

        private KeyboardState _tecladoAnterior;
        private readonly int _anchoPantalla;
        private readonly int _altoPantalla;

        public MainMenu(ContentManager content, int anchoPantalla, int altoPantalla,
                        System.Action onJugar, System.Action onOpciones, System.Action onSalir)
        {
            _anchoPantalla = anchoPantalla;
            _altoPantalla = altoPantalla;

            // === IMAGENES: se cargan SIN extension, desde Content/Menu/ ===
            _fondo  = content.Load<Texture2D>("fondo del menu oscuro");
            _titulo = content.Load<Texture2D>("titulo del juego");

            Texture2D btnJugar    = content.Load<Texture2D>("boton jugar");
            Texture2D btnOpciones = content.Load<Texture2D>("boton opciones");
            Texture2D btnSalir    = content.Load<Texture2D>("boton salir");

            float centroX = anchoPantalla / 2f;
            float yInicial = altoPantalla * 0.50f;
            float separacion = 110f; // espacio vertical entre botones

            _botones.Add(new Button(btnJugar,    new Vector2(centroX, yInicial),                  onJugar));
            _botones.Add(new Button(btnOpciones, new Vector2(centroX, yInicial + separacion),     onOpciones));
            _botones.Add(new Button(btnSalir,    new Vector2(centroX, yInicial + separacion * 2), onSalir));

            _tecladoAnterior = Keyboard.GetState();
            ActualizarSeleccion();
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState teclado = Keyboard.GetState();

            // Bajar
            if (TeclaPresionada(teclado, Keys.Down) || TeclaPresionada(teclado, Keys.S))
            {
                _indiceSeleccionado = (_indiceSeleccionado + 1) % _botones.Count;
                ActualizarSeleccion();
            }

            // Subir
            if (TeclaPresionada(teclado, Keys.Up) || TeclaPresionada(teclado, Keys.W))
            {
                _indiceSeleccionado = (_indiceSeleccionado - 1 + _botones.Count) % _botones.Count;
                ActualizarSeleccion();
            }

            // Elegir
            if (TeclaPresionada(teclado, Keys.Enter) || TeclaPresionada(teclado, Keys.Space))
            {
                _botones[_indiceSeleccionado].OnClick?.Invoke();
            }

            _tecladoAnterior = teclado;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Fondo estirado a toda la pantalla
            spriteBatch.Draw(_fondo, new Rectangle(0, 0, _anchoPantalla, _altoPantalla), Color.White);

            // Titulo / logo centrado arriba
            Vector2 posTitulo = new Vector2((_anchoPantalla - _titulo.Width) / 2f, _altoPantalla * 0.08f);
            spriteBatch.Draw(_titulo, posTitulo, Color.White);

            foreach (Button b in _botones)
                b.Draw(spriteBatch);
        }

        // true solo en el frame en que se presiona la tecla (no mientras se mantiene)
        private bool TeclaPresionada(KeyboardState actual, Keys tecla)
        {
            return actual.IsKeyDown(tecla) && _tecladoAnterior.IsKeyUp(tecla);
        }

        private void ActualizarSeleccion()
        {
            for (int i = 0; i < _botones.Count; i++)
                _botones[i].IsSelected = (i == _indiceSeleccionado);
        }
    }
}
