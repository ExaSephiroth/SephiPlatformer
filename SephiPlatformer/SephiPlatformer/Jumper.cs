using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SephiPlatformer
{
    public class Jumper : Sprite
    {
        public Vector2 Movement { get; set; }
        public Vector2 oldPosition;
        


        public bool doubleJumpAvailable, upreleased;

        public Jumper(Texture2D texture, Vector2 position, SpriteBatch spritebatch)
            : base(texture, position, spritebatch)
        {
        }

        public void Update(GameTime gameTime)
        {
            CheckKeyboardAndUpdateMovement();
            SimulateFriction();

            AffectWithGravity();

            MoveIfPossible(gameTime);

            StopMovingIfBlocked();
        }

        private void MoveIfPossible(GameTime gameTime)
        {
            oldPosition = Position;
            UpdatePositionBasedOnMovement(gameTime);
            Position = Board.CurrentBoard.WhereCanIGetTo(oldPosition, Position, Bounds);
        }

        private void CheckKeyboardAndUpdateMovement()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Left)) { Movement += new Vector2(-1, 0); }
            if (keyboardState.IsKeyDown(Keys.Right)) { Movement += new Vector2(1, 0); }
           // if (keyboardState.IsKeyDown(Keys.Up)) { Movement += new Vector2(0, -1); }
            if (keyboardState.IsKeyDown(Keys.Space) && !IsOnFirmGround() && doubleJumpAvailable && upreleased) //doubleJumpAvailable) //&& upreleased)
            {
                Movement = -Vector2.UnitY * 25;
                
                doubleJumpAvailable = false;
                
               
                    
                
            }
            else
            {
                if (keyboardState.IsKeyDown(Keys.Space) && IsOnFirmGround())
                {
                    Movement = -Vector2.UnitY * 25;
                    doubleJumpAvailable = true;
                    upreleased = false;
                    

                }
            }
            
            if (keyboardState.IsKeyUp(Keys.Space))
            {
                upreleased = true;
                angle = 0f;
            }
        }

        private void SimulateFriction()
        {
            Movement -= Movement * new Vector2(.1f, .1f);
        }

        private void UpdatePositionBasedOnMovement(GameTime gameTime)
        {
            Position += Movement * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 30;
        }

        private void AffectWithGravity()
        {
            Movement += Vector2.UnitY * .8f;
        }

        public bool IsOnFirmGround()
        {
            Rectangle onePixelLower = Bounds;
            onePixelLower.Offset(0, 1);
            return !Board.CurrentBoard.HasRoomForRectangle(onePixelLower);
        }

        private void StopMovingIfBlocked()
        {
            Vector2 lastMovement = Position - oldPosition;
            if (lastMovement.X == 0) { Movement *= Vector2.UnitY; }
            if (lastMovement.Y == 0) { Movement *= Vector2.UnitX; }
        }
    }
}
