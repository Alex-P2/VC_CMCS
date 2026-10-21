// References
// https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ST10445832_PROG6212_POE.Areas.HR.Data
{
    public class StatisticsReportDocument : IDocument
    {
        /// <summary>
        /// Lecturer usage data
        /// </summary>
        public LecturerStatistics LecturerStats { get; }

        /// <summary>
        /// Approver usage data
        /// </summary>
        public ApproverStatistics ApproverStats { get; }

        /// <summary>
        /// HR usage data
        /// </summary>
        public HrStatistics HrStats { get; }

        /// <summary>
        /// Number of months the report covers
        /// </summary>
        public int Timeframe { get; }


        //=============================================================================================================================//
        /// <summary>
        /// Default constructor
        /// </summary>
        public StatisticsReportDocument(LecturerStatistics lecturerStats, ApproverStatistics approverStats,
            HrStatistics hrStats, int? timeframe = null)
        {
            LecturerStats = lecturerStats;
            ApproverStats = approverStats;
            HrStats = hrStats;
            Timeframe = timeframe ?? 0;
        }


        //=============================================================================================================================//
        /// <summary>
        /// Creates a system usage statistics report
        /// </summary>
        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.Header()
                        .Background(Colors.BlueGrey.Lighten1)
                        .Element(this.ComposeHeader);

                    page.Content()
                        .Background(Colors.White)
                        .Element(this.ComposeContent);

                    page.Footer()
                        .Height(50)
                        .Background(Colors.BlueGrey.Lighten1)
                        .AlignMiddle()
                        .AlignRight().PaddingHorizontal(10)
                        .Text(text =>
                        {
                            text.CurrentPageNumber();
                            text.Span(" / ");
                            text.TotalPages();
                        });
                });
        }


        //=============================================================================================================================//
        /// <summary>
        /// Creates the report header
        /// </summary>
        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item()
                        .Padding(10)
                        .Text("CMCS Reporting")
                            .FontSize(24)
                            .FontColor(Colors.BlueGrey.Darken4);
                });

                row.RelativeItem().Column(column =>
                {
                    if (Timeframe > 0)
                    {
                        column.Item()
                            .AlignRight()
                            .Padding(10)
                            .Text($"Reporting Period: {Timeframe} Months");

                        column.Item()
                            .AlignRight()
                            .Padding(10)
                            .Text($"{DateOnly.FromDateTime(DateTime.Now.AddMonths(-Timeframe))} - {DateTime.Now.ToShortDateString()}");
                    }
                    else
                    {
                        column.Item()
                            .AlignRight()
                            .Padding(10)
                            .Text($"Reporting Period: All time");
                        column.Item()
                            .AlignRight()
                            .Padding(10)
                            .Text(text =>
                            {
                                text.Span("Report Date: ")
                                    .SemiBold()
                                    .FontColor(Colors.BlueGrey.Darken4);
                                text.Span($"{DateTime.Now.ToShortDateString()}")
                                    .FontColor(Colors.BlueGrey.Darken4);
                            });
                    }
                });
            });
        }


        //=============================================================================================================================//
        /// <summary>
        /// Creates the report body
        /// </summary>
        private void ComposeContent(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.Spacing(10);

                    row.RelativeItem().Column(col =>
                    {
                        col.Item()
                            .BorderBottom(1)
                            .PaddingTop(10)
                            .PaddingBottom(5)
                            .Text("Lecturer Statistics")
                            .SemiBold();

                        col.Item()
                            .PaddingTop(5)
                            .Text($"Average Hourly Rate: {LecturerStats.AverageHourlyRate.ToString("C")}");
                        col.Item()
                            .Text($"Average Claims Submitted: {LecturerStats.AverageClaimsSubmitted.ToString("F")}");
                        col.Item()
                            .Text($"Average Hours Claimed: {LecturerStats.AverageHoursClaimed.ToString("F")}");
                        col.Item()
                            .Text($"Average Claim Value: {LecturerStats.AverageClaimValue.ToString("C")}");
                        col.Item()
                            .Text($"Total Claim Value: {LecturerStats.TotalClaimValue.ToString("C")}");
                    });


                });

                column.Item().Row(row =>
                {
                    row.Spacing(10);

                    row.RelativeItem().Column(col =>
                    {
                        col.Item()
                            .BorderBottom(1)
                            .PaddingTop(10)
                            .PaddingBottom(5)
                            .Text("Programme Coordinator & Academic Manager Statistics")
                            .SemiBold();

                        col.Item()
                            .PaddingTop(5)
                            .Text($"Average Claims Processed: {ApproverStats.AverageClaimsProcessed}");
                        col.Item()
                            .Text($"Claim Rejection Rate: {ApproverStats.ClaimRejectionRate.ToString("F")}%");
                        col.Item()
                            .Text($"Average Time To Invoice: {ApproverStats.AverageTimeToInvoice} Days");
                    });
                });

                column.Item().Row(row =>
                {
                    row.Spacing(10);

                    row.RelativeItem().Column(col =>
                    {
                        col.Item()
                            .BorderBottom(1)
                            .PaddingTop(10)
                            .PaddingBottom(5)
                            .Text("HR Statistics")
                            .SemiBold();

                        col.Item()
                            .PaddingTop(5)
                            .Text($"Total Invoices Created: {HrStats.TotalInvoicesCreated}");
                        col.Item()
                            .Text($"Total Invoiced Value: {HrStats.TotalInvoiceValue.ToString("C")}");
                    });
                });
            });
        }
    }
}
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\