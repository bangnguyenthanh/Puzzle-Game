using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleGame
{
    public class Cons
    {
        /// <summary>
        /// MainBoard'll show up by buttons,
        /// this is width of those buttons
        /// </summary>
        public static int MAIN_BOARD_BUTTON_WIDTH = 75;

        /// <summary>
        /// MainBoard'll show up by buttons,
        /// this is height of those buttons
        ///</summary>
        public static int MAIN_BOARD_BUTTON_HEIGHT = 75;

        /// <summary>
        /// BaseImage will be cut in to pieces,
        /// this is width of those pieces
        /// </summary>
        public static int PICTURE_CROP_PIECES_WIDTH = 0;

        /// <summary>
        /// BaseImage will be cut into pieces
        /// this is height of those pieces
        /// </summary>
        public static int PICTURE_CROP_PIECES_HEIGHT = 0;

        /// <summary>
        /// the piece in a row you want to crop,
        /// if value = x,
        /// you'll crop baseImage into x*x pieces
        /// </summary>
        public static int PIECES_WANT_TO_CUT = 3;
    }
}
