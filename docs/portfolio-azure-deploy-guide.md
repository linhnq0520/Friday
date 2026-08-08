# Hướng dẫn deploy Portfolio lên Azure Static Web Apps (miễn phí) + CI/CD GitHub

Tài liệu này hướng dẫn **từng bước** để chạy local, đẩy code lên GitHub, tạo Azure Static Web Apps (free), kết nối CI/CD, và kiểm tra site live.

Áp dụng cho project:

- Path: `src/UI/Friday.Portfolio`
- Stack: **Blazor WebAssembly** (.NET 10)
- Hosting: **Azure Static Web Apps — Free tier**
- CI/CD: **GitHub Actions** (file `.github/workflows/azure-static-web-apps-portfolio.yml`)

---

## Mục lục

1. [Kiến trúc & vì sao chọn Azure Static Web Apps](#1-kiến-trúc--vì-sao-chọn-azure-static-web-apps)
2. [Điều kiện tiên quyết](#2-điều-kiện-tiên-quyết)
3. [Chạy thử local](#3-chạy-thử-local)
4. [Đẩy branch lên GitHub](#4-đẩy-branch-lên-github)
5. [Tạo Azure Static Web App (Free)](#5-tạo-azure-static-web-app-free)
6. [Cấu hình GitHub Secret cho CI/CD](#6-cấu-hình-github-secret-cho-cicd)
7. [Chạy pipeline & xác nhận deploy](#7-chạy-pipeline--xác-nhận-deploy)
8. [Cập nhật nội dung CV trên site](#8-cập-nhật-nội-dung-cv-trên-site)
9. [Custom domain (tuỳ chọn)](#9-custom-domain-tuỳ-chọn)
10. [Giới hạn Free tier & chi phí](#10-giới-hạn-free-tier--chi-phí)
11. [Troubleshooting](#11-troubleshooting)
12. [Checklist nhanh](#12-checklist-nhanh)

---

## 1. Kiến trúc & vì sao chọn Azure Static Web Apps

```
GitHub repo (branch push)
        │
        ▼
GitHub Actions
  - setup-dotnet
  - dotnet publish (Blazor WASM)
  - Azure/static-web-apps-deploy
        │
        ▼
Azure Static Web Apps (Free)
  - phục vụ wwwroot (HTML/CSS/JS/_framework WASM)
  - URL dạng: https://<name>.azurestaticapps.net
```

**Vì sao phù hợp học tập + miễn phí:**

| Tiêu chí | Azure Static Web Apps Free | Azure App Service Free (F1) |
|---|---|---|
| Blazor WASM (static) | Rất phù hợp | Được nhưng dư thừa |
| CI/CD GitHub | Tích hợp sẵn | Tự cấu hình thêm |
| HTTPS / CDN | Có | Có (hạn chế) |
| Chi phí | $0 (free SKU) | $0 nhưng sleep khi idle, giới hạn CPU |
| Staging PR | Có (preview environment) | Không có sẵn |

Blazor WASM compile ra file tĩnh → không cần server ASP.NET chạy 24/7 → **Static Web Apps Free** là lựa chọn đúng.

---

## 2. Điều kiện tiên quyết

### 2.1. Phần mềm trên máy

1. [.NET 10 SDK](https://dotnet.microsoft.com/download)  
   Kiểm tra:

   ```powershell
   dotnet --version
   ```

   Kỳ vọng: `10.0.x` trở lên.

2. [Git](https://git-scm.com/)

3. Tài khoản [GitHub](https://github.com/) (repo đã có hoặc sẽ push lên)

4. Tài khoản [Azure](https://azure.microsoft.com/free/)  
   - Có thể dùng **Azure Free Account** (credit dùng thử) hoặc tài khoản trả phí — riêng **Static Web Apps Free SKU** không tính phí hosting (xem mục 10).
   - Cài [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) (tuỳ chọn, dùng khi tạo resource bằng CLI).

### 2.2. Quyền cần có

- Push được vào GitHub repo (hoặc fork riêng).
- Tạo resource trên Azure subscription của bạn.
- Thêm **GitHub Actions secret** trên repo (Settings → Secrets).

---

## 3. Chạy thử local

Mở terminal tại root repo `Friday`:

```powershell
cd E:\Workspace\My\Friday
dotnet restore src/UI/Friday.Portfolio/Friday.Portfolio.csproj
dotnet run --project src/UI/Friday.Portfolio/Friday.Portfolio.csproj
```

Hoặc publish rồi serve thư mục tĩnh (gần giống production hơn):

```powershell
dotnet publish src/UI/Friday.Portfolio/Friday.Portfolio.csproj -c Release -o .\artifacts\portfolio
```

Output tĩnh nằm tại:

```text
artifacts\portfolio\wwwroot\
```

Bạn có thể mở bằng bất kỳ static server nào, ví dụ:

```powershell
dotnet tool install -g dotnet-serve
dotnet serve --directory .\artifacts\portfolio\wwwroot -p 5500
```

Mở trình duyệt: `http://localhost:5500`

---

## 4. Đẩy branch lên GitHub

Branch hiện tại của feature: `feature/personal-portfolio`.

### 4.1. Commit (khi bạn sẵn sàng)

```powershell
git status
git add portfolio .github/workflows/azure-static-web-apps-portfolio.yml docs/portfolio-azure-deploy-guide.md
git commit -m "Add personal portfolio (Blazor WASM) with Azure SWA CI/CD docs"
```

> Chỉ commit khi bạn chủ động muốn. Không bắt buộc ngay sau khi gen code.

### 4.2. Push branch

```powershell
git push -u origin feature/personal-portfolio
```

### 4.3. (Khuyến nghị) Merge vào `main` hoặc `develop`

Workflow lắng nghe các branch:

- `main`
- `develop`
- `feature/personal-portfolio`

Sau khi ổn định, nên merge vào `main`/`develop` để pipeline production chạy từ branch chính.

---

## 5. Tạo Azure Static Web App (Free)

Có **2 cách**: Portal (GUI) hoặc Azure CLI. Làm **một** trong hai.

### Cách A — Azure Portal (khuyến nghị lần đầu)

1. Đăng nhập [https://portal.azure.com](https://portal.azure.com)
2. Ô tìm kiếm → gõ **Static Web Apps** → **Create**
3. Điền form:

   | Field | Giá trị gợi ý |
   |---|---|
   | Subscription | Subscription của bạn |
   | Resource Group | `rg-portfolio` (tạo mới) |
   | Name | `quoclinh-portfolio` (phải unique toàn cầu) |
   | Plan type | **Free** |
   | Region | chọn gần (ví dụ East Asia / Southeast Asia nếu có) |
   | Deployment details | **GitHub** (hoặc Other nếu bạn gắn secret thủ công) |

4. **Deployment details → GitHub**

   - Bấm **Sign in with GitHub** → authorize Azure.
   - Organization / Account: chọn account của bạn.
   - Repository: chọn repo `Friday` (hoặc repo bạn push portfolio vào).
   - Branch: `main` (hoặc `develop` / `feature/personal-portfolio`).
   - Build Presets: **Blazor**
   - App location: `/src/UI/Friday.Portfolio`
   - Api location: *(để trống)*
   - Output location: `wwwroot`

   > **Lưu ý:** Repo này đã có workflow tự viết (publish trước rồi deploy với `skip_app_build`).  
   > Nếu Portal tự tạo workflow khác, bạn có thể:
   > - xoá workflow auto-gen của Portal, **giữ** file `.github/workflows/azure-static-web-apps-portfolio.yml`, **hoặc**
   > - chọn **Other** ở Deployment source rồi làm mục 6 (thủ công) — cách này rõ ràng hơn khi học CI/CD.

5. Bấm **Review + create** → **Create**.
6. Sau khi xong, vào resource → **Overview** → copy **URL** (dạng `https://....azurestaticapps.net`).

#### Cách Portal “Other” (khuyến nghị với workflow có sẵn trong repo)

1. Create Static Web App như trên.
2. **Deployment details** chọn **Other** (không gắn GitHub ngay).
3. Create xong → vào resource → **Manage deployment token** → **Copy**.
4. Dùng token đó làm GitHub Secret (mục 6).

### Cách B — Azure CLI

```powershell
az login
az account set --subscription "<SUBSCRIPTION_ID_OR_NAME>"

az group create --name rg-portfolio --location eastasia

az staticwebapp create `
  --name quoclinh-portfolio `
  --resource-group rg-portfolio `
  --location eastasia `
  --sku Free
```

Lấy deployment token:

```powershell
az staticwebapp secrets list `
  --name quoclinh-portfolio `
  --resource-group rg-portfolio `
  --query "properties.apiKey" -o tsv
```

Copy giá trị in ra → dùng ở mục 6.

---

## 6. Cấu hình GitHub Secret cho CI/CD

Workflow cần secret tên đúng:

```text
AZURE_STATIC_WEB_APPS_API_TOKEN_PORTFOLIO
```

### Các bước trên GitHub

1. Mở repo trên GitHub.
2. **Settings** → **Secrets and variables** → **Actions**.
3. **New repository secret**
   - Name: `AZURE_STATIC_WEB_APPS_API_TOKEN_PORTFOLIO`
   - Secret: dán **deployment token** vừa copy từ Azure.
4. **Add secret**.

### Workflow đang dùng làm gì?

File: `.github/workflows/azure-static-web-apps-portfolio.yml`

1. Checkout code  
2. Setup .NET 10  
3. `dotnet publish` Blazor WASM ra `portfolio-publish/`  
4. Deploy thư mục `portfolio-publish/wwwroot` lên Azure Static Web Apps (`skip_app_build: true`)  
5. Khi đóng PR: đóng staging environment

Trigger khi có thay đổi trong:

- `src/UI/Friday.Portfolio/**`
- `.github/workflows/azure-static-web-apps-portfolio.yml`

---

## 7. Chạy pipeline & xác nhận deploy

### 7.1. Kích hoạt workflow

Sau khi đã có secret, push một commit nhỏ hoặc re-run workflow:

```powershell
git commit --allow-empty -m "ci: trigger portfolio deploy"
git push
```

Hoặc vào GitHub → **Actions** → chọn workflow **Portfolio — Azure Static Web Apps** → **Run workflow**.

### 7.2. Xem log

1. GitHub → **Actions**
2. Click run mới nhất
3. Kiểm tra step:
   - Setup .NET SDK → xanh
   - Restore & publish → xanh
   - Deploy to Azure Static Web Apps → xanh

### 7.3. Mở site

Azure Portal → Static Web App → **URL**  
hoặc xem output của action deploy (thường có link).

Kiểm tra:

- [ ] Hero hiện tên **Nguyen Quoc Linh**
- [ ] Sections About / Experience / Projects / Skills / Contact
- [ ] Link LinkedIn, NuGet, GitHub Bon mở được
- [ ] Refresh trang `/` không lỗi 404 (nhờ `staticwebapp.config.json`)

---

## 8. Cập nhật nội dung site (Profile / Blog / Courses)

Site gồm 3 trụ (Profile · Blog · Courses):

| Nội dung | File |
|---|---|
| Profile / CV | `src/UI/Friday.Portfolio/Models/ProfileData.cs` |
| Catalog blog + courses | `src/UI/Friday.Portfolio/Models/SiteContent.cs` |
| Body bài viết (Markdown) | `src/UI/Friday.Portfolio/wwwroot/content/blog/*.md` |

Sửa text → commit → push → GitHub Actions tự deploy lại (vài phút).

CSS / layout:

- `wwwroot/css/app.css`
- `Components/*.razor`
- `Pages/` (`Home`, `Blog`, `Courses`, `About`)

---

## 9. Custom domain (tuỳ chọn)

1. Azure Portal → Static Web App → **Custom domains** → **Add**
2. Nhập domain (ví dụ `linh.dev` hoặc `www.yourdomain.com`)
3. Thêm DNS record theo hướng dẫn Azure (thường CNAME → `<app>.azurestaticapps.net`)
4. Đợi validate + HTTPS certificate (Let’s Encrypt tự cấp trên SWA)

Free tier **có hỗ trợ custom domain**.

---

## 10. Giới hạn Free tier & chi phí

Theo tài liệu Microsoft (có thể thay đổi theo thời điểm):

| Hạng mục | Free SKU (ước lượng) |
|---|---|
| Bandwidth | ~100 GB / tháng |
| Storage | ~0.5 GB |
| Custom domain | Có |
| Staging environments (PR) | Có (giới hạn số lượng) |
| Azure Functions (managed API) | Có nhưng giới hạn execution |

**Lưu ý học tập:**

- Không lưu secret / API key trong code frontend Blazor WASM (user có thể đọc được).
- Portfolio hiện là static site → không cần database, không phát sinh chi phí SQL.
- Nếu sau này thêm API (contact form, CMS), cân nhắc Azure Functions kèm SWA hoặc tách backend.

Luôn đối chiếu: [Azure Static Web Apps pricing](https://azure.microsoft.com/pricing/details/app-service/static/).

---

## 11. Troubleshooting

### 11.1. Workflow fail: `AZURE_STATIC_WEB_APPS_API_TOKEN_PORTFOLIO` missing

→ Chưa tạo secret hoặc sai tên. Xem lại mục 6 (tên phải khớp 100%).

### 11.2. Deploy thành công nhưng trang trắng / 404

1. Xác nhận `wwwroot/staticwebapp.config.json` được publish (có trong artifact).
2. Hard refresh (`Ctrl+F5`).
3. Mở DevTools → Network: `_framework/blazor.webassembly.js` phải **200**.
4. Nếu base href sai khi host subdirectory: đảm bảo `<base href="/" />` và SWA host ở root domain.

### 11.3. Build fail trên GitHub: SDK không tìm thấy

Workflow đã pin `dotnet-version: "10.0.x"`.  
Nếu GitHub runner chưa có, `actions/setup-dotnet` sẽ tải về.  
Kiểm tra log step **Setup .NET SDK**.

### 11.4. Portal tạo workflow thứ hai bị conflict

Xoá workflow auto-gen của Portal, giữ:

```text
.github/workflows/azure-static-web-apps-portfolio.yml
```

Chỉ cần **một** workflow deploy một Static Web App.

### 11.5. Token bị lộ / rotate token

1. Azure Portal → Static Web App → **Manage deployment token** → **Reset**
2. Cập nhật lại GitHub Secret
3. Re-run workflow

### 11.6. Muốn deploy repo riêng (tách khỏi Friday)

1. Tạo repo mới, copy thư mục `src/UI/Friday.Portfolio/` + workflow (chỉnh paths cho khớp).
2. Tạo Static Web App gắn repo mới.
3. Set secret tương tự.

---

## 12. Checklist nhanh

- [ ] `dotnet run` local OK
- [ ] Branch đã push GitHub
- [ ] Azure Static Web App **Free** đã tạo
- [ ] Deployment token đã copy
- [ ] GitHub Secret `AZURE_STATIC_WEB_APPS_API_TOKEN_PORTFOLIO` đã set
- [ ] Actions run xanh
- [ ] Mở URL `.azurestaticapps.net` thấy portfolio
- [ ] (Tuỳ chọn) Custom domain

---

## Tài liệu tham khảo

- [Azure Static Web Apps overview](https://learn.microsoft.com/azure/static-web-apps/overview)
- [Deploy Blazor to Azure Static Web Apps](https://learn.microsoft.com/azure/static-web-apps/deploy-blazor)
- [GitHub Actions for Azure Static Web Apps](https://learn.microsoft.com/azure/static-web-apps/github-actions-workflow)
- [staticwebapp.config.json](https://learn.microsoft.com/azure/static-web-apps/configuration)
- [Blazor WebAssembly hosting models](https://learn.microsoft.com/aspnet/core/blazor/hosting-models)

---

## Liên hệ / nội dung site

Nội dung lấy từ CV: `docs/Nguyen_Quoc_linh_DotNet_Developer.pdf`  
Owner: Nguyen Quoc Linh (Thomas) — `zquoclinh@gmail.com`
