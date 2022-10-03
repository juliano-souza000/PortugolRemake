using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portugol_Remake.Utils
{
    public class FlowchartStructureRoot
    {
        public string Type { get; set; }
        public FlowchartStructure Content { get; set; }
    }

    public class FlowchartStructure
    {
        public XY TopLeft { get; set; }
        public XY BottonRight { get; set; }
        public XY Center { get; set; }
        public string Text { get; set; }
        public FormColor FormColor { get; set; }
        public List<int> Previous { get; set; } = new List<int>();
        public bool IsFromTrue { get; set; }
        public bool IsFromFalse { get; set; }
        public Next Next { get; set; }
        public Next IfTrue { get; set; }
        public Next IfFalse { get; set; }
        public int Processed { get; set; }
    }

    public class Next
    {
        public FlowchartStructure Dest { get; set; }
        public Position OrigPos { get; set; }
        public Position DestPos { get; set; }
        public int LineIndex { get; set; } = -1;
    }

    public class XY
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class FormColor
    { 
        public byte Alpha { get; set; }
        public byte Blue { get; set; }
        public byte Green { get; set; }
        public int RGB { get; set; }
        public byte Red { get; set; }
        public int Transparency { get; set; }
    }

    public class Profile
    {
        public float[] MediaWhitePoint { get; set; }
        public string Data { get; set; }
        public int NumComponents { get; set; }
        public int ColorSpaceType { get; set; }
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public int PCSType { get; set; }
        public int ProfileClass { get; set; }
    }

    public enum Position
    {
        Top,
        Bottom,
        Left,
        Right
    }
}
