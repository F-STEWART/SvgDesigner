using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeData
{
    /// <summary>
    /// Bundles a list of shapes into one object for saving as json
    /// </summary>
    public class ShapeBundle
    {
        public List<Shape> Shapes { get; set; }

        public ShapeBundle(List<Shape> Shapes) 
        {
            this.Shapes = Shapes;
        }
    }
}
