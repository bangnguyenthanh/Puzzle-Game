using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuzzleGame
{
   public class BoardManager
    {
        #region Properties
        /// <summary>
        /// this is the current picture
        /// </summary>
        private PictureBox BaseImage { get; set; }
        /// <summary>
        /// this is the main game board you play with
        /// </summary>
        private Panel MainBoard { get; set; }
        private Button SufferButton { get; set; }

        private Button UploadButton { get; set; }

        /// <summary>
        ///buttons in mainboard will show up into this matrix
        /// </summary>
        List<List<Button>> MainBoardMatrix;

        /// <summary>
        ///baseimage crop pieces will be added into this list
        /// </summary>
        List<Image> CropPieces;
        #endregion

        #region Initialize
        public BoardManager(PictureBox picBase, Panel pnBoard, Button suffer, Button upload)
        {
            BaseImage = picBase;
            MainBoard = pnBoard;
            SufferButton = suffer;
            UploadButton = upload;
            MainBoardMatrix = new List<List<Button>>();
            CropPieces = new List<Image>();
            SufferButton.Click += SufferButton_Click;
            UploadButton.Click += UploadButton_Click;
        }

        /// <summary>
        /// Upload a local image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UploadButton_Click(object sender, EventArgs e)
        {
            String imageLocation = "";
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "All Files(*.*)|*.*| JPEG files(*.jpg)|*.jgp| PNG files(*.png)|*.png";
                                
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    imageLocation = dialog.FileName;
                    BaseImage.Image = Image.FromFile(imageLocation);

                    DrawBoard();
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Error while upload image");
            }
        }
        #endregion

        #region Methods
        private void SufferButton_Click(object sender, EventArgs e)
        {
            SufferAndInsertImageToMainBoardMatrix();
        }
        /// <summary>
        /// Draw the MainBoard of this game,
        /// and crop BaseImage (into pieces),
        /// suffer those pieces , insert them to mainboardmatrix
        /// </summary>
        public void DrawBoard()
        {
            //if upload a file you have to clear the current involve things
            MainBoard.Controls.Clear();
            MainBoardMatrix.Clear();
            CropPieces.Clear();

            //use this button as a template
            Button oldButton = new Button()
            {
                Width = 0,
                Location = new Point(0, 0)
            };            

            //inserting into MainBoard and Matrix
            for (int i = 0; i < Cons.PIECES_WANT_TO_CUT; i++)
            {
                //the i_th row
                MainBoardMatrix.Add(new List<Button>());
                for (int j = 0; j < Cons.PIECES_WANT_TO_CUT; j++)
                {
                    //Add this button to Board
                    Button btn = new Button()
                    {
                        Width = Cons.MAIN_BOARD_BUTTON_WIDTH,
                        Height = Cons.MAIN_BOARD_BUTTON_HEIGHT,
                        BackColor = Color.White,
                        Location = new Point(
                            oldButton.Location.X + oldButton.Width,
                            oldButton.Location.Y
                            ),
                        BackgroundImageLayout = ImageLayout.Stretch,
                        Tag=i+";"+j
                    };

                    btn.Click += Btn_Click;
                    MainBoard.Controls.Add(btn);
                    MainBoardMatrix[i].Add(btn);
                    
                    //just copy the location and width
                    //if oldButton = btn
                    //when set oldButton you also set btn to the wrong place (if I'm right)
                    oldButton.Location = btn.Location;
                    oldButton.Width = btn.Width;
                }

                //set oldbutton to next line
                oldButton.Width = 0;
                oldButton.Location = new Point(
                    0,
                    oldButton.Location.Y + Cons.MAIN_BOARD_BUTTON_HEIGHT
                    );
            }

            CropBaseImage();
            SufferAndInsertImageToMainBoardMatrix();
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            Point point = GetPointLocation(btn);
            //if this is an empty piece
            if (btn.BackgroundImage == null) return;
            //if not an empty, check around and swap
            CheckLeft(point);
            CheckRight(point);
            CheckUp(point);
            CheckBottom(point);

            //check win
            if (isEndGame())
            {
                EndGame();
            }
        }

        private void EndGame()
        {
           DialogResult ret =  MessageBox.Show(
                "You Win! Play Again?",
                "WON",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
                );
            if (ret == DialogResult.Yes)
                SufferAndInsertImageToMainBoardMatrix();
            else
                System.Windows.Forms.Application.Exit();
        }

        private bool isEndGame()
        {
            //MainBoard is shown up by pieces(buttons), each piece have its own image,
            //each image contains a Tag = [location] which is its location in the 'won' case,
            // (loaction : from 0 to (PIECES_WANT_TO_CUT) ^ 2),
            //so we simply check if each image is in the right place
            //ofcourse that the first piece is null (if right), so we start at 1
            int checkLocation = 1;

            //if the first isn't null -> doesn't end
            if (MainBoardMatrix[0][0].BackgroundImage != null) return false;

            //else
            for(int i = 0; i < Cons.PIECES_WANT_TO_CUT; i++)
            {
                //set j = (i==0?1:0) means if i == 0 : we checked this before, start at 1 instead
                //we just check if both i!=0 and j!=0
                for (int j = i==0?1:0 ; j < Cons.PIECES_WANT_TO_CUT; j++)
                {
                    //get the Tag which contains the location in the 'won' case
                    string s = (string) MainBoardMatrix[i][j].BackgroundImage.Tag;

                    //Is the piece in the right place?
                    if (checkLocation == int.Parse(s))//yes
                    {
                        checkLocation++;
                    }
                    else//no
                    {
                        return false;
                    }
                }
            }
            return true; ;
        }

        private void CheckLeft(Point point)
        {
            if (point.X > 0)
            {
                if (MainBoardMatrix[point.X - 1][point.Y].BackgroundImage == null)
                {
                    SwapToPiece(MainBoardMatrix[point.X - 1][point.Y], MainBoardMatrix[point.X][point.Y]);
                }
            }
        }

        private void CheckRight(Point point)
        {
            if(point.X < Cons.PIECES_WANT_TO_CUT-1)
            {
                if (MainBoardMatrix[point.X + 1][point.Y].BackgroundImage == null)
                {
                    SwapToPiece(MainBoardMatrix[point.X + 1][point.Y], MainBoardMatrix[point.X][point.Y]);
                }
            }
        }

        private void CheckUp(Point point)
        {
            if (point.Y > 0)
            {
                if (MainBoardMatrix[point.X][point.Y - 1].BackgroundImage == null)
                {
                    SwapToPiece(MainBoardMatrix[point.X][ point.Y-1], MainBoardMatrix[point.X][point.Y]);
                }
            }
        }

        private void CheckBottom(Point point)
        {
            if (point.Y < Cons.PIECES_WANT_TO_CUT-1)
            {
                if (MainBoardMatrix[point.X][point.Y+1].BackgroundImage == null)
                {
                    SwapToPiece(MainBoardMatrix[point.X][point.Y+1], MainBoardMatrix[point.X][point.Y]);
                }
            }
        }
        /// <summary>
        /// Swap image of 2 button,
        /// the button1 doesn't have image (image = null)
        /// </summary>
        /// <param name="button1"></param>
        /// <param name="button2"></param>
        private void SwapToPiece(Button button1, Button button2)
        {            
            button1.BackgroundImage = button2.BackgroundImage;
            button2.BackgroundImage = null;
        }
        private Point GetPointLocation(Button btn)
        {
            string[] s = btn.Tag.ToString().Split(';').ToArray();
            int x = int.Parse(s[0]);
            int y = int.Parse(s[1]);
            return new Point(x, y);
        }

        private void SufferAndInsertImageToMainBoardMatrix()
        {
            //this game have an none-image pieces so
            //set the very first piece of picture is null (none-image)
            CropPieces[0] = null;
            //suffer croppieces (into this list) before insert to MainBoardMatrix
            var sufferCropPieces = CropPieces.OrderBy(a => Guid.NewGuid()).ToList();
            
            
            //insert the suffered to MainBoardMatrix
            for(int i = 0; i < Cons.PIECES_WANT_TO_CUT; i++)
            {
                for(int j = 0; j < Cons.PIECES_WANT_TO_CUT; j++)
                {
                    MainBoardMatrix[i][j].BackgroundImage = sufferCropPieces[i*Cons.PIECES_WANT_TO_CUT+j];
                }
            }
        }

        /// <summary>
        /// Crop the BaseImage into pieces, save them in CropPieces list
        /// </summary>
        private void CropBaseImage()
        {
            //original image
            Bitmap originalImage = new Bitmap(BaseImage.Image);

            //crop area
            Rectangle rect;

            //piece after crop
            Bitmap piece;

            //set this to crop BaseImage into (PIECES_WANT_TO_CUT ^ 2) pieces
            Cons.PICTURE_CROP_PIECES_WIDTH = originalImage.Width / Cons.PIECES_WANT_TO_CUT;
            Cons.PICTURE_CROP_PIECES_HEIGHT = originalImage.Height / Cons.PIECES_WANT_TO_CUT;

            //use as a template for the location
            Button oldCrop = new Button()
            {
                Width = 0,
                Location = new Point(0, 0)
            };

            //cropping
            for (int i = 0; i < Cons.PIECES_WANT_TO_CUT; i++)
            {
                 for(int j = 0; j < Cons.PIECES_WANT_TO_CUT; j++)
                {
                    //button, use for setting piece details
                    Button btnCrop = new Button()
                    {
                        Width = Cons.PICTURE_CROP_PIECES_WIDTH,
                        Height = Cons.PICTURE_CROP_PIECES_HEIGHT,
                        Location = new Point(
                            oldCrop.Location.X + oldCrop.Width,
                            oldCrop.Location.Y
                            )                       
                    };

                    //Cropping baseimage                    
                     rect = new Rectangle(
                       btnCrop.Location.X,
                       btnCrop.Location.Y,
                      Cons.PICTURE_CROP_PIECES_WIDTH,
                      Cons.PICTURE_CROP_PIECES_HEIGHT
                      );
                    piece = originalImage.Clone(rect, originalImage.PixelFormat);

                    //set Tag = 'location' which is the piece located in the 'won' case  (Is it optimal?)
                    piece.Tag = i * Cons.PIECES_WANT_TO_CUT + j + "";
                   
                    //add this piece to CropPieces
                    CropPieces.Add(piece);
                    
                    //new location of oldCrop
                    oldCrop.Location = btnCrop.Location;
                    oldCrop.Width = btnCrop.Width;
                }

                 //the next line of the cutting process
                oldCrop.Location = new Point(
                    0,
                    oldCrop.Location.Y+Cons.PICTURE_CROP_PIECES_HEIGHT
                    );
                oldCrop.Width = 0;
            }            
        }
        #endregion
    }
}
