using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ZombiePong
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D background, spritesheet;
        Texture2D titleScreen;
        private float titleScreenTimer = 0f;
        private float titleScreenDelayTime = 1f;
        float speed = 400;

        Sprite paddle1, paddle2, ball;
        Random rand;
        bool playerHit = false;
        int points = 0;

        List<Sprite> zombies = new List<Sprite>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            graphics.ApplyChanges();
          
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
             
           
                   
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            background = Content.Load<Texture2D>("background");
            spritesheet = Content.Load<Texture2D>("spritesheet");

            paddle1 = new Sprite(new Vector2(20, 20), spritesheet, new Rectangle(0, 516, 25, 150), Vector2.Zero);
            paddle2 = new Sprite(new Vector2(970, 20), spritesheet, new Rectangle(32, 516, 25, 150), Vector2.Zero);
            ball = new Sprite(new Vector2(700, 350), spritesheet, new Rectangle(76, 510, 40, 40), new Vector2(250, 60));

            SpawnZombie(new Vector2(400, 400), new Vector2(-20, 20));
           
           
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public void SpawnZombie(Vector2 location, Vector2 velocity)
        {
            Sprite zombie = new Sprite(location, spritesheet, new Rectangle(0, 25, 160, 150), velocity);

            for (int i = 1; i < 10; i++)
            {
                zombie.AddFrame(new Rectangle(i * 165, 25, 160, 150));
            }

            zombie.CollisionRadius = 60;
            zombies.Add(zombie);
        }
       
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();


            // TODO: Add your update logic here
            ball.Update(gameTime);
            MouseState ms = Mouse.GetState();
            paddle1.Location = new Vector2(paddle1.Location.X, ms.Y);
            //paddle2.Location = new Vector2(paddle2.Location.X, ball.Center.Y);

            float vdiff = paddle2.Center.Y - ball.Center.Y;
            if (paddle2.Center.Y <= ball.Center.Y)
            {
                paddle2.Velocity = new Vector2(0, 200);
            }

            else
            {
                paddle2.Velocity = new Vector2(0, -200);
            }

            if (Math.Abs(vdiff) < 50)
                paddle2.Velocity = Vector2.Zero;
            {
                if (paddle1.Location.Y <= 0) paddle1.Location = new Vector2(paddle1.Location.X, 0); //ceiling

                if (paddle1.Location.Y >= 620) paddle1.Location = new Vector2(paddle1.Location.X, 620); //bottom
            }

            paddle2.Update(gameTime);

            if (ball.Location.X >= 1024)
            {
                ball.Location = new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2); points++;
            }
            if (ball.Location.X <= 0)
                ball.Location = new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);

            /*
            if (ball.IsBoxColliding(paddle2.BoundingBoxRect) && ball.Location.Y < paddle2.Center.Y)
            {
                playerHit = false;
                Vector2 vel = ball.Velocity;
                vel.Normalize();

                // Now control the directional changes
                vel.X = -vel.X;

                speed *= 1.f;
                speed = Math.Min(speed, 200);  // Make speed the lesser of speed and 200

                ball.Velocity = vel * speed;
               
            
            }
            */



            if (ball.Location.Y <= 5) ball.Velocity = new Vector2(ball.Velocity.X, ball.Velocity.Y * -1); //ceiling

            if (ball.Location.Y >= 760) ball.Velocity = new Vector2(ball.Velocity.X, ball.Velocity.Y * -1); //bottom


            if (paddle1.IsBoxColliding(ball.BoundingBoxRect))
            {

                // Now deal with the fact that the two have collided with each other
                playerHit = true;

                /*
                Vector2 vel = ball.Velocity;
                vel.Normalize();

              

                // Now control the directional changes
                vel.X = -vel.X;
                vel.Y = -vel.Y; 
                ball.Velocity = vel * speed;
                */
                // diff goes from 0 to +/-75
                float diff = ball.Center.Y - paddle1.Center.Y;
                if (ball.Velocity.Y < 0 && diff < 0)
                {
                    ball.Velocity = new Vector2(ball.Velocity.X, (diff / 75f) * 280);
                    ball.Velocity *= new Vector2(-1, 1);
                }
                else if (ball.Velocity.Y < 0 && diff >= 0)
                {
                    ball.Velocity = new Vector2(ball.Velocity.X, -(diff / 75f) * 280);
                    ball.Velocity *= new Vector2(-1, -1);
                }


                if (ball.Velocity.Y > 0 && diff < 0)
                {
                    ball.Velocity = new Vector2(ball.Velocity.X, -(diff / 75f) * 280);
                    ball.Velocity *= new Vector2(-1, -1);
                }
                else if (ball.Velocity.Y > 0 && diff >= 0)
                {
                    ball.Velocity = new Vector2(ball.Velocity.X, (diff / 75f) * 280);
                    ball.Velocity *= new Vector2(-1, 1);
                }

                Vector2 vel = ball.Velocity;
                vel.Normalize();
                ball.Velocity = vel * speed;
                ball.Location = new Vector2(paddle1.BoundingBoxRect.Right + 1, ball.Location.Y);

            }

            Window.Title = "Player 1: " + points;




            if (paddle2.IsBoxColliding(ball.BoundingBoxRect))
            {
                // Now deal with the fact that the two have collided with each other
                playerHit = false;
                Vector2 vel = ball.Velocity;


                vel.Normalize();

                // Now control the directional changes
                vel.X = -vel.X;
                vel.Y = -vel.Y;
                ball.Velocity = vel * speed;
                ball.Location = new Vector2(paddle2.Location.X - ball.BoundingBoxRect.Width, ball.Location.Y);

            }




            for (int i = 0; i < zombies.Count; i++)
            {
                zombies[i].Update(gameTime);

                if (zombies[i].Location.Y <= 20) zombies[i].Velocity = new Vector2(zombies[i].Velocity.X, zombies[i].Velocity.Y * -1); //ceiling

                if (zombies[i].Location.Y >= 700) zombies[i].Velocity = new Vector2(zombies[i].Velocity.X, zombies[i].Velocity.Y * -1); //bottom

                // Zombie logic goes here.. 
                zombies[i].FlipHorizontal = false;

                if (zombies[i].IsCircleColliding(ball.Center, 20))
                {
                    ball.Velocity *= -1;
                    ball.Update(gameTime);

                    if (playerHit) points++;


                }
                if (zombies[i].Location.X >= 800)
                {
                    zombies[i].Location = new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
                }
                if (zombies[i].Location.X <= 0)
                    zombies[i].Location = new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
            }

            base.Update(gameTime);
        }

       

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            
            spriteBatch.Draw(background, Vector2.Zero, Color.White);

            paddle1.Draw(spriteBatch);
            paddle2.Draw(spriteBatch);
            ball.Draw(spriteBatch);

            for (int x = 0; x < zombies.Count; x++)
            {
                zombies[x].Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
