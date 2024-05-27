namespace Diploma3.JsonModels
{
    public class TTriangle(int triangleNumber, List<int> pointNumbers)
    {
        public int TriangleNumber { get; set; } = triangleNumber;
        public List<int> PointNumbers { get; set; } = pointNumbers;
    }
}
