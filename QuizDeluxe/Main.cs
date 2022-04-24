using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace QuizDeluxe
{
    public class Main : Game
    {
        public const int WIDTH = 1280;
        public const int HEIGHT = 720;
        public const int PORT = 28288;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        static public SpriteFont font;
        internal Texture2D background;

        // singleton
        static public Main game;

        // multiplayer
        internal Client client;
        public string ip;
        public string name;

        public List<string> players;

        public Main(string ip, string nick)
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = WIDTH;
            _graphics.PreferredBackBufferHeight = HEIGHT;
            Window.Title = "Quiz Deluxe";

            this.ip = ip;
            name = nick;

            Exiting += (object sender, System.EventArgs e) => client.Disconnect();
        }

        protected override void Initialize()
        {
            game = this;

            players = new List<string>();
            players.Add(name);

            client = new Client(Handler);
            client.Connect(ip, PORT);
            client.Send($"connect={name}");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("font");
            background = Content.Load<Texture2D>("background");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            string playersTable = "Players:\n";
            foreach (string nick in players)
            {
                playersTable += $"{nick}\n";
            }

            _spriteBatch.Begin();
            _spriteBatch.Draw(background, Vector2.Zero, Color.White);
            _spriteBatch.DrawString(font, playersTable, Vector2.Zero, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }


        
        //обработчик
        private void Handler(string[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].Contains("\0"))
                    break;

                string[] msg = data[i].Split('=');
                string com = msg[0];
                string param = msg[1];

                if (com == "connect")
                {
                    if(!players.Contains(param))
                        players.Add(param);
                    client.Send($"nickname={name}");
                } 
                if(com == "nickname")
                {
                    if (!players.Contains(param))
                        players.Add(param);
                }
            }
        }
    }
}
