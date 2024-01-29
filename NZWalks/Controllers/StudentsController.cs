using Microsoft.AspNetCore.Mvc;

namespace NZWalks.Controllers;

// https://localhost:portnumber/api/students
[Route("api/[controller]")]
[ApiController]
public class StudentsController : ControllerBase
{
    // GET: https://localhost:portnumber/api/students
    [HttpGet]
    public IActionResult GetAllStudents()
    {
        string[] studentsNames = { "John", "Jane", "Mark", "Emily", "David" };
        return Ok(studentsNames);
    }
}