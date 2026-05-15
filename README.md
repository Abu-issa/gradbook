# 🎓 GradBook — Digital Graduation Memory Book

> A premium, real-time digital graduation platform built for **Eng. Mohammed Abu-Issa** — where friends, family, and classmates can leave heartfelt messages, share memories, and celebrate this milestone forever.

---

## ✨ Features

| Feature | Description |
|---|---|
| 🏠 **Landing Page** | Stunning black & gold hero with animations, countdown timer, and floating particles |
| 💌 **Message Wall** | Live message wall with SignalR real-time updates |
| 📸 **Memories Gallery** | Masonry gallery with lightbox preview |
| 👤 **Admin Dashboard** | Full content management — approve, reject, upload |
| 🔴 **Real-time** | SignalR pushes new approved messages to all visitors instantly |
| 🌙 **Dark / Light Mode** | Persistent theme preference via localStorage |
| 📱 **Fully Responsive** | Desktop, tablet, and mobile-ready |
| 🔗 **Share** | WhatsApp, Facebook, and copy-link share buttons |
| 📷 **Image Uploads** | Visitors can attach photos to messages |


---

## 🏗️ Architecture — Clean Architecture

```
GradBook.sln
├── GradBook.Domain          # Entities (Graduate, Message, Memory, Visitor)
├── GradBook.Application     # Interfaces, DTOs, ViewModels
├── GradBook.Infrastructure  # EF Core DbContext, Services, Migrations
└── GradBook.Web             # ASP.NET Core MVC, Controllers, Views, SignalR Hub
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) or LocalDB (included with Visual Studio)

### 1. Clone / Extract
```bash
cd GradBook
```

### 2. Update Connection String
Edit `GradBook.Web/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=GradBookDb;Trusted_Connection=True;"
  }
}
```

### 3. Apply Migrations & Run
```bash
cd GradBook.Web
dotnet ef database update --project ../GradBook.Infrastructure
dotnet run
```

Or from solution root:
```bash
dotnet run --project GradBook.Web
```

Open: `https://localhost:5001`

---

## 🐳 Docker Deployment

```bash
# Build and run everything (app + SQL Server)
docker-compose up --build

# App runs at http://localhost:8080
```

---

## ☁️ Deployment on Render / Railway

### Environment Variables to set:
| Key | Value |
|---|---|
| `ConnectionStrings__DefaultConnection` | Your SQL Server connection string |
| `AdminCredentials__Username` | `admin` |
| `AdminCredentials__Password` | Your secure password |
| `ASPNETCORE_ENVIRONMENT` | `Production` |

### Render
1. Connect GitHub repo
2. Set Build Command: `dotnet publish GradBook.Web/GradBook.Web.csproj -c Release -o out`
3. Set Start Command: `dotnet out/GradBook.Web.dll`
4. Add environment variables above

### Railway
1. Connect GitHub repo
2. Railway auto-detects .NET
3. Add environment variables
4. Deploy

---

## 🔐 Admin Panel

Navigate to `/Admin/Login`

Default credentials (change in `appsettings.json`):
- **Username:** `admin`
- **Password:** `GradBook@2025!`

Admin capabilities:
- ✅ Approve / reject messages
- 🗑️ Delete messages
- 📸 Upload gallery memories
- 📊 View visitor analytics
- 🔳 Generate and print QR code

---

## 🎨 Design System

| Token | Value |
|---|---|
| Primary Gold | `#C9A84C` |
| Dark Background | `#0d0d0d` |
| Surface | `#1a1a1a` |
| Font Display | Cinzel (serif) |
| Font Body | Jost (sans-serif) |
| Font Accent | Cormorant Garamond |

---

## 📁 Project Structure

```
GradBook.Web/
├── Controllers/
│   ├── HomeController.cs       # Landing page, visitor tracking
│   ├── MessagesController.cs   # Public message wall & submission
│   ├── MemoriesController.cs   # Public gallery
│   └── AdminController.cs      # Secure admin panel
├── Views/
│   ├── Home/Index.cshtml       # Hero, countdown, stats, previews
│   ├── Messages/
│   │   ├── Index.cshtml        # Live message wall (SignalR)
│   │   ├── Create.cshtml       # Message submission form
│   │   └── Thanks.cshtml       # Post-submission thank you
│   ├── Memories/Index.cshtml   # Masonry gallery + lightbox
│   ├── Admin/
│   │   ├── Login.cshtml
│   │   ├── Dashboard.cshtml
│   │   ├── Messages.cshtml
│   │   ├── Memories.cshtml
│   │   ├── CreateMemory.cshtml
│   │   └── QrCode.cshtml
│   └── Shared/
│       ├── _Layout.cshtml      # Main layout (navbar, footer, particles)
│       └── _AdminLayout.cshtml # Admin sidebar layout
├── Hubs/MessageHub.cs          # SignalR real-time hub
├── ViewModels/ViewModels.cs    # All view models
└── wwwroot/
    ├── css/site.css            # Full black & gold premium theme
    ├── css/admin.css           # Admin panel styles
    ├── js/site.js              # Theme toggle, AOS, navbar, helpers
    ├── js/particles.js         # Floating particle canvas animation
    └── uploads/                # User-uploaded images (gitignored)
```

---

## 🔧 Tech Stack

- **Backend:** ASP.NET Core 8 MVC, Entity Framework Core, SignalR
- **Database:** SQL Server / LocalDB
- **Auth:** Cookie Authentication
- **Frontend:** Bootstrap 5, AOS, SweetAlert2, Font Awesome
- **Fonts:** Cinzel, Cormorant Garamond, Jost (Google Fonts)
- **Architecture:** Clean Architecture (Domain → Application → Infrastructure → Web)

---

## 📝 Customization

### Change Graduate Info
Edit the seed data in `GradBook.Infrastructure/Data/GradBookDbContext.cs`:
```csharp
entity.HasData(new Graduate
{
    Id = 1,
    FullName = "Your Graduate Name",
    Title = "Bachelor of ...",
    Bio = "Your bio here...",
    GraduationDate = new DateTime(2025, 6, 15)
});
```
Then run: `dotnet ef database update --project GradBook.Infrastructure`

### Change Admin Password
Update `appsettings.json`:
```json
"AdminCredentials": {
  "Username": "admin",
  "Password": "YourNewSecurePassword!"
}
```

### Add Graduate Photo
Upload a photo and set `MainImageUrl` in the database, or add an admin endpoint to update the graduate profile.

---

## 📄 License

Built with ❤️ for **Eng. Mohammed Abu-Issa** — Class of 2026.

> *"Every great journey begins with a single step. Yours led here — to this proud moment. Now, the world awaits."*
