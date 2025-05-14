// Data/StationData.cs
using System.Collections.Generic;

namespace TestCrudUser.Data
{
    public class StationData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public List<RadialBarSegment> RadialSegments { get; set; } = new List<RadialBarSegment>();

        public string InputValue1 { get; set; } = string.Empty;
        public string InputValue2 { get; set; } = string.Empty;
        public string InputValue3 { get; set; } = string.Empty;
    }
}
