﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UltimateChess.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using UltimateChess.Common;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UltimateChess
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public Coordinate firstCoordinate;
        public GridModel grid;
        public bool firstClick = true;
        public App obj = App.Current as App;

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private int squareSize;
        private String savedPiecesString = "";
        private List<String> capturedWhitePieces = new List<String>();
        private List<String> capturedBlackPieces = new List<String>();
        private string teamOneColor = "White";
        private string teamTwoColor = "Black";
        private bool loadedFromSaveState = false;

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public MainPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            grid = new GridModel();
            //grid.Start();
            LayoutGridSetUp();
            var obj = App.Current as App;
            teamOneColor = obj.passedColors.TeamOne;
            teamTwoColor = obj.passedColors.TeamTwo;

            //SendWithDelay();
        }

        private async Task SendWithDelay()
        {
            await Task.Delay(300);
            CanvasSetUp();
            LoadPieceImages();
            CapturedCanvasSetUp();
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
        }

        private void CapturedCanvasSetUp()
        {
            whiteCapturedCanvas.Height = blackCapturedCanvas.Height = canvasBoard.ActualHeight;
            whiteCapturedCanvas.Width = blackCapturedCanvas.Width = squareSize * 2;
        }

        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            //e.PageState["piecesOnGrid"] = "orange,black|King,false,6,0,black|Queen,false,6,4,black";
            e.PageState["piecesOnGrid"] = SavePiecesState();
        }

        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.PageState != null && e.PageState.ContainsKey("piecesOnGrid"))
            {
                loadedFromSaveState = true;
                savedPiecesString = e.PageState["piecesOnGrid"].ToString();
            }
        }

        private String SavePiecesState()
        {
            //TODO: remove this generic statement
            String saveState = "";
            var obj = App.Current as App;
            saveState += obj.passedColors.TeamOne + "," + obj.passedColors.TeamTwo + "?";

            TextBox box = new TextBox();

            foreach (UIElement child in whiteCapturedCanvas.Children.ToList())
            {
                if (box.GetType() == child.GetType())
                {
                    box = child as TextBox;
                    String posType = box.Tag as String;
                    String type = posType.Split('|')[1];

                    String boxText = box.Text;
                    String count = boxText.Split('x')[1];
                    int num = Convert.ToInt32(count);
                    
                    for(int i = 0; i < num; i++)
                    {
                        saveState += type + ",White|";
                    }
                }
            }

            foreach (UIElement child in blackCapturedCanvas.Children.ToList())
            {
                if (box.GetType() == child.GetType())
                {
                    box = child as TextBox;
                    String posType = box.Tag as String;
                    String type = posType.Split('|')[1];

                    String boxText = box.Text;
                    String count = boxText.Split('x')[1];
                    int num = Convert.ToInt32(count);

                    for (int i = 0; i < num; i++)
                    {
                        saveState += type + ",Black|";
                    }
                }
            }
            
            saveState += "?";

            //Create active pieces part of the state string
            foreach (UIElement child in canvasBoard.Children.ToList())
            {
                Image pieceImage = new Image();

                if (pieceImage.GetType() == child.GetType())
                {
                    pieceImage = child as Image;
                    PieceClass pieceInfo = pieceImage.Tag as PieceClass;
                    saveState += pieceInfo.pieceType.ToString() + "," + pieceInfo.hasMoved.ToString() + "," + pieceInfo.position.row + "," + pieceInfo.position.col
                        + "," + pieceInfo.team.ToString() + "|";
                }
            }
            
            return saveState;
        }

        private void LoadSavedPieces(String gridData)
        {
            //gridData is in this format: "team1Color,team2color?capturedPieceType,team|capturedPieceType,team?pieceType,hasMoved,row,col,team|pieceType,hasMoved,row,col,team|" etc...
            //"team" will always be black or white (white is on bottom of the main page)
            var obj = App.Current as App;
            String[] dataArray = gridData.Split('?');
            String[] piecesArray = dataArray[2].Split('|');
            String[] capturedPiecesArray = dataArray[1].Split('|');
            //dataArray[1] is the string array of captured pieces

            if (!loadedFromSaveState)
            {
                obj.passedColors.TeamOne = dataArray[0].Split(',')[0];
                obj.passedColors.TeamTwo = dataArray[0].Split(',')[1];
            }

            canvasBoard.Children.Clear();
            CanvasSetUp();

            foreach (String captured in capturedPiecesArray)
            {
                if (!string.IsNullOrWhiteSpace(captured))
                {
                    String[] splitCaptured = captured.Split(',');
                    PieceClass piece = new PieceClass();

                    #region Creating captured piece from string
                    switch (splitCaptured[0])
                    {
                        case "Pawn":
                            piece.pieceType = Piece.Pawn;
                            break;
                        case "Rook":
                            piece.pieceType = Piece.Rook;
                            break;
                        case "Knight":
                            piece.pieceType = Piece.Knight;
                            break;
                        case "Bishop":
                            piece.pieceType = Piece.Bishop;
                            break;
                        case "Queen":
                            piece.pieceType = Piece.Queen;
                            break;
                        case "King":
                            piece.pieceType = Piece.King;
                            break;
                    }

                    if (splitCaptured[1] == "white")
                    {
                        piece.team = Team.White;
                    }
                    else
                    {
                        piece.team = Team.Black;
                    }
                    #endregion

                    AddCapturedImage(piece);
                    grid.UpdateActiveListFromSaveState(piece);
                    //piece = null;
                }
            }

            foreach (String pieceString in piecesArray)
            {
                if (!string.IsNullOrWhiteSpace(pieceString))
                {
                    String[] splitPieceString = pieceString.Split(',');
                    PieceClass piece = new PieceClass() { position = new Coordinate() { row = Convert.ToInt32(splitPieceString[2]),
                        col = Convert.ToInt32(splitPieceString[3]) } };

                    #region Creating the piece from the string...
                    switch (splitPieceString[0])
                    {
                        case "Pawn":
                            piece.pieceType = Piece.Pawn;
                            break;
                        case "Rook":
                            piece.pieceType = Piece.Rook;
                            break;
                        case "Knight":
                            piece.pieceType = Piece.Knight;
                            break;
                        case "Bishop":
                            piece.pieceType = Piece.Bishop;
                            break;
                        case "Queen":
                            piece.pieceType = Piece.Queen;
                            break;
                        case "King":
                            piece.pieceType = Piece.King;
                            break;
                    }

                    if (splitPieceString[1] == "True")
                    {
                        piece.hasMoved = true;
                    }
                    else
                    {
                        piece.hasMoved = false;
                    }

                    if (splitPieceString[4] == "White")
                    {
                        piece.team = Team.White;
                        piece.position.team = Team.White;
                    }
                    else
                    {
                        piece.team = Team.Black;
                        piece.position.team = Team.Black;
                    }
                    #endregion

                    Image savedPieceImage = new Image();
                    savedPieceImage.Tag = piece;
                    Coordinate position = piece.position;
                    savedPieceImage = SetImageProperties(savedPieceImage, position, true);
                    canvasBoard.Children.Add(savedPieceImage);
                    //Add piece to grid model
                    grid.AddSavedPiece(piece);
                    //savedPieceImage = null;
                    //piece = null;
                }                
            }

            grid.SetEmptyGridCellsBlank();
        }

        #region NavigationHelper registration

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);

            //if (loaded)
            //{
            //    var obj = App.Current as App;
            //    teamOneColor = obj.passedColors.TeamOne;
            //    teamTwoColor = obj.passedColors.TeamTwo;

            //    foreach (UIElement child in canvasBoard.Children.ToList())
            //    {
            //        Image i = new Image();
            //        if (child.GetType() == i.GetType())
            //        {
            //            i = child as Image;
            //            canvasBoard.Children.Remove(i);
            //            Image newImage = new Image();
            //            PieceClass p = new PieceClass();
            //            p = i.Tag as PieceClass;
            //            newImage.Tag = i.Tag;
            //            canvasBoard.Children.Add(SetImageProperties(newImage, p.position));
            //        }
            //    }
            //}
            //else
            //{
            //    loaded = true;
            //}
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            newGame();
        }

        private void newGame()
        {
            canvasBoard.Children.Clear();
            CanvasSetUp();
            LoadPieceImages();
            grid.Start();
            firstClick = true;

            whiteCapturedCanvas.Children.Clear();
            blackCapturedCanvas.Children.Clear();
            capturedBlackPieces.Clear();
            capturedWhitePieces.Clear();
        }

        private void btnColor_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ColorPage));
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AboutPage));
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private async void canvasBoard_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Image clickedImage = new Image();
            SolidColorBrush gray = new SolidColorBrush(Colors.Gray);
            SolidColorBrush white = new SolidColorBrush(Colors.White);
            List<Coordinate> moves = new List<Coordinate>();

            if (clickedImage.GetType() == e.OriginalSource.GetType())
            {
                //Code for if a chess piece image was clicked
                clickedImage = e.OriginalSource as Image;
                PieceClass p = clickedImage.Tag as PieceClass;
                bool continueCode = true;

                if (firstClick)
                {
                    if (p.team == grid.currentPlayerTurn)
                    {
                        moves = new List<Coordinate>(grid.PossibleMoves(p.position, p.position.team));
                        firstCoordinate = new Coordinate { row = p.position.row, col = p.position.col, team = p.team };
                        firstClick = false;
                        continueCode = true;
                    }
                    else
                    {
                        //NOT YOUR MOVE
                        var dialog = new MessageDialog("", "Patience!");
                        dialog.Content += "It is " + grid.currentPlayerTurn.ToString() + "'s turn." + " Please wait for your turn.";
                        await dialog.ShowAsync();
                    }
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
                        AddCapturedImage(p);
                        continueCode = false;
                    }
                }

                p = null;

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
                    Rectangle rect = new Rectangle();
                    rect = e.OriginalSource as Rectangle;
                    Coordinate coord = rect.Tag as Coordinate;
                    moves = new List<Coordinate>(grid.PossibleMoves(firstCoordinate, firstCoordinate.team));

                    if (moves.Exists(x => (x.row == coord.row && x.col == coord.col)))
                    {
                        MoveImages(rect, new Image(), moves, white, gray, false);
                        grid.DetermineAction(firstCoordinate, coord, firstCoordinate.team);
                    }
                    else
                    {
                        ResetHighlightedSquares(white, gray);
                        firstClick = true;
                    }

                    coord = null;
                    moves.Clear();
                    rect = null;
                }
                else
                {
                    moves.Clear();
                    firstClick = true;
                }
            }

            clickedImage = null;
        }

        //Move a piece (standard move action) or pieces (attack move action)
        private async void MoveImages(Rectangle rect, Image clickedImage, List<Coordinate> moves, SolidColorBrush white, SolidColorBrush gray, bool isAttack)
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
                            canvasBoard.Children.Add(SetImageProperties(clickedImage, p, false));
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
                            canvasBoard.Children.Add(SetImageProperties(clickedImage, imagePiece.position, false));
                            ResetHighlightedSquares(white, gray);
                            firstClick = true;
                        }
                        //Check if image is the attacked piece
                        else if (tagInfo.position.row == imagePiece.position.row && tagInfo.position.col == imagePiece.position.col)
                        {
                            canvasBoard.Children.Remove(child);
                            if (tagInfo.pieceType == Piece.King)
                            {
                                if (tagInfo.team == Team.Black)
                                {
                                    var dialog = new MessageDialog("Congrats! You are the winner!", "White Team Wins!");
                                    await dialog.ShowAsync();
                                }
                                else
                                {
                                    var dialog = new MessageDialog("Congrats! You are the winner!", "Black Team Wins!");
                                    await dialog.ShowAsync();
                                }
                            }
                        }
                    }
                }
            }
        }

        //Take the attacked piece and move to relevant captured panel or increase value already on panel for captured piece
        private void AddCapturedImage(PieceClass piece)
        {
            Image cappedImage = new Image();
            cappedImage.Tag = piece;
            cappedImage = SetImageProperties(cappedImage, new Coordinate() { row = 0, col = 0 }, false);

            if (piece.team == Team.White)
            {
                String type = piece.pieceType.ToString();
                if (capturedWhitePieces.Contains(type))
                {
                    //Increase piece counter
                    IncrementCapturedCounter(type, piece.team);
                }
                else
                {
                    //Add piece to panel
                    capturedWhitePieces.Add(piece.pieceType.ToString());
                    TextBox textBox = new TextBox();
                    textBox.Text = "x1";
                    textBox.Tag = Convert.ToString(((whiteCapturedCanvas.Children.Count() / 2) * squareSize) + (squareSize / 2)) + "|" + type;
                    Canvas.SetLeft(textBox, squareSize / 4);
                    Canvas.SetTop(textBox, (((whiteCapturedCanvas.Children.Count() / 2) * squareSize) + (squareSize / 2)));
                    whiteCapturedCanvas.Children.Add(textBox);

                    Canvas.SetLeft(cappedImage, squareSize);
                    Canvas.SetTop(cappedImage, squareSize * (capturedWhitePieces.Count() - 1));
                    whiteCapturedCanvas.Children.Add(cappedImage);
                }
            }
            else
            {
                String type = piece.pieceType.ToString();
                if (capturedBlackPieces.Contains(type))
                {
                    //Increase piece counter
                    IncrementCapturedCounter(type, piece.team);
                }
                else
                {
                    //Add piece to panel
                    capturedBlackPieces.Add(piece.pieceType.ToString());
                    TextBox textBox = new TextBox();
                    textBox.Text = "x1";
                    textBox.Tag = Convert.ToString(((blackCapturedCanvas.Children.Count() / 2) * squareSize) + (squareSize / 2)) + "|" + type;
                    Canvas.SetLeft(textBox, (squareSize + (squareSize / 4)));
                    Canvas.SetTop(textBox, (((blackCapturedCanvas.Children.Count() / 2) * squareSize) + (squareSize / 2)));
                    blackCapturedCanvas.Children.Add(textBox);

                    Canvas.SetLeft(cappedImage, 0);
                    Canvas.SetTop(cappedImage, squareSize * (capturedBlackPieces.Count() - 1));
                    blackCapturedCanvas.Children.Add(cappedImage);
                }

            }
        }

        private void IncrementCapturedCounter(String type, Team team)
        {
            TextBox box = new TextBox();
            if (team == Team.White)
            {
                foreach (UIElement child in whiteCapturedCanvas.Children.ToList())
                {
                    if (box.GetType() == child.GetType())
                    {
                        box = child as TextBox;
                        String testType = box.Tag as String;
                        String position = (testType.Split('|')[0]);
                        int pos = Convert.ToInt32(position);
                        testType = testType.Split('|')[1];

                        if (testType == type)
                        {
                            whiteCapturedCanvas.Children.Remove(child);
                            testType = box.Text;
                            testType = testType.Split('x')[1];
                            int num = Convert.ToInt32(testType);
                            num++;
                            testType = "x" + Convert.ToString(num);

                            box.Text = testType;
                            box.Tag = position + "|" + type;
                            Canvas.SetLeft(box, squareSize / 4);
                            Canvas.SetTop(box, (pos));
                            whiteCapturedCanvas.Children.Add(box);
                            break;
                        }
                    }
                }
            }
            else
            {
                foreach (UIElement child in blackCapturedCanvas.Children.ToList())
                {
                    if (box.GetType() == child.GetType())
                    {
                        box = child as TextBox;
                        String testType = box.Tag as String;
                        String position = (testType.Split('|')[0]);
                        int pos = Convert.ToInt32(position);
                        testType = testType.Split('|')[1];

                        if (testType == type)
                        {
                            blackCapturedCanvas.Children.Remove(child);
                            testType = box.Text;
                            testType = testType.Split('x')[1];
                            int num = Convert.ToInt32(testType);
                            num++;
                            testType = "x" + Convert.ToString(num);

                            box.Text = testType;
                            box.Tag = position + "|" + type;
                            Canvas.SetLeft(box, squareSize + (squareSize / 4));
                            Canvas.SetTop(box, (pos));
                            blackCapturedCanvas.Children.Add(box);
                            break;
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

        private Image SetImageProperties(Image image, Coordinate coord, bool loadState)
        {
            PieceClass imagePiece = image.Tag as PieceClass;
            var obj = App.Current as App;

            if (imagePiece.team == Team.White)
            {
                switch (imagePiece.pieceType)
                {
                    case Piece.Pawn:
                        //Add White pawn to canvas
                        image = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/pawn" + obj.passedColors.TeamOne + ".png")), Width = squareSize, Height = squareSize };
                        imagePiece.position = new Coordinate() { row = coord.row, col = coord.col, team = Team.White };

                        if (!imagePiece.hasMoved && !loadState)
                        {
                            imagePiece.hasMoved = true;
                        }

                        image.Tag = imagePiece;
                        Canvas.SetTop(image, coord.row * squareSize);
                        Canvas.SetLeft(image, coord.col * squareSize);
                        Canvas.SetZIndex(image, 1);
                        return image;
                    case Piece.Bishop:
                        //Add White bishop to canvas
                        image = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/bishop" + obj.passedColors.TeamOne + ".png")), Width = squareSize, Height = squareSize };
                        imagePiece.position = new Coordinate() { row = coord.row, col = coord.col, team = Team.White };
                        image.Tag = imagePiece;
                        Canvas.SetTop(image, coord.row * squareSize);
                        Canvas.SetLeft(image, coord.col * squareSize);
                        Canvas.SetZIndex(image, 1);
                        return image;
                    case Piece.King:
                        //Add White king to canvas
                        image = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/king" + obj.passedColors.TeamOne + ".png")), Width = squareSize, Height = squareSize };
                        imagePiece.position = new Coordinate() { row = coord.row, col = coord.col, team = Team.White };
                        image.Tag = imagePiece;
                        Canvas.SetTop(image, coord.row * squareSize);
                        Canvas.SetLeft(image, coord.col * squareSize);
                        Canvas.SetZIndex(image, 1);
                        return image;
                    case Piece.Knight:
                        //Add White knight to canvas
                        image = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/knight" + obj.passedColors.TeamOne + ".png")), Width = squareSize, Height = squareSize };
                        imagePiece.position = new Coordinate() { row = coord.row, col = coord.col, team = Team.White };
                        image.Tag = imagePiece;
                        Canvas.SetTop(image, coord.row * squareSize);
                        Canvas.SetLeft(image, coord.col * squareSize);
                        Canvas.SetZIndex(image, 1);
                        return image;
                    case Piece.Queen:
                        //Add White queen to canvas
                        image = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/queen" + obj.passedColors.TeamOne + ".png")), Width = squareSize, Height = squareSize };
                        imagePiece.position = new Coordinate() { row = coord.row, col = coord.col, team = Team.White };
                        image.Tag = imagePiece;
                        Canvas.SetTop(image, coord.row * squareSize);
                        Canvas.SetLeft(image, coord.col * squareSize);
                        Canvas.SetZIndex(image, 1);
                        return image;
                    case Piece.Rook:
                        //Add White Rook
                        image = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/rook" + obj.passedColors.TeamOne + ".png")), Width = squareSize, Height = squareSize };
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
                        image = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/pawn" + obj.passedColors.TeamTwo + ".png")), Width = squareSize, Height = squareSize };
                        imagePiece.position = new Coordinate() { row = coord.row, col = coord.col, team = Team.Black };

                        if (!imagePiece.hasMoved && !loadState)
                        {
                            imagePiece.hasMoved = true;
                        }

                        image.Tag = imagePiece;
                        Canvas.SetTop(image, coord.row * squareSize);
                        Canvas.SetLeft(image, coord.col * squareSize);
                        Canvas.SetZIndex(image, 1);
                        return image;
                    case Piece.Bishop:
                        //Add Black bishop to canvas
                        image = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/bishop" + obj.passedColors.TeamTwo + ".png")), Width = squareSize, Height = squareSize };
                        imagePiece.position = new Coordinate() { row = coord.row, col = coord.col, team = Team.Black };
                        image.Tag = imagePiece;
                        Canvas.SetTop(image, coord.row * squareSize);
                        Canvas.SetLeft(image, coord.col * squareSize);
                        Canvas.SetZIndex(image, 1);
                        return image;
                    case Piece.King:
                        //Add Black king to canvas
                        image = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/king" + obj.passedColors.TeamTwo + ".png")), Width = squareSize, Height = squareSize };
                        imagePiece.position = new Coordinate() { row = coord.row, col = coord.col, team = Team.Black };
                        image.Tag = imagePiece;
                        Canvas.SetTop(image, coord.row * squareSize);
                        Canvas.SetLeft(image, coord.col * squareSize);
                        Canvas.SetZIndex(image, 1);
                        return image;
                    case Piece.Knight:
                        //Add Black knight to canvas
                        image = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/knight" + obj.passedColors.TeamTwo + ".png")), Width = squareSize, Height = squareSize };
                        imagePiece.position = new Coordinate() { row = coord.row, col = coord.col, team = Team.Black };
                        image.Tag = imagePiece;
                        Canvas.SetTop(image, coord.row * squareSize);
                        Canvas.SetLeft(image, coord.col * squareSize);
                        Canvas.SetZIndex(image, 1);
                        return image;
                    case Piece.Queen:
                        //Add Black queen to canvas
                        image = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/queen" + obj.passedColors.TeamTwo + ".png")), Width = squareSize, Height = squareSize };
                        imagePiece.position = new Coordinate() { row = coord.row, col = coord.col, team = Team.Black };
                        image.Tag = imagePiece;
                        Canvas.SetTop(image, coord.row * squareSize);
                        Canvas.SetLeft(image, coord.col * squareSize);
                        Canvas.SetZIndex(image, 1);
                        return image;
                    case Piece.Rook:
                        //Add Black Rook
                        image = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/rook" + obj.passedColors.TeamTwo + ".png")), Width = squareSize, Height = squareSize };
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
                pawn = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/pawn" + obj.passedColors.TeamTwo + ".png")), Width = squareSize, Height = squareSize };
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
                pawn = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/pawn" + obj.passedColors.TeamOne + ".png")), Width = squareSize, Height = squareSize };
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
                Image blackRook = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/rook" + obj.passedColors.TeamTwo + ".png")), Width = squareSize, Height = squareSize };
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
                Image whiteRook = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/rook" + obj.passedColors.TeamOne + ".png")), Width = squareSize, Height = squareSize };
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
                Image blackKnight = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/knight" + obj.passedColors.TeamTwo + ".png")), Width = squareSize, Height = squareSize };
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
                Image whiteKnight = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/knight" + obj.passedColors.TeamOne + ".png")), Width = squareSize, Height = squareSize };
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
                Image blackBishop = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/bishop" + obj.passedColors.TeamTwo + ".png")), Width = squareSize, Height = squareSize };
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
                Image whiteBishop = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/bishop" + obj.passedColors.TeamOne + ".png")), Width = squareSize, Height = squareSize };
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
            Image blackQueen = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/queen" + obj.passedColors.TeamTwo + ".png")), Width = squareSize, Height = squareSize };
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
            Image whiteQueen = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/queen" + obj.passedColors.TeamOne + ".png")), Width = squareSize, Height = squareSize };
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
            Image blackKing = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/king" + obj.passedColors.TeamTwo + ".png")), Width = squareSize, Height = squareSize };
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
            Image whiteKing = new Image { Source = new BitmapImage(new Uri("ms-appx:///Images/king" + obj.passedColors.TeamOne + ".png")), Width = squareSize, Height = squareSize };
            imagePiece.position = new Coordinate() { row = 7, col = 4, team = Team.White };
            whiteKing.Tag = imagePiece;
            Canvas.SetTop(whiteKing, 7 * squareSize);
            Canvas.SetLeft(whiteKing, 4 * squareSize);
            Canvas.SetZIndex(whiteKing, 1);
            canvasBoard.Children.Add(whiteKing);
        }

        private void page_Loaded(object sender, RoutedEventArgs e)
        {
            CanvasSetUp();
            CapturedCanvasSetUp();

            if (loadedFromSaveState)
            {
                grid.StartIfSaveData();
                LoadSavedPieces(savedPiecesString);
            }
            else
            {
                LoadPieceImages();
                grid.Start();
            }
        }
    }
}