using Microsoft.AspNetCore.Mvc;
using CbeMeetingAttendance.Services;

namespace CbeMeetingAttendance.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;
        private readonly ExportService _exportService;

        public AdminController(
            AdminService adminService,
            ExportService exportService)
        {
            _adminService = adminService;
            _exportService = exportService;
        }

        // GET: api/admin/dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _adminService.GetDashboardAsync();

            return Ok(result);
        }
        [HttpGet("export/csv")]
        public async Task<IActionResult> ExportCsv()
        {
            var file = await _exportService.ExportCsvAsync();

            return File(
                file,
                "text/csv",
                "CBE_Meeting_Attendance_Report.csv"
            );
        }
        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportExcel()
        {
            var file = await _exportService.ExportExcelAsync();

            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "CBE_Meeting_Attendance_Report.xlsx"
            );
        }
        [HttpGet("export/pdf")]
        public async Task<IActionResult> ExportPdf()
        {
            var file = await _exportService.ExportPdfAsync();

            return File(
                file,
                "application/pdf",
                "CBE_Meeting_Attendance_Report.pdf"
            );
        }
    }
}