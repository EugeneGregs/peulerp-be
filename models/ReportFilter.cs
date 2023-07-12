namespace PeyulErp.Models
{
    public record ReportFilter
    {
        public int Top { get; set; }
        public ReportFilterType FilterType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}