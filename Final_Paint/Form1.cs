using Newtonsoft.Json;
using ShapeData;
using Svg;

namespace Final_Paint
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<Shape> Shapes = new List<Shape>();

        ShapeType DrawMode = ShapeType.Ellipse;
        Color StrokeColour = Color.Black;
        Color FillColour = Color.White;
        bool Fill = false;
        bool IsDrawing = false;
        DateTime CancelTime = DateTime.UtcNow;
        double CancelDelay = 0.5;

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Sets the Stroke colour
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void strokeColourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
                StrokeColour = colorDialog1.Color;
        }

        /// <summary>
        /// Sets the fill colour
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fillColourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog2.ShowDialog() == DialogResult.OK)
                FillColour = colorDialog2.Color;
        }

        /// <summary>
        /// Sets the type of shape to be drawn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (toolStripComboBox1.SelectedIndex)
            {
                case 0:
                    DrawMode = ShapeType.Ellipse;
                    break;
                case 1:
                    DrawMode = ShapeType.Rectangle;
                    break;
                case 2:
                    DrawMode = ShapeType.Triangle;
                    break;
            }
        }

        /// <summary>
        /// Clears all shapes in shapes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Shapes.Clear();
            pictureBox1.Invalidate();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undo();
        }

        /// <summary>
        /// Removes the last shape in the list
        /// </summary>
        private void undo()
        {
            if (Shapes.Count > 0)
            {
                Shapes.RemoveAt(Shapes.Count - 1);
            }
            pictureBox1.Invalidate();
        }

        /// <summary>
        /// Draws of objects in shapes to the picturebox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

            // Draws all shapes in list based of their type.
            // Original code taken from code example
            // Modified to add triangles
            foreach (var shape in Shapes)
            {
                using var pen = new Pen(shape.StrokeColor, shape.StrokeThickness);
                using var brush = new SolidBrush(shape.FillColor);
                switch (shape.Type)
                {
                    case ShapeType.Ellipse:
                        if (shape.Fill)
                        {
                            e.Graphics.FillEllipse(brush, shape.Position[0], shape.Position[1], shape.Width, shape.Height);
                        }

                        e.Graphics.DrawEllipse(pen, shape.Position[0], shape.Position[1], shape.Width, shape.Height);
                        break;
                    case ShapeType.Rectangle:
                        if (shape.Fill)
                        {
                            e.Graphics.FillRectangle(brush, Math.Min(shape.Position[0], shape.Position[0] + shape.Width), Math.Min(shape.Position[1], shape.Position[1] + shape.Height), Math.Abs(shape.Width), Math.Abs(shape.Height));
                        }

                        e.Graphics.DrawRectangle(pen, Math.Min(shape.Position[0], shape.Position[0] + shape.Width), Math.Min(shape.Position[1], shape.Position[1] + shape.Height), Math.Abs(shape.Width), Math.Abs(shape.Height));
                        break;
                    case ShapeType.Triangle:
                        if (shape.Fill)
                        {
                            e.Graphics.FillPolygon(brush, new Point[] {
                            new Point((int)(shape.Position[0]), (int)(shape.Position[1]+shape.Height)),
                            new Point((int)(shape.Position[0]+(shape.Width/2)), (int)shape.Position[1]),
                            new Point((int)(shape.Position[0]+shape.Width), (int)(shape.Position[1]+shape.Height))
                        });
                        }
                        e.Graphics.DrawPolygon(pen, new Point[] {
                            new Point((int)(shape.Position[0]), (int)(shape.Position[1]+shape.Height)),
                            new Point((int)(shape.Position[0]+(shape.Width/2)), (int)shape.Position[1]),
                            new Point((int)(shape.Position[0]+shape.Width), (int)(shape.Position[1]+shape.Height))
                        });
                        break;
                }
            }

        }

        /// <summary>
        /// Adds a shape to shapes when the user begins to drag one in the picturebox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            // Adds a shape
            // Dimensions are changed in mouse move event
            if ((Control.MouseButtons & MouseButtons.Left) != 0)
            {
                IsDrawing = true;
                var point = pictureBox1.PointToScreen(new Point(0, 0));
                var x = MousePosition.X - point.X;
                var y = MousePosition.Y - point.Y;
                Shapes.Add(new Shape()
                {
                    Position = new int[] { x, y },
                    Width = 0,
                    Height = 0,
                    StrokeColor = StrokeColour,
                    StrokeThickness = toolStripComboBox2.SelectedIndex + 1,
                    FillColor = FillColour,
                    Fill = Fill,
                    Type = DrawMode
                });
                pictureBox1.Invalidate();
            }
        }

        /// <summary>
        /// Toggles whether the shape being drawn is filled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Fill = fillToolStripMenuItem.Checked;
        }

        /// <summary>
        /// Updates the size of a shape as the user moves their mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // These if statements are messy

            // The conditions are:
            // Left mouse button down
            // Been at lease [CancelDelay] since a cancel (maybe no longer necessary)
            // At least one shape
            // IsDrawing
            if ((Control.MouseButtons & MouseButtons.Left) != 0 && ((DateTime.UtcNow - CancelTime).TotalSeconds > CancelDelay) && Shapes.Count > 0 && IsDrawing)
            {
                var point = pictureBox1.PointToScreen(new Point(0, 0));
                var x = MousePosition.X - point.X;
                var y = MousePosition.Y - point.Y;
                Shapes.Last().Width = x - Shapes.Last().Position[0];
                Shapes.Last().Height = y - Shapes.Last().Position[1];
            }

            // The conditions are:
            // Right mouse button down
            // Been at lease [CancelDelay] since a cancel (maybe no longer necessary)
            // IsDrawing
            if ((Control.MouseButtons & MouseButtons.Right) != 0 && ((DateTime.UtcNow - CancelTime).TotalSeconds > CancelDelay) && IsDrawing)
            {
                undo();
                undo();
                CancelTime = DateTime.UtcNow;
                IsDrawing = false;
            }
            pictureBox1.Invalidate();
        }

        /// <summary>
        /// Updates "drawing" status when the user releases their mouse button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            IsDrawing = false;
            pictureBox1.Invalidate();
        }

        /// <summary>
        /// Creates an svg object for each shape in the list then writes them to a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportSVGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SvgDocument ExportDoc = new SvgDocument();
            foreach (Shape obj in Shapes)
            {
                ExportDoc.Children.Add(obj.ToSvg());
            }
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                ExportDoc.Write(saveFileDialog1.FileName);
        }

        /// <summary>
        /// Bundles the shapes and serialises the bundle with json. Saves to a file.
        /// </summary>
        private void save()
        {
            string json = JsonConvert.SerializeObject(new ShapeBundle(Shapes));
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                File.WriteAllText(saveFileDialog1.FileName, json);
        }

        /// <summary>
        /// Loads saved json shape data
        /// </summary>
        private void load()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Shapes = JsonConvert.DeserializeObject<ShapeBundle>(File.ReadAllText(openFileDialog1.FileName)).Shapes;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            load();
        }
    }
}