using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SephiPlatformer
{
    class Board
    {
        private Tile[,] tiles;
        int rows { set; get; }
        int columns { set; get; }
        Texture2D tileTexture { set; get; }
        SpriteBatch spriteBatch { set; get; }

        private Random random = new Random();

        public static Board CurrentBoard { get; private set; }

        public Board(Texture2D t, SpriteBatch sb, int r, int c)
        {
            tileTexture = t;
            spriteBatch = sb;
            rows = r;
            columns = c;
            

            CreateNewBoard();
            CurrentBoard = this;
            
        }



        public void CreateNewBoard()
        {
            tiles = new Tile[columns, rows];
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    Vector2 tilePosition =
                        new Vector2(x * tileTexture.Width, y * tileTexture.Height);

                    tiles[x, y] =
                        new Tile(tileTexture, tilePosition, spriteBatch, random.Next(5) == 0);
                }
            }

            blockBorderTiles();
            tiles[1, 1].IsBlocked = false;
        }

        private void blockBorderTiles()
        {
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    if (x == 0 || x == columns - 1 || y == 0 || y == rows - 1)
                    { tiles[x, y].IsBlocked = true; }
                }
            }
        }



        public void Draw()
        {
            foreach (var tile in tiles)
            {
                tile.Draw();
            }
        }

        public bool HasRoomForRectangle(Rectangle rectangleToCheck)
        {
            foreach (var tile in tiles)
            {
                if (tile.IsBlocked && tile.Bounds.Intersects(rectangleToCheck))
                {
                    return false;
                }
            }
            return true;

        }

        public Vector2 WhereCanIGetTo(Vector2 originalPosition, Vector2 destination, Rectangle boundingRectangle)
        {

            Vector2 movementToTry = destination - originalPosition;
            Vector2 furthestAvailableLocationSoFar = originalPosition;
            int numberOfStepsToBreakMovementInto = (int)(movementToTry.Length() * 2) + 1;
            Vector2 oneStep = movementToTry / numberOfStepsToBreakMovementInto;

            for (int i = 1; i <= numberOfStepsToBreakMovementInto; i++)
            {
                Vector2 positionToTry = originalPosition + oneStep * i;
                Rectangle newBoundary = CreateRectangleAtPosition(positionToTry, boundingRectangle.Width, boundingRectangle.Height);
                if (HasRoomForRectangle(newBoundary)) { furthestAvailableLocationSoFar = positionToTry; }
                else
                {
                    bool isDiagonalMove = movementToTry.X != 0 && movementToTry.Y != 0;
                    if (isDiagonalMove)
                    {
                        int stepsLeft = numberOfStepsToBreakMovementInto - (i - 1);

                        Vector2 remainingHorizontalMovement = oneStep.X * Vector2.UnitX * stepsLeft;
                        Vector2 finalPositionIfMovingHorizontally = furthestAvailableLocationSoFar + remainingHorizontalMovement;
                        furthestAvailableLocationSoFar =
                            WhereCanIGetTo(furthestAvailableLocationSoFar, finalPositionIfMovingHorizontally, boundingRectangle);

                        Vector2 remainingVerticalMovement = oneStep.Y * Vector2.UnitY * stepsLeft;
                        Vector2 finalPositionIfMovingVertically = furthestAvailableLocationSoFar + remainingVerticalMovement;
                        furthestAvailableLocationSoFar =
                            WhereCanIGetTo(furthestAvailableLocationSoFar, finalPositionIfMovingVertically, boundingRectangle);
                    }
                    break;
                }
            }
            return furthestAvailableLocationSoFar;
        }

        private Rectangle CreateRectangleAtPosition(Vector2 positionToTry, int width, int height)
        {
            return new Rectangle((int)positionToTry.X, (int)positionToTry.Y, width, height);
        }






    }

}
