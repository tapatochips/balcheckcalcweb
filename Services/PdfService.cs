using balcheckcalcweb.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;

namespace balcheckcalcweb.Services
{
    public class PdfService : IPdfService
    {
        public byte[] GenerateCalculationPdf(CheckHistory checkHistory)
        {
            // Configure QuestPDF
            QuestPDF.Settings.License = LicenseType.Community;

            // Generate PDF
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(ComposeHeader);

                    page.Content().Element(content =>
                    {
                        ComposeContent(content, checkHistory);
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });
                });
            });

            // Return the PDF as byte array
            return document.GeneratePdf();
        }

        private void ComposeHeader(QuestPDF.Infrastructure.IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().AlignCenter().Text("Balance Check Calculator")
                        .FontSize(20).Bold();

                    column.Item().AlignCenter().Text($"Generated on: {DateTime.Now:MM/dd/yyyy hh:mm tt}")
                        .FontSize(10);

                    column.Item().PaddingVertical(5).LineHorizontal(1);
                });
            });
        }

        private void ComposeContent(QuestPDF.Infrastructure.IContainer container, CheckHistory checkHistory)
        {
            container.Column(column =>
            {
                // Calculation Info Section
                column.Item().PaddingVertical(10).Element(contentContainer =>
                {
                    contentContainer.Column(c =>
                    {
                        c.Item().Text("Calculation Information").FontSize(14).Bold();
                        c.Item().Text($"User: {checkHistory.UserAlias}");
                        c.Item().Text($"Calculation Date: {checkHistory.CalculationDate:MM/dd/yyyy hh:mm tt}");
                        c.Item().Text($"Number of Policies: {checkHistory.PolicyCount}");
                    });
                });

                // Policy Details Section
                column.Item().PaddingVertical(10).Element(contentContainer =>
                {
                    contentContainer.Column(c =>
                    {
                        c.Item().Text("Policy Details").FontSize(14).Bold();

                        c.Item().PaddingVertical(5).Table(table =>
                        {
                            // Define columns
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1); // Policy #
                                columns.RelativeColumn(2); // Balance
                                columns.RelativeColumn(2); // Installment
                                columns.RelativeColumn(2); // Effective Date
                                columns.RelativeColumn(2); // Expiration Date
                                columns.RelativeColumn(2); // Current Date
                                columns.RelativeColumn(2); // Revised Amount
                            });

                            // Add header
                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Text("Policy #").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Text("Balance").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Text("Installment").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Text("Effective Date").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Text("Expiration Date").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Text("Current Date").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Text("Revised Amount").Bold();
                            });

                            // Add data rows
                            foreach (var detail in checkHistory.PolicyDetails)
                            {
                                table.Cell().Text(detail.PolicyNumber.ToString());
                                table.Cell().Text($"${detail.Balance:N2}");
                                table.Cell().Text($"${detail.Installment:N2}");
                                table.Cell().Text($"{detail.EffectiveDate:MM/dd/yyyy}");
                                table.Cell().Text($"{detail.ExpirationDate:MM/dd/yyyy}");
                                table.Cell().Text($"{detail.CurrentDate:MM/dd/yyyy}");
                                table.Cell().Text($"${detail.RevisedAmount:N2}");
                            }

                            // Add total row
                            table.Cell().ColumnSpan(6).Text("Total: ").Bold().AlignRight();
                            table.Cell().Text($"${checkHistory.TotalAmount:N2}").Bold();
                        });
                    });
                });

                // FAQ Section
                column.Item().PaddingVertical(10).Element(contentContainer =>
                {
                    contentContainer.Column(c =>
                    {
                        c.Item().Text("FAQ").FontSize(14).Bold();
                        c.Item().Text("Q: Why is the revised amount more than what was billed?").Bold();
                        c.Item().Text("A: If the policy has a spreading adjustment, it will include the entire spread balance.");
                        c.Item().Text("A: Underwriting could have sent charges over; if no changes were submitted, contact the respective department.");
                        c.Item().Text("A: A payment could have returned.");
                    });
                });

                // Footer note
                column.Item().AlignCenter().PaddingTop(30).Text("This is an automated calculation. Please review for accuracy.").Italic();
            });
        }
    }
}