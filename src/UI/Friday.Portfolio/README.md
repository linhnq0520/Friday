# Friday.Portfolio

Trang web cá nhân của **Nguyen Quoc Linh (Thomas)** — Backend Engineer.

Mục tiêu: học Blazor WASM, static hosting, Markdown content, và CI/CD lên Azure miễn phí; đồng thời có nơi công khai **Profile**, **Blog**, và **Courses** (catalog đang *In progress*).

| | |
|---|---|
| Path | `src/UI/Friday.Portfolio` |
| Stack | Blazor WebAssembly · .NET 10 · Markdig |
| Hosting mục tiêu | Azure Static Web Apps (Free) |
| CI/CD | GitHub Actions → `.github/workflows/azure-static-web-apps-portfolio.yml` |
| Deploy guide | [docs/portfolio-azure-deploy-guide.md](../../../docs/portfolio-azure-deploy-guide.md) |

---

## 1. Mục đích & phạm vi

**Có trong scope**

- SPA Blazor WASM: Home, Blog, Courses, Profile/About
- Nội dung tĩnh (CV, catalog, Markdown) — không cần API/DB lúc runtime
- Deploy free-tier + pipeline GitHub

**Ngoài scope (hiện tại)**

- CMS / admin viết bài
- Auth, thanh toán course, progress tracking học viên
- Gọi `Friday.API` (portfolio độc lập với modular monolith)

Courses chưa có lesson đầy đủ → status **In progress** (ví dụ roadmap). Blog sample là bài mẫu để học luồng Markdown → HTML.

---

## 2. Vị trí trong monorepo Friday

```text
Friday/
├── src/
│   ├── API/                 # Friday.API (modular monolith) — không phụ thuộc portfolio
│   ├── BuildingBlocks/
│   ├── Modules/
│   ├── Directory.Build.props
│   ├── Directory.Packages.props   # central package versions (dùng chung)
│   └── UI/
│       └── Friday.Portfolio/      # ← project này
├── .github/workflows/
│   └── azure-static-web-apps-portfolio.yml
└── docs/
    └── portfolio-azure-deploy-guide.md
```

- Đặt dưới `src/UI/` theo convention UI Blazor của repo (cùng hướng với `Friday.AdminPortal` trên branch `feature/blazor`).
- Dùng **central package management**: version khai báo ở `src/Directory.Packages.props`, csproj chỉ `PackageReference` không ghi Version.
- `Directory.Build.props` set sẵn `net10.0`, nullable, implicit usings.

Portfolio **không reference** BuildingBlocks / Modules — giữ deploy static đơn giản.

---

## 3. Architecture

### 3.1. Hosting model

```text
┌─────────────────────────────────────────────────────────┐
│  Browser                                                │
│  ┌───────────────────────────────────────────────────┐  │
│  │  Blazor WebAssembly (Friday.Portfolio.dll)        │  │
│  │  Router → Pages → Components                      │  │
│  │  HttpClient → wwwroot/content/**/*.md             │  │
│  │  Markdig → HTML (MarkupString)                    │  │
│  └───────────────────────────────────────────────────┘  │
└───────────────────────────┬─────────────────────────────┘
                            │ HTTPS (static files)
                            ▼
┌─────────────────────────────────────────────────────────┐
│  Azure Static Web Apps                                  │
│  wwwroot: index.html · css · _framework · content/      │
│  staticwebapp.config.json (SPA fallback + headers)      │
└─────────────────────────────────────────────────────────┘
```

Blazor WASM chạy **hoàn toàn trên client**. Sau `dotnet publish`, output là file tĩnh → phù hợp Static Web Apps Free (không cần App Service chạy 24/7).

### 3.2. Layering trong project

