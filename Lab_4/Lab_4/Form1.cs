// Eight Queens Puzzle, by Omar Janoudi 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Drawing.Drawing2D;

namespace Lab_4
{
    public partial class Form1 : Form
    {
        //This variable contains all of the positions of the board
        public BoardPosition[,] board = new BoardPosition[8, 8];

        //This variable counts the number of queens added thus far, another to store current indices of queens
        public List<Point> Queens = new List<Point>();
        public int queenCount; //need this for the exception when Queens.Count = NULL

        //Boolean value to determine if hints are enabled, used by paint event
        int hintsEnabled;

        //Setup for initializing board positions and properties
        public void boardSetup()
        {
            queenCount = 0;
            //Initialize the hints value to false
            hintsEnabled = 0;
            //This loop initializes the board
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    //Positions and draws based on index
                    board[i, j] = new BoardPosition((100 + (i * 50)), (100 + (j * 50)), 50, 50);
                    //One black and white pattern if row is even
                    if (i % 2 == 0)
                    {
                        if (j % 2 == 0)
                        {
                            board[i, j].isBlack = 0;
                        }
                        else
                        {
                            board[i, j].isBlack = 1;
                        }
                    }
                    //Other black and white pattern if row is odd
                    else
                    {
                        if (j % 2 == 0)
                        {
                            board[i, j].isBlack = 1;
                        }
                        else
                        {
                            board[i, j].isBlack = 0;
                        }
                    }
                }
            }
        }

        //Invoked by the click method every time a queen is added in order to remap the unclickable positions
        public void resetRed()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j].unClickable = 0;
                    board[i, j].isClicked = 0;
                }
            }
        }

        //Invoked to decide which positions should now be colored red
        public void makeRed()
        {
            for (int i = 0; i < Queens.Count; i++)
            {
                int x = Queens[i].X; int y = Queens[i].Y;
                for (int z = 0; z < 8; z++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        //If on same position of queen, make unsafe/clickable and turn red later, also set isClicked to 1
                        if(x == board[z,j].rect.X && y == board[z,j].rect.Y) { board[z, j].unClickable = 1; board[z, j].isClicked = 1; }
                        //Also if on same row or column
                        if(x == board[z,j].rect.X || y == board[z, j].rect.Y) { board[z, j].unClickable = 1; }
                        //Also if on one of the diagonals, in which case delta x coordinate and delta y coordinateb between queen position and 
                        //the position that is currently being checked are the same (Math.abs is used to avoid neg/pos values)
                        if(Math.Abs(x - board[z,j].rect.X) == Math.Abs(y - board[z, j].rect.Y)) { board[z, j].unClickable = 1; }

                    }
                }
            }
        }

        //Invoked when the clear button is pressed
        public void clearQueens()
        {
            Queens = new List<Point>();
            queenCount = 0;
            //Reset all "isClicked"'s
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j].isClicked == 1)
                    {
                        board[i, j].isClicked = 0;
                    }
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
            boardSetup();
            this.Size = new System.Drawing.Size(635, 635); //Calculated this so that the board is exactly in the center
            this.Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //First, refresh the clicked states based on the current number of queens
            resetRed();
            makeRed();
            Graphics g = e.Graphics;
            Pen p = new Pen(Color.Black, 1);
            //Used for the centered Q
            StringFormat fmt = new StringFormat();
            fmt.Alignment = StringAlignment.Center;
            fmt.LineAlignment = StringAlignment.Center;
            //Update the text for the number of queens
            string s = "You have " + queenCount + " queens on the board.";
            g.DrawString(s, Font, Brushes.Black, 205, 25);
            //Draw the frame of the entire board
            g.DrawRectangle(p, 100, 100, 400, 400);
            //In case of unchecked Hints box
            if (hintsEnabled == 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (board[i, j].isBlack == 1)
                        {
                            //First, fill the rectangle
                            g.FillRectangle(Brushes.Black, board[i, j].rect);
                            //Second, if it has a queen in it, draw the centered Q
                            if (board[i, j].isClicked == 1)
                            {
                                Font font = new Font("Ariel", 30, FontStyle.Bold);
                                g.DrawString("Q", font, Brushes.White, board[i, j].rect, fmt);
                                font.Dispose();
                            }
                            //Last, draw the border of each of the ractangle (on top of everything)
                            p.Alignment = PenAlignment.Inset;
                            g.DrawRectangle(p, board[i, j].rect);
                        }
                        if (board[i, j].isBlack == 0)
                        {
                            //First,, fill the rectangle 
                            g.FillRectangle(Brushes.White, board[i, j].rect);
                            //Second, if it has a queen in it, draw the centered Q
                            if (board[i, j].isClicked == 1)
                            {
                                Font font = new Font("Ariel", 30, FontStyle.Bold);
                                g.DrawString("Q", font, Brushes.Black, board[i, j].rect, fmt);
                                font.Dispose();
                            }
                            //Last, draw the border of each of the ractangle (on top of everything)
                            p.Alignment = PenAlignment.Inset;
                            g.DrawRectangle(p, board[i, j].rect);
                        }
                    }
                }
            }
            //In case of checked box so that reds show
            else
            {
                //Then, do the same as before, this time painting the "unclickables" red
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (board[i, j].isBlack == 1)
                        {
                            //First, fill the rectangle, if unclickable or isclicked, then paint red
                            if (board[i, j].isClicked == 1 || board[i, j].unClickable == 1) { g.FillRectangle(Brushes.Red, board[i, j].rect); }
                            else { g.FillRectangle(Brushes.Black, board[i, j].rect); }
                            //Second, if it has a queen in it, draw the centered Q
                            if (board[i, j].isClicked == 1)
                            {
                                //This time paint it black due to the red background
                                Font font = new Font("Ariel", 30, FontStyle.Bold);
                                g.DrawString("Q", font, Brushes.Black, board[i, j].rect, fmt);
                                font.Dispose();
                            }
                            //Last, draw the border of each of the ractangle (on top of everything)
                            p.Alignment = PenAlignment.Inset;
                            g.DrawRectangle(p, board[i, j].rect);

                        }
                        if (board[i, j].isBlack == 0)
                        {
                            //First, fill the rectangle, if unclickable or isclicked, then paint red
                            if (board[i, j].isClicked == 1 || board[i, j].unClickable == 1) { g.FillRectangle(Brushes.Red, board[i, j].rect); }
                            else { g.FillRectangle(Brushes.White, board[i, j].rect); }
                            //Second, if it has a queen in it, draw the centered Q
                            if (board[i, j].isClicked == 1)
                            {
                                Font font = new Font("Ariel", 30, FontStyle.Bold);
                                g.DrawString("Q", font, Brushes.Black, board[i, j].rect, fmt);
                                font.Dispose();
                            }
                            //Last, draw the border of each of the ractangle (on top of everything)
                            p.Alignment = PenAlignment.Inset;
                            g.DrawRectangle(p, board[i, j].rect);
                        }
                    }
                }
            }
            if(queenCount == 8) {MessageBox.Show("You have placed 8 queens. You won! Game has been reset. ", "Congratulations!");
                clearQueens(); //reset the game if you won
                this.Invalidate();
                
            }
        }

        public partial class BoardPosition
        {
            public Rectangle rect;
            public int isClicked;
            public int unClickable; //Used to determine if you can place a queen here or if its red, reset upon every paint event
            public int isBlack; //determines if the spot should be shaded black, based on position
            public BoardPosition(int X, int Y, int Width, int Height)
            {
                this.rect = new Rectangle(X, Y, Width, Height);
                isClicked = 0; //False as default (decides if it should be highlighted red when hints are checked or if a Q should be printed)
            }
        }

        private void Hints_CheckedChanged(object sender, EventArgs e)
        {
            //Determine Hints state to change logical value and invoke a Paint event
            if (Hints.Checked) { hintsEnabled = 1; this.Invalidate(); }
            if (!Hints.Checked) { hintsEnabled = 0; this.Invalidate(); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clearQueens();
            this.Invalidate();
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            //Grab mouse click coordinates
            int x = e.X; int y = e.Y;
            //If left click, add a queen if it's allowed
            if (e.Button == MouseButtons.Left)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        int minX = board[i, j].rect.X;
                        int minY = board[i, j].rect.Y;
                        int maxX = board[i, j].rect.X + 50;
                        int maxY = board[i, j].rect.Y + 50;
                        //If the click was within range of a position on the board
                        if (x >=minX && x<=maxX && y >=minY && y <= maxY)
                        {
                            if(board[i,j].isClicked == 0 && board[i,j].unClickable != 1)
                            {
                                //Set the button to clicked and add the queen
                                board[i, j].isClicked = 1;
                                Point point = new Point(board[i, j].rect.X, board[i, j].rect.Y);
                                Queens.Add(point);
                                queenCount++;
                                this.Invalidate();
                            }
                            else if(board[i,j].isClicked == 1)
                            {
                                //Play a beep because you cannot add a queen
                                System.Media.SystemSounds.Beep.Play();
                            }
                            if(board[i,j].unClickable == 1)
                            {
                                //If this position is colored red (when hints are on) because it is in the path of the queen
                                //Then play a beep and do nothing
                                System.Media.SystemSounds.Beep.Play();
                            }
                        }
                    }
                }
            }
                //If right click, remove a queen if it's there
                if (e.Button == MouseButtons.Right)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                        int minX = board[i, j].rect.X;
                        int minY = board[i, j].rect.Y;
                        int maxX = board[i, j].rect.X + 50;
                        int maxY = board[i, j].rect.Y + 50;
                        //If the click was within range of a position on the board
                        if (x >= minX && x <= maxX && y >= minY && y <= maxY)
                        {
                            if(board[i,j].isClicked == 1)
                            {
                                //Define the coordinates of the point for removal and remove the Queen
                                Point point = new Point(board[i, j].rect.X, board[i, j].rect.Y);
                                Queens.Remove(point);
                                queenCount--;
                                //Set the isClicked property here to 0
                                board[i, j].isClicked = 0;
                                this.Invalidate();
                            }
                        }
                    }
                    }
                }
            }
        }
    }