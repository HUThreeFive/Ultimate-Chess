using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateChess
{
    public class PieceClass
    {
        public Piece pieceType;
        public Team team;
        public Coordinate position;
        public bool hasMoved;
    }
    
    public enum Piece
	{
	    King,
        Queen,
        Bishop,
        Knight,
        Rook,
        Pawn,
        Blank
	}

    public enum Team
    {
        White,
        Black,
        Blank
    }

    public class Coordinate
    {
        public int row;
        public int col;
        //Used only for the list PossibleMoves returns; used for highlighting
        public Team team;
    }
}
