// References
// https://www.questpdf.com/api-reference/page/basics.html
// https://www.questpdf.com/api-reference/table/basics.html
// https://www.questpdf.com/getting-started.html

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ST10445832_PROG6212_POE.Areas.HR.Models;

namespace ST10445832_PROG6212_POE.Areas.HR.Data
{
    public class InvoiceDocument : IDocument
    {
        public ClaimInvoiceDetail InvoiceDetail { get; }

        public InvoiceDocument (ClaimInvoiceDetail invoiceDetail)
        {
            InvoiceDetail = invoiceDetail;
        }

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.Header()
                        .Background(Colors.BlueGrey.Lighten1)
                        .Element(ComposeHeader);

                    page.Content()
                        .Background(Colors.White)
                        .Element(ComposeContent);

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

        private void ComposeHeader(IContainer container)
        {
            // title and dates
            container.Row(row =>
            {

                row.RelativeItem().Column(column =>
                {
                    column.Item()
                        .Padding(10)
                        .Text("CMCS")
                            .FontSize(36)
                            .Bold()
                            .FontColor(Colors.BlueGrey.Darken4);
                });
                // dates
                row.RelativeItem().Column(column =>
                {
                    column.Item()
                        .Text($"Invoice #{InvoiceDetail.InvoiceNumber}")
                            .FontSize(24)
                            .SemiBold()
                            .FontColor(Colors.BlueGrey.Darken4);
                    column.Item()
                        .Text(text =>
                        {
                            text.Span("Date created: ")
                                .SemiBold()
                                .FontColor(Colors.BlueGrey.Darken4);
                            text.Span($"{InvoiceDetail.DateCreated:dd/MM/yyyy}")
                                .FontColor(Colors.BlueGrey.Darken3);
                        });
                    column.Item()
                    .Text(text =>
                    {
                        text.Span("Date due: ")
                            .SemiBold()
                            .FontColor(Colors.BlueGrey.Darken4);
                        text.Span($"{InvoiceDetail.PaymentDue:dd/MM/yyyy}")
                            .FontColor(Colors.BlueGrey.Darken3);
                    });
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.Column(column =>
            {
                // From: To:
                column.Item().Row(row =>
                {
                    row.Spacing(10);

                    row.RelativeItem().Column(col =>
                    {
                        col.Item()
                            .BorderBottom(1)
                            .PaddingTop(10)
                            .PaddingBottom(5)
                            .Text("From")
                            .SemiBold();

                        col.Item()
                            .PaddingTop(5)
                            .Text(InvoiceDetail.InvoiceeName);
                        col.Item()
                            .Text(InvoiceDetail.InvoiceeEmail);
                        col.Item()
                            .PaddingTop(10)
                            .Text("Banking Details");
                        col.Item()
                            .Text($"Account holder: {InvoiceDetail.BankAccount.AccountHolder}");
                        col.Item()
                            .Text($"Bank name: {InvoiceDetail.BankAccount.BankName}");
                        col.Item()
                            .Text($"Branch code: {InvoiceDetail.BankAccount.BranchCode}");
                        col.Item()
                            .Text($"Account number: {InvoiceDetail.BankAccount.AccountNumber}");
                    });

                    row.RelativeItem().Column(col =>
                    {
                        col.Item()
                            .BorderBottom(1)
                            .PaddingTop(10)
                            .PaddingBottom(5)
                            .Text("To")
                            .SemiBold();

                        col.Item()
                            .PaddingTop(5)
                            .Text(InvoiceDetail.Issuer);
                        col.Item()
                            .Text(InvoiceDetail.IssuerEmail);
                    });
                });

                // Invoice items table
                column.Item()
                    .PaddingTop(20)
                    .Element(ComposeTable);

                // Invoice total
                column.Item()
                    .BorderBottom(1)
                    .PaddingVertical(10)
                    .AlignRight()
                    .Text($"Total amount due: R {InvoiceDetail.TotalAmount}")
                    .FontSize(16)
                    .SemiBold();
            });
        }

        private void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(50);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell()
                        .BorderBottom(1)
                        .Padding(4)
                        .Text("#")
                            .SemiBold();
                    header.Cell()
                        .BorderBottom(1)
                        .Padding(4)
                        .Text("Course")
                            .SemiBold();
                    header.Cell()
                        .BorderBottom(1)
                        .Padding(4)
                        .Text("Module")
                            .SemiBold();
                    header.Cell()
                        .BorderBottom(1)
                        .Padding(4)
                        .Text("R/h")
                            .SemiBold()
                            .AlignRight();
                    header.Cell()
                        .BorderBottom(1)
                        .Padding(4)
                        .Text("Hours Worked")
                            .SemiBold()
                            .AlignRight();
                    header.Cell()
                        .BorderBottom(1)
                        .Padding(4)
                        .Text("Total")
                            .SemiBold()
                            .AlignRight();
                });

                foreach (var item in InvoiceDetail.InvoiceItems)
                {
                    table.Cell()
                        .BorderBottom(1)
                        .Padding(4)
                        .Text($"{InvoiceDetail.InvoiceItems.IndexOf(item) + 1}");
                    table.Cell()
                        .BorderBottom(1)
                        .Padding(4)
                        .Text($"{item.CourseCode}")
                            .SemiBold();
                    table.Cell()
                        .BorderBottom(1)
                        .Padding(4)
                        .Text($"{item.ModuleCode}")
                            .SemiBold();
                    table.Cell()
                        .BorderBottom(1)
                        .Padding(4)
                        .Text($"{item.HourlyRate}")
                            .SemiBold()
                            .AlignRight();
                    table.Cell()
                        .BorderBottom(1)
                        .Padding(4)
                        .Text($"{item.HoursWorked}")
                            .SemiBold()
                            .AlignRight();
                    table.Cell()
                        .BorderBottom(1)
                        .Padding(4)
                        .Text($"R {item.Total}")
                            .SemiBold()
                            .AlignRight();
                }
            });
        }
    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\