using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using QuizDeluxe.UI;
using System.Collections.Generic;
using System.Media;

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

        // кнопки для ответов
        public Button[] answers;
        // поле вопроса
        public string question;
        // никнейм хода
        public string step;

        // звук
        private SoundPlayer player;

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
            answers = new Button[4];

            player = new SoundPlayer();

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

            for (int i = 0; i < answers.Length; i++)
            {
                answers[i] = new Button(this, 528, 256 + 96 * i, buttonSprite);
                answers[i].Size = 1;
                answers[i].Text = "-";
                answers[i].TextColor = Color.White;
                answers[i].font = font;
                answers[i].Click += (Button source) => { if (step == name) client.Send("ans=" + source.Text); };
            }
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



            _spriteBatch.Begin();
            _spriteBatch.Draw(background, Vector2.Zero, Color.White);
            _spriteBatch.DrawString(font, "[Players]", Vector2.Zero, Color.White);

            for (int i = 0; i < players.Count; i++)
            {
                _spriteBatch.DrawString(font, players[i], new Vector2(0, 48 + 32 * i), players[i] == step ? Color.Green : Color.White);
            }

            _spriteBatch.DrawString(font, question, new Vector2(528, 64), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }


        // звук
        private void PlayTrue()
        {
            player.SoundLocation = "Content\\true.wav";
            player.Load();
            player.Play();
        }

        // звук
        private void PlayFalse()
        {
            player.SoundLocation = "Content\\false.wav";
            player.Load();
            player.Play();
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
                if (com == "nicknames")
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



                if (com == "q")
                {
                    string[] a = param.Split('|');
                    question = a[0];
                    for (int j = 0; j < 4; j++)
                    {
                        answers[j].Text = a[1 + j];
                    }
                }



                if (com == "step")
                {
                    step = param;
                }


                if (com == "check")
                {
                    if (param == "true")
                        PlayTrue();
                    else
                        PlayFalse();
                }
            }
        }
    }
}
