using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using AuthService.DTOs.LoanDtos;
using AuthService.Models.Enums;
using AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;
        public LoansController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpGet]
        [Authorize(Roles ="Librarian")]
        public async Task<IActionResult> GetAll()
        {
            var loans = await _loanService.GetAllAsync();
            return Ok(loans);
        }

        [HttpGet("{id}")]
        [Authorize(Roles ="Librarian")]
        public async Task<IActionResult> GetById(int id)
        {
            var loan = await _loanService.GetByIdAsync(id);
            if (loan == null) return NotFound($"Loan with id {id} not found.");
            return Ok(loan);
        }

        [HttpGet("my-loans")]
        public async Task<IActionResult> GetMyLoans()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var loans = await _loanService.GetMyLoansAsync(userId);
            return Ok(loans);
        }
        [HttpGet("status/{status}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GeByStatus(LoanStatus status)
        {
            var loans = await _loanService.GetByStatusAsync(status);
            return Ok(loans);
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestLoan([FromBody] LoanRequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var loan = await _loanService.RequestLoanAsync(userId, dto);
                return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPatch("{id}/approve")]
        [Authorize(Roles ="Librarian")]
        public  async Task<IActionResult> Approve(int id, [FromQuery] int dueDays = 14)
        {
            try
            {
                var result = await _loanService.ApproveLoanAsync(id, dueDays);
                if (!result) return NotFound();
                return NoContent();
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPatch("{id}/return")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Return(int id)
        {
            try
            {
                var result = await _loanService.ReturnLoanAsync(id);
                if (!result) return NotFound();
                return NoContent();
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPatch("{id}/cancel")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var result = await _loanService.CancellationAsync(id);
                if (!result) return NotFound();
                return NoContent(); 
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
