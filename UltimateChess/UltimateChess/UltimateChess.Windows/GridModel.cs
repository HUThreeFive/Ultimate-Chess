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
        
        public List<Coordinate> PossibleMoves(Coordinate coord, Team player)
        {
            List<Coordinate> possibleMoves = new List<Coordinate>();

            return possibleMoves;
        }

        private void InitializeGrid()
        {
            Array.Clear(grid, 0, grid.Length);

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
                grid[1, i] = new PieceClass { pieceType = Piece.Pawn, team = Team.White, position = new Coordinate { row = 1, col = i } };
                grid[6, i] = new PieceClass { pieceType = Piece.Pawn, team = Team.Black, position = new Coordinate { row = 6, col = i } };
            }

            for (int R = 2; R < 6; R++)
            {
                for (int C = 0; C < 8; C++)
                {
                    grid[R, C] = new PieceClass { pieceType = Piece.Blank, position = new Coordinate { row = R, col = C } };
                }
            }
        }
    }
}
