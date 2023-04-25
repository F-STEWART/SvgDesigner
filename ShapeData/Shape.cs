using ExCSS;
using Svg;
using System.Drawing;
using System.Numerics;
using Color = System.Drawing.Color;

namespace ShapeData
{
    public enum ShapeType 
    {
        Ellipse, Rectangle, Triangle
    }
    public class Shape
    {
        public int[] Position;
        public int Width;
        public int Height;
        public Color StrokeColor;
        public int StrokeThickness;
        public Color FillColor;
        public bool Fill;
        public ShapeType Type;

        public double CenterX => Position[0] + Width / 2;
        public double CenterY => Position[1] + Height / 2;
        public double Radius => Math.Min(Width, Height) / 2;

        public Shape(
            int[] Position,
            int Width,
            int Height,
            Color StrokeColor,
            int StrokeThickness,
            Color FillColor,
            bool Fill,
            ShapeType Type)
        {
            this.Position = Position;
            this.Width = Width;
            this.Height = Height;
            this.StrokeColor = StrokeColor;
            this.StrokeThickness = StrokeThickness;
            this.FillColor = FillColor;
            this.Fill = Fill;
            this.Type = Type;
        }

        public Shape()
        {
        }

        public SvgElement? ToSvg()
        {
            switch (this.Type)
            {
                case ShapeType.Ellipse:
                    return new SvgEllipse()
                    {
                        CenterX = (float)this.CenterX,
                        CenterY = (float)this.CenterY,
                        RadiusX = (float)this.Width / 2,
                        RadiusY = (float)this.Height / 2,
                        Stroke = new SvgColourServer(Color.FromArgb(this.StrokeColor.R, this.StrokeColor.G, this.StrokeColor.B)),
                        StrokeWidth = (float)this.StrokeThickness,
                        Fill = this.Fill ? new SvgColourServer(Color.FromArgb(this.FillColor.R, this.FillColor.G, this.FillColor.B)) : SvgPaintServer.None,
                    };
                case ShapeType.Rectangle:
                    return new SvgRectangle()
                    {
                        X = (float)this.Position[0],
                        Y = (float)this.Position[1],
                        Width = (float)this.Width,
                        Height = (float)this.Height,
                        Stroke = new SvgColourServer(Color.FromArgb(this.StrokeColor.R, this.StrokeColor.G, this.StrokeColor.B)),
                        StrokeWidth = (float)this.StrokeThickness,
                        Fill = this.Fill ? new SvgColourServer(Color.FromArgb(this.FillColor.R, this.FillColor.G, this.FillColor.B)) : SvgPaintServer.None,
                    };
                case ShapeType.Triangle:
                    var collection = new SvgPointCollection();
                    collection.Add(new SvgUnit(this.Position[0]));
                    collection.Add(new SvgUnit(this.Position[1] + this.Height));
                    collection.Add(new SvgUnit(this.Position[0] + this.Width / 2));
                    collection.Add(new SvgUnit(this.Position[1]));
                    collection.Add(new SvgUnit(this.Position[0] + this.Width));
                    collection.Add(new SvgUnit(this.Position[1] + this.Height));
                    return new SvgPolygon()
                    {
                        Points = collection,
                        Stroke = new SvgColourServer(Color.FromArgb(this.StrokeColor.R, this.StrokeColor.G, this.StrokeColor.B)),
                        StrokeWidth = (float)this.StrokeThickness,
                        Fill = this.Fill ? new SvgColourServer(Color.FromArgb(this.FillColor.R, this.FillColor.G, this.FillColor.B)) : SvgPaintServer.None,
                    };
            }
            return null;
        }
    }
}