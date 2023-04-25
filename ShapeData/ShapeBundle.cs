using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeData
{
    public class ShapeBundle
    {
        public List<Shape> Shapes { get; set; }

        public ShapeBundle(List<Shape> Shapes) 
        {
            this.Shapes = Shapes;
        }
    }
}
