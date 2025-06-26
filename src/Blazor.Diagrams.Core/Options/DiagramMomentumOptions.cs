using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.Diagrams.Core.Options
{
    public class DiagramMomentumOptions
    {
        public bool Enabled { get; set; } = false;
        public double friction { get; set; } = 0.80;
        public double threshold { get; set; } = 5.0; // velocidade mínima para continuar
        public int interval { get; set; } = 16; // intervalo de atualização em milissegundos (aproximadamente 60fps)

        //public bool OnNodes { get; set; } = true;
        //public bool OnGroups { get; set; }
        //public bool OnLinks { get; set; }
    }
}
