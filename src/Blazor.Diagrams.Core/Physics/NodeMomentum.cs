using Blazor.Diagrams.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.Diagrams.Core.Physics
{
    public class NodeMomentum
    {
        public Point LastPosition { get; set; }
        public DateTime LastTime { get; set; }
        public Point Velocity { get; set; } = new(0, 0);
    }
}
