using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateChess
{
    public class GridModel
    {
        private const int NUM_CELLS = 8;
        private PieceClass[,] grid;
        public List<PieceClass> capturedWhite = new List<PieceClass>();
        public List<PieceClass> capturedBlack = new List<PieceClass>();
        
        public void Start()
        {
            InitializeGrid();
            capturedBlack.Clear();
            capturedWhite.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public List<Coordinate> PossibleMoves(Coordinate coord, Team player)
        {
            List<Coordinate> possibleMoves = new List<Coordinate>();

            //Call appropriate piece function
            switch (grid[coord.row, coord.col].pieceType)
            {
                case Piece.King: possibleMoves = GetKingMoves(coord);
                    break;
                case Piece.Queen: possibleMoves = GetQueenMoves(coord);
                    break;
                case Piece.Bishop: possibleMoves = GetBishopMoves(coord);
                    break;
                case Piece.Knight: possibleMoves = GetKnightMoves(coord);
                    break;
                case Piece.Rook: possibleMoves = GetRookMoves(coord);
                    break;
                case Piece.Pawn: possibleMoves = GetPawnMoves(coord);
                    break;
            }

            foreach (Coordinate c in possibleMoves)
            {
                if(c.row < 0 || c.row > 7 || c.col < 0 || c.col > 7)
                {
                    possibleMoves.Remove(c);
                }
                else if (grid[c.row, c.col].team == player)
                {
                    possibleMoves.Remove(c);
                }
            }

            return possibleMoves;
        }

        private List<Coordinate> GetKingMoves(Coordinate baseCoord)
        {
            List<Coordinate> possibleMoves = new List<Coordinate>();

            possibleMoves.Add(new Coordinate { row = baseCoord.row + 1, col = baseCoord.col });       //Down Vertically
            possibleMoves.Add(new Coordinate { row = baseCoord.row - 1, col = baseCoord.col });       //Up Vertically
            possibleMoves.Add(new Coordinate { row = baseCoord.row, col = baseCoord.col - 1 });       //Left Horizontally
            possibleMoves.Add(new Coordinate { row = baseCoord.row, col = baseCoord.col + 1 });       //Down Vertically
            possibleMoves.Add(new Coordinate { row = baseCoord.row - 1, col = baseCoord.col - 1 });   //Diagonal Up-Left
            possibleMoves.Add(new Coordinate { row = baseCoord.row - 1, col = baseCoord.col + 1 });   //Diagonal Up-Right
            possibleMoves.Add(new Coordinate { row = baseCoord.row + 1, col = baseCoord.col + 1 });   //Diagonal Down-Right
            possibleMoves.Add(new Coordinate { row = baseCoord.row + 1, col = baseCoord.col - 1 });   //Diagonal Down-Left

            return possibleMoves;
        }

        private List<Coordinate> GetQueenMoves(Coordinate baseCoord)
        {
            List<Coordinate> possibleMoves = new List<Coordinate>();

            for (int i = 1; i < 8; i++)
            {
                possibleMoves.Add(new Coordinate { row = baseCoord.row + i, col = baseCoord.col });       //Down Vertically
                possibleMoves.Add(new Coordinate { row = baseCoord.row - i, col = baseCoord.col });       //Up Vertically
                possibleMoves.Add(new Coordinate { row = baseCoord.row, col = baseCoord.col - i });       //Left Horizontally
                possibleMoves.Add(new Coordinate { row = baseCoord.row, col = baseCoord.col + i });       //Down Vertically
                possibleMoves.Add(new Coordinate { row = baseCoord.row - i, col = baseCoord.col - i });   //Diagonal Up-Left
                possibleMoves.Add(new Coordinate { row = baseCoord.row - i, col = baseCoord.col + i });   //Diagonal Up-Right
                possibleMoves.Add(new Coordinate { row = baseCoord.row + i, col = baseCoord.col + i });   //Diagonal Down-Right
                possibleMoves.Add(new Coordinate { row = baseCoord.row + i, col = baseCoord.col - i });   //Diagonal Down-Left
            }

            return possibleMoves;
        }

        private List<Coordinate> GetBishopMoves(Coordinate baseCoord)
        {
            List<Coordinate> possibleMoves = new List<Coordinate>();

            for (int i = 1; i < 8; i++)
            {
                possibleMoves.Add(new Coordinate { row = baseCoord.row - i, col = baseCoord.col - i });   //Diagonal Up-Left
                possibleMoves.Add(new Coordinate { row = baseCoord.row - i, col = baseCoord.col + i });   //Diagonal Up-Right
                possibleMoves.Add(new Coordinate { row = baseCoord.row + i, col = baseCoord.col + i });   //Diagonal Down-Right
                possibleMoves.Add(new Coordinate { row = baseCoord.row + i, col = baseCoord.col - i });   //Diagonal Down-Left
            }

            return possibleMoves;
        }

        /// <summary>
        /// Calculate and return all coordinates that the knight piece can move given a starting coordinate
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        private List<Coordinate> GetKnightMoves(Coordinate baseCoord)
        {
            List<Coordinate> possibleMoves = new List<Coordinate>();
            Coordinate newCoordinate;

            //Think clockwise rotation
            newCoordinate = new Coordinate { row = baseCoord.row + 1, col = baseCoord.col - 2 };     //Left-Bottom
            possibleMoves.Add(newCoordinate);
            newCoordinate = new Coordinate { row = baseCoord.row - 1, col = baseCoord.col - 2 };     //Left-Top
            possibleMoves.Add(newCoordinate);
            newCoordinate = new Coordinate { row = baseCoord.row - 2, col = baseCoord.col - 1 };     //Top-Left
            possibleMoves.Add(newCoordinate);
            newCoordinate = new Coordinate { row = baseCoord.row - 2, col = baseCoord.col + 1 };     //Top-Right
            possibleMoves.Add(newCoordinate);
            newCoordinate = new Coordinate { row = baseCoord.row - 1, col = baseCoord.col + 2 };     //Right-Top
            possibleMoves.Add(newCoordinate);
            newCoordinate = new Coordinate { row = baseCoord.row + 1, col = baseCoord.col + 2 };     //Right-Bottom
            possibleMoves.Add(newCoordinate);
            newCoordinate = new Coordinate { row = baseCoord.row + 2, col = baseCoord.col + 1 };     //Bottom-Right
            possibleMoves.Add(newCoordinate);
            newCoordinate = new Coordinate { row = baseCoord.row + 2, col = baseCoord.col - 1 };     //Bottom-Left
            possibleMoves.Add(newCoordinate);

            return possibleMoves;
        }

        private List<Coordinate> GetRookMoves(Coordinate baseCoord)
        {
            List<Coordinate> possibleMoves = new List<Coordinate>();

            for (int i = 1; i < 8; i++)
            {
                possibleMoves.Add(new Coordinate { row = baseCoord.row + i, col = baseCoord.col });       //Down Vertically
                possibleMoves.Add(new Coordinate { row = baseCoord.row - i, col = baseCoord.col });       //Up Vertically
                possibleMoves.Add(new Coordinate { row = baseCoord.row, col = baseCoord.col - i });       //Left Horizontally
                possibleMoves.Add(new Coordinate { row = baseCoord.row, col = baseCoord.col + i });       //Down Vertically
            }

            return possibleMoves;
        }

        private List<Coordinate> GetPawnMoves(Coordinate baseCoord)
        {
            List<Coordinate> possibleMoves = new List<Coordinate>();

            possibleMoves.Add(new Coordinate { row = baseCoord.row + 1, col = baseCoord.col });

            if(!grid[baseCoord.row,baseCoord.col].hasMoved)
            {
                possibleMoves.Add(new Coordinate { row = baseCoord.row + 2, col = baseCoord.col });
            }

            if (grid[baseCoord.row + 1, baseCoord.col - 1].team != grid[baseCoord.row, baseCoord.col].team &&
                grid[baseCoord.row + 1, baseCoord.col - 1].team != Team.Blank)
            {
                possibleMoves.Add(new Coordinate { row = baseCoord.row + 1, col = baseCoord.col - 1 });
            }

            if (grid[baseCoord.row + 1, baseCoord.col + 1].team != grid[baseCoord.row, baseCoord.col].team &&
                grid[baseCoord.row + 1, baseCoord.col + 1].team != Team.Blank)
            {
                possibleMoves.Add(new Coordinate { row = baseCoord.row + 1, col = baseCoord.col + 1 });
            }

            return possibleMoves;
        }

        private void InitializeGrid()
        {
            if (grid != null)
            {
                Array.Clear(grid, 0, grid.Length);
            }
            else
            {
                grid = new PieceClass[NUM_CELLS,NUM_CELLS];
            }

            grid[0, 0] = new PieceClass { pieceType = Piece.Rook, team = Team.White, position = new Coordinate { row = 0, col = 0 } };
            grid[0, 1] = new PieceClass { pieceType = Piece.Knight, team = Team.White, position = new Coordinate { row = 0, col = 1 } };
            grid[0, 2] = new PieceClass { pieceType = Piece.Bishop, team = Team.White, position = new Coordinate { row = 0, col = 2 } };
            grid[0, 3] = new PieceClass { pieceType = Piece.Queen, team = Team.White, position = new Coordinate { row = 0, col = 3 } };
            grid[0, 4] = new PieceClass { pieceType = Piece.King, team = Team.White, position = new Coordinate { row = 0, col = 4 } };
            grid[0, 5] = new PieceClass { pieceType = Piece.Bishop, team = Team.White, position = new Coordinate { row = 0, col = 5 } };
            grid[0, 6] = new PieceClass { pieceType = Piece.Knight, team = Team.White, position = new Coordinate { row = 0, col = 6 } };
            grid[0, 7] = new PieceClass { pieceType = Piece.Rook, team = Team.White, position = new Coordinate { row = 0, col = 7 } };

            grid[7, 0] = new PieceClass { pieceType = Piece.Rook, team = Team.Black, position = new Coordinate { row = 7, col = 0 } };
            grid[7, 1] = new PieceClass { pieceType = Piece.Knight, team = Team.Black, position = new Coordinate { row = 7, col = 1 } };
            grid[7, 2] = new PieceClass { pieceType = Piece.Bishop, team = Team.Black, position = new Coordinate { row = 7, col = 2 } };
            grid[7, 3] = new PieceClass { pieceType = Piece.Queen, team = Team.Black, position = new Coordinate { row = 7, col = 3 } };
            grid[7, 4] = new PieceClass { pieceType = Piece.King, team = Team.Black, position = new Coordinate { row = 7, col = 4 } };
            grid[7, 5] = new PieceClass { pieceType = Piece.Bishop, team = Team.Black, position = new Coordinate { row = 7, col = 5 } };
            grid[7, 6] = new PieceClass { pieceType = Piece.Knight, team = Team.Black, position = new Coordinate { row = 7, col = 6 } };
            grid[7, 7] = new PieceClass { pieceType = Piece.Rook, team = Team.Black, position = new Coordinate { row = 7, col = 7 } };

            for (int i = 0; i < 8; i++)
            {
                grid[1, i] = new PieceClass { pieceType = Piece.Pawn, team = Team.White, position = new Coordinate { row = 1, col = i }, hasMoved = false };
                grid[6, i] = new PieceClass { pieceType = Piece.Pawn, team = Team.Black, position = new Coordinate { row = 6, col = i }, hasMoved = false };
            }

            for (int R = 2; R < 6; R++)
            {
                for (int C = 0; C < 8; C++)
                {
                    grid[R, C] = new PieceClass { pieceType = Piece.Blank, team = Team.Blank, position = new Coordinate { row = R, col = C } };
                }
            }
        }
    }
}
