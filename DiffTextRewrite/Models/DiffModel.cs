using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiffTextRewrite.Models
{
    public class DiffModel
    {
        public List<string> left { get; set; }
        public List<string> right { get; set; }
    }
}
