using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Pong
{
    public class AutoBounce : Game
    {   // not best practice, should use class for each object: speed, texture, position
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

        public AutoBounce()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {   
            rnd = new Random(); // instantiate random object only once to use randomness correctly

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
        protected void ResetRound()
        {
            ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,
                _graphics.PreferredBackBufferHeight / 2); // reset ball to middle
            ballVelocity.X = 0;
            ballVelocity.Y = 0;
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (scoreLeft == 7) // left wins
            {
                starLeftTexture = Content.Load<Texture2D>("star");
                if (Keyboard.GetState().IsKeyDown(Keys.Space) ){
                    Initialize(); // restart entire game
                }
            }
            if (scoreRight == 7) // right wins
            {
                starRightTexture = Content.Load<Texture2D>("star");
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    Initialize();
                }
            }

            // bounce off wall by changing direction vector
            if (ballPosition.Y > _graphics.PreferredBackBufferHeight - ballTexture.Height / 2) // bottom wall
            {
                ballVelocity.Y = -1; // perpendicular component changed, parallel preserved (X not modified)
            }
            else if (ballPosition.Y < ballTexture.Height / 2) // top wall
            {
                ballVelocity.Y = 1;
            }

            /* bounce off paddle */
            // check ball hits paddle in Y coord
            bool paddleLeftHeight = (ballPosition.Y  + ballTexture.Height/2 ) > paddleLeftPosition.Y
                && (ballPosition.Y + ballTexture.Height / 2) < (paddleLeftPosition.Y + paddleLeftTexture.Height);
            bool paddleRightHeight = (ballPosition.Y + ballTexture.Height / 2) > paddleRightPosition.Y
                && (ballPosition.Y + ballTexture.Height / 2) < (paddleRightPosition.Y + paddleRightTexture.Height);
            // AND it with paddle's X coord; i.e. ball hits paddle vertically and horizontally
            bool hitsLeftPaddle = (ballPosition.X < paddleLeftTexture.Width + ballTexture.Width / 2) && paddleLeftHeight;
            bool hitsRightPaddle = (ballPosition.X > _graphics.PreferredBackBufferWidth - ballTexture.Width / 2 - paddleRightTexture.Width) && paddleRightHeight;

            if (hitsRightPaddle) // right paddle
            {
                ballVelocity.X = -1; // vertical preserved, horizontal changes direction
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
                ResetRound();
            }
            else if (ballPosition.X < ballTexture.Width / 2) // left wall
            {
                scoreRight += 1;
                scoreRightTexture = Content.Load<Texture2D>("number_" + scoreRight.ToString());
                ResetRound();
            }

            // update ball position
            ballPosition.X += ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds * ballVelocity.X;
            ballPosition.Y += ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds * ballVelocity.Y;

            // user input
            var kstate = Keyboard.GetState();

            if (ballVelocity.X == 0 && ballVelocity.Y == 0) // if ball is stopped
            {
                if(kstate.IsKeyDown(Keys.Space)) // press space to start it
                {
                    // choose one of four random directions: 45, 135, 225, 315 degrees
                    int xRnd = rnd.Next(0, 2); // 0 or 1
                    int yRnd = rnd.Next(0, 2);
                    ballVelocity.X = -1 + xRnd * 2; // -1 or 1
                    ballVelocity.Y = -1 + yRnd * 2;
                }
            }
            // Right paddle controller: Up, Down
            if (kstate.IsKeyDown(Keys.Up))
            {
                paddleRightPosition.Y -= paddleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (kstate.IsKeyDown(Keys.Down))
            {
                paddleRightPosition.Y += paddleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // Left paddle controller: W, S
            if (kstate.IsKeyDown(Keys.W))
            {
                paddleLeftPosition.Y -= paddleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (kstate.IsKeyDown(Keys.S))
            {
                paddleLeftPosition.Y += paddleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // keep paddle in bounds
            paddleLeftPosition.Y = boundPaddle(paddleLeftPosition);
            paddleRightPosition.Y = boundPaddle(paddleRightPosition);
            base.Update(gameTime);
        }

        // returns Y coord of where paddle should be
        // 0 if it exceeds top, bottom - paddle height if exceeds bottom
        // otherwise paddle in bounds, return current Y coord
        protected float boundPaddle(Vector2 paddleLRPosition)
        {
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
            _spriteBatch.Draw( // draw ball centered, refer to https://docs.monogame.net/articles/getting_started/5_adding_basic_code.html
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
            // draw scores and stars at 1/4 and 3/4 the way from left of screen
            _spriteBatch.Draw(scoreLeftTexture, new Vector2(_graphics.PreferredBackBufferWidth/4, _graphics.PreferredBackBufferHeight / 6), Color.White);
            _spriteBatch.Draw(scoreRightTexture, new Vector2(_graphics.PreferredBackBufferWidth*3/4, _graphics.PreferredBackBufferHeight / 6), Color.White);
            _spriteBatch.Draw(starLeftTexture, new Vector2(_graphics.PreferredBackBufferWidth / 4 , _graphics.PreferredBackBufferHeight / 6 - 2 * scoreRightTexture.Height), Color.White);
            _spriteBatch.Draw(starRightTexture, new Vector2(_graphics.PreferredBackBufferWidth * 3 / 4 , _graphics.PreferredBackBufferHeight / 6 - 2* scoreRightTexture.Height), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}