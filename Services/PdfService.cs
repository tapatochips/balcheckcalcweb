using DinkToPdf;
using DinkToPdf.Contracts;
using balcheckcalcweb.Models;
using System;
using System.Text;

namespace balcheckcalcweb.Services
{
    public class PdfService : IPdfService
    {
        private readonly IConverter _converter;

        public PdfService(IConverter converter)
        {
            _converter = converter;
        }

        public byte[] GenerateCalculationPdf(CheckHistory checkHistory)
        {
            var htmlContent = GenerateHtmlContent(checkHistory);

            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 20, Bottom = 20, Left = 20, Right = 20 },
                DocumentTitle = $"Policy Calculation - {checkHistory.UserAlias} - {checkHistory.CalculationDate:MM/dd/yyyy}"
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = htmlContent,
                WebSettings = { DefaultEncoding = "utf-8" },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 9, Center = "Balance Check Calculator", Line = true }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            return _converter.Convert(pdf);
        }

        private string GenerateHtmlContent(CheckHistory checkHistory)
        {
            var sb = new StringBuilder();

            //header
            sb.Append(@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='UTF-8'>
                <title>Balance Check Calculation</title>
                <style>
                    body { font-family: Arial, sans-serif; margin: 0; padding: 20px; color: #333; }
                    .header { text-align: center; margin-bottom: 30px; border-bottom: 1px solid #ccc; padding-bottom: 10px; }
                    .header h1 { color: #dc3545; margin: 0; }
                    .header p { margin: 5px 0; }
                    .info-section { margin-bottom: 20px; }
                    .info-section h2 { color: #dc3545; border-bottom: 1px solid #eee; padding-bottom: 5px; }
                    table { width: 100%; border-collapse: collapse; margin-bottom: 20px; }
                    th { background-color: #007bff; color: white; text-align: left; padding: 8px; }
                    td { padding: 8px; border: 1px solid #ddd; }
                    tr:nth-child(even) { background-color: #f2f2f2; }
                    .total-row { background-color: #e6f7ff; font-weight: bold; }
                    .footer { margin-top: 30px; text-align: center; font-size: 12px; color: #777; }
                </style>
            </head>
            <body>
                <div class='header'>
                    <h1>Balance Check Calculator</h1>
                    <p>Generated on: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + @"</p>
                </div>
                
                <div class='info-section'>
                    <h2>Calculation Information</h2>
                    <p><strong>User:</strong> " + checkHistory.UserAlias + @"</p>
                    <p><strong>Calculation Date:</strong> " + checkHistory.CalculationDate.ToString("MM/dd/yyyy hh:mm tt") + @"</p>
                    <p><strong>Number of Policies:</strong> " + checkHistory.PolicyCount + @"</p>
                </div>
                
                <div class='info-section'>
                    <h2>Policy Details</h2>
                    <table>
                        <thead>
                            <tr>
                                <th>Policy #</th>
                                <th>Balance</th>
                                <th>Installment</th>
                                <th>Effective Date</th>
                                <th>Expiration Date</th>
                                <th>Current Date</th>
                                <th>Revised Amount</th>
                            </tr>
                        </thead>
                        <tbody>");

            //pol details
            foreach (var detail in checkHistory.PolicyDetails)
            {
                sb.Append("<tr>");
                sb.Append($"<td>{detail.PolicyNumber}</td>");
                sb.Append($"<td>${detail.Balance:N2}</td>");
                sb.Append($"<td>${detail.Installment:N2}</td>");
                sb.Append($"<td>{detail.EffectiveDate:MM/dd/yyyy}</td>");
                sb.Append($"<td>{detail.ExpirationDate:MM/dd/yyyy}</td>");
                sb.Append($"<td>{detail.CurrentDate:MM/dd/yyyy}</td>");
                sb.Append($"<td>${detail.RevisedAmount:N2}</td>");
                sb.Append("</tr>");
            }

            //total row
            sb.Append(@"
                            <tr class='total-row'>
                                <td colspan='6' style='text-align: right;'>Total:</td>
                                <td>$" + checkHistory.TotalAmount.ToString("N2") + @"</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                
                <div class='info-section'>
                    <h2>FAQ</h2>
                    <p><strong>Q: Why is the revised amount more than what was billed?</strong></p>
                    <p>A: If the policy has a spreading adjustment, it will include the entire spread balance.</p>
                    <p>A: Underwriting could have sent charges over; if no changes were submitted, contact the respective department.</p>
                    <p>A: A payment could have returned.</p>
                </div>
                
                <div class='footer'>
                    <p>This is an automated calculation. Please review for accuracy.</p>
                </div>
            </body>
            </html>");

            return sb.ToString();
        }
    }
}