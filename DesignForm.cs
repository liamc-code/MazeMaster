/* DesignForm.cs
* MazeMaster
* Revision History
* Liam Conn, 2024.10.28: Created
* Liam Conn, 2020.10.28: Added code
* Liam Conn, 2024.11.02: Added Code
* Liam Conn, 2024.11.03: Debugging complete
* Liam Conn, 2020.01.03: Comments added
*
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MazeMaster
{
    /// <summary>
    /// Parent Class of DesignForm handling all design elements
    /// </summary>
    public partial class DesignForm : Form
    {
        // Declaring class variables for design setup
        private int rows, cols;
        private int currentTool = 0;
        private ImageList imgTools;

        /// <summary>
        /// Default contructor of DesignForm class.
        /// Used for initialization Component and Toolbox entities
        /// </summary>
        public DesignForm()
        {
            InitializeComponent();
            InitializeToolbox();
        }

        /// <summary>
        /// Initializes toolbox controls with images and sets up event handlers for tool selection.
        /// </summary>
        private void InitializeToolbox()
        {
            // Adds all img resources to an ImageList
            // Used to represent toolbox controls and to place in grid cells
            imgTools = new ImageList();
            imgTools.Images.Add(Properties.Resources.empty);
            imgTools.Images.Add(Properties.Resources.wall);
            imgTools.Images.Add(Properties.Resources.reddoor);
            imgTools.Images.Add(Properties.Resources.greendoor);
            imgTools.Images.Add(Properties.Resources.redbox);
            imgTools.Images.Add(Properties.Resources.greenbox);
            
            // Assign ImageList to each respective toolbox btn control
            btnEmpty.Image = imgTools.Images[0];
            btnWall.Image = imgTools.Images[1];
            btnRedDoor.Image = imgTools.Images[2];
            btnGreenDoor.Image = imgTools.Images[3];
            btnRedBox.Image = imgTools.Images[4];
            btnGreenBox.Image = imgTools.Images[5];

            // Assign click events to each button, specify the num to represent each tool
            btnEmpty.Click += (s, e) => currentTool = 0;
            btnWall.Click += (s, e) => currentTool = 1;
            btnRedDoor.Click += (s, e) => currentTool = 2;
            btnGreenDoor.Click += (s, e) => currentTool = 3;
            btnRedBox.Click += (s, e) => currentTool = 4;
            btnGreenBox.Click += (s, e) => currentTool = 5;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            // validate rows/columns txtboxes for positive whole number, assign to rows/cols vars
            if(int.TryParse(txtRows.Text, out rows) && int.TryParse(txtCols.Text, out cols)
                && rows > 0 && cols > 0)
            {
                // Grid already exits, thus having a positive count
                if(dgvLevel.Rows.Count > 0 || dgvLevel.Columns.Count > 0)
                {
                    // confirm abadoning current level to create anew
                    DialogResult result = MessageBox.Show("Abandon current level and create a new one?", "MazeMaster Confirmation",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    // cancel generate
                    if (result == DialogResult.No)
                        return;
                }
                CreateGrid();
            } else
            {
                // invalid input
                MessageBox.Show("Invalid input for row or columns. Please provide a whole positive number for both.", 
                    "MazeMaster");
            }
        }

        /// <summary>
        /// Generate a new grid using DataGridView, references class vars rows and cols for creation
        /// Each cell in grid is initialized with the empty img as default
        /// Any previous grid cells are cleared for all rows and columns
        /// </summary>
        private void CreateGrid()
        {
            // clears previous row/column cells
            dgvLevel.Rows.Clear();
            dgvLevel.Columns.Clear();

            // DataGridView column setup, apply default img
            for(int c = 0; c < cols; c++)
            {
                DataGridViewImageColumn column = new DataGridViewImageColumn
                
                {
                    // format imgs for grid cells to fill cell
                    ImageLayout = DataGridViewImageCellLayout.Zoom
                    
                };
                
                // set default cell img
                column.DefaultCellStyle.NullValue = imgTools.Images[0];
                dgvLevel.Columns.Add((DataGridViewColumn)column);
                // column cell width
                dgvLevel.Columns[c].Width = 64;
            }
            // row cell height
            dgvLevel.RowTemplate.Height = 64;

            // DataGridView row setup
            for(int r = 0; r < rows; r++)
            {
                dgvLevel.Rows.Add();
            }
            // add click event to each cell to assign tool
            dgvLevel.CellClick += dgvLevel_CellClick;
        }

        /// <summary>
        /// Handle cell click events from DataGridView (dgvLevel)
        /// Assign respective img, tool number to cell based on selected tool
        /// </summary>
        /// <param name="sender">event source</param>
        /// <param name="e">instance of cell containing event data (row/column index)</param>
        private void dgvLevel_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // check for non-empty grid
            if(e.RowIndex >= 0 &&  e.ColumnIndex >= 0)
            {
                // assign tool img and selected tool number to cell
                dgvLevel[e.ColumnIndex, e.RowIndex].Value = imgTools.Images[currentTool];
                dgvLevel[e.ColumnIndex, e.RowIndex].Tag = currentTool;
            }
        }

        private void tsmiSave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                // text file format
                sfd.Filter = "Text Files (*.txt)|*.txt";
                // confirm save file
                if(sfd.ShowDialog() == DialogResult.OK)
                {
                    SaveLevelToFile(sfd.FileName);
                }
            }
        }

        private void tsmiClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Save current grid layout to a txt file, include each cell (r value, c value), 
        /// selected tool (doors, boxes, walls, empty)
        /// layout also includes total row count, total column count
        /// </summary>
        /// <param name="fileName"></param>
        private void SaveLevelToFile(string fileName)
        {
            int cellState;
            Dictionary<string, int> typeCounts = new Dictionary<string, int>()
            {
                {"walls", 0},
                {"doors", 0},
                {"boxes", 0}
            };


            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine(rows);
                writer.WriteLine(cols);

                for(int r = 0; r < rows; r++)
                {
                    for(int c = 0;  c < cols; c++)
                    {
                        // validate tag value for valid int (used for selectedTool)
                        cellState = dgvLevel[c, r].Tag is int cellTag ? cellTag : 0;
                        writer.WriteLine($"{r},{c},{cellState}");

                        // assign count based on selectedTool type
                        if(cellState == 1)
                        {
                            typeCounts["walls"]++;
                        } else if (cellState == 2 || cellState == 3)
                        {
                            typeCounts["doors"]++;
                        } else if (cellState == 4 || cellState == 5)
                        {
                            typeCounts["boxes"]++;
                        }

                    }
                }
                writer.Close();
            }
            // success message, display entity stats
            MessageBox.Show($@"Level has been saved successfully. Generated the following:
                Total Walls: {typeCounts["walls"]},
                Total Doors {typeCounts["doors"]},
                Total Boxes: {typeCounts["boxes"]}", "MazeMaster", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
