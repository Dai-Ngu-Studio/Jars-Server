namespace JARS_API.BusinessModels
{
    public class AnalyticsReport
    {
        public AnalyticsReport(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}
