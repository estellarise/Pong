using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using static System.Random;

namespace Pong
{
    public class AutoBounce : Game
    {
        Texture2D ballTexture;
        Texture2D paddleLeftTexture;
        Texture2D paddleRightTexture;
        Texture2D scoreLeftTexture;
        Texture2D scoreRightTexture;
        Texture2D starRightTexture;
        Texture2D starLeftTexture;
        Vector2 paddleLeftPosition;
        Vector2 paddleRightPosition;
        Vector2 ballPosition;
        Vector2 ballVelocity; // direction vector with 1's, mult by ballSpeed for full vector
        float ballSpeed;
        float paddleSpeed;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        int scoreLeft;
        int scoreRight;
        Random rnd;
        string winner;

        public AutoBounce()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            rnd = new Random();
            //ball
            ballTexture = Content.Load<Texture2D>("ball_blue_small");
            ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,
_graphics.PreferredBackBufferHeight / 2);
            ballVelocity = new Vector2(0, 0);
            ballSpeed = 400f;

            //paddle
            paddleLeftTexture = Content.Load<Texture2D>("block_narrow");
            paddleLeftPosition = new Vector2(0, _graphics.PreferredBackBufferHeight/2);
            paddleRightTexture = Content.Load<Texture2D>("block_narrow");
            paddleRightPosition = new Vector2(_graphics.PreferredBackBufferWidth - paddleRightTexture.Width, _graphics.PreferredBackBufferHeight / 2);
            paddleSpeed = 1000f;

            //game
            scoreRight = 0;
            scoreLeft = 0;
            scoreLeftTexture = Content.Load<Texture2D>("number_0");
            scoreRightTexture = Content.Load<Texture2D>("number_0");
            starLeftTexture = Content.Load<Texture2D>("star_outline");
            starRightTexture = Content.Load<Texture2D>("star_outline");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }
        protected void resetRound()
        {
            ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,
                _graphics.PreferredBackBufferHeight / 2);
            ballVelocity.X = 0;
            ballVelocity.Y = 0;
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (scoreLeft == 7)
            {
                winner = "left";
                starLeftTexture = Content.Load<Texture2D>("star");
                if (Keyboard.GetState().IsKeyDown(Keys.Space) ){
                    Initialize();
                }
            }
            if (scoreRight == 7)
            {
                winner = "right";
                starRightTexture = Content.Load<Texture2D>("star");
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    Initialize();
                }
            }

            // bounce off wall by changing direction vector
            if (ballPosition.Y > _graphics.PreferredBackBufferHeight - ballTexture.Height / 2) // bottom wall
            {
                ballVelocity.Y = -1;
            }
            else if (ballPosition.Y < ballTexture.Height / 2) // top wall
            {
                ballVelocity.Y = 1;
            }

            // bounce off paddle
            bool paddleLeftHeight = (ballPosition.Y  + ballTexture.Height/2 ) > paddleLeftPosition.Y
                && (ballPosition.Y + ballTexture.Height / 2) < (paddleLeftPosition.Y + paddleLeftTexture.Height);
            bool paddleRightHeight = (ballPosition.Y + ballTexture.Height / 2) > paddleRightPosition.Y
                && (ballPosition.Y + ballTexture.Height / 2) < (paddleRightPosition.Y + paddleRightTexture.Height);
            bool hitsLeftPaddle = (ballPosition.X < paddleLeftTexture.Width + ballTexture.Width / 2) && paddleLeftHeight;
            bool hitsRightPaddle = (ballPosition.X > _graphics.PreferredBackBufferWidth - ballTexture.Width / 2 - paddleRightTexture.Width) && paddleRightHeight;
            if (hitsRightPaddle) // right paddle
            {
                ballVelocity.X = -1;
            }
            else if (hitsLeftPaddle) // left paddle
            {
                ballVelocity.X = 1;
            }

            // add score if player misses
            if (ballPosition.X > _graphics.PreferredBackBufferWidth - ballTexture.Width / 2) // right wall
            {
                scoreLeft += 1;
                scoreLeftTexture = Content.Load<Texture2D>("number_" + scoreLeft.ToString());
                resetRound();
            }
            else if (ballPosition.X < ballTexture.Width / 2) // left wall
            {
                scoreRight += 1;
                scoreRightTexture = Content.Load<Texture2D>("number_" + scoreRight.ToString());
                resetRound();
            }

            // update ball position
            ballPosition.X += ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds * ballVelocity.X;
            ballPosition.Y += ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds * ballVelocity.Y;

            // user input
            var kstate = Keyboard.GetState();

            if (ballVelocity.X == 0 && ballVelocity.Y == 0) // if ball is stopped
            {
                if(kstate.IsKeyDown(Keys.Space)) // TODO: add random start vector
                {

                    int xRnd = rnd.Next(0, 2); // 0 or 1
                    int yRnd = rnd.Next(0, 2);
                    ballVelocity.X = -1 + xRnd * 2; // -1 or 1
                    ballVelocity.Y = -1 + yRnd * 2;
                }
            }
            if (kstate.IsKeyDown(Keys.Up))
            {
                paddleRightPosition.Y -= paddleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (kstate.IsKeyDown(Keys.Down))
            {
                paddleRightPosition.Y += paddleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (kstate.IsKeyDown(Keys.W))
            {
                paddleLeftPosition.Y -= paddleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (kstate.IsKeyDown(Keys.S))
            {
                paddleLeftPosition.Y += paddleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // keep paddle in bounds
            paddleLeftPosition.Y = boundPaddle(paddleLeftPosition, paddleLeftTexture);
            paddleRightPosition.Y = boundPaddle(paddleRightPosition, paddleLeftTexture);
            base.Update(gameTime);
        }

        protected float boundPaddle(Vector2 paddleLRPosition, Texture2D paddleLeftTexture)
        {   // not best practice, should use class for each paddle
            if (paddleLRPosition.Y > _graphics.PreferredBackBufferHeight - paddleLeftTexture.Height) // paddleLeft and paddleRight use same texture
            {
                return _graphics.PreferredBackBufferHeight - paddleLeftTexture.Height;
            }
            else if (paddleLRPosition.Y < 0)
            {
                return 0;
            }
            else
            {
                return paddleLRPosition.Y;
            }
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGoldenrodYellow);

            _spriteBatch.Begin();
            _spriteBatch.Draw(
                ballTexture,
                ballPosition,
                null,
                Color.White,
                0f,
                new Vector2(ballTexture.Width / 2, ballTexture.Height / 2),
                Vector2.One,
                SpriteEffects.None,
                0f
            );
            _spriteBatch.Draw(paddleLeftTexture, paddleLeftPosition, Color.White);
            _spriteBatch.Draw(paddleRightTexture, paddleRightPosition, Color.White);
            _spriteBatch.Draw(scoreLeftTexture, new Vector2(_graphics.PreferredBackBufferWidth/4, _graphics.PreferredBackBufferHeight / 6), Color.White);
            _spriteBatch.Draw(scoreRightTexture, new Vector2(_graphics.PreferredBackBufferWidth*3/4, _graphics.PreferredBackBufferHeight / 6), Color.White);
            _spriteBatch.Draw(starLeftTexture, new Vector2(_graphics.PreferredBackBufferWidth / 4 , _graphics.PreferredBackBufferHeight / 6 - 2 * scoreRightTexture.Height), Color.White);
            _spriteBatch.Draw(starRightTexture, new Vector2(_graphics.PreferredBackBufferWidth * 3 / 4 , _graphics.PreferredBackBufferHeight / 6 - 2* scoreRightTexture.Height), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}