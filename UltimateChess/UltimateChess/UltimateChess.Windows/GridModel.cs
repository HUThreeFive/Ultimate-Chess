﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateChess
{
    public class GridModel
    {
        public bool isBlackInCheck = false;
        public bool isWhiteInCheck = false;
        public Team currentPlayerTurn = Team.White;
        public List<PieceClass> whiteCaptured = new List<PieceClass>();
        public List<PieceClass> blackCaptured = new List<PieceClass>();

        private const int NUM_CELLS = 8;
        private PieceClass[,] grid;
        private List<PieceClass> whiteActive = new List<PieceClass>();
        private List<PieceClass> blackActive = new List<PieceClass>();

        public void Start()
        {
            blackCaptured.Clear();
            whiteCaptured.Clear();
            whiteActive.Clear();
            blackActive.Clear();
            InitializeGrid();
            currentPlayerTurn = Team.White;
        }

        public void StartIfSaveData()
        {
            if (grid != null)
            {
                Array.Clear(grid, 0, grid.Length);
            }
            else
            {
                grid = new PieceClass[NUM_CELLS, NUM_CELLS];
            }

            blackCaptured.Clear();
            whiteCaptured.Clear();
            whiteActive.Clear();
            blackActive.Clear();
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

            foreach (Coordinate c in possibleMoves.ToList())
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
            //possibleMoves = WillKingBeInCheck(possibleMoves, coord, player);

            return possibleMoves;
        }

        private List<Coordinate> GetKingMoves(Coordinate baseCoord)
        {
            List<Coordinate> possibleMoves = new List<Coordinate>();

            if (baseCoord.row + 1 < 8)
            {
                possibleMoves.Add(new Coordinate { row = baseCoord.row + 1, col = baseCoord.col, team = grid[baseCoord.row + 1, baseCoord.col].team });       //Down Vertically
            }

            if (baseCoord.row - 1 > -1)
            {
                possibleMoves.Add(new Coordinate { row = baseCoord.row - 1, col = baseCoord.col, team = grid[baseCoord.row - 1, baseCoord.col].team });       //Up Vertically
            }

            if (baseCoord.col - 1 > -1)
            {
                possibleMoves.Add(new Coordinate { row = baseCoord.row, col = baseCoord.col - 1, team = grid[baseCoord.row, baseCoord.col - 1].team });       //Left Horizontally
            }

            if (baseCoord.col + 1 < 8)
            {
                possibleMoves.Add(new Coordinate { row = baseCoord.row, col = baseCoord.col + 1, team = grid[baseCoord.row, baseCoord.col + 1].team });       //Down Vertically
            }

            if (baseCoord.row - 1 > 0 && baseCoord.col - 1 > -1)
            {
                possibleMoves.Add(new Coordinate { row = baseCoord.row - 1, col = baseCoord.col - 1, team = grid[baseCoord.row - 1, baseCoord.col - 1].team });   //Diagonal Up-Left
            }

            if (baseCoord.row - 1 > -1 && baseCoord.col + 1 < 8)
            {
                possibleMoves.Add(new Coordinate { row = baseCoord.row - 1, col = baseCoord.col + 1, team = grid[baseCoord.row - 1, baseCoord.col + 1].team });   //Diagonal Up-Right
            }

            if (baseCoord.row + 1 < 8 && baseCoord.col + 1 < 8)
            {
                possibleMoves.Add(new Coordinate { row = baseCoord.row + 1, col = baseCoord.col + 1, team = grid[baseCoord.row + 1, baseCoord.col + 1].team });   //Diagonal Down-Right
            }

            if (baseCoord.row + 1 < 8 && baseCoord.col - 1 > -1)
            {
                possibleMoves.Add(new Coordinate { row = baseCoord.row + 1, col = baseCoord.col - 1, team = grid[baseCoord.row + 1, baseCoord.col - 1].team });   //Diagonal Down-Left
            }

            //possibleMoves = ValidateKingMoves(possibleMoves, grid[baseCoord.row, baseCoord.col].team);

            return possibleMoves;
        }

        private List<Coordinate> GetQueenMoves(Coordinate baseCoord)
        {
            List<Coordinate> possibleMoves = new List<Coordinate>();

            #region Move Down
            for (int i = 1; i < 8; i++)
            {
                if (baseCoord.row + i < 8)
                {
                    if (grid[baseCoord.row + i, baseCoord.col].team == baseCoord.team)
                        break;
                    else if (grid[baseCoord.row + i, baseCoord.col].team == Team.Blank)
                        possibleMoves.Add(new Coordinate { row = baseCoord.row + i, col = baseCoord.col, team = grid[baseCoord.row + i, baseCoord.col].team });
                    else
                    {
                        possibleMoves.Add(new Coordinate { row = baseCoord.row + i, col = baseCoord.col, team = grid[baseCoord.row + i, baseCoord.col].team });
                        break;
                    }
                }
                else
                    break;
            }
            #endregion

            #region Move Up
            for (int i = 1; i < 8; i++)
            {
                if (baseCoord.row - i > -1)
                {
                    if (grid[baseCoord.row - i, baseCoord.col].team == baseCoord.team)
                        break;
                    else if (grid[baseCoord.row - i, baseCoord.col].team == Team.Blank)
                        possibleMoves.Add(new Coordinate { row = baseCoord.row - i, col = baseCoord.col, team = grid[baseCoord.row - i, baseCoord.col].team });
                    else
                    {
                        possibleMoves.Add(new Coordinate { row = baseCoord.row - i, col = baseCoord.col, team = grid[baseCoord.row - i, baseCoord.col].team });
                        break;
                    }
                }
                else
                    break;
            }
            #endregion

            #region Move Left
            for (int i = 1; i < 8; i++)
            {
                if (baseCoord.col - i > -1)
                {
                    if (grid[baseCoord.row, baseCoord.col - i].team == baseCoord.team)
                        break;
                    else if (grid[baseCoord.row, baseCoord.col - i].team == Team.Blank)
                        possibleMoves.Add(new Coordinate { row = baseCoord.row, col = baseCoord.col - i, team = grid[baseCoord.row, baseCoord.col - i].team });
                    else
                    {
                        possibleMoves.Add(new Coordinate { row = baseCoord.row, col = baseCoord.col - i, team = grid[baseCoord.row, baseCoord.col - i].team });
                        break;
                    }

                }
                else
                    break;
            }
            #endregion

            #region Move Right
            for (int i = 1; i < 8; i++)
            {
                if (baseCoord.col + i < 8)
                {
                    if (grid[baseCoord.row, baseCoord.col + i].team == baseCoord.team)
                        break;
                    else if (grid[baseCoord.row, baseCoord.col + i].team == Team.Blank)
                        possibleMoves.Add(new Coordinate { row = baseCoord.row, col = baseCoord.col + i, team = grid[baseCoord.row, baseCoord.col + i].team });
                    else
                    {
                        possibleMoves.Add(new Coordinate { row = baseCoord.row, col = baseCoord.col + i, team = grid[baseCoord.row, baseCoord.col + i].team });
                        break;
                    }
                }
                else
                    break;
            }
            #endregion

            #region Move Up Left
            for (int i = 1; i < 8; i++)
            {
                if (baseCoord.row - i > -1 && baseCoord.col - i > -1)
                {
                    if (grid[baseCoord.row - i, baseCoord.col - i].team == baseCoord.team)
                        break;
                    else if (grid[baseCoord.row - i, baseCoord.col - i].team == Team.Blank)
                        possibleMoves.Add(new Coordinate { row = baseCoord.row - i, col = baseCoord.col - i, team = grid[baseCoord.row - i, baseCoord.col - i].team });
                    else
                    {
                        possibleMoves.Add(new Coordinate { row = baseCoord.row - i, col = baseCoord.col - i, team = grid[baseCoord.row - i, baseCoord.col - i].team });
                        break;
                    }
                }
                else
                    break;
            }
            #endregion

            #region Move Up Right
            for (int i = 1; i < 8; i++)
            {
                if (baseCoord.row - i > -1 && baseCoord.col + i < 8)
                {
                    if (grid[baseCoord.row - i, baseCoord.col + i].team == baseCoord.team)
                        break;
                    else if (grid[baseCoord.row - i, baseCoord.col + i].team == Team.Blank)
                        possibleMoves.Add(new Coordinate { row = baseCoord.row - i, col = baseCoord.col + i, team = grid[baseCoord.row - i, baseCoord.col + i].team });
                    else
                    {
                        possibleMoves.Add(new Coordinate { row = baseCoord.row - i, col = baseCoord.col + i, team = grid[baseCoord.row - i, baseCoord.col + i].team });
                        break;
                    }
                }
                else
                    break;
            }
            #endregion

            #region Move Down Right
            for (int i = 1; i < 8; i++)
            {
                if (baseCoord.row + i < 8 && baseCoord.col + i < 8)
                {
                    if (grid[baseCoord.row + i, baseCoord.col + i].team == baseCoord.team)
                        break;
                    else if (grid[baseCoord.row + i, baseCoord.col + i].team == Team.Blank)
                        possibleMoves.Add(new Coordinate { row = baseCoord.row + i, col = baseCoord.col + i, team = grid[baseCoord.row + i, baseCoord.col + i].team });
                    else
                    {
                        possibleMoves.Add(new Coordinate { row = baseCoord.row + i, col = baseCoord.col + i, team = grid[baseCoord.row + i, baseCoord.col + i].team });
                        break;
                    }
                }
                else
                    break;
            }
            #endregion

            #region Move Down Left
            for (int i = 1; i < 8; i++)
            {
                if (baseCoord.row + i < 8 && baseCoord.col - i > -1)
                {
                    if (grid[baseCoord.row + i, baseCoord.col - i].team == baseCoord.team)
                        break;
                    else if (grid[baseCoord.row + i, baseCoord.col - i].team == Team.Blank)
                        possibleMoves.Add(new Coordinate { row = baseCoord.row + i, col = baseCoord.col - i, team = grid[baseCoord.row + i, baseCoord.col - i].team });
                    else
                    {
                        possibleMoves.Add(new Coordinate { row = baseCoord.row + i, col = baseCoord.col - i, team = grid[baseCoord.row + i, baseCoord.col - i].team });
                        break;
                    }
                }
                else
                    break;
            }
            #endregion

            return possibleMoves;
        }

        private List<Coordinate> GetBishopMoves(Coordinate baseCoord)
        {
            List<Coordinate> possibleMoves = new List<Coordinate>();

            #region Move Up Left
            for (int i = 1; i < 8; i++)
            {
                if (baseCoord.row - i > -1 && baseCoord.col - i > -1)
                {
                    if (grid[baseCoord.row - i, baseCoord.col - i].team == baseCoord.team)
                        break;
                    else if (grid[baseCoord.row - i, baseCoord.col - i].team == Team.Blank)
                        possibleMoves.Add(new Coordinate { row = baseCoord.row - i, col = baseCoord.col - i, team = grid[baseCoord.row - i, baseCoord.col - i].team });
                    else
                    {
                        possibleMoves.Add(new Coordinate { row = baseCoord.row - i, col = baseCoord.col - i, team = grid[baseCoord.row - i, baseCoord.col - i].team });
                        break;
                    }
                }
                else
                    break;
            }
            #endregion

            #region Move Up Right
            for (int i = 1; i < 8; i++)
            {
                if (baseCoord.row - i > -1 && baseCoord.col + i < 8)
                {
                    if (grid[baseCoord.row - i, baseCoord.col + i].team == baseCoord.team)
                        break;
                    else if (grid[baseCoord.row - i, baseCoord.col + i].team == Team.Blank)
                        possibleMoves.Add(new Coordinate { row = baseCoord.row - i, col = baseCoord.col + i, team = grid[baseCoord.row - i, baseCoord.col + i].team });
                    else
                    {
                        possibleMoves.Add(new Coordinate { row = baseCoord.row - i, col = baseCoord.col + i, team = grid[baseCoord.row - i, baseCoord.col + i].team });
                        break;
                    }
                }
                else
                    break;
            }
            #endregion

            #region Move Down Right
            for (int i = 1; i < 8; i++)
            {
                if (baseCoord.row + i < 8 && baseCoord.col + i < 8)
                {
                    if (grid[baseCoord.row + i, baseCoord.col + i].team == baseCoord.team)
                        break;
                    else if (grid[baseCoord.row + i, baseCoord.col + i].team == Team.Blank)
                        possibleMoves.Add(new Coordinate { row = baseCoord.row + i, col = baseCoord.col + i, team = grid[baseCoord.row + i, baseCoord.col + i].team });
                    else
                    {
                        possibleMoves.Add(new Coordinate { row = baseCoord.row + i, col = baseCoord.col + i, team = grid[baseCoord.row + i, baseCoord.col + i].team });
                        break;
                    }
                }
                else
                    break;
            }
            #endregion

            #region Move Down Left
            for (int i = 1; i < 8; i++)
            {
                if (baseCoord.row + i < 8 && baseCoord.col - i > -1)
                {
                    if (grid[baseCoord.row + i, baseCoord.col - i].team == baseCoord.team)
                        break;
                    else if (grid[baseCoord.row + i, baseCoord.col - i].team == Team.Blank)
                        possibleMoves.Add(new Coordinate { row = baseCoord.row + i, col = baseCoord.col - i, team = grid[baseCoord.row + i, baseCoord.col - i].team });
                    else
                    {
                        possibleMoves.Add(new Coordinate { row = baseCoord.row + i, col = baseCoord.col - i, team = grid[baseCoord.row + i, baseCoord.col - i].team });
                        break;
                    }
                }
                else
                    break;
            }
            #endregion

            return possibleMoves;
        }

        private List<Coordinate> GetKnightMoves(Coordinate baseCoord)
        {
            List<Coordinate> possibleMoves = new List<Coordinate>();

            //Think clockwise rotation
            if (baseCoord.row + 1 < 8 && baseCoord.col - 2 > -1)
            {
                possibleMoves.Add(new Coordinate { row = baseCoord.row + 1, col = baseCoord.col - 2, team = grid[baseCoord.row + 1, baseCoord.col - 2].team });  //Left-Bottom
            }

            if (baseCoord.row - 1 > -1 && baseCoord.col - 2 > -1)
            {
                possibleMoves.Add(new Coordinate { row = baseCoord.row - 1, col = baseCoord.col - 2, team = grid[baseCoord.row - 1, baseCoord.col - 2].team });  //Left-Top
            }

            if (baseCoord.row - 2 > -1 && baseCoord.col - 1 > -1)
            {
                possibleMoves.Add(new Coordinate { row = baseCoord.row - 2, col = baseCoord.col - 1, team = grid[baseCoord.row - 2, baseCoord.col - 1].team });  //Top-Left)
            }

            if (baseCoord.row - 2 > -1 && baseCoord.col + 1 < 8)
            {
                possibleMoves.Add(new Coordinate { row = baseCoord.row - 2, col = baseCoord.col + 1, team = grid[baseCoord.row - 2, baseCoord.col + 1].team });  //Top-Right
            }

            if (baseCoord.row - 1 > -1 && baseCoord.col + 2 < 8)
            {
                possibleMoves.Add(new Coordinate { row = baseCoord.row - 1, col = baseCoord.col + 2, team = grid[baseCoord.row - 1, baseCoord.col + 2].team });  //Right-Top
            }

            if (baseCoord.row + 1 < 8 && baseCoord.col + 2 < 8)
            {
                possibleMoves.Add(new Coordinate { row = baseCoord.row + 1, col = baseCoord.col + 2, team = grid[baseCoord.row + 1, baseCoord.col + 2].team });  //Right-Bottom
            }

            if (baseCoord.row + 2 < 8 && baseCoord.col + 1 < 8)
            {
                possibleMoves.Add(new Coordinate { row = baseCoord.row + 2, col = baseCoord.col + 1, team = grid[baseCoord.row + 2, baseCoord.col + 1].team });  //Bottom-Right
            }

            if (baseCoord.row + 2 < 8 && baseCoord.col - 1 > -1)
            {
                possibleMoves.Add(new Coordinate { row = baseCoord.row + 2, col = baseCoord.col - 1, team = grid[baseCoord.row + 2, baseCoord.col - 1].team });  //Bottom-Left
            }

            return possibleMoves;
        }

        private List<Coordinate> GetRookMoves(Coordinate baseCoord)
        {
            List<Coordinate> possibleMoves = new List<Coordinate>();

            #region Move Down
            for (int i = 1; i < 8; i++)
            {
                if (baseCoord.row + i < 8)
                {
                    if (grid[baseCoord.row + i, baseCoord.col].team == baseCoord.team)
                        break;
                    else if (grid[baseCoord.row + i, baseCoord.col].team == Team.Blank)
                        possibleMoves.Add(new Coordinate { row = baseCoord.row + i, col = baseCoord.col, team = grid[baseCoord.row + i, baseCoord.col].team });
                    else
                    {
                        possibleMoves.Add(new Coordinate { row = baseCoord.row + i, col = baseCoord.col, team = grid[baseCoord.row + i, baseCoord.col].team });
                        break;
                    }
                }
                else
                    break;
            }
            #endregion

            #region Move Up
            for (int i = 1; i < 8; i++)
            {
                if (baseCoord.row - i > -1)
                {
                    if (grid[baseCoord.row - i, baseCoord.col].team == baseCoord.team)
                        break;
                    else if (grid[baseCoord.row - i, baseCoord.col].team == Team.Blank)
                        possibleMoves.Add(new Coordinate { row = baseCoord.row - i, col = baseCoord.col, team = grid[baseCoord.row - i, baseCoord.col].team });
                    else
                    {
                        possibleMoves.Add(new Coordinate { row = baseCoord.row - i, col = baseCoord.col, team = grid[baseCoord.row - i, baseCoord.col].team });
                        break;
                    }
                }
                else
                    break;
            }
            #endregion

            #region Move Left
            for (int i = 1; i < 8; i++)
            {
                if (baseCoord.col - i > -1)
                {
                    if (grid[baseCoord.row, baseCoord.col - i].team == baseCoord.team)
                        break;
                    else if (grid[baseCoord.row, baseCoord.col - i].team == Team.Blank)
                        possibleMoves.Add(new Coordinate { row = baseCoord.row, col = baseCoord.col - i, team = grid[baseCoord.row, baseCoord.col - i].team });
                    else
                    {
                        possibleMoves.Add(new Coordinate { row = baseCoord.row, col = baseCoord.col - i, team = grid[baseCoord.row, baseCoord.col - i].team });
                        break;
                    }

                }
                else
                    break;
            }
            #endregion

            #region Move Right
            for (int i = 1; i < 8; i++)
            {
                if (baseCoord.col + i < 8)
                {
                    if (grid[baseCoord.row, baseCoord.col + i].team == baseCoord.team)
                        break;
                    else if (grid[baseCoord.row, baseCoord.col + i].team == Team.Blank)
                        possibleMoves.Add(new Coordinate { row = baseCoord.row, col = baseCoord.col + i, team = grid[baseCoord.row, baseCoord.col + i].team });
                    else
                    {
                        possibleMoves.Add(new Coordinate { row = baseCoord.row, col = baseCoord.col + i, team = grid[baseCoord.row, baseCoord.col + i].team });
                        break;
                    }
                }
                else
                    break;
            }
            #endregion

            return possibleMoves;
        }

        private List<Coordinate> GetPawnMoves(Coordinate baseCoord)
        {
            List<Coordinate> possibleMoves = new List<Coordinate>();

            if (baseCoord.team == Team.Black)
            {
                #region Black Move
                //Forwards
                if (baseCoord.row + 1 < 8 && grid[baseCoord.row + 1, baseCoord.col].team == Team.Blank)
                {
                    possibleMoves.Add(new Coordinate { row = baseCoord.row + 1, col = baseCoord.col, team = grid[baseCoord.row + 1, baseCoord.col].team });

                    if (!grid[baseCoord.row, baseCoord.col].hasMoved)
                    {
                        //Forwards x2
                        if (baseCoord.row + 2 < 8 || grid[baseCoord.row + 2, baseCoord.col].team == Team.Blank)
                        {
                            possibleMoves.Add(new Coordinate { row = baseCoord.row + 2, col = baseCoord.col, team = grid[baseCoord.row + 2, baseCoord.col].team });
                        }
                    }
                }

                //Diagonally Down Left
                if ((baseCoord.col != 0 && baseCoord.row != 7) && grid[baseCoord.row + 1, baseCoord.col - 1].team != grid[baseCoord.row, baseCoord.col].team &&
                    grid[baseCoord.row + 1, baseCoord.col - 1].team != Team.Blank)
                {
                    possibleMoves.Add(new Coordinate { row = baseCoord.row + 1, col = baseCoord.col - 1, team = grid[baseCoord.row + 1, baseCoord.col - 1].team });
                }

                //Diagonally Down Right
                if ((baseCoord.col != 7 && baseCoord.row != 7) && grid[baseCoord.row + 1, baseCoord.col + 1].team != grid[baseCoord.row, baseCoord.col].team &&
                    grid[baseCoord.row + 1, baseCoord.col + 1].team != Team.Blank)
                {
                    possibleMoves.Add(new Coordinate { row = baseCoord.row + 1, col = baseCoord.col + 1, team = grid[baseCoord.row + 1, baseCoord.col + 1].team });
                }
                #endregion
            }
            else
            {
                #region White Move
                //Forwards
                if (baseCoord.row - 1 > -1 && grid[baseCoord.row - 1, baseCoord.col].team == Team.Blank)
                {
                    possibleMoves.Add(new Coordinate { row = baseCoord.row - 1, col = baseCoord.col, team = grid[baseCoord.row - 1, baseCoord.col].team });

                    if (!grid[baseCoord.row, baseCoord.col].hasMoved)
                    {
                        //Forwards x2
                        if (baseCoord.row - 2 > -1 && grid[baseCoord.row - 2, baseCoord.col].team == Team.Blank)
                        {
                            possibleMoves.Add(new Coordinate { row = baseCoord.row - 2, col = baseCoord.col, team = grid[baseCoord.row - 2, baseCoord.col].team });
                        }
                    }
                }

                //Diagonally Left
                if ((baseCoord.col != 0 && baseCoord.row != 0) && grid[baseCoord.row - 1, baseCoord.col - 1].team != grid[baseCoord.row, baseCoord.col].team &&
                    grid[baseCoord.row - 1, baseCoord.col - 1].team != Team.Blank)
                {
                    possibleMoves.Add(new Coordinate { row = baseCoord.row - 1, col = baseCoord.col - 1, team = grid[baseCoord.row - 1, baseCoord.col - 1].team });
                }

                //Diagonally Right
                if ((baseCoord.col != 7 && baseCoord.row != 0) && grid[baseCoord.row - 1, baseCoord.col + 1].team != grid[baseCoord.row, baseCoord.col].team &&
                    grid[baseCoord.row - 1, baseCoord.col + 1].team != Team.Blank)
                {
                    possibleMoves.Add(new Coordinate { row = baseCoord.row - 1, col = baseCoord.col + 1, team = grid[baseCoord.row - 1, baseCoord.col + 1].team });
                }
                #endregion
            }

            return possibleMoves;
        }

        public void DetermineAction(Coordinate playerPiece, Coordinate destination, Team player)
        {
            if (grid[destination.row, destination.col].team == Team.Blank)
            {
                Move(playerPiece, destination, player);
                if (!grid[destination.row, destination.col].hasMoved)
                {
                    grid[destination.row, destination.col].hasMoved = true;
                }
            }
            else
            {
                Attack(playerPiece, destination, player);
            }

            if (currentPlayerTurn == Team.White)
            {
                currentPlayerTurn = Team.Black;
            }
            else
            {
                currentPlayerTurn = Team.White;
            }

            //CheckForCheck(player);
        }

        private void Attack(Coordinate source, Coordinate destination, Team player)
        {
            if (player == Team.White)
            {
                whiteCaptured.Add(grid[destination.row, destination.col]);
                blackActive.Remove(blackActive.Find(x => (x.position.col == destination.col && x.position.row == destination.row)));
            }
            else
            {
                blackCaptured.Add(grid[destination.row, destination.col]);
                whiteActive.Remove(whiteActive.Find(x => (x.position.col == destination.col && x.position.row == destination.row)));
            }

            Move(source, destination, player);
        }

        private void Move(Coordinate source, Coordinate destination, Team player)
        {
            grid[destination.row, destination.col] = grid[source.row, source.col];
            if (player == Team.White)
            {
                PieceClass piece = whiteActive.Find(x => (x.position.col == source.col && x.position.row == source.row));
                int index = whiteActive.IndexOf(piece);
                whiteActive.RemoveAt(index);
                piece.position = destination;
                whiteActive.Add(piece);
                piece = null;
            }
            else
            {
                PieceClass piece = blackActive.Find(x => (x.position.col == source.col && x.position.row == source.row));
                int index = blackActive.IndexOf(piece);
                blackActive.RemoveAt(index);
                piece.position = destination;
                blackActive.Add(piece);
                piece = null;
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
        private List<Coordinate> ValidateKingMoves(List<Coordinate> coordList, Team player, ref PieceClass[,] copyOfGrid)
        {
            List<Coordinate> masterList = new List<Coordinate>();

            if (player == Team.White)
            {
                foreach (PieceClass piece in whiteActive)
                {
                    masterList.AddRange(PossibleMoves(piece.position, player, ref copyOfGrid));
                }
            }
            else
            {
                foreach (PieceClass piece in blackActive)
                {
                    masterList.AddRange(PossibleMoves(piece.position, player, ref copyOfGrid));
                }
            }

            foreach (Coordinate c in coordList)
            {
                if (masterList.Exists(x => x == c))
                {
                    coordList.Remove(coordList.Find(x => (x.col == c.col && x.row == c.row)));
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
                    blackActiveCopy.Remove(blackActiveCopy.Find(x => (x.position.col == destination.col && x.position.row == destination.row)));
                }
                else
                {
                    whiteActiveCopy.Remove(whiteActiveCopy.Find(x => (x.position.col == destination.col && x.position.row == destination.row)));
                }
            }

            copyOfGrid[destination.row, destination.col] = copyOfGrid[source.row, source.col];
            copyOfGrid[destination.row, destination.col].position = new Coordinate { row = destination.row, col = destination.col, team = player };
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

            foreach (Coordinate c in possibleMoves.ToList())
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
            PieceClass[,] copyOfGrid = grid.Clone() as PieceClass[,];
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
                    masterList.Remove(masterList.Find(x => (x.col == move.col && x.row == move.row)));
                }

                isBlackInCheck = isBlackInCheckCopy;
                isWhiteInCheck = isWhiteInCheckCopy;
            }

            return masterList;
        }

        public bool IsCheckMate(Team player)
        {
            if (!(isWhiteInCheck || isBlackInCheck))
            {
                return false;
            }

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
                foreach (PieceClass piece in blackActive)
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

        //If isCheckMate returns false, then check for a stalemate condition (neither side can make a legal move)
        public bool IsStalemate(Team player)
        {
            List<Coordinate> whiteMoves = new List<Coordinate>();
            List<Coordinate> blackMoves = new List<Coordinate>();

            //Search the active lists for pieces that have possible moves that will remove the "In Check" condition
            foreach (PieceClass piece in whiteActive)
            {
                whiteMoves.AddRange(PossibleMoves(piece.position, Team.White));
            }

            foreach (PieceClass piece in blackActive)
            {
                blackMoves.AddRange(PossibleMoves(piece.position, Team.Black));
            }

            //If the next turn is white's or blacks' and there are no moves possible for that player, then stalemate (Draw) occurs
            if ((player == Team.White && whiteMoves.Count() <= 0) || (player == Team.Black && blackMoves.Count() <= 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public void UpdateActiveListFromSaveState(PieceClass piece)
        {
            if (piece.team == Team.White)
            {
                whiteActive.Add(piece);
            }
            else
            {
                blackActive.Add(piece);
            }
            piece = null;
        }
        //If a piece was saved in the saved data in controller, add the piece at its coordinate in the grid
        public void AddSavedPiece(PieceClass savedPiece)
        {
            PieceClass newSavedPiece = new PieceClass() { hasMoved = savedPiece.hasMoved, pieceType = savedPiece.pieceType,
                    position = savedPiece.position, team = savedPiece.team };
            savedPiece = null;
            grid[newSavedPiece.position.row, newSavedPiece.position.col] = newSavedPiece;

            if (newSavedPiece.team == Team.White)
            {
                whiteActive.Add(newSavedPiece);
            }
            else
            {
                blackActive.Add(newSavedPiece);
            }
            newSavedPiece = null;
        }

        public void SetEmptyGridCellsBlank()
        {
            for (int rowCount = 0; rowCount < 8; rowCount++)
            {
                for (int colCount = 0; colCount < 8; colCount++)
                {
                    if (grid[rowCount,colCount] == null)
                    {
                        grid[rowCount, colCount] = new PieceClass { pieceType = Piece.Blank, team = Team.Blank, position = new Coordinate { row = rowCount, col = colCount, team = Team.Blank } };
                    }
                }
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

            grid[0, 0] = new PieceClass { pieceType = Piece.Rook, team = Team.Black, position = new Coordinate { row = 0, col = 0, team = Team.Black } };
            grid[0, 1] = new PieceClass { pieceType = Piece.Knight, team = Team.Black, position = new Coordinate { row = 0, col = 1, team = Team.Black } };
            grid[0, 2] = new PieceClass { pieceType = Piece.Bishop, team = Team.Black, position = new Coordinate { row = 0, col = 2, team = Team.Black } };
            grid[0, 3] = new PieceClass { pieceType = Piece.Queen, team = Team.Black, position = new Coordinate { row = 0, col = 3, team = Team.Black } };
            grid[0, 4] = new PieceClass { pieceType = Piece.King, team = Team.Black, position = new Coordinate { row = 0, col = 4, team = Team.Black } };
            grid[0, 5] = new PieceClass { pieceType = Piece.Bishop, team = Team.Black, position = new Coordinate { row = 0, col = 5, team = Team.Black } };
            grid[0, 6] = new PieceClass { pieceType = Piece.Knight, team = Team.Black, position = new Coordinate { row = 0, col = 6, team = Team.Black } };
            grid[0, 7] = new PieceClass { pieceType = Piece.Rook, team = Team.Black, position = new Coordinate { row = 0, col = 7, team = Team.Black } };

            grid[7, 0] = new PieceClass { pieceType = Piece.Rook, team = Team.White, position = new Coordinate { row = 7, col = 0, team = Team.White } };
            grid[7, 1] = new PieceClass { pieceType = Piece.Knight, team = Team.White, position = new Coordinate { row = 7, col = 1, team = Team.White } };
            grid[7, 2] = new PieceClass { pieceType = Piece.Bishop, team = Team.White, position = new Coordinate { row = 7, col = 2, team = Team.White } };
            grid[7, 3] = new PieceClass { pieceType = Piece.Queen, team = Team.White, position = new Coordinate { row = 7, col = 3, team = Team.White } };
            grid[7, 4] = new PieceClass { pieceType = Piece.King, team = Team.White, position = new Coordinate { row = 7, col = 4, team = Team.White } };
            grid[7, 5] = new PieceClass { pieceType = Piece.Bishop, team = Team.White, position = new Coordinate { row = 7, col = 5, team = Team.White } };
            grid[7, 6] = new PieceClass { pieceType = Piece.Knight, team = Team.White, position = new Coordinate { row = 7, col = 6, team = Team.White } };
            grid[7, 7] = new PieceClass { pieceType = Piece.Rook, team = Team.White, position = new Coordinate { row = 7, col = 7, team = Team.White } };

            for (int i = 0; i < 8; i++)
            {
                grid[1, i] = new PieceClass { pieceType = Piece.Pawn, team = Team.Black, position = new Coordinate { row = 1, col = i, team = Team.Black }, hasMoved = false };
                grid[6, i] = new PieceClass { pieceType = Piece.Pawn, team = Team.White, position = new Coordinate { row = 6, col = i, team = Team.White }, hasMoved = false };
            }

            for (int R = 2; R < 6; R++)
            {
                for (int C = 0; C < 8; C++)
                {
                    grid[R, C] = new PieceClass { pieceType = Piece.Blank, team = Team.Blank, position = new Coordinate { row = R, col = C, team = Team.Blank } };
                }
            }

            for (int i = 0; i < 8; i++)
            {
                whiteActive.Add(grid[6, i]);
                whiteActive.Add(grid[7, i]);
                blackActive.Add(grid[0, i]);
                blackActive.Add(grid[1, i]);
            }
        }
    }
}
