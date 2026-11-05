using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GrabandRun;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D _background;
    private Texture2D _sand;
    private Texture2D _sandSurface;

    private const int TileSize = 64;
    private const float BackgroundScale = 4.0f;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);

        _graphics.PreferredBackBufferWidth = 1024;
        _graphics.PreferredBackBufferHeight = 576;

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _background = Content.Load<Texture2D>("imagenes/fondo desierto");
        _sand = Content.Load<Texture2D>("imagenes/arena");
        _sandSurface = Content.Load<Texture2D>("imagenes/arena_superficie");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _spriteBatch.Draw(
            _background,
            Vector2.Zero,
            null,
            Color.White,
            0.0f,
            Vector2.Zero,
            BackgroundScale,
            SpriteEffects.None,
            0.0f
        );

        int windowWidth = Window.ClientBounds.Width;
        int windowHeight = Window.ClientBounds.Height;

        int tilesAcross = windowWidth / TileSize;
        int surfaceY = windowHeight - (TileSize * 2);

        for (int i = 0; i < tilesAcross; i++)
        {
            float x = i * TileSize;

            _spriteBatch.Draw(_sandSurface, new Vector2(x, surfaceY), Color.White);
            _spriteBatch.Draw(_sand, new Vector2(x, surfaceY + TileSize), Color.White);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}