﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuzzleGame
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //CropBaseImage();
            BoardManager Program = new BoardManager(picBase,pnBoard,btnSuffer,btnUpload);
            Program.DrawBoard();
        }        
    }
}
