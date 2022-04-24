using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using QuizDeluxe.UI;
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
        internal Texture2D buttonSprite;

        // singleton
        static public Main game;

        // multiplayer
        internal Client client;
        public string ip;
        public string name;

        public List<string> players;

        // таблица вопросов
        public Table table;
        // кнопки для ответов
        public Button answer1;
        public Button answer2;
        public Button answer3;
        public Button answer4;
        // поле вопроса
        public string question;

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

            question = "";

            table = new Table();
            table.Load("table\\standart.txt");

            client = new Client(Handler);
            client.Connect(ip, PORT);
            client.Send($"connect={name}");
            client.Send("request_nick=x");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("font");
            background = Content.Load<Texture2D>("background");
            buttonSprite = Content.Load<Texture2D>("button");

            answer1 = new Button(this, 528, 256, buttonSprite);
            answer2 = new Button(this, 528, 320, buttonSprite);
            answer3 = new Button(this, 528, 384, buttonSprite);
            answer4 = new Button(this, 528, 448, buttonSprite);
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
            _spriteBatch.DrawString(font, question, new Vector2(528, 64), Color.White);
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
                    client.Send("request_nick=x");
                if(com == "nicknames")
                {
                    string[] nicks = param.Split('|');
                    foreach (string nick in nicks)
                        if (!players.Contains(nick))
                            players.Add(nick);
                }
                if (com == "disconnect")
                    players.Remove(param);

                if (players.Contains(""))
                    players.Remove("");
            }
        }
    }
}
