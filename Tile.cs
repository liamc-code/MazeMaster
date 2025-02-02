/* Tile.cs
* MazeMaster
* Revision History
* Liam Conn, 2024.11.22: Created
* Liam Conn, 2020.11.24: Added code
* Liam Conn, 2024.11.27: Debugging complete
* Liam Conn, 2020.12.01: Comments added
*
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace MazeMaster
{
    public class Tile : PictureBox
    {
        public int Row { get; set; }
        public int Col { get; set; }
        // Tile's type (wall, box, door)
        public int TileState { get; set; }

        /// <summary>
        /// Initialize new instance of the Tile class
        /// </summary>
        /// <param name="row">row of tile in grid</param>
        /// <param name="col">col of tile in grid</param>
        /// <param name="tileState">type of tile</param>
        public Tile(int row, int col, int tileState)
        {
            Row = row;
            Col = col;
            TileState = tileState;
            BorderStyle = BorderStyle.FixedSingle;
            SizeMode = PictureBoxSizeMode.AutoSize;
            Image = GetImageForTile();
        }

        /// <summary>
        /// Get the correlating image for the tile based on state (type)
        /// </summary>
        /// <returns>image resource for tile's state</returns>
        private Image GetImageForTile()
        {
            Image tileImage;
            switch(TileState)
            {
                case 1:
                    tileImage = Properties.Resources.wall;
                    break;
                case 2:
                    tileImage = Properties.Resources.reddoor;
                    break;
                case 3:
                    tileImage = Properties.Resources.greendoor;
                    break;
                case 4:
                    tileImage = Properties.Resources.redbox;
                    break;
                case 5:
                    tileImage = Properties.Resources.greenbox;
                    break;
                default:
                    tileImage = Properties.Resources.empty;
                    break;
            }
            return tileImage;
        }
    }
}
