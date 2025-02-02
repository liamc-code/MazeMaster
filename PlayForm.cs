/* PlayForm.cs
* MazeMaster
* Revision History
* Liam Conn, 2024.11.22: Created
* Liam Conn, 2020.11.24: Added code
* Liam Conn, 2024.11.27: Added Code
* Liam Conn, 2024.11.29: Debugging complete
* Liam Conn, 2020.12.01: Comments added
*
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace MazeMaster
{
    public partial class PlayForm : Form
    {
        private Tile[,] tileGrid;
        private int rows, cols;
        private Tile selectedTile;
        private const int TILE_WIDTH = 64;
        private const int TILE_HEIGHT = 64;
        private int moves = 0;
        public PlayForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Load level from a text file and initialize the game grid
        /// </summary>
        private void LoadLevel()
        {
            // create open file dialog var
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text Files (*.txt)|*.txt";

            if (ofd.ShowDialog() == DialogResult.OK) // validate correct file received
            {
                try
                {
                    string[] fLines = File.ReadAllLines(ofd.FileName);

                    // Validate file structure
                    if (fLines.Length < 2)
                    {
                        MessageBox.Show("Invalid file format. File must contain grid dimensions and tile data.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Read grid dimensions
                    int.TryParse(fLines[0], out rows);
                    int.TryParse(fLines[1], out cols);

                    // Validate tile data length
                    if (fLines.Length < 2 + rows * cols)
                    {
                        MessageBox.Show("File format error: Insufficient tile data for specified rows and columns.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Initialize grid and clear panel
                    tileGrid = new Tile[rows, cols];
                    pnlTiles.Controls.Clear();

                    // width and height for each tile
                    int TILE_WIDTH = 64;
                    int TILE_HEIGHT = 64;

                    // Fill grid with file tiles
                    for (int fLineIndex = 2; fLineIndex < fLines.Length; fLineIndex++)
                    {
                        string[] tileProperties = fLines[fLineIndex].Split(',');

                        if (tileProperties.Length < 3) continue; // Skip invalid lines (validation)

                        int.TryParse(tileProperties[0], out int row);
                        int.TryParse(tileProperties[1], out int col);
                        int.TryParse(tileProperties[2], out int tileState);

                        // Non-empty tiles
                        if (tileState != 0)
                        {
                            Tile tile = new Tile(row, col, tileState);
                            tile.Left = 10 + col * TILE_WIDTH; // Position the tile
                            tile.Top = 10 + row * TILE_HEIGHT;
                            tile.Click += Tile_Click; // Attach click handler

                            // Add tile to grid and panel
                            tileGrid[row, col] = tile;
                            pnlTiles.Controls.Add(tile);
                        } else
                        {
                            // intialize empty tiles
                            Tile emptyTile = new Tile(row, col, 0);
                            emptyTile.Left = 10 + col * TILE_WIDTH;
                            emptyTile.Top = 10 + row * TILE_HEIGHT;
                            tileGrid[row, col] = emptyTile; // Store in grid
                            pnlTiles.Controls.Add(emptyTile); // Add to panel
                        }
                    }

                    // Initialize score details
                    moves = 0;
                    txtMoves.Text = moves.ToString();
                    txtBoxesLeft.Text = CountBoxes().ToString();
                }
                catch (Exception ex)
                {
                    //unable to read the level file properly
                    MessageBox.Show($"Error loading level: {ex.Message} Choose a valid level file (.txt)", 
                        "MazeMaster Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(@"Error loading level: No file chosen. Please choose a correct file (.txt) to load.",
                    "MazeMaster Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// When any non-empty tile in the grid is selected,
        /// check for box state and assign to selectedTile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tile_Click(object sender, EventArgs e)
        {
            // valid box tile
            if(sender is Tile tile && (tile.TileState == 4 || tile.TileState == 5))
            {
                // Reset border of previously selected tile
                if (selectedTile != null)
                {
                    selectedTile.Padding = new Padding(0); // Reset to normal
                    selectedTile.BackColor = Color.Transparent; // Reset background
                }
                selectedTile = tile;
                selectedTile.Padding = new Padding(1); // Add padding for a border-like effect
                selectedTile.BackColor = Color.Black; // Optional: Change color for emphasis
            }
            else
            {
                MessageBox.Show("Please select a box to move.", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Retrieves tile from grid based on position
        /// </summary>
        /// <param name="row">row of retrieved tile</param>
        /// <param name="col">col of retrieved tile</param>
        /// <returns>Tile object at specified position, or null if out of bounds</returns>
        private Tile getTile(int row, int col)
        {
            // bounds condition
            if (row >= 0 && row < rows && col >= 0 && col < cols)
            {
                Tile tile = tileGrid[row, col];
                return tile;
            }
            return null;
        }

        /// <summary>
        /// Move selected box to the specified direction if possible
        /// </summary>
        /// <param name="rowDiff">difference in rows for movement</param>
        /// <param name="colDiff">difference in columns for movement</param>
        private void MoveBox(int rowDiff, int colDiff)
        {
            // invalid tile/no tile selected to move
            if (selectedTile == null)
            {
                MessageBox.Show("Please select a box to move.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int currentRow = selectedTile.Row;
            int currentCol = selectedTile.Col;

            // track box movement
            bool boxMoved = false;

            // tile movement loop
            while (true)
            {
                int newRow = currentRow + rowDiff;
                int newCol = currentCol + colDiff;

                // Ensure the newRow and newCol are within bounds
                if (newRow < 0 || newRow >= rows || newCol < 0 || newCol >= cols)
                {
                    break;
                }

                Tile nextTile = getTile(newRow, newCol);

                if (nextTile == null || nextTile.TileState == 0) // Empty space
                {
                    // Replace the old tile with an empty tile
                    Tile emptyTile = CreateEmptyTile(currentRow, currentCol);
                    pnlTiles.Controls.Add(emptyTile); // Add empty tile to the UI
                    tileGrid[currentRow, currentCol] = emptyTile; // Update the grid

                    // Move the box to the new position
                    tileGrid[newRow, newCol] = selectedTile;
                    selectedTile.Row = newRow;
                    selectedTile.Col = newCol;
                    selectedTile.Left = 10 + newCol * TILE_WIDTH;
                    selectedTile.Top = 10 + newRow * TILE_HEIGHT;
                    selectedTile.BringToFront();

                    currentRow = newRow;
                    currentCol = newCol;

                    // Set flag that the box moved
                    boxMoved = true; 
                }
                else if (nextTile.TileState == 1 || nextTile.TileState == 4 || nextTile.TileState == 5) // Wall or another box
                {
                    break;
                }
                else if ((nextTile.TileState == 2 || nextTile.TileState == 3) && 
                    nextTile.TileState == selectedTile.TileState - 2) // box movement to door, removing box
                {
                    // count box before removing
                    moves++;
                    txtMoves.Text = moves.ToString();
                    boxMoved = false;

                    // Remove the box
                    pnlTiles.Controls.Remove(selectedTile);
                    tileGrid[currentRow, currentCol] = CreateEmptyTile(currentRow, currentCol); // Replace with empty tile

                    // Keep the door intact
                    tileGrid[newRow, newCol] = nextTile;

                    selectedTile = null;

                    // Update the remaining boxes count
                    txtBoxesLeft.Text = CountBoxes().ToString();

                    CheckGameWin();
                    return;
                }
                else
                {
                    break;
                }
            }

            if(boxMoved) // successful box movement
            {
                // update movement count
                moves++;
                txtMoves.Text = moves.ToString();
            }
        }

        /// <summary>
        /// Create empty tile at the specified position
        /// </summary>
        /// <param name="row">row of empty tile</param>
        /// <param name="col">col of empty tile</param>
        /// <returns>created tile object</returns>
        private Tile CreateEmptyTile(int row, int col)
        {
            Tile emptyTile = new Tile(row, col, 0)
            {
                Left = 10 + col * TILE_WIDTH,
                Top = 10 + row * TILE_HEIGHT
            };

            return emptyTile;
        }

        /// <summary>
        /// Count remaining boxes on grid
        /// </summary>
        /// <returns>Number of Remaining boxes as int</returns>
        private int CountBoxes()
        {
            int count = 0;
            foreach(Tile tile in tileGrid)
            {
                // count only tiles that are boxes
                if(tile != null && (tile.TileState == 4 || tile.TileState == 5))
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Check for game win (no boxes remaining)
        /// </summary>
        private void CheckGameWin()
        {
            if(CountBoxes() == 0)
            {
                // winning message
                MessageBox.Show($@"Congratulations! Game over!
                Moves: {moves}", "Game Over!", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
                // reset game
                RestartGame();
            }
        }

        /// <summary>
        /// Reset game to initial state for new level
        /// </summary>
        private void RestartGame()
        {
            pnlTiles.Controls.Clear();
            tileGrid = new Tile[rows, cols];
            moves = 0;
            txtMoves.Text = moves.ToString();
            txtBoxesLeft.Text = moves.ToString();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            MoveBox(-1, 0);
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            MoveBox(1, 0);
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            MoveBox(0, -1);
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            MoveBox(0, 1);
        }

        private void LoadMenuItem_Click(object sender, EventArgs e)
        {
            LoadLevel();
        }

        private void CloseMenuItem_Click(Object sender, EventArgs e)
        {
            Application.Exit();
        }  
    }
}
