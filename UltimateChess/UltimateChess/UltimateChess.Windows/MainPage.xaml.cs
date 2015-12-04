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
        public Coordinate firstClick;
        public Coordinate secondClick;
        private int squareSize;

        public MainPage()
        {
            this.InitializeComponent();
            GridModel grid = new GridModel();
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
            if(canvasBoard.ActualWidth < canvasBoard.ActualHeight)
            {
                canvasBoard.Height = canvasBoard.Width;
            }
            else
            {
                canvasBoard.Width = canvasBoard.ActualHeight;
            }

            squareSize = (int)canvasBoard.Width / 8;

            canvasBoard.Children.Clear();

            SolidColorBrush gray = new SolidColorBrush(Colors.Gray);
            SolidColorBrush white = new SolidColorBrush(Colors.White);

            bool flipflop = true;

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Rectangle square = new Rectangle();

                    if(flipflop)
                    {
                        square.Fill = white;
                    }
                    else
                    {
                        square.Fill = gray;
                    }

                    flipflop = !flipflop;

                    square.Width = square.Height = squareSize;

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

        private void StoryBoardSetup()
        {

        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {

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

            if (clickedImage.GetType() == e.OriginalSource.GetType())
            {
                clickedImage = e.OriginalSource as Image;
                
            }
        }

        private void LoadPieceImages()
        {
            PieceClass imagePiece = new PieceClass();
            imagePiece.pieceType = Piece.Pawn;

            //Adding Pawns
            for (int i = 0; i < 8; i++)
            {
                //Add White pawn to canvas
                Image pawn = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/pawnBlack.png")), Width = squareSize, Height = squareSize };
                imagePiece.team = Team.Black;
                imagePiece.position = new Coordinate() { row = 1, col = i };
                pawn.Tag = imagePiece;
                Canvas.SetTop(pawn, squareSize);
                Canvas.SetLeft(pawn, i * squareSize);
                Canvas.SetZIndex(pawn, 1);
                canvasBoard.Children.Add(pawn);

                //Add Black pawn to canvas
                pawn = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/pawnWhite.png")), Width = squareSize, Height = squareSize };
                imagePiece.team = Team.White;
                imagePiece.position = new Coordinate() { row = 6, col = i };
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
            PieceClass imagePiece = new PieceClass();
            imagePiece.pieceType = Piece.Rook;

            for (int i = 0; i < 2; i++)
            {
                if (i == 1)
                {
                    i = 7;      //Set i to 7 for the rooks on the right of the screen (at column 7)
                }

                for (int j = 0; j < 2; j++)
                {
                    //Add Black Rook
                    Image blackRook = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/rookBlack.png")), Width = squareSize, Height = squareSize };
                    imagePiece.team = Team.Black;
                    imagePiece.position = new Coordinate() { row = 0, col = i };
                    blackRook.Tag = imagePiece;
                    Canvas.SetTop(blackRook, 0);
                    Canvas.SetLeft(blackRook, i * squareSize);
                    Canvas.SetZIndex(blackRook, 1);
                    canvasBoard.Children.Add(blackRook);

                    //Add White Rook
                    Image whiteRook = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/rookWhite.png")), Width = squareSize, Height = squareSize };
                    imagePiece.team = Team.White;
                    imagePiece.position = new Coordinate() { row = 7, col = i };
                    whiteRook.Tag = imagePiece;
                    Canvas.SetTop(whiteRook, 7 * squareSize);
                    Canvas.SetLeft(whiteRook, i * squareSize);
                    Canvas.SetZIndex(whiteRook, 1);
                    canvasBoard.Children.Add(whiteRook);
                }
            }
        }

        private void AddKnightsToCanvas()
        {
            PieceClass imagePiece = new PieceClass();
            imagePiece.pieceType = Piece.Knight;

            for (int i = 1; i < 3; i++)
            {
                if (i == 2)
                {
                    i = 6;      //Set i to 7 for the rooks on the right of the screen (at column 7)
                }

                for (int j = 0; j < 2; j++)
                {
                    //Add Black Knight
                    Image blackKnight = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/knightBlack.png")), Width = squareSize, Height = squareSize };
                    imagePiece.team = Team.Black;
                    imagePiece.position = new Coordinate() { row = 0, col = i };
                    blackKnight.Tag = imagePiece;
                    Canvas.SetTop(blackKnight, 0);
                    Canvas.SetLeft(blackKnight, i * squareSize);
                    Canvas.SetZIndex(blackKnight, 1);
                    canvasBoard.Children.Add(blackKnight);

                    //Add White Knight
                    Image whiteKnight = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/knightWhite.png")), Width = squareSize, Height = squareSize };
                    imagePiece.team = Team.White;
                    imagePiece.position = new Coordinate() { row = 7, col = i };
                    whiteKnight.Tag = imagePiece;
                    Canvas.SetTop(whiteKnight, 7 * squareSize);
                    Canvas.SetLeft(whiteKnight, i * squareSize);
                    Canvas.SetZIndex(whiteKnight, 1);
                    canvasBoard.Children.Add(whiteKnight);
                }
            }
        }

        private void AddBishopsToCanvas()
        {
            PieceClass imagePiece = new PieceClass();
            imagePiece.pieceType = Piece.Bishop;

            for (int i = 2; i < 4; i++)
            {
                if (i == 3)
                {
                    i = 5;      //Set i to 7 for the rooks on the right of the screen (at column 7)
                }

                for (int j = 0; j < 2; j++)
                {
                    //Add Black Bishop
                    Image blackBishop = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/bishopBlack.png")), Width = squareSize, Height = squareSize };
                    imagePiece.team = Team.Black;
                    imagePiece.position = new Coordinate() { row = 0, col = i };
                    blackBishop.Tag = imagePiece;
                    Canvas.SetTop(blackBishop, 0);
                    Canvas.SetLeft(blackBishop, i * squareSize);
                    Canvas.SetZIndex(blackBishop, 1);
                    canvasBoard.Children.Add(blackBishop);

                    //Add White Bishop
                    Image whiteBishop = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/bishopWhite.png")), Width = squareSize, Height = squareSize };
                    imagePiece.team = Team.White;
                    imagePiece.position = new Coordinate() { row = 7, col = i };
                    whiteBishop.Tag = imagePiece;
                    Canvas.SetTop(whiteBishop, 7 * squareSize);
                    Canvas.SetLeft(whiteBishop, i * squareSize);
                    Canvas.SetZIndex(whiteBishop, 1);
                    canvasBoard.Children.Add(whiteBishop);
                }
            }
        }

        private void AddKingsAndQueensToCanvas()
        {
            PieceClass imagePiece = new PieceClass();
            imagePiece.pieceType = Piece.Queen;

            //Add Black Queen
            Image blackQueen = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/queenBlack.png")), Width = squareSize, Height = squareSize };
            imagePiece.team = Team.Black;
            imagePiece.position = new Coordinate() { row = 0, col = 4 };
            blackQueen.Tag = imagePiece;
            Canvas.SetTop(blackQueen, 0);
            Canvas.SetLeft(blackQueen, 3 * squareSize);
            Canvas.SetZIndex(blackQueen, 1);
            canvasBoard.Children.Add(blackQueen);

            //Add White Queen
            Image whiteQueen = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/queenWhite.png")), Width = squareSize, Height = squareSize };
            imagePiece.team = Team.White;
            imagePiece.position = new Coordinate() { row = 7, col = 4 };
            whiteQueen.Tag = imagePiece;
            Canvas.SetTop(whiteQueen, 7 * squareSize);
            Canvas.SetLeft(whiteQueen, 3 * squareSize);
            Canvas.SetZIndex(whiteQueen, 1);
            canvasBoard.Children.Add(whiteQueen);

            imagePiece.pieceType = Piece.King;

            //Add Black King
            Image blackKing = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/kingBlack.png")), Width = squareSize, Height = squareSize };
            imagePiece.team = Team.Black;
            imagePiece.position = new Coordinate() { row = 0, col = 5 };
            blackKing.Tag = imagePiece;
            Canvas.SetTop(blackKing, 0);
            Canvas.SetLeft(blackKing, 4 * squareSize);
            Canvas.SetZIndex(blackKing, 1);
            canvasBoard.Children.Add(blackKing);

            //Add White King
            Image whiteKing = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/kingWhite.png")), Width = squareSize, Height = squareSize };
            imagePiece.team = Team.White;
            imagePiece.position = new Coordinate() { row = 7, col = 5 };
            whiteKing.Tag = imagePiece;
            Canvas.SetTop(whiteKing, 7 * squareSize);
            Canvas.SetLeft(whiteKing, 4 * squareSize);
            Canvas.SetZIndex(whiteKing, 1);
            canvasBoard.Children.Add(whiteKing);
        }
    }
}
