# WaliSync (Sistem Informasi Akademik Sederhana)

WaliSync adalah sistem informasi akademik berbasis arsitektur *Client-Server Separation* yang dibangun menggunakan **.NET 10 MAUI Blazor Hybrid** (Frontend Client) dan **ASP.NET Core Web API + SQLite EF Core** (Backend Server), dirancang untuk mempermudah Bagian Administrasi Akademik dalam mengelola data dosen, mahasiswa, dan plotting dosen wali.

---

## 1. Arsitektur Proyek

Solution (`WaliSync.slnx`) terdiri dari 3 sub-proyek:
1. **`WaliSync`** (`WaliSync/WaliSync.csproj`): Klien .NET MAUI Blazor Hybrid (Mendukung target Windows Desktop dan Android).
2. **`WaliSync.Api`** (`WaliSync.Api/WaliSync.Api.csproj`): Backend ASP.NET Core Web API dengan SQLite EF Core.
3. **`WaliSync.Api.Tests`** (`WaliSync.Api.Tests/WaliSync.Api.Tests.csproj`): Proyek pengujian unit otomatis menggunakan xUnit dan EF Core InMemory Database.

---

## 2. Persyaratan Sistem
- [.NET SDK 10.0](https://dotnet.microsoft.com/download) atau yang lebih baru.
- Workload `maui` (untuk kompilasi aplikasi mobile/desktop).
- Android SDK & kabel data (untuk pengujian di perangkat fisik Android).

---

## 3. Panduan Menjalankan Aplikasi

### A. Menjalankan Backend API Server
Buka terminal dan jalankan perintah:
```bash
dotnet run --project WaliSync.Api/WaliSync.Api.csproj
```
API akan aktif pada: `http://localhost:5000`
*Catatan:* Saat pertama kali dijalankan, SQLite database `AcademicDb.db` akan dibuat secara otomatis dan disemai (*auto-seed*) dengan data fiktif (Dr. Anita Wijaya, Budi Santoso M.Kom, Jimmy Nugraha Putra, dll.).

### B. Menjalankan Unit Tests
Untuk memverifikasi seluruh aturan bisnis dan relasi database:
```bash
dotnet test WaliSync.Api.Tests/WaliSync.Api.Tests.csproj
```
Pengujian mencakup:
- Penolakan status `409 Conflict` saat menghapus Dosen yang masih memiliki mahasiswa bimbingan aktif (Skenario T07).
- Suksesnya status `204 NoContent` saat menghapus Dosen yang tidak memiliki bimbingan.
- Penolakan status `409 Conflict` saat memasukkan NIDN atau NIM duplikat (Skenario T03).
- Verifikasi proyeksi nama Dosen Wali pada hasil query Mahasiswa.

### C. Menjalankan Frontend di Windows Desktop
Buka terminal baru dan jalankan:
```bash
dotnet build WaliSync/WaliSync.csproj -t:Run -f net10.0-windows10.0.19041.0
```
Aplikasi desktop akan terbuka dengan sidebar navigasi, responsif, dan terhubung langsung ke `http://localhost:5000/`.

### D. Menjalankan Frontend di Android (Real Device via USB)
1. Hubungkan ponsel Android ke laptop via kabel data dengan mode *USB Debugging* aktif.
2. Jalankan perintah *USB Port Forwarding* agar aplikasi di HP dapat memanggil API localhost laptop:
   ```bash
   adb reverse tcp:5000 tcp:5000
   ```
3. Deploy dan jalankan aplikasi ke perangkat Android:
   ```bash
   dotnet build WaliSync/WaliSync.csproj -t:Run -f net10.0-android
   ```

---

## 4. Fitur Utama & Kepatuhan Spesifikasi
- **Tema Desain Eduplex:** Sesuai `DESIGN.md` dengan palet warna Canvas (`#F4F4F2`), Surface (`#FFFFFF`), Sidebar (`#1A1B23`), dan Aksen Lime (`#D2F366`).
- **Floating Row-Cards:** Menampilkan daftar data memanjang dengan efek hover halus dan tanpa garis border Bootstrap yang kaku.
- **Safe-Area Android:** Layout mobile dilengkapi padding aman atas sehingga teks tidak terpotong oleh bilah jam/status bar ponsel.
- **Proteksi Integritas Relasi (DeleteBehavior.Restrict):** Dosen yang berstatus wali aktif tidak dapat dihapus secara sepihak dan memunculkan toast peringatan 409 Conflict yang aman.
- **Data Fiktif Murni:** Seluruh data yang disemai mematuhi prinsip privasi data akademik (UU PDP) dengan menggunakan identitas buatan.

