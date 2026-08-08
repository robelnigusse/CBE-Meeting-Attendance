using CbeMeetingAttendance.DTOs.Admin;
using System.Text;
using ClosedXML.Excel;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CbeMeetingAttendance.Services
{
    public class ExportService
    {
        private readonly AdminService _adminService;

        public ExportService(AdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<byte[]> ExportCsvAsync()
        {
            var dashboardResponse = await _adminService.GetDashboardAsync();

            if (!dashboardResponse.Success || dashboardResponse.Data == null)
            {
                throw new Exception("Unable to generate dashboard report.");
            }

            DashboardDto dashboard = dashboardResponse.Data;

            StringBuilder csv = new StringBuilder();


            // Dashboard Summary

            csv.AppendLine("CBE Meeting Attendance Dashboard");
            csv.AppendLine();

            csv.AppendLine("Metric,Value");

            csv.AppendLine(
                $"Total Employees,{dashboard.TotalEmployees}");

            csv.AppendLine(
                $"Today's Attendance,{dashboard.TodayAttendance}");

            csv.AppendLine(
                $"Morning Attendance,{dashboard.MorningAttendance}");

            csv.AppendLine(
                $"Afternoon Attendance,{dashboard.AfternoonAttendance}");


            csv.AppendLine();


            // Attendance Details

            csv.AppendLine(
                "EmployeeId,FullName,Department,Division,MeetingDate,Session,AttendanceTime");


            foreach (var attendee in dashboard.TodayAttendees)
            {
                csv.AppendLine(
                    $"{attendee.EmployeeId}," +
                    $"{attendee.FullName}," +
                    $"{attendee.Department}," +
                    $"{attendee.Division}," +
                    $"{attendee.MeetingDate}," +
                    $"{attendee.Session}," +
                    $"{attendee.AttendanceTime}");
            }


            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        public async Task<byte[]> ExportExcelAsync()
        {
            var dashboardResponse = await _adminService.GetDashboardAsync();

            if (!dashboardResponse.Success || dashboardResponse.Data == null)
            {
                throw new Exception("Unable to generate dashboard report.");
            }

            var dashboard = dashboardResponse.Data;


            using var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Attendance Report");


            // Title

            worksheet.Cell("A1").Value = "CBE Meeting Attendance Dashboard";
            worksheet.Cell("A1").Style.Font.Bold = true;


            // Summary Section

            worksheet.Cell("A3").Value = "Metric";
            worksheet.Cell("B3").Value = "Value";


            worksheet.Cell("A4").Value = "Total Employees";
            worksheet.Cell("B4").Value = dashboard.TotalEmployees;


            worksheet.Cell("A5").Value = "Today's Attendance";
            worksheet.Cell("B5").Value = dashboard.TodayAttendance;


            worksheet.Cell("A6").Value = "Morning Attendance";
            worksheet.Cell("B6").Value = dashboard.MorningAttendance;


            worksheet.Cell("A7").Value = "Afternoon Attendance";
            worksheet.Cell("B7").Value = dashboard.AfternoonAttendance;



            // Attendance Table

            int row = 10;


            worksheet.Cell(row, 1).Value = "EmployeeId";
            worksheet.Cell(row, 2).Value = "FullName";
            worksheet.Cell(row, 3).Value = "Department";
            worksheet.Cell(row, 4).Value = "Division";
            worksheet.Cell(row, 5).Value = "MeetingDate";
            worksheet.Cell(row, 6).Value = "Session";
            worksheet.Cell(row, 7).Value = "AttendanceTime";


            var headerRange = worksheet.Range(row, 1, row, 7);

            headerRange.Style.Font.Bold = true;



            row++;


            foreach (var attendee in dashboard.TodayAttendees)
            {
                worksheet.Cell(row, 1).Value = attendee.EmployeeId;
                worksheet.Cell(row, 2).Value = attendee.FullName;
                worksheet.Cell(row, 3).Value = attendee.Department;
                worksheet.Cell(row, 4).Value = attendee.Division;
                worksheet.Cell(row, 5).Value = attendee.MeetingDate.ToString();
                worksheet.Cell(row, 6).Value = attendee.Session;
                worksheet.Cell(row, 7).Value = attendee.AttendanceTime;

                row++;
            }


            // Make columns fit content

            worksheet.Columns().AdjustToContents();



            using MemoryStream stream = new MemoryStream();

            workbook.SaveAs(stream);


            return stream.ToArray();
        }

        public async Task<byte[]> ExportPdfAsync()
        {
            var dashboardResponse = await _adminService.GetDashboardAsync();

            if (!dashboardResponse.Success || dashboardResponse.Data == null)
            {
                throw new Exception("Unable to generate dashboard report.");
            }


            var dashboard = dashboardResponse.Data;


            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);

                    page.Margin(30);


                    page.Header()
                        .Text("CBE Meeting Attendance Dashboard")
                        .Bold()
                        .FontSize(18);



                    page.Content()
                        .Column(column =>
                        {

                            column.Item()
                            .Text($"Total Employees: {dashboard.TotalEmployees}");

                            column.Item()
                            .Text($"Today's Attendance: {dashboard.TodayAttendance}");

                            column.Item()
                            .Text($"Morning Attendance: {dashboard.MorningAttendance}");

                            column.Item()
                            .Text($"Afternoon Attendance: {dashboard.AfternoonAttendance}");


                            column.Item()
                            .PaddingTop(20)
                            .Text("Today's Attendees")
                            .Bold();



                            column.Item()
                            .Table(table =>
                            {

                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });



                                table.Header(header =>
                                {
                                    header.Cell().Text("Employee ID").Bold();
                                    header.Cell().Text("Name").Bold();
                                    header.Cell().Text("Department").Bold();
                                    header.Cell().Text("Session").Bold();
                                    header.Cell().Text("Date").Bold();
                                    header.Cell().Text("Time").Bold();
                                });



                                foreach (var attendee in dashboard.TodayAttendees)
                                {
                                    table.Cell()
                                        .Text(attendee.EmployeeId);

                                    table.Cell()
                                        .Text(attendee.FullName);

                                    table.Cell()
                                        .Text(attendee.Department);

                                    table.Cell()
                                        .Text(attendee.Session);

                                    table.Cell()
                                        .Text(attendee.MeetingDate.ToString());

                                    table.Cell()
                                        .Text(
                                        attendee.AttendanceTime
                                        .ToString("HH:mm"));
                                }

                            });

                        });


                    page.Footer()
                        .AlignCenter()
                        .Text("Generated by CBE Meeting Attendance System");

                });
            });



            return document.GeneratePdf();
        }
    }
}