using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UltimateChess
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public bool firstClick = true;
        public Coordinate firstCoordinate;
        private int squareSize;
        public GridModel grid;

        public MainPage()
        {
            this.InitializeComponent();
            grid = new GridModel();
            grid.Start();       
            LayoutGridSetUp();
        }

        private void LayoutGridSetUp()
        {
            layoutGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Window.Current.Bounds.Width / 5) });
            layoutGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            layoutGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Window.Current.Bounds.Width / 5) });
            layoutGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100) });
            layoutGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            layoutGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100) });
        }

        private void CanvasSetUp()
        {
            if (canvasBoard.ActualWidth < canvasBoard.ActualHeight)
            {
                canvasBoard.Height = canvasBoard.Width;
            }
            else
            {
                canvasBoard.Width = canvasBoard.ActualHeight;
            }

            squareSize = (int)canvasBoard.Width / 8;

            SolidColorBrush gray = new SolidColorBrush(Colors.Gray);
            SolidColorBrush white = new SolidColorBrush(Colors.White);

            bool flipflop = true;

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Rectangle square = new Rectangle();

                    if (flipflop)
                    {
                        square.Fill = white;
                    }
                    else
                    {
                        square.Fill = gray;
                    }

                    flipflop = !flipflop;

                    square.Width = square.Height = squareSize;

                    square.Tag = new Coordinate { row = r, col = c };

                    int x = c * squareSize;
                    int y = r * squareSize;

                    Canvas.SetTop(square, y);
                    Canvas.SetLeft(square, x);

                    canvasBoard.Children.Add(square);
                }
                flipflop = !flipflop;
            }

            //Image blackKing = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/King_Black.png")), Width = squareSize, Height = squareSize };
            //Canvas.SetTop(blackKing, 0);
            //Canvas.SetLeft(blackKing, 3 * squareSize);
            //Canvas.SetZIndex(blackKing, 1);
            //canvasBoard.Children.Add(blackKing);

            //Image blackPawn = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/pawnBlack.png")), Width = squareSize, Height = squareSize };
            //Canvas.SetTop(blackPawn, 1);
            //Canvas.SetLeft(blackPawn, 5 * squareSize);
            //Canvas.SetZIndex(blackPawn, 5);
            //canvasBoard.Children.Add(blackPawn);
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            canvasBoard.Children.Clear();
            CanvasSetUp();
            LoadPieceImages();
        }

        private void btnColor_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CanvasSetUp();
            LoadPieceImages();
        }

        private void canvasBoard_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Image clickedImage = new Image();
            Rectangle rect = new Rectangle();
            SolidColorBrush gray = new SolidColorBrush(Colors.Gray);
            SolidColorBrush white = new SolidColorBrush(Colors.White);
            List<Coordinate> moves = new List<Coordinate>();

            if (clickedImage.GetType() == e.OriginalSource.GetType())
            {
                clickedImage = e.OriginalSource as Image;
                PieceClass p = clickedImage.Tag as PieceClass;
                bool continueCode = true;

                if (firstClick)
                {
                    moves = new List<Coordinate>(grid.PossibleMoves(p.position, p.position.team));
                    firstCoordinate = new Coordinate { row = p.position.row, col = p.position.col, team = p.team };
                    firstClick = false;
                }
                else
                {
                    firstClick = true;
                    moves = new List<Coordinate>(grid.PossibleMoves(firstCoordinate, firstCoordinate.team));

                    //If image is an enemy piece from possible moves of the originally selected piece
                    if (moves.Exists(x => (x.row == p.position.row && x.col == p.position.col)))
                    {
                        //Make Attack move
                        grid.DetermineAction(firstCoordinate, new Coordinate() { row = p.position.row, col = p.position.col }, firstCoordinate.team);
                        MoveImages(null, clickedImage, moves, white, gray, true);
                        continueCode = false;
                    }
                }

                if (continueCode)
                {
                    ResetHighlightedSquares(white, gray);
                    HighlightSquares(moves, white, gray);
                }
            }
            else
            {
                if (!firstClick)
                {
                    //Clicked on rectangle
                    rect = e.OriginalSource as Rectangle;
                    Coordinate coord = rect.Tag as Coordinate;
                    moves = new List<Coordinate>(grid.PossibleMoves(firstCoordinate, firstCoordinate.team));

                    if (moves.Exists(x => (x.row == coord.row && x.col == coord.col)))
                    {
                        grid.DetermineAction(firstCoordinate, new Coordinate() { row = coord.row, col = coord.col }, firstCoordinate.team);
                        MoveImages(rect, new Image(), moves, white, gray, false);
                    }
                    else
                    {
                        ResetHighlightedSquares(white, gray);
                        moves.Clear();
                        firstClick = true;
                    }

                    //Unnecessary comments
                    #region
                    ////Clicked on rectangle
                    //rect = e.OriginalSource as Rectangle;
                    //Coordinate p = rect.Tag as Coordinate;
                    //moves = new List<Coordinate>(grid.PossibleMoves(firstCoordinate, firstCoordinate.team));

                    //if (moves.Exists(x => (x.row == p.row && x.col == p.col)))
                    //{
                    //    grid.DetermineAction(firstCoordinate, new Coordinate() { row = p.row, col = p.col }, firstCoordinate.team);

                    //    foreach (UIElement child in canvasBoard.Children.ToList())
                    //    {
                    //        if (clickedImage.GetType() == child.GetType())
                    //        {
                    //            clickedImage = child as Image;
                    //            PieceClass tagInfo = clickedImage.Tag as PieceClass;

                    //            //Check if image is the originally selected piece to move
                    //            if (tagInfo.position.row == firstCoordinate.row && tagInfo.position.col == firstCoordinate.col)
                    //            {
                    //                canvasBoard.Children.Remove(child);
                    //                canvasBoard.Children.Add(SetImageProperties(clickedImage, p));
                    //                ResetHighlightedSquares(rect, white, gray);
                    //                firstClick = true;
                    //            }
                    //            //Check if image is the attacked piece
                    //            else if (tagInfo.position.row == firstCoordinate.row && tagInfo.position.col == firstCoordinate.col)
                    //            {
                    //                canvasBoard.Children.Remove(child);
                    //            }
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    ResetHighlightedSquares(rect, white, gray);
                    //    moves.Clear();
                    //    firstClick = true;
                    //}
                    #endregion
                }
                else
                {
                    moves.Clear();
                    firstClick = true;
                }
            }
        }

        //Move a piece (standard move action) or pieces (attack move action)
        private void MoveImages(Rectangle rect, Image clickedImage, List<Coordinate> moves, SolidColorBrush white, SolidColorBrush gray, bool isAttack)
        {
            if (!isAttack)
            {
                Coordinate p = rect.Tag as Coordinate;

                foreach (UIElement child in canvasBoard.Children.ToList())
                {
                    if (clickedImage.GetType() == child.GetType())
                    {
                        clickedImage = child as Image;
                        PieceClass tagInfo = clickedImage.Tag as PieceClass;

                        //Check if image is the originally selected piece to move
                        if (tagInfo.position.row == firstCoordinate.row && tagInfo.position.col == firstCoordinate.col)
                        {
                            canvasBoard.Children.Remove(child);
                            canvasBoard.Children.Add(SetImageProperties(clickedImage, p));
                            ResetHighlightedSquares(white, gray);
                            firstClick = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                //Attack
                PieceClass imagePiece = clickedImage.Tag as PieceClass;

                foreach (UIElement child in canvasBoard.Children.ToList())
                {
                    if (clickedImage.GetType() == child.GetType())
                    {
                        clickedImage = child as Image;
                        PieceClass tagInfo = clickedImage.Tag as PieceClass;

                        //Check if image is the originally selected piece to move
                        if (tagInfo.position.row == firstCoordinate.row && tagInfo.position.col == firstCoordinate.col)
                        {
                            canvasBoard.Children.Remove(child);
                            canvasBoard.Children.Add(SetImageProperties(clickedImage, imagePiece.position));
                            ResetHighlightedSquares(white, gray);
                            firstClick = true;
                        }
                        //Check if image is the attacked piece
                        else if (tagInfo.position.row == imagePiece.position.row && tagInfo.position.col == imagePiece.position.col)
                        {
                            canvasBoard.Children.Remove(child);
                        }
                    }
                }  
            }
        }

        //This function will reset the highlight squares on the board back to their original color (gray or white)
        private void ResetHighlightedSquares(SolidColorBrush white, SolidColorBrush gray)
        {
            Rectangle rect = new Rectangle();

            foreach (UIElement children in canvasBoard.Children.ToList())
            {
                if (children.GetType() == rect.GetType())
                {
                    rect = children as Rectangle;

                    SolidColorBrush brushOld = rect.Fill as SolidColorBrush;
                    Coordinate coord = new Coordinate();
                    coord = rect.Tag as Coordinate;

                    if (brushOld.Color.G == 225 || brushOld.Color.R == 225)
                    {
                        if (brushOld.Color.B == 128)
                        {
                            rect.Fill = white;
                            canvasBoard.Children.Remove(children);
                            Canvas.SetLeft(rect, coord.col * squareSize);
                            Canvas.SetTop(rect, coord.row * squareSize);
                            Canvas.SetZIndex(rect, 0);
                            rect.Height = rect.Width = squareSize;
                            canvasBoard.Children.Add(rect);
                        }
                        else if (brushOld.Color.B == 0)
                        {
                            rect.Fill = gray;
                            canvasBoard.Children.Remove(children);
                            Canvas.SetLeft(rect, coord.col * squareSize);
                            Canvas.SetTop(rect, coord.row * squareSize);
                            Canvas.SetZIndex(rect, 0);
                            rect.Height = rect.Width = squareSize;
                            canvasBoard.Children.Add(rect);
                        }
                    }
                }
            }
        }

        private void HighlightSquares(List<Coordinate> moves, SolidColorBrush white, SolidColorBrush gray)
        {
            Rectangle rect = new Rectangle();

            foreach (UIElement child in canvasBoard.Children.ToList())
            {
                if (child.GetType() == rect.GetType())
                {
                    rect = child as Rectangle;

                    SolidColorBrush brushOld = rect.Fill as SolidColorBrush;
                    Coordinate coord = new Coordinate();
                    coord = rect.Tag as Coordinate;

                    foreach (Coordinate move in moves)
                    {
                        if (move.col == coord.col && move.row == coord.row)
                        {
                            if (move.team == Team.Blank)
                            {
                                //make green
                                if (brushOld.Color.B == 255)
                                {
                                    SolidColorBrush brushWhite = new SolidColorBrush(Color.FromArgb(255, 128, 225, 128));
                                    rect.Fill = brushWhite;
                                }
                                else if (brushOld.Color.B == 128)
                                {
                                    SolidColorBrush brushGrey = new SolidColorBrush(Color.FromArgb(255, 0, 225, 0));
                                    rect.Fill = brushGrey;
                                }

                                canvasBoard.Children.Remove(child);
                                Canvas.SetLeft(rect, coord.col * squareSize);
                                Canvas.SetTop(rect, coord.row * squareSize);
                                Canvas.SetZIndex(rect, 0);
                                rect.Height = rect.Width = squareSize;
                                canvasBoard.Children.Add(rect);
                            }
                            else
                            {
                                //make red
                                if (brushOld.Color.B == 255)
                                {
                                    SolidColorBrush brushWhite = new SolidColorBrush(Color.FromArgb(255, 225, 128, 128));
                                    rect.Fill = brushWhite;
                                }
                                else if (brushOld.Color.B == 128)
                                {
                                    SolidColorBrush brushGrey = new SolidColorBrush(Color.FromArgb(255, 225, 0, 0));
                                    rect.Fill = brushGrey;
                                }

                                canvasBoard.Children.Remove(child);
                                Canvas.SetLeft(rect, coord.col * squareSize);
                                Canvas.SetTop(rect, coord.row * squareSize);
                                Canvas.SetZIndex(rect, 0);
                                rect.Height = rect.Width = squareSize;
                                canvasBoard.Children.Add(rect);
                            }

                        }
                    }
                }
            }
        }

        private Image SetImageProperties(Image image, Coordinate coord)
        {
            PieceClass imagePiece = image.Tag as PieceClass;

            if (imagePiece.team == Team.White)
            {
                switch (imagePiece.pieceType)
                {
                    case Piece.Pawn:
                        //Add White pawn to canvas
                        image = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/pawnWhite.png")), Width = squareSize, Height = squareSize };
                        imagePiece.position = new Coordinate() { row = coord.row, col = coord.col, team = Team.White };
                        image.Tag = imagePiece;
                        Canvas.SetTop(image, coord.row * squareSize);
                        Canvas.SetLeft(image, coord.col * squareSize);
                        Canvas.SetZIndex(image, 1);
                        return image;
                    case Piece.Bishop:
                        //Add White bishop to canvas
                        image = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/bishopWhite.png")), Width = squareSize, Height = squareSize };
                        imagePiece.position = new Coordinate() { row = coord.row, col = coord.col, team = Team.White };
                        image.Tag = imagePiece;
                        Canvas.SetTop(image, coord.row * squareSize);
                        Canvas.SetLeft(image, coord.col * squareSize);
                        Canvas.SetZIndex(image, 1);
                        return image;
                    case Piece.King:
                        //Add White king to canvas
                        image = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/kingWhite.png")), Width = squareSize, Height = squareSize };
                        imagePiece.position = new Coordinate() { row = coord.row, col = coord.col, team = Team.White };
                        image.Tag = imagePiece;
                        Canvas.SetTop(image, coord.row * squareSize);
                        Canvas.SetLeft(image, coord.col * squareSize);
                        Canvas.SetZIndex(image, 1);
                        return image;
                    case Piece.Knight:
                        //Add White knight to canvas
                        image = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/knightWhite.png")), Width = squareSize, Height = squareSize };
                        imagePiece.position = new Coordinate() { row = coord.row, col = coord.col, team = Team.White };
                        image.Tag = imagePiece;
                        Canvas.SetTop(image, coord.row * squareSize);
                        Canvas.SetLeft(image, coord.col * squareSize);
                        Canvas.SetZIndex(image, 1);
                        return image;
                    case Piece.Queen:
                        //Add White queen to canvas
                        image = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/queenWhite.png")), Width = squareSize, Height = squareSize };
                        imagePiece.position = new Coordinate() { row = coord.row, col = coord.col, team = Team.White };
                        image.Tag = imagePiece;
                        Canvas.SetTop(image, coord.row * squareSize);
                        Canvas.SetLeft(image, coord.col * squareSize);
                        Canvas.SetZIndex(image, 1);
                        return image;
                    case Piece.Rook:
                        //Add White Rook
                        image = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/rookWhite.png")), Width = squareSize, Height = squareSize };
                        imagePiece.position = new Coordinate() { row = coord.row, col = coord.col, team = Team.White };
                        image.Tag = imagePiece;
                        Canvas.SetTop(image, coord.row * squareSize);
                        Canvas.SetLeft(image, coord.col * squareSize);
                        Canvas.SetZIndex(image, 1);
                        return image;
                }
            }
            else
            {
                switch (imagePiece.pieceType)
                {
                    case Piece.Pawn: 
                        //Add Black pawn to canvas
                        image = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/pawnBlack.png")), Width = squareSize, Height = squareSize };
                        imagePiece.position = new Coordinate() { row = coord.row, col = coord.col, team = Team.Black };
                        image.Tag = imagePiece;
                        Canvas.SetTop(image, coord.row * squareSize);
                        Canvas.SetLeft(image, coord.col * squareSize);
                        Canvas.SetZIndex(image, 1);
                        return image;
                    case Piece.Bishop:
                        //Add Black bishop to canvas
                        image = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/bishopBlack.png")), Width = squareSize, Height = squareSize };
                        imagePiece.position = new Coordinate() { row = coord.row, col = coord.col, team = Team.Black };
                        image.Tag = imagePiece;
                        Canvas.SetTop(image, coord.row * squareSize);
                        Canvas.SetLeft(image, coord.col * squareSize);
                        Canvas.SetZIndex(image, 1);
                        return image;
                    case Piece.King:
                        //Add Black king to canvas
                        image = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/kingBlack.png")), Width = squareSize, Height = squareSize };
                        imagePiece.position = new Coordinate() { row = coord.row, col = coord.col, team = Team.Black };
                        image.Tag = imagePiece;
                        Canvas.SetTop(image, coord.row * squareSize);
                        Canvas.SetLeft(image, coord.col * squareSize);
                        Canvas.SetZIndex(image, 1);
                        return image;
                    case Piece.Knight:
                        //Add Black knight to canvas
                        image = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/knightBlack.png")), Width = squareSize, Height = squareSize };
                        imagePiece.position = new Coordinate() { row = coord.row, col = coord.col, team = Team.Black };
                        image.Tag = imagePiece;
                        Canvas.SetTop(image, coord.row * squareSize);
                        Canvas.SetLeft(image, coord.col * squareSize);
                        Canvas.SetZIndex(image, 1);
                        return image;
                    case Piece.Queen:
                        //Add Black queen to canvas
                        image = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/queenBlack.png")), Width = squareSize, Height = squareSize };
                        imagePiece.position = new Coordinate() { row = coord.row, col = coord.col, team = Team.Black };
                        image.Tag = imagePiece;
                        Canvas.SetTop(image, coord.row * squareSize);
                        Canvas.SetLeft(image, coord.col * squareSize);
                        Canvas.SetZIndex(image, 1);
                        return image;
                    case Piece.Rook:
                        //Add Black Rook
                        image = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/rookBlack.png")), Width = squareSize, Height = squareSize };
                        imagePiece.position = new Coordinate() { row = coord.row, col = coord.col, team = Team.Black };
                        image.Tag = imagePiece;
                        Canvas.SetTop(image, coord.row * squareSize);
                        Canvas.SetLeft(image, coord.col * squareSize);
                        Canvas.SetZIndex(image, 1);
                        return image;
                }
            }

            //If program reaches this line; something went wrong with the piece type
            return image;
        }

        private void LoadPieceImages()
        {
            PieceClass imagePiece;
            Image pawn;

            //Adding Pawns
            for (int i = 0; i < 8; i++)
            {
                imagePiece = new PieceClass();
                imagePiece.pieceType = Piece.Pawn;
                imagePiece.team = Team.Black;

                //Add Black pawn to canvas
                pawn = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/pawnBlack.png")), Width = squareSize, Height = squareSize };
                imagePiece.position = new Coordinate() { row = 1, col = i, team = Team.Black };
                pawn.Tag = imagePiece;
                Canvas.SetTop(pawn, squareSize);
                Canvas.SetLeft(pawn, i * squareSize);
                Canvas.SetZIndex(pawn, 1);
                canvasBoard.Children.Add(pawn);

                imagePiece = new PieceClass();
                imagePiece.pieceType = Piece.Pawn;
                imagePiece.team = Team.White;

                //Add White pawn to canvas
                pawn = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/pawnWhite.png")), Width = squareSize, Height = squareSize };
                imagePiece.position = new Coordinate() { row = 6, col = i, team = Team.White };
                pawn.Tag = imagePiece;
                Canvas.SetTop(pawn, 6 * squareSize);
                Canvas.SetLeft(pawn, i * squareSize);
                Canvas.SetZIndex(pawn, 1);
                canvasBoard.Children.Add(pawn);
            }

            AddRooksToCanvas();
            AddKnightsToCanvas();
            AddBishopsToCanvas();
            AddKingsAndQueensToCanvas();
        }

        private void AddRooksToCanvas()
        {
            PieceClass imagePiece;

            for (int i = 0; i < 2; i++)
            {
                if (i == 1)
                {
                    i = 7;      //Set i to 7 for the rooks on the right of the screen (at column 7)
                }

                imagePiece = new PieceClass();
                imagePiece.pieceType = Piece.Rook;
                imagePiece.team = Team.Black;

                //Add Black Rook
                Image blackRook = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/rookBlack.png")), Width = squareSize, Height = squareSize };
                imagePiece.position = new Coordinate() { row = 0, col = i, team = Team.Black };
                blackRook.Tag = imagePiece;
                Canvas.SetTop(blackRook, 0);
                Canvas.SetLeft(blackRook, i * squareSize);
                Canvas.SetZIndex(blackRook, 1);
                canvasBoard.Children.Add(blackRook);

                imagePiece = new PieceClass();
                imagePiece.pieceType = Piece.Rook;
                imagePiece.team = Team.White;

                //Add White Rook
                Image whiteRook = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/rookWhite.png")), Width = squareSize, Height = squareSize };
                imagePiece.position = new Coordinate() { row = 7, col = i, team = Team.White };
                whiteRook.Tag = imagePiece;
                Canvas.SetTop(whiteRook, 7 * squareSize);
                Canvas.SetLeft(whiteRook, i * squareSize);
                Canvas.SetZIndex(whiteRook, 1);
                canvasBoard.Children.Add(whiteRook);
            }
        }

        private void AddKnightsToCanvas()
        {
            PieceClass imagePiece;

            for (int i = 1; i < 3; i++)
            {
                if (i == 2)
                {
                    i = 6;      //Set i to 7 for the rooks on the right of the screen (at column 7)
                }

                imagePiece = new PieceClass();
                imagePiece.pieceType = Piece.Knight;
                imagePiece.team = Team.Black;

                //Add Black Knight
                Image blackKnight = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/knightBlack.png")), Width = squareSize, Height = squareSize };
                imagePiece.position = new Coordinate() { row = 0, col = i, team = Team.Black };
                blackKnight.Tag = imagePiece;
                Canvas.SetTop(blackKnight, 0);
                Canvas.SetLeft(blackKnight, i * squareSize);
                Canvas.SetZIndex(blackKnight, 1);
                canvasBoard.Children.Add(blackKnight);

                imagePiece = new PieceClass();
                imagePiece.pieceType = Piece.Knight;
                imagePiece.team = Team.White;

                //Add White Knight
                Image whiteKnight = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/knightWhite.png")), Width = squareSize, Height = squareSize };
                imagePiece.position = new Coordinate() { row = 7, col = i, team = Team.White };
                whiteKnight.Tag = imagePiece;
                Canvas.SetTop(whiteKnight, 7 * squareSize);
                Canvas.SetLeft(whiteKnight, i * squareSize);
                Canvas.SetZIndex(whiteKnight, 1);
                canvasBoard.Children.Add(whiteKnight);
            }
        }

        private void AddBishopsToCanvas()
        {
            PieceClass imagePiece;

            for (int i = 2; i < 4; i++)
            {
                if (i == 3)
                {
                    i = 5;      //Set i to 7 for the rooks on the right of the screen (at column 7)
                }

                imagePiece = new PieceClass();
                imagePiece.pieceType = Piece.Bishop;
                imagePiece.team = Team.Black;

                //Add Black Bishop
                Image blackBishop = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/bishopBlack.png")), Width = squareSize, Height = squareSize };
                imagePiece.position = new Coordinate() { row = 0, col = i, team = Team.Black };
                blackBishop.Tag = imagePiece;
                Canvas.SetTop(blackBishop, 0);
                Canvas.SetLeft(blackBishop, i * squareSize);
                Canvas.SetZIndex(blackBishop, 1);
                canvasBoard.Children.Add(blackBishop);

                imagePiece = new PieceClass();
                imagePiece.pieceType = Piece.Bishop;
                imagePiece.team = Team.White;

                //Add White Bishop
                Image whiteBishop = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/bishopWhite.png")), Width = squareSize, Height = squareSize };
                imagePiece.position = new Coordinate() { row = 7, col = i, team = Team.White };
                whiteBishop.Tag = imagePiece;
                Canvas.SetTop(whiteBishop, 7 * squareSize);
                Canvas.SetLeft(whiteBishop, i * squareSize);
                Canvas.SetZIndex(whiteBishop, 1);
                canvasBoard.Children.Add(whiteBishop);
            }
        }

        private void AddKingsAndQueensToCanvas()
        {
            PieceClass imagePiece = new PieceClass();
            imagePiece.pieceType = Piece.Queen;
            imagePiece.team = Team.Black;

            //Add Black Queen
            Image blackQueen = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/queenBlack.png")), Width = squareSize, Height = squareSize };
            imagePiece.position = new Coordinate() { row = 0, col = 3, team = Team.Black };
            blackQueen.Tag = imagePiece;
            Canvas.SetTop(blackQueen, 0);
            Canvas.SetLeft(blackQueen, 3 * squareSize);
            Canvas.SetZIndex(blackQueen, 1);
            canvasBoard.Children.Add(blackQueen);

            imagePiece = new PieceClass();
            imagePiece.pieceType = Piece.Queen;
            imagePiece.team = Team.White;

            //Add White Queen
            Image whiteQueen = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/queenWhite.png")), Width = squareSize, Height = squareSize };
            imagePiece.position = new Coordinate() { row = 7, col = 3, team = Team.White };
            whiteQueen.Tag = imagePiece;
            Canvas.SetTop(whiteQueen, 7 * squareSize);
            Canvas.SetLeft(whiteQueen, 3 * squareSize);
            Canvas.SetZIndex(whiteQueen, 1);
            canvasBoard.Children.Add(whiteQueen);

            imagePiece = new PieceClass();
            imagePiece.pieceType = Piece.King;
            imagePiece.team = Team.Black;

            //Add Black King
            Image blackKing = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/kingBlack.png")), Width = squareSize, Height = squareSize };
            imagePiece.position = new Coordinate() { row = 0, col = 4, team = Team.Black };
            blackKing.Tag = imagePiece;
            Canvas.SetTop(blackKing, 0);
            Canvas.SetLeft(blackKing, 4 * squareSize);
            Canvas.SetZIndex(blackKing, 1);
            canvasBoard.Children.Add(blackKing);

            imagePiece = new PieceClass();
            imagePiece.pieceType = Piece.King;
            imagePiece.team = Team.White;

            //Add White King
            Image whiteKing = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/kingWhite.png")), Width = squareSize, Height = squareSize };
            imagePiece.position = new Coordinate() { row = 7, col = 4, team = Team.White };
            whiteKing.Tag = imagePiece;
            Canvas.SetTop(whiteKing, 7 * squareSize);
            Canvas.SetLeft(whiteKing, 4 * squareSize);
            Canvas.SetZIndex(whiteKing, 1);
            canvasBoard.Children.Add(whiteKing);
        }
    }
}