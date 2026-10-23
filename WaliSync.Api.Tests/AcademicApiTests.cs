using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using WaliSync.Api.Controllers;
using WaliSync.Api.Data;
using WaliSync.Api.DTOs;
using WaliSync.Api.Models;
using Xunit;

namespace WaliSync.Api.Tests;

public class AcademicApiTests
{
    private AcademicDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AcademicDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AcademicDbContext(options);
    }

    [Fact]
    public async Task DeleteDosen_WhenDosenHasActiveMahasiswa_Returns409Conflict()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var dosen = new Dosen
        {
            Nidn = "0512345678",
            NamaDosen = "Dr. Anita Wijaya",
            Email = "anita@kampus.ac.id",
            Prodi = "Sistem Informasi"
        };
        context.Dosens.Add(dosen);
        await context.SaveChangesAsync();

        var mhs = new Mahasiswa
        {
            Nim = "72240675",
            NamaMahasiswa = "Jimmy Nugraha Putra",
            Email = "jimmy@kampus.ac.id",
            Angkatan = 2024,
            DosenId = dosen.DosenId
        };
        context.Mahasiswas.Add(mhs);
        await context.SaveChangesAsync();

        var controller = new DosenController(context, NullLogger<DosenController>.Instance);

        // Act
        var result = await controller.Delete(dosen.DosenId);

        // Assert: Skenario T07 - Dosen wali aktif ditolak dengan 409 Conflict
        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal(409, conflictResult.StatusCode);

        var errorResponse = Assert.IsType<ApiErrorResponseDto>(conflictResult.Value);
        Assert.Contains("Operasi ditolak", errorResponse.Message);
        Assert.Contains("wali aktif", errorResponse.Message);
    }

    [Fact]
    public async Task DeleteDosen_WhenDosenHasNoMahasiswa_Returns204NoContent()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var dosen = new Dosen
        {
            Nidn = "0534567890",
            NamaDosen = "Cynthia Tan, S.T., M.T.",
            Email = "cynthia@kampus.ac.id",
            Prodi = "Sistem Informasi"
        };
        context.Dosens.Add(dosen);
        await context.SaveChangesAsync();

        var controller = new DosenController(context, NullLogger<DosenController>.Instance);

        // Act
        var result = await controller.Delete(dosen.DosenId);

        // Assert: Dosen tanpa bimbingan berhasil dihapus (204 NoContent)
        Assert.IsType<NoContentResult>(result);
        Assert.False(await context.Dosens.AnyAsync(d => d.DosenId == dosen.DosenId));
    }

    [Fact]
    public async Task CreateDosen_WithDuplicateNidn_Returns409Conflict()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        context.Dosens.Add(new Dosen
        {
            Nidn = "0512345678",
            NamaDosen = "Dr. Anita Wijaya",
            Email = "anita@kampus.ac.id",
            Prodi = "Sistem Informasi"
        });
        await context.SaveChangesAsync();

        var controller = new DosenController(context, NullLogger<DosenController>.Instance);

        var duplicateDto = new DosenCreateUpdateDto
        {
            Nidn = "0512345678", // Duplikat NIDN
            NamaDosen = "Anita Kloning",
            Email = "anita2@kampus.ac.id",
            Prodi = "Informatika"
        };

        // Act
        var result = await controller.Create(duplicateDto);

        // Assert: Skenario T03 - Duplikasi NIDN ditolak dengan 409 Conflict
        var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.Equal(409, conflictResult.StatusCode);

        var errorResponse = Assert.IsType<ApiErrorResponseDto>(conflictResult.Value);
        Assert.Contains("sudah terdaftar", errorResponse.Message);
    }

    [Fact]
    public async Task CreateMahasiswa_WithDuplicateNim_Returns409Conflict()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var dosen = new Dosen
        {
            Nidn = "0512345678",
            NamaDosen = "Dr. Anita Wijaya",
            Email = "anita@kampus.ac.id",
            Prodi = "Sistem Informasi"
        };
        context.Dosens.Add(dosen);
        await context.SaveChangesAsync();

        context.Mahasiswas.Add(new Mahasiswa
        {
            Nim = "72240675",
            NamaMahasiswa = "Jimmy Nugraha Putra",
            Email = "jimmy@kampus.ac.id",
            Angkatan = 2024,
            DosenId = dosen.DosenId
        });
        await context.SaveChangesAsync();

        var controller = new MahasiswaController(context, NullLogger<MahasiswaController>.Instance);

        var duplicateMhsDto = new MahasiswaCreateUpdateDto
        {
            Nim = "72240675", // Duplikat NIM
            NamaMahasiswa = "Jimmy Kembaran",
            Email = "jimmy.alt@kampus.ac.id",
            Angkatan = 2024,
            DosenId = dosen.DosenId
        };

        // Act
        var result = await controller.Create(duplicateMhsDto);

        // Assert: Skenario T03 - Duplikasi NIM ditolak dengan 409 Conflict
        var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.Equal(409, conflictResult.StatusCode);

        var errorResponse = Assert.IsType<ApiErrorResponseDto>(conflictResult.Value);
        Assert.Contains("sudah terdaftar", errorResponse.Message);
    }

    [Fact]
    public async Task GetMahasiswa_ProjectsNamaDosenWaliCorrectly()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var dosen = new Dosen
        {
            Nidn = "0587654321",
            NamaDosen = "Budi Santoso, M.Kom",
            Email = "budi@kampus.ac.id",
            Prodi = "Informatika"
        };
        context.Dosens.Add(dosen);
        await context.SaveChangesAsync();

        var mhs = new Mahasiswa
        {
            Nim = "71220045",
            NamaMahasiswa = "Dewi Lestari",
            Email = "dewi@kampus.ac.id",
            Angkatan = 2022,
            DosenId = dosen.DosenId
        };
        context.Mahasiswas.Add(mhs);
        await context.SaveChangesAsync();

        var controller = new MahasiswaController(context, NullLogger<MahasiswaController>.Instance);

        // Act
        var result = await controller.GetById(mhs.MahasiswaId);

        // Assert: Memverifikasi proyeksi relasi NamaDosenWali muncul pada DTO
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var responseDto = Assert.IsType<MahasiswaResponseDto>(okResult.Value);

        Assert.Equal("Dewi Lestari", responseDto.NamaMahasiswa);
        Assert.Equal("Budi Santoso, M.Kom", responseDto.NamaDosenWali);
    }
}

