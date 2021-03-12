using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MergeSort
{
    public class Row
    {
        public int Number { get; set; }
        public int YValue { get; set; }
        public int XValue { get; set; }
        public int MaxElementsCount { get; set; }
        public List<Series> Elements { get; set; } = new List<Series>();
    }
}
