namespace JARS_API.BusinessModels
{
    public class AnalyticsReport
    {
        public AnalyticsReport(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public int Value { get; set; }
    }
}
