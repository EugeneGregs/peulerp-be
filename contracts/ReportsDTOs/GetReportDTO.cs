namespace PeyulErp.Models
{
    public record GetReportDTO
    {
        public ReportType Type { get; set; }
        public ReportFilter Filter { get; set; }
    }
}