| Layer | Thư mục | Trách nhiệm |
|---|---|---|
| Entry | `Program.cs`, `App.razor` | Host builder, root components, DI |
| Presentation | `Pages/`, `Layout/`, `Components/` | UI + routing |
| Content model | `Models/` | CV (`ProfileData`), catalog (`SiteContent`), records |
| Application service | `Services/MarkdownContentService.cs` | Fetch Markdown + render HTML |
| Static assets | `wwwroot/` | CSS, `index.html`, Markdown, SWA config |

Không có Domain/Infrastructure kiểu API — đủ cho static content site.

### 3.3. Content architecture

Hai nguồn nội dung tách biệt:

1. **Compile-time catalog (C#)** — `ProfileData`, `SiteContent`  
   Metadata: slug, title, tags, dates, course status, paths tới file Markdown.
2. **Runtime files (wwwroot)** — `content/blog/*.md`  
   Body bài viết; load bằng `HttpClient` khi mở `/blog/{slug}`.

```text
SiteContent.Posts[slug]
        │
        ├─ Title / Summary / Tags / Published   → render list & header
        └─ MarkdownPath                         → MarkdownContentService
                                                      │
                                                      ▼
                                               GET content/blog/{slug}.md
                                                      │
                                                      ▼
                                               Markdig → MarkupString
```

**Vì sao tách thế này**

- List blog/courses không cần đọc mọi file Markdown.
- Sửa metadata (featured, order) không đụng body.
- Sau này có thể thay catalog bằng JSON/API mà UI gần như giữ nguyên.

### 3.4. Routing (SPA)

| Route | Page | Ý nghĩa |
|---|---|---|
| `/` | `Home` | Hero + publish pillars + latest posts + courses teaser |
| `/blog` | `Blog` | Danh sách bài |
| `/blog/{Slug}` | `BlogPost` | Chi tiết + Markdown |
| `/courses` | `Courses` | Catalog (In progress) |
| `/courses/{Slug}` | `CourseDetail` | Roadmap / outcomes |
| `/about` | `About` | Full profile (CV sections) |
| `/not-found` | `NotFound` | 404 |

`MainLayout` chứa header/footer. Deep link trên Azure nhờ `navigationFallback` → `index.html` trong `staticwebapp.config.json`.

---

## 4. Luồng xử lý chính

### 4.1. Startup

```text
Program.cs
  → WebAssemblyHostBuilder
  → RootComponents: App, HeadOutlet
  → DI: HttpClient (BaseAddress = host), MarkdownContentService
  → RunAsync
```

Giống pattern DI `HttpClient` scoped của các Blazor WASM app khác trong Friday UI.

### 4.2. Xem danh sách blog

```text
User → /blog
  → Blog.razor
  → SiteContent.Posts (in-memory, sorted)
  → PostList.razor
```

Không I/O mạng cho list.

### 4.3. Đọc một bài

```text
User → /blog/{slug}
  → BlogPost.OnParametersSetAsync
  → SiteContent.FindPost(slug)
       ├─ null → "Post not found"
       └─ article → MarkdownContentService.GetHtmlAsync(path)
                      → HttpClient.GetStringAsync
                      → Markdig pipeline (advanced extensions)
                      → cache Dictionary<path, MarkupString>
                      → bind @html trong .markdown-body
```

Cache trong scope service (lifetime của WASM circuit/tab) tránh fetch lại khi navigate qua lại.

### 4.4. Courses (In progress)

```text
User → /courses hoặc /courses/{slug}
  → SiteContent.Courses / FindCourse
  → UI hiển thị Status = "In progress"
  → Chưa có lesson player / progress API
```

Đây là **placeholder có chủ đích** để giữ IA (information architecture) sẵn khi viết lesson thật.

### 4.5. Profile

```text
User → /about
  → ProfileData.* + section components
       About / Experience / Projects / Skills / Education / Contact
```

Toàn bộ CV nằm trong `Models/ProfileData.cs` (nguồn từ file CV PDF trong `docs/`).

### 4.6. CI/CD deploy

```text
git push (paths: src/UI/Friday.Portfolio/**, Directory.*.props, workflow)
        │
        ▼
GitHub Actions
  setup-dotnet 10
  dotnet publish -c Release -o portfolio-publish
  Azure/static-web-apps-deploy
    app_location = portfolio-publish/wwwroot
    skip_app_build = true
        │
        ▼
Azure Static Web Apps (Free)
  URL: https://<name>.azurestaticapps.net
```

Secret bắt buộc: `AZURE_STATIC_WEB_APPS_API_TOKEN_PORTFOLIO`.  
Chi tiết từng bước Azure/GitHub: xem deploy guide.

---

## 5. Cấu trúc thư mục

```text
src/UI/Friday.Portfolio/
├── App.razor
├── Program.cs
├── _Imports.razor
├── Friday.Portfolio.csproj
├── Components/          # UI tái sử dụng (PostList, CourseList, CV sections)
├── Layout/MainLayout.razor
├── Models/
│   ├── ContentModels.cs # records: BlogArticle, Course, ExperienceItem, ...
│   ├── ProfileData.cs   # CV / profile constants
│   └── SiteContent.cs   # blog + course catalog
├── Pages/               # routable pages
├── Services/
│   └── MarkdownContentService.cs
├── Properties/launchSettings.json
└── wwwroot/
    ├── index.html
    ├── css/app.css
    ├── content/blog/*.md
    └── staticwebapp.config.json
```

---

## 6. Dependencies

| Package | Vai trò |
|---|---|
| `Microsoft.AspNetCore.Components.WebAssembly` | Runtime Blazor WASM |
| `Microsoft.AspNetCore.Components.WebAssembly.DevServer` | Dev server (PrivateAssets) |
| `Markdig` | Markdown → HTML |

Version: `src/Directory.Packages.props`.

---

## 7. Chạy local

Yêu cầu: .NET 10 SDK.

```powershell
cd E:\Workspace\My\Friday
dotnet run --project src/UI/Friday.Portfolio/Friday.Portfolio.csproj
```

Mặc định (launchSettings): `https://localhost:7257` / `http://localhost:5053`.

Publish giống CI:

```powershell
dotnet publish src/UI/Friday.Portfolio/Friday.Portfolio.csproj -c Release -o .\artifacts\portfolio
# static output: artifacts\portfolio\wwwroot
```

---

## 8. Thêm nội dung mới

### Bài blog

1. Tạo `wwwroot/content/blog/my-slug.md`
2. Thêm entry trong `SiteContent.Posts` (slug khớp path)
3. Chạy local → mở `/blog/my-slug`

### Course (khi còn In progress)

1. Thêm record trong `SiteContent.Courses` với `Status: "In progress"`
2. Khi có lesson thật: đổi status (ví dụ `"Available"`) và bổ sung UI/player sau

### Cập nhật CV

Sửa `Models/ProfileData.cs` → commit → push → pipeline deploy.

---

## 9. Best practices đang áp dụng

- Central package versions (đồng bộ monorepo)
- Tách catalog metadata khỏi body Markdown
- SPA fallback + security headers trên SWA
- Secret chỉ nằm ở GitHub Actions / Azure — không commit token
- `prefers-reduced-motion` trong CSS
- Courses chưa sẵn sàng thì ghi rõ **In progress**, không giả vờ hoàn chỉnh

---

## 10. Hướng mở rộng (tuỳ chọn)

| Ý tưởng | Ghi chú |
|---|---|
| JSON catalog thay vì C# static | Vẫn dùng `MarkdownContentService` |
| Azure Functions contact form | SWA managed functions |
| Gắn `Friday.API` cho dynamic posts | Khi cần CMS/auth |
| PWA (`-p` Blazor template) | Offline đọc bài |

---

## Liên hệ

- Email: zquoclinh@gmail.com  
- LinkedIn: linkedin.com/in/quoclinh0520  
- CV nguồn: `docs/Nguyen_Quoc_linh_DotNet_Developer.pdf`
