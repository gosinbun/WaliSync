using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WaliSync.Api.Data;
using WaliSync.Api.DTOs;
using WaliSync.Api.Models;

namespace WaliSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DosenController : ControllerBase
{
    private readonly AcademicDbContext _context;
    private readonly ILogger<DosenController> _logger;

    public DosenController(AcademicDbContext context, ILogger<DosenController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<DosenResponseDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var query = _context.Dosens.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(d =>
                EF.Functions.Like(d.NamaDosen, $"%{s}%") ||
                EF.Functions.Like(d.Nidn, $"%{s}%") ||
                EF.Functions.Like(d.Prodi, $"%{s}%"));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(d => d.NamaDosen)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new DosenResponseDto
            {
                DosenId = d.DosenId,
                Nidn = d.Nidn,
                NamaDosen = d.NamaDosen,
                Email = d.Email,
                Prodi = d.Prodi,
                JumlahMahasiswaBimbingan = d.Mahasiswas.Count
            })
            .ToListAsync();

        return Ok(new PagedResultDto<DosenResponseDto>
        {
            TotalCount = total,
            Items = items,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DosenResponseDto>> GetById(int id)
    {
        var dosen = await _context.Dosens
            .AsNoTracking()
            .Where(d => d.DosenId == id)
            .Select(d => new DosenResponseDto
            {
                DosenId = d.DosenId,
                Nidn = d.Nidn,
                NamaDosen = d.NamaDosen,
                Email = d.Email,
                Prodi = d.Prodi,
                JumlahMahasiswaBimbingan = d.Mahasiswas.Count
            })
            .FirstOrDefaultAsync();

        if (dosen == null)
        {
            return NotFound(new ApiErrorResponseDto
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = $"Dosen dengan ID {id} tidak ditemukan."
            });
        }

        return Ok(dosen);
    }

    [HttpGet("lookup")]
    public async Task<ActionResult<List<DosenLookupDto>>> GetLookup()
    {
        var lookup = await _context.Dosens
            .AsNoTracking()
            .OrderBy(d => d.NamaDosen)
            .Select(d => new DosenLookupDto
            {
                DosenId = d.DosenId,
                Nidn = d.Nidn,
                NamaDosen = d.NamaDosen,
                Prodi = d.Prodi
            })
            .ToListAsync();

        return Ok(lookup);
    }

    [HttpPost]
    public async Task<ActionResult<DosenResponseDto>> Create([FromBody] DosenCreateUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiErrorResponseDto
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Data yang dikirim tidak valid.",
                Errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>())
            });
        }

        var cleanNidn = dto.Nidn.Trim();
        var cleanEmail = dto.Email.Trim().ToLowerInvariant();

        // Check unique NIDN
        if (await _context.Dosens.AnyAsync(d => d.Nidn == cleanNidn))
        {
            return Conflict(new ApiErrorResponseDto
            {
                StatusCode = StatusCodes.Status409Conflict,
                Message = $"NIDN '{cleanNidn}' sudah terdaftar dalam sistem."
            });
        }

        // Check unique Email
        if (await _context.Dosens.AnyAsync(d => d.Email == cleanEmail))
        {
            return Conflict(new ApiErrorResponseDto
            {
                StatusCode = StatusCodes.Status409Conflict,
                Message = $"Email '{cleanEmail}' sudah terdaftar dalam sistem."
            });
        }

        var entity = new Dosen
        {
            Nidn = cleanNidn,
            NamaDosen = dto.NamaDosen.Trim(),
            Email = cleanEmail,
            Prodi = dto.Prodi.Trim()
        };

        _context.Dosens.Add(entity);
        await _context.SaveChangesAsync();

        var responseDto = new DosenResponseDto
        {
            DosenId = entity.DosenId,
            Nidn = entity.Nidn,
            NamaDosen = entity.NamaDosen,
            Email = entity.Email,
            Prodi = entity.Prodi,
            JumlahMahasiswaBimbingan = 0
        };

        return CreatedAtAction(nameof(GetById), new { id = entity.DosenId }, responseDto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<DosenResponseDto>> Update(int id, [FromBody] DosenCreateUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiErrorResponseDto
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Data yang dikirim tidak valid."
            });
        }

        var entity = await _context.Dosens.FindAsync(id);
        if (entity == null)
        {
            return NotFound(new ApiErrorResponseDto
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = $"Dosen dengan ID {id} tidak ditemukan."
            });
        }

        var cleanNidn = dto.Nidn.Trim();
        var cleanEmail = dto.Email.Trim().ToLowerInvariant();

        if (await _context.Dosens.AnyAsync(d => d.DosenId != id && d.Nidn == cleanNidn))
        {
            return Conflict(new ApiErrorResponseDto
            {
                StatusCode = StatusCodes.Status409Conflict,
                Message = $"NIDN '{cleanNidn}' sudah digunakan oleh dosen lain."
            });
        }

        if (await _context.Dosens.AnyAsync(d => d.DosenId != id && d.Email == cleanEmail))
        {
            return Conflict(new ApiErrorResponseDto
            {
                StatusCode = StatusCodes.Status409Conflict,
                Message = $"Email '{cleanEmail}' sudah digunakan oleh dosen lain."
            });
        }

        entity.Nidn = cleanNidn;
        entity.NamaDosen = dto.NamaDosen.Trim();
        entity.Email = cleanEmail;
        entity.Prodi = dto.Prodi.Trim();

        await _context.SaveChangesAsync();

        var studentCount = await _context.Mahasiswas.CountAsync(m => m.DosenId == id);

        return Ok(new DosenResponseDto
        {
            DosenId = entity.DosenId,
            Nidn = entity.Nidn,
            NamaDosen = entity.NamaDosen,
            Email = entity.Email,
            Prodi = entity.Prodi,
            JumlahMahasiswaBimbingan = studentCount
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _context.Dosens.FindAsync(id);
        if (entity == null)
        {
            return NotFound(new ApiErrorResponseDto
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = $"Dosen dengan ID {id} tidak ditemukan."
            });
        }

        // Relational Integrity Constraint Check (T07)
        var studentCount = await _context.Mahasiswas.CountAsync(m => m.DosenId == id);
        if (studentCount > 0)
        {
            return Conflict(new ApiErrorResponseDto
            {
                StatusCode = StatusCodes.Status409Conflict,
                Message = $"Operasi ditolak. Dosen masih menjadi wali aktif bagi {studentCount} mahasiswa."
            });
        }

        try
        {
            _context.Dosens.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Gagal menghapus dosen ID {Id} karena kendala database.", id);
            return Conflict(new ApiErrorResponseDto
            {
                StatusCode = StatusCodes.Status409Conflict,
                Message = "Operasi ditolak. Data dosen masih memiliki relasi aktif di database."
            });
        }
    }
}

