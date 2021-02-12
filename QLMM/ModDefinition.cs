using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLMM
{
    public class ModDefinition
    {
        public string name;
        public string author;
        public string version;
        public string description;

        public string path;

        public bool IsDisabled;

        public ModDefinition(string pathTemp) {
            path = pathTemp;
        }
    }
}
