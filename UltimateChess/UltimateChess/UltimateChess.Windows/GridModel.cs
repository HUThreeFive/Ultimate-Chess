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
        public bool isBlackInCheck = false;
        public bool isWhiteInCheck = false;
        public List<PieceClass> whiteCaptured = new List<PieceClass>();
        public List<PieceClass> blackCaptured = new List<PieceClass>();
        private List<PieceClass> whiteActive = new List<PieceClass>();
        private List<PieceClass> blackActive = new List<PieceClass>();

        public void Start()
        {
            InitializeGrid();
            blackCaptured.Clear();
            whiteCaptured.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coord">The coordinate of the selected piece</param>
        /// <param name="player">The player making the move</param>
        /// <returns>A list of coordinates that could be possible moves</returns>
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
                if (c.row < 0 || c.row > 7 || c.col < 0 || c.col > 7)
                {
                    possibleMoves.Remove(c);
                }
                else if (grid[c.row, c.col].team == player)
                {
                    possibleMoves.Remove(c);
                }
            }

            //function to check moves against putting own king in check
            possibleMoves = WillKingBeInCheck(possibleMoves, coord, player);

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

            possibleMoves = ValidateKingMoves(possibleMoves, grid[baseCoord.row, baseCoord.col].team);

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

            if (!grid[baseCoord.row, baseCoord.col].hasMoved)
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

        public void DetermineAction(Coordinate playerPiece, Coordinate destination, Team player)
        {
            if (grid[destination.row, destination.col].team == Team.Blank)
            {
                Move(playerPiece, destination, player);
            }
            else
            {
                Attack(playerPiece, destination, player);
            }

            CheckForCheck(player);
        }

        private void Attack(Coordinate source, Coordinate destination, Team player)
        {
            if (player == Team.White)
            {
                whiteCaptured.Add(grid[destination.row, destination.col]);
                blackActive.Remove(blackActive.Find(x => x.position == destination));
            }
            else
            {
                blackCaptured.Add(grid[destination.row, destination.col]);
                whiteActive.Remove(whiteActive.Find(x => x.position == destination));
            }

            Move(source, destination, player);
        }

        private void Move(Coordinate source, Coordinate destination, Team player)
        {
            grid[destination.row, destination.col] = grid[source.row, source.col];
            if (player == Team.White)
            {
                whiteActive.Find(x => x.position == source).position = destination;
            }
            else
            {
                blackActive.Find(x => x.position == source).position = destination;
            }

            grid[destination.row, destination.col].position = destination;

            grid[source.row, source.col] = new PieceClass { pieceType = Piece.Blank, team = Team.Blank, position = source };
        }

        private void CheckForCheck(Team player)
        {
            List<Coordinate> masterList = new List<Coordinate>();

            //get teams possible moves
            if (player == Team.White)
            {
                foreach (PieceClass piece in whiteActive)
                {
                    masterList.AddRange(PossibleMoves(piece.position, player));
                }

                //lamba expression to determine if the black king's coordinate is in the master list of possible move coordinates
                if (masterList.Exists(x => x == blackActive.Find(z => z.pieceType == Piece.King).position))
                {
                    isBlackInCheck = true;
                }
                else
                {
                    isBlackInCheck = false;
                }
            }
            else
            {
                foreach (PieceClass piece in blackActive)
                {
                    masterList.AddRange(PossibleMoves(piece.position, player));
                }

                //lamba expression to determine if the whiate king's coordinate is in the master list of possible move coordinates
                if (masterList.Exists(x => x == whiteActive.Find(z => z.pieceType == Piece.King).position))
                {
                    isWhiteInCheck = true;
                }
                else
                {
                    isWhiteInCheck = false;
                }
            }
        }

        //remove the king's moves that would put it in check
        private List<Coordinate> ValidateKingMoves(List<Coordinate> coordList, Team player)
        {
            List<Coordinate> masterList = new List<Coordinate>();

            if (player == Team.White)
            {
                foreach (PieceClass piece in whiteActive)
                {
                    masterList.AddRange(PossibleMoves(piece.position, player));
                }
            }
            else
            {
                foreach (PieceClass piece in blackActive)
                {
                    masterList.AddRange(PossibleMoves(piece.position, player));
                }
            }

            foreach (Coordinate c in coordList)
            {
                if (masterList.Exists(x => x == c))
                {
                    coordList.Remove(coordList.Find(x => x == c));
                }
            }

            return coordList;
        }

        //Function used to simulate a move for the WillKingBeInCheck function
        private void SimulateMove(Coordinate source, Coordinate destination, Team player, ref PieceClass[,] copyOfGrid, List<PieceClass> whiteActiveCopy, List<PieceClass> blackActiveCopy)
        {
            if (copyOfGrid[destination.row, destination.col].team != Team.Blank)
            {
                //Remove attacked piece from respective active list
                if (player == Team.White)
                {
                    blackActiveCopy.Remove(blackActiveCopy.Find(x => x.position == destination));
                }
                else
                {
                    whiteActiveCopy.Remove(whiteActiveCopy.Find(x => x.position == destination));
                }
            }

            copyOfGrid[destination.row, destination.col] = copyOfGrid[source.row, source.col];

            if (player == Team.White)
            {
                whiteActiveCopy.Find(x => x.position == source).position = destination;
            }
            else
            {
                blackActiveCopy.Find(x => x.position == source).position = destination;
            }

            copyOfGrid[destination.row, destination.col].position = destination;
            copyOfGrid[source.row, source.col] = new PieceClass { pieceType = Piece.Blank, team = Team.Blank, position = source };
        }

        //Get pawn's simulated moves from the copy of the grid
        private List<Coordinate> GetPawnMoves(Coordinate baseCoord, PieceClass[,] copyOfGrid)
        {
            List<Coordinate> possibleMoves = new List<Coordinate>();

            possibleMoves.Add(new Coordinate { row = baseCoord.row + 1, col = baseCoord.col });

            if (!copyOfGrid[baseCoord.row, baseCoord.col].hasMoved)
            {
                possibleMoves.Add(new Coordinate { row = baseCoord.row + 2, col = baseCoord.col });
            }

            if (copyOfGrid[baseCoord.row + 1, baseCoord.col - 1].team != copyOfGrid[baseCoord.row, baseCoord.col].team &&
                copyOfGrid[baseCoord.row + 1, baseCoord.col - 1].team != Team.Blank)
            {
                possibleMoves.Add(new Coordinate { row = baseCoord.row + 1, col = baseCoord.col - 1 });
            }

            if (copyOfGrid[baseCoord.row + 1, baseCoord.col + 1].team != copyOfGrid[baseCoord.row, baseCoord.col].team &&
                copyOfGrid[baseCoord.row + 1, baseCoord.col + 1].team != Team.Blank)
            {
                possibleMoves.Add(new Coordinate { row = baseCoord.row + 1, col = baseCoord.col + 1 });
            }

            return possibleMoves;
        }

        //Simulation of possible moves
        public List<Coordinate> PossibleMoves(Coordinate coord, Team player, ref PieceClass[,] copyOfGrid)
        {
            List<Coordinate> possibleMoves = new List<Coordinate>();

            //Call appropriate piece function
            switch (copyOfGrid[coord.row, coord.col].pieceType)
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
                if (c.row < 0 || c.row > 7 || c.col < 0 || c.col > 7)
                {
                    possibleMoves.Remove(c);
                }
                else if (copyOfGrid[c.row, c.col].team == player)
                {
                    possibleMoves.Remove(c);
                }
            }

            return possibleMoves;
        }

        //Check for check with a copy of the grid
        private void CheckForCheck(Team player, ref PieceClass[,] copyOfGrid, List<PieceClass> whiteActiveCopy, List<PieceClass> blackActiveCopy)
        {
            List<Coordinate> masterList = new List<Coordinate>();

            //get teams possible moves
            if (player == Team.White)
            {
                foreach (PieceClass piece in whiteActiveCopy)
                {
                    masterList.AddRange(PossibleMoves(piece.position, player, ref copyOfGrid));
                }

                //lamba expression to determine if the black king's coordinate is in the master list of possible move coordinates
                if (masterList.Exists(x => x == blackActiveCopy.Find(z => z.pieceType == Piece.King).position))
                {
                    isBlackInCheck = true;
                }
                else
                {
                    isBlackInCheck = false;
                }
            }
            else
            {
                foreach (PieceClass piece in blackActiveCopy)
                {
                    masterList.AddRange(PossibleMoves(piece.position, player, ref copyOfGrid));
                }

                //lamba expression to determine if the whiate king's coordinate is in the master list of possible move coordinates
                if (masterList.Exists(x => x == whiteActiveCopy.Find(z => z.pieceType == Piece.King).position))
                {
                    isWhiteInCheck = true;
                }
                else
                {
                    isWhiteInCheck = false;
                }
            }
        }

        private List<Coordinate> WillKingBeInCheck(List<Coordinate> masterList, Coordinate source, Team player)
        {
            PieceClass[,] copyOfGrid = grid;
            List<PieceClass> whiteActiveCopy = whiteActive;
            List<PieceClass> blackActiveCopy = blackActive;
            bool isBlackInCheckCopy = isBlackInCheck;
            bool isWhiteInCheckCopy = isWhiteInCheck;

            //Simulate move and check for check
            foreach (Coordinate move in masterList)
            {
                SimulateMove(source, move, player, ref copyOfGrid, whiteActiveCopy, blackActiveCopy);
                CheckForCheck(player, ref copyOfGrid, whiteActiveCopy, blackActiveCopy);

                if (isBlackInCheck || isWhiteInCheck)
                {
                    masterList.Remove(masterList.Find(x => x == move));
                }

                isBlackInCheck = isBlackInCheckCopy;
                isWhiteInCheck = isWhiteInCheckCopy;
            }

            return masterList;
        }

        public bool isCheckMate(Team player)
        {
            List<Coordinate> moves = new List<Coordinate>();

            //Search the active list for pieces that have possible moves that will remove the "In Check" condition
            if (player == Team.White)
            {
                foreach (PieceClass piece in whiteActive)
                {
                    moves.AddRange(PossibleMoves(piece.position, player));
                }
            }
            else
            {
                foreach (PieceClass piece in whiteActive)
                {
                    moves.AddRange(PossibleMoves(piece.position, player));
                }
            }

            //If there are no possible moves then CHECKMATE
            if (moves.Count() > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void InitializeGrid()
        {
            if (grid != null)
            {
                Array.Clear(grid, 0, grid.Length);
            }
            else
            {
                grid = new PieceClass[NUM_CELLS, NUM_CELLS];
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

            for (int i = 0; i < 8; i++)
            {
                whiteActive.Add(grid[0, i]);
                whiteActive.Add(grid[1, i]);
                blackActive.Add(grid[6, i]);
                blackActive.Add(grid[7, i]);
            }
        }
    }
}
