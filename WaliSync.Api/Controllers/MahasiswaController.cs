using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WaliSync.Api.Data;
using WaliSync.Api.DTOs;
using WaliSync.Api.Models;

namespace WaliSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MahasiswaController : ControllerBase
{
    private readonly AcademicDbContext _context;
    private readonly ILogger<MahasiswaController> _logger;

    public MahasiswaController(AcademicDbContext context, ILogger<MahasiswaController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<MahasiswaResponseDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var query = _context.Mahasiswas.AsNoTracking().Include(m => m.Dosen).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(m =>
                EF.Functions.Like(m.NamaMahasiswa, $"%{s}%") ||
                EF.Functions.Like(m.Nim, $"%{s}%") ||
                m.Angkatan.ToString().Contains(s));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(m => m.Angkatan)
            .ThenBy(m => m.NamaMahasiswa)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new MahasiswaResponseDto
            {
                MahasiswaId = m.MahasiswaId,
                Nim = m.Nim,
                NamaMahasiswa = m.NamaMahasiswa,
                Email = m.Email,
                Angkatan = m.Angkatan,
                DosenId = m.DosenId,
                NamaDosenWali = m.Dosen != null ? m.Dosen.NamaDosen : "-"
            })
            .ToListAsync();

        return Ok(new PagedResultDto<MahasiswaResponseDto>
        {
            TotalCount = total,
            Items = items,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MahasiswaResponseDto>> GetById(int id)
    {
        var mhs = await _context.Mahasiswas
            .AsNoTracking()
            .Include(m => m.Dosen)
            .Where(m => m.MahasiswaId == id)
            .Select(m => new MahasiswaResponseDto
            {
                MahasiswaId = m.MahasiswaId,
                Nim = m.Nim,
                NamaMahasiswa = m.NamaMahasiswa,
                Email = m.Email,
                Angkatan = m.Angkatan,
                DosenId = m.DosenId,
                NamaDosenWali = m.Dosen != null ? m.Dosen.NamaDosen : "-"
            })
            .FirstOrDefaultAsync();

        if (mhs == null)
        {
            return NotFound(new ApiErrorResponseDto
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = $"Mahasiswa dengan ID {id} tidak ditemukan."
            });
        }

        return Ok(mhs);
    }

    [HttpPost]
    public async Task<ActionResult<MahasiswaResponseDto>> Create([FromBody] MahasiswaCreateUpdateDto dto)
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

        var cleanNim = dto.Nim.Trim();
        var cleanEmail = dto.Email.Trim().ToLowerInvariant();

        // Check unique NIM
        if (await _context.Mahasiswas.AnyAsync(m => m.Nim == cleanNim))
        {
            return Conflict(new ApiErrorResponseDto
            {
                StatusCode = StatusCodes.Status409Conflict,
                Message = $"NIM '{cleanNim}' sudah terdaftar dalam sistem."
            });
        }

        // Check unique Email
        if (await _context.Mahasiswas.AnyAsync(m => m.Email == cleanEmail))
        {
            return Conflict(new ApiErrorResponseDto
            {
                StatusCode = StatusCodes.Status409Conflict,
                Message = $"Email '{cleanEmail}' sudah terdaftar dalam sistem."
            });
        }

        // Verify Dosen exists
        var dosen = await _context.Dosens.FindAsync(dto.DosenId);
        if (dosen == null)
        {
            return BadRequest(new ApiErrorResponseDto
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Dosen Wali yang dipilih tidak valid atau tidak ditemukan."
            });
        }

        var entity = new Mahasiswa
        {
            Nim = cleanNim,
            NamaMahasiswa = dto.NamaMahasiswa.Trim(),
            Email = cleanEmail,
            Angkatan = dto.Angkatan,
            DosenId = dto.DosenId
        };

        _context.Mahasiswas.Add(entity);
        await _context.SaveChangesAsync();

        var responseDto = new MahasiswaResponseDto
        {
            MahasiswaId = entity.MahasiswaId,
            Nim = entity.Nim,
            NamaMahasiswa = entity.NamaMahasiswa,
            Email = entity.Email,
            Angkatan = entity.Angkatan,
            DosenId = entity.DosenId,
            NamaDosenWali = dosen.NamaDosen
        };

        return CreatedAtAction(nameof(GetById), new { id = entity.MahasiswaId }, responseDto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<MahasiswaResponseDto>> Update(int id, [FromBody] MahasiswaCreateUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiErrorResponseDto
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Data yang dikirim tidak valid."
            });
        }

        var entity = await _context.Mahasiswas.FindAsync(id);
        if (entity == null)
        {
            return NotFound(new ApiErrorResponseDto
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = $"Mahasiswa dengan ID {id} tidak ditemukan."
            });
        }

        var cleanNim = dto.Nim.Trim();
        var cleanEmail = dto.Email.Trim().ToLowerInvariant();

        if (await _context.Mahasiswas.AnyAsync(m => m.MahasiswaId != id && m.Nim == cleanNim))
        {
            return Conflict(new ApiErrorResponseDto
            {
                StatusCode = StatusCodes.Status409Conflict,
                Message = $"NIM '{cleanNim}' sudah digunakan oleh mahasiswa lain."
            });
        }

        if (await _context.Mahasiswas.AnyAsync(m => m.MahasiswaId != id && m.Email == cleanEmail))
        {
            return Conflict(new ApiErrorResponseDto
            {
                StatusCode = StatusCodes.Status409Conflict,
                Message = $"Email '{cleanEmail}' sudah digunakan oleh mahasiswa lain."
            });
        }

        var dosen = await _context.Dosens.FindAsync(dto.DosenId);
        if (dosen == null)
        {
            return BadRequest(new ApiErrorResponseDto
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Dosen Wali yang dipilih tidak valid atau tidak ditemukan."
            });
        }

        entity.Nim = cleanNim;
        entity.NamaMahasiswa = dto.NamaMahasiswa.Trim();
        entity.Email = cleanEmail;
        entity.Angkatan = dto.Angkatan;
        entity.DosenId = dto.DosenId;

        await _context.SaveChangesAsync();

        return Ok(new MahasiswaResponseDto
        {
            MahasiswaId = entity.MahasiswaId,
            Nim = entity.Nim,
            NamaMahasiswa = entity.NamaMahasiswa,
            Email = entity.Email,
            Angkatan = entity.Angkatan,
            DosenId = entity.DosenId,
            NamaDosenWali = dosen.NamaDosen
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _context.Mahasiswas.FindAsync(id);
        if (entity == null)
        {
            return NotFound(new ApiErrorResponseDto
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = $"Mahasiswa dengan ID {id} tidak ditemukan."
            });
        }

        _context.Mahasiswas.Remove(entity);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

