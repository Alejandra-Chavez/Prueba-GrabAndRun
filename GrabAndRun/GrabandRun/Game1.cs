using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
 
namespace GrabAndRun
{
    public enum GameState
    {
        Menu,
        Jugando,
        Opciones
    }
 
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private MainMenu _menu;
        private GameWorld _mundo;
        private GameState _estado = GameState.Menu;
 
        private const int Ancho = 1280;
        private const int Alto = 720;
 
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false; // solo teclado
 
            _graphics.PreferredBackBufferWidth = Ancho;
            _graphics.PreferredBackBufferHeight = Alto;
        }
 
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
 
            _mundo = new GameWorld(GraphicsDevice, Content, Ancho, Alto);
 
            _menu = new MainMenu(
                Content, Ancho, Alto,
                onJugar: () =>
                {
                    _mundo.Reiniciar();
                    _estado = GameState.Jugando;
                },
                onOpciones: () => _estado = GameState.Opciones,
                onSalir: () => Exit()
            );
        }
 
        protected override void Update(GameTime gameTime)
        {
            switch (_estado)
            {
                case GameState.Menu:
                    _menu.Update(gameTime);
                    break;
 
                case GameState.Jugando:
                    _mundo.Update(gameTime);
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        _estado = GameState.Menu;
                    break;
 
                case GameState.Opciones:
                    // TODO: pantalla de opciones
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        _estado = GameState.Menu;
                    break;
            }
 
            base.Update(gameTime);
        }
 
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
 
            _spriteBatch.Begin(samplerState: SamplerState.LinearClamp);
 
            switch (_estado)
            {
                case GameState.Menu:
                    _menu.Draw(_spriteBatch);
                    break;
 
                case GameState.Jugando:
                    _mundo.Draw(_spriteBatch);
                    break;
 
                case GameState.Opciones:
                    // TODO: dibujar opciones
                    break;
            }
 
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}