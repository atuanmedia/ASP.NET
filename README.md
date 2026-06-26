# Đồ Án: Website Bán Đồ nội thất thông minh — CMS NguyenDuyAnhTuan

> **Họ và tên:** Nguyễn Duy Anh Tuấn 
> **MSSV:** 2123110503  
> **Lớp:** CCQ2311D  
> **Môn học:** ASP.NET

---

## Mục lục

1. [Giới thiệu dự án](#1-giới-thiệu-dự-án)
2. [Công nghệ sử dụng](#2-công-nghệ-sử-dụng)
3. [Cấu trúc dự án](#3-cấu-trúc-dự-án)
4. [Cài đặt & Khởi chạy](#4-cài-đặt--khởi-chạy)
5. [Cấu hình biến môi trường (.env)](#5-cấu-hình-biến-môi-trường-env)
6. [Cơ sở dữ liệu](#6-cơ-sở-dữ-liệu)
7. [Chức năng hệ thống](#7-chức-năng-hệ-thống)
8. [API Endpoints](#8-api-endpoints)
9. [Xác thực & Phân quyền](#9-xác-thực--phân-quyền)
10. [Ràng buộc & Validation](#10-ràng-buộc--validation)
11. [Hướng dẫn kiểm thử](#11-hướng-dẫn-kiểm-thử)
12. [Các trường hợp lỗi thường gặp](#12-các-trường-hợp-lỗi-thường-gặp)
13. [Cấu hình & Triển khai](#13-cấu-hình--triển-khai)

---

## 1. Giới thiệu dự án

Website bán gốm sứ nghệ thuật **NganDesign** là hệ thống thương mại điện tử hoàn chỉnh gồm hai phần:

- **Admin Panel** (ASP.NET Core MVC): Quản trị viên quản lý sản phẩm, đơn hàng, bài viết, người dùng.
- **Frontend SPA** (React): Khách hàng duyệt sản phẩm, đặt hàng, quản lý tài khoản.

### Mục tiêu

| Tiêu chí | Chi tiết |
|---------|---------|
| Quản lý danh mục | Danh mục blog (Category) và danh mục sản phẩm (CategoryProduct) riêng biệt |
| Sản phẩm | CRUD đầy đủ, hỗ trợ nhiều ảnh (gallery), quản lý tồn kho |
| Đặt hàng | Giỏ hàng, checkout, trừ tồn kho, gửi email xác nhận |
| Tài khoản | Đăng ký/đăng nhập, quản lý hồ sơ, nhiều địa chỉ nhận hàng |
| Bài viết | Blog theo danh mục, chi tiết bài viết |

---

## 2. Công nghệ sử dụng

### Backend

| Thành phần | Phiên bản | Mục đích |
|-----------|----------|---------|
| ASP.NET Core | .NET 8.0 | Web framework (MVC + Web API) |
| Entity Framework Core | 8.0.27 | ORM, migrations |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.27 | Provider SQL Server |
| Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.28 | JWT cho API khách hàng |
| ASP.NET Core Cookie Auth | built-in | Đăng nhập admin |
| Swashbuckle.AspNetCore | 10.1.7 | Swagger UI tại `/swagger` |
| System.Net.Mail | built-in | Gửi email SMTP |
| BCrypt.Net-Next | — | Hash mật khẩu |

### Frontend

| Thành phần | Phiên bản | Mục đích |
|-----------|----------|---------|
| React | 19.2.6 | UI framework |
| react-router-dom | 6.30.3 | Client-side routing |
| axios | 1.17.0 | HTTP client gọi API |
| react-icons | 5.6.0 | Bộ icon (Feather Icons) |
| react-scripts | 5.0.1 | Build tool (CRA) |

### Database & Công cụ

| Thành phần | Chi tiết |
|-----------|---------|
| SQL Server | MSI\SQLEXPRESS02 |
| Database | CMS_NGUYENDUYANHTUAN_DB |
| SSMS | Quản lý DB thủ công |
| Visual Studio / VS Code | IDE |

---

## 3. Cấu trúc dự án

```
CMS_NguyenNgocBaoNgan/
│
├── CMS.Backend/                        # ASP.NET Core Web App (.NET 8)
│   ├── Controllers/
│   │   ├── api/                        # REST API (JWT auth, CORS)
│   │   │   ├── AuthController.cs           register, login, forgot-password, change-password
│   │   │   ├── ProductsController.cs        CRUD + gallery upload
│   │   │   ├── CustomersController.cs       CRUD + quản lý địa chỉ
│   │   │   ├── OrdersController.cs          checkout + lịch sử đơn hàng
│   │   │   ├── PostsController.cs           bài viết (đọc)
│   │   │   ├── CategoriesController.cs      danh mục blog
│   │   │   └── CategoriesProductController.cs  danh mục sản phẩm
│   │   ├── AccountController.cs            Admin login/logout (Cookie)
│   │   ├── HomeController.cs               Dashboard admin
│   │   ├── UserController.cs               CRUD admin users
│   │   ├── PostController.cs               CRUD bài viết (MVC)
│   │   ├── CategoryController.cs           CRUD danh mục blog (MVC)
│   │   ├── CategoriesProductsController.cs CRUD danh mục SP (MVC)
│   │   ├── ProductController.cs            CRUD sản phẩm + gallery (MVC)
│   │   ├── CustomerController.cs           Quản lý khách hàng (MVC)
│   │   └── OrderController.cs              Quản lý đơn hàng (MVC)
│   ├── Views/                          # Razor Views (Admin UI)
│   │   ├── Shared/_LayoutAdmin.cshtml      Layout chính admin
│   │   ├── Account/Login.cshtml
│   │   ├── Home/Index.cshtml               Dashboard
│   │   ├── Product/                        Index, Create, Edit
│   │   ├── Post/                           Index, Create, Edit
│   │   ├── Category/                       Index, Create, Edit
│   │   ├── CategoriesProducts/             Index, Create, Edit
│   │   ├── Customer/Index.cshtml
│   │   ├── Order/                          Index, Details
│   │   └── User/                           Index, Create, Edit
│   ├── Services/
│   │   ├── IEmailService.cs
│   │   ├── EmailService.cs             SMTP via System.Net.Mail (Gmail)
│   │   └── PasswordHelper.cs           BCrypt hash/verify
│   ├── wwwroot/uploads/                Ảnh upload của sản phẩm
│   ├── Program.cs                      DI, JWT, Cookie, CORS, Swagger
│   ├── appsettings.json
│   └── CMS.Backend.csproj
│
├── CMS.Data/                           # Class Library — EF Core
│   ├── Entities/
│   │   ├── Category.cs                 Danh mục blog
│   │   ├── Post.cs                     Bài viết
│   │   ├── User.cs                     Admin user
│   │   ├── CategoryProduct.cs          Danh mục sản phẩm
│   │   ├── Product.cs                  Sản phẩm
│   │   ├── ProductImage.cs             Ảnh phụ sản phẩm
│   │   ├── Customer.cs                 Khách hàng
│   │   ├── CustomerAddress.cs          Địa chỉ giao hàng
│   │   ├── Order.cs                    Đơn hàng
│   │   ├── OrderDetail.cs              Chi tiết đơn hàng
│   │   └── Advertisement.cs            Banner quảng cáo
│   ├── ApplicationDbContext.cs         Unique indexes, cascade delete
│   ├── Migrations/                     Lịch sử migration EF
│   └── CMS.Data.csproj
│
└── cms.frontend/                       # React 19 SPA
    ├── .env                            ← Biến môi trường DEV (không commit git)
    ├── .env.development                ← Ghi đè khi npm start
    ├── .env.production                 ← Ghi đè khi npm run build
    ├── .env.example                    ← Template mẫu (commit lên git)
    ├── public/
    └── src/
        ├── config/
        │   └── constants.js            ★ Hằng số từ .env: API_BASE_URL, IMAGE_BASE_URL, getImageUrl()
        ├── api/
        │   └── axiosClient.js          baseURL từ REACT_APP_API_URL + JWT header tự động
        ├── context/
        │   ├── AuthContext.jsx         user, login, logout, updateUser + localStorage
        │   └── CartContext.jsx         items, addToCart, removeFromCart, clearCart + resolve ảnh
        ├── services/
        │   ├── authService.js          register, login, forgotPassword, changePassword
        │   ├── orderService.js         checkout, getMyOrders, getOrderDetail
        │   └── addressService.js       CRUD địa chỉ nhận hàng
        ├── components/
        │   ├── Layout/                 Header, Footer
        │   ├── Product/ProductCard.jsx getImageUrl() để hiển thị ảnh sản phẩm
        │   └── Blog/BlogCardList, BlogCategoryList
        └── pages/
            ├── home/Home.jsx           Trang chủ
            ├── product/
            │   ├── Product.jsx         Danh sách (phân trang 9/trang)
            │   ├── ProductDetail.jsx   Chi tiết + gallery + tab + sản phẩm liên quan
            │   ├── CategoryProductList.jsx  Lọc theo danh mục
            │   └── ProductSearch.jsx   Tìm kiếm (getImageUrl() cho ảnh kết quả)
            ├── blog/
            │   ├── Blog.jsx            Danh sách bài viết
            │   ├── BlogDetail.jsx      Chi tiết bài viết
            │   └── BlogCategory.jsx    Blog theo danh mục
            ├── cart/
            │   ├── Cart.jsx            Giỏ hàng
            │   └── Checkout.jsx        Thanh toán + chọn địa chỉ
            ├── auth/
            │   ├── Login.jsx
            │   ├── Register.jsx
            │   └── ForgotPassword.jsx
            ├── Account.jsx             Profile + Đơn hàng + Địa chỉ + Đổi mật khẩu
            ├── ComingSoon.jsx          Trang đang phát triển
            └── App.js                  Routes định nghĩa
```

---

## 4. Cài đặt & Khởi chạy

### Yêu cầu môi trường

- .NET 8.0 SDK
- Node.js >= 18
- SQL Server (Express hoặc Developer Edition)
- Visual Studio 2022 / VS Code

### Bước 1 — Clone & cấu hình DB

```powershell
# Mở appsettings.json, sửa connection string
# CMS.Backend/appsettings.json
"DefaultConnection": "Server=<TEN_SERVER>;Database=<TEN_DB>;Trusted_Connection=True;TrustServerCertificate=True"
```

### Bước 2 — Tạo database bằng EF Migration

```powershell
# Từ thư mục gốc solution
# Bước 2a: Tạo migration ban đầu (nếu chưa có)
dotnet ef migrations add InitialCreate `
  --project CMS.Data/CMS.Data.csproj `
  --startup-project CMS.Backend/CMS.Backend.csproj

# Bước 2b: Apply migration lên DB
dotnet ef database update `
  --project CMS.Data/CMS.Data.csproj `
  --startup-project CMS.Backend/CMS.Backend.csproj
```

> **Hoặc** chạy trực tiếp SQL trong SSMS (xem file [TODO.md](TODO.md) phần "Lịch sử Migration").

### Bước 3 — Tạo bảng bổ sung (nếu apply migration thủ công)

```sql
-- Chạy trong SSMS với DB đã chọn

-- Bảng lưu ảnh sản phẩm
CREATE TABLE ProductImages (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    Url NVARCHAR(500) NOT NULL DEFAULT '',
    SortOrder INT NOT NULL DEFAULT 0,
    CONSTRAINT FK_ProductImages_Products
        FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
);

-- Bảng địa chỉ nhận hàng của khách
CREATE TABLE CustomerAddresses (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL,
    RecipientName NVARCHAR(100) NOT NULL DEFAULT '',
    Phone NVARCHAR(20) NULL,
    Address NVARCHAR(500) NOT NULL DEFAULT '',
    IsDefault BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_CustomerAddresses_Customers
        FOREIGN KEY (CustomerId) REFERENCES Customers(Id) ON DELETE CASCADE
);

-- Thêm cột giao hàng vào Orders
ALTER TABLE Orders ADD ShippingAddress NVARCHAR(500) NULL;
ALTER TABLE Orders ADD RecipientName   NVARCHAR(100) NULL;
ALTER TABLE Orders ADD RecipientPhone  NVARCHAR(20)  NULL;
```

### Bước 4 — Tạo tài khoản admin đầu tiên

```sql
-- Mật khẩu: Admin@123 (hash BCrypt)
INSERT INTO Users (Username, PasswordHash, FullName, Role)
VALUES (
    'admin',
    '$2a$11$7XCFjtIbEFc2fhMMhxfHT.GtPeKTSLn1n7XH/2OiV5AiVkXJkO6ni',
    'Quản trị viên',
    'Admin'
);
```

### Bước 5 — Chạy Backend

```powershell
cd CMS.Backend
dotnet run --urls "https://localhost:7090"

# Swagger UI: https://localhost:7090/swagger
# Admin panel: https://localhost:7090/Account/Login
```

### Bước 6 — Cấu hình biến môi trường Frontend

```bash
cd cms.frontend

# File .env đã có sẵn giá trị mặc định cho localhost, không cần sửa khi dev
# Nếu muốn tạo từ template mẫu:
copy .env.example .env
```

> Chi tiết xem mục [5. Cấu hình biến môi trường (.env)](#5-cấu-hình-biến-môi-trường-env).

### Bước 7 — Chạy Frontend

```bash
cd cms.frontend
npm install
npm start
# Mở trình duyệt: http://localhost:3000
```

---

## 5. Cấu hình biến môi trường (.env)

Dự án **không hardcode** domain backend trong source code. Toàn bộ URL được đọc từ file `.env` theo chuẩn Create React App — biến môi trường phải có tiền tố `REACT_APP_`.

### 5.1 Các file .env

```
cms.frontend/
├── .env               ← Dùng khi development (KHÔNG commit lên git)
├── .env.development   ← Tự động load khi chạy npm start
├── .env.production    ← Tự động load khi chạy npm run build
└── .env.example       ← Template mẫu (commit lên git để teamwork)
```

| File | Khi nào dùng | Có commit git không |
|------|-------------|-------------------|
| `.env` | Fallback mọi môi trường | Không (trong `.gitignore`) |
| `.env.development` | `npm start` | Không (trong `.gitignore`) |
| `.env.production` | `npm run build` | Không (trong `.gitignore`) |
| `.env.example` | Template hướng dẫn | **Có** — commit để team biết cần điền gì |

### 5.2 Các biến môi trường

| Biến | Mô tả | Giá trị mặc định (dev) |
|------|-------|----------------------|
| `REACT_APP_API_URL` | URL gốc REST API backend | `https://localhost:7090/api` |
| `REACT_APP_IMAGE_BASE_URL` | URL gốc để ghép đường dẫn ảnh upload | `https://localhost:7090` |

**Nội dung file `.env` (dev):**
```env
# URL gốc của Backend API (không có dấu / ở cuối)
REACT_APP_API_URL=https://localhost:7090/api

# URL gốc của Backend để ghép đường dẫn ảnh
REACT_APP_IMAGE_BASE_URL=https://localhost:7090
```

**Nội dung file `.env.production` (khi deploy):**
```env
REACT_APP_API_URL=https://your-domain.com/api
REACT_APP_IMAGE_BASE_URL=https://your-domain.com
```

### 5.3 File cấu hình trung tâm — `src/config/constants.js`

Tất cả các component đều import từ file này, **không đọc `process.env` trực tiếp**:

```js
// src/config/constants.js

export const API_BASE_URL =
  process.env.REACT_APP_API_URL || 'https://localhost:7090/api';

export const IMAGE_BASE_URL =
  process.env.REACT_APP_IMAGE_BASE_URL || 'https://localhost:7090';

/**
 * Ghép URL ảnh đầy đủ từ đường dẫn tương đối trả về bởi API.
 * Ví dụ: "/uploads/product.jpg" → "https://localhost:7090/uploads/product.jpg"
 */
export function getImageUrl(path, fallback = '') {
  if (!path) return fallback;
  if (path.startsWith('http://') || path.startsWith('https://')) return path;
  const separator = path.startsWith('/') ? '' : '/';
  return `${IMAGE_BASE_URL}${separator}${path}`;
}
```

### 5.4 Nơi sử dụng trong dự án

| File | Cách dùng |
|------|----------|
| `src/api/axiosClient.js` | `baseURL: API_BASE_URL` — mọi request API đều qua đây |
| `src/context/CartContext.jsx` | `getImageUrl()` trong `buildItem()` — resolve ảnh khi thêm vào giỏ |
| `src/components/Product/ProductCard.jsx` | `getImageUrl()` trong hàm `getImage()` |
| `src/pages/product/ProductDetail.jsx` | `getImageUrl()` trong hàm `resolveImg()` |
| `src/pages/search/SearchResults.jsx` | `getImageUrl()` khi render ảnh sản phẩm trong kết quả tìm kiếm |

### 5.5 Sơ đồ luồng dữ liệu URL

```
.env / .env.development / .env.production
            │
            ▼
  src/config/constants.js
  ├── API_BASE_URL   ──────────► axiosClient.js (baseURL)
  │                                    │
  │                                    ▼
  │                            Tất cả API call
  │
  └── IMAGE_BASE_URL ──┐
  └── getImageUrl()  ──┤──► CartContext.jsx (buildItem)
                       ├──► ProductCard.jsx (getImage)
                       ├──► ProductDetail.jsx (resolveImg)
                       └──► SearchResults.jsx (src={getImageUrl(...)})
```

> **Lợi ích:** Khi deploy lên server thật, chỉ cần sửa **một chỗ** trong `.env.production` — toàn bộ API call và link ảnh trong ứng dụng tự động cập nhật theo.

---

## 6. Cơ sở dữ liệu

### Sơ đồ quan hệ

```
Categories ──< Posts
                 └── CategoryId (FK)

CategoryProduct ──< Products ──< ProductImages
                       └── CategoryProductId (FK)    └── ProductId (FK)

Customers ──< Orders ──< OrderDetails >── Products
    │              └── CustomerId (FK)      └── ProductId (FK)
    │
    └──< CustomerAddresses
              └── CustomerId (FK)

Users (admin accounts — độc lập)
Advertisements (banner — độc lập)
```

### Mô tả các bảng

| Bảng | Mô tả | Trường quan trọng |
|------|-------|------------------|
| `Categories` | Danh mục bài viết blog | Name, Description |
| `Posts` | Bài viết | Title, Content, ImageUrl, CategoryId, CreatedDate |
| `Users` | Tài khoản quản trị | Username (unique), PasswordHash, Role |
| `CategoryProduct` | Danh mục sản phẩm | Name, Description, ImageUrl |
| `Products` | Sản phẩm | Name, Price, StockQuantity, ImageUrl, CategoryProductId |
| `ProductImages` | Ảnh phụ sản phẩm | ProductId, Url, SortOrder |
| `Customers` | Khách hàng | FullName, Email (unique), Phone, Address, Password |
| `CustomerAddresses` | Địa chỉ giao hàng | CustomerId, RecipientName, Phone, Address, IsDefault |
| `Orders` | Đơn hàng | CustomerId, Status (0/1/2), ShippingAddress, RecipientName |
| `OrderDetails` | Chi tiết đơn | OrderId, ProductId, Quantity, UnitPrice |
| `Advertisements` | Banner trang chủ | Title, Subtitle, ImageUrl, IsActive, SortOrder |

### Unique Index

```sql
IX_Customers_Email_Unique   ON Customers(Email)
IX_Users_Username_Unique    ON Users(Username)
```

### Cascade Delete

| Bảng cha | Bảng con | Hành động |
|---------|---------|----------|
| Customers | Orders | CASCADE |
| Customers | CustomerAddresses | CASCADE |
| Orders | OrderDetails | CASCADE |
| Products | ProductImages | CASCADE |

---

## 7. Chức năng hệ thống

### 7.1 Admin Panel (`https://localhost:7090`)

#### Đăng nhập Admin
- URL: `/Account/Login`
- Xác thực bằng Cookie (phiên 60 phút)
- Tài khoản mặc định: `admin` / `Admin@123`

#### Dashboard
- URL: `/Home`
- Hiển thị tổng số: Sản phẩm, Danh mục, Đơn hàng, Khách hàng

#### Quản lý Danh mục Blog (`/Category`)
| Thao tác | Mô tả |
|---------|-------|
| Xem danh sách | Bảng tất cả danh mục, phân trang |
| Thêm mới | Form: Tên (bắt buộc), Mô tả |
| Chỉnh sửa | Sửa tên, mô tả |
| Xóa | Xóa danh mục (cảnh báo nếu còn bài viết) |

#### Quản lý Bài viết (`/Post`)
| Thao tác | Mô tả |
|---------|-------|
| Xem danh sách | Tiêu đề, danh mục, ngày tạo |
| Thêm mới | Form: Tiêu đề, Nội dung, Ảnh đại diện, Danh mục |
| Chỉnh sửa | Cập nhật toàn bộ thông tin |
| Xóa | Xóa bài viết |

#### Quản lý Danh mục Sản phẩm (`/CategoriesProducts`)
| Thao tác | Mô tả |
|---------|-------|
| Xem danh sách | Tên, mô tả, ảnh đại diện |
| Thêm mới | Form: Tên, Mô tả, Upload ảnh |
| Chỉnh sửa | Sửa thông tin + đổi ảnh |
| Xóa | Xóa danh mục |

#### Quản lý Sản phẩm (`/Product`)
| Thao tác | Mô tả |
|---------|-------|
| Xem danh sách | Tên, giá, tồn kho, danh mục, ảnh |
| Thêm mới | Form: Tên, Giá, Tồn kho, Mô tả, Ảnh đại diện, Danh mục |
| Chỉnh sửa | Sửa thông tin + **Gallery ảnh**: upload nhiều ảnh, xóa ảnh, đặt ảnh làm cover |
| Xóa | Xóa sản phẩm (cascade xóa ProductImages) |

#### Quản lý Khách hàng (`/Customer`)
| Thao tác | Mô tả |
|---------|-------|
| Xem danh sách | ID, tên, email, SĐT, địa chỉ |
| Xem chi tiết | Thông tin đầy đủ + lịch sử đơn hàng |

#### Quản lý Đơn hàng (`/Order`)
| Thao tác | Mô tả |
|---------|-------|
| Xem danh sách | Mã đơn, khách hàng, ngày, trạng thái |
| Xem chi tiết | Danh sách sản phẩm, địa chỉ giao, tổng tiền |
| Cập nhật trạng thái | 0: Chờ duyệt → 1: Đang giao → 2: Hoàn tất |

#### Quản lý Người dùng Admin (`/User`)
| Thao tác | Mô tả |
|---------|-------|
| Xem danh sách | Username, họ tên, vai trò |
| Thêm mới | Username, mật khẩu, họ tên, vai trò (Admin/Editor) |
| Chỉnh sửa | Sửa thông tin |
| Xóa | Xóa tài khoản admin |

---

### 7.2 Frontend Customer (`http://localhost:3000`)

#### Trang chủ (`/`)
- Banner carousel (Advertisements)
- Sản phẩm nổi bật
- Danh mục sản phẩm
- Bài viết mới nhất

#### Trang sản phẩm (`/products`)
- Phân trang 9 sản phẩm/trang
- Card sản phẩm: ảnh, tên, giá, nút thêm giỏ hàng
- Lọc theo danh mục

#### Chi tiết sản phẩm (`/products/:id`)
- Gallery ảnh dọc bên trái + ảnh chính bên phải
- Breadcrumb navigation
- Thông tin: tên, giá, tình trạng tồn kho (chip màu)
- Điều chỉnh số lượng (giới hạn theo tồn kho)
- Nút: Thêm vào giỏ / Mua ngay
- Tabs: Mô tả | Chất liệu | Vận chuyển & Đổi trả
- Sản phẩm liên quan (cùng danh mục, tối đa 4 sản phẩm)

#### Giỏ hàng (`/cart`)
- Danh sách sản phẩm, điều chỉnh số lượng, xóa
- Tổng tiền, nút Tiến hành thanh toán

#### Thanh toán (`/checkout`)
- Chọn nhanh địa chỉ đã lưu (nếu đã đăng nhập)
- Nút "Địa chỉ mới" để nhập thông tin mới
- Form: Tên người nhận, SĐT, Địa chỉ, Ghi chú
- Tóm tắt đơn hàng bên phải
- Sau đặt: Hiện màn hình thành công + gửi email xác nhận
- **Tự động lưu** địa chỉ mới vào danh sách địa chỉ của tài khoản

#### Đăng ký (`/register`)
- Thông tin: Họ tên, Email, SĐT (tuỳ chọn), Địa chỉ (tuỳ chọn), Mật khẩu, Xác nhận mật khẩu
- Chuyển hướng sang trang đăng nhập sau 2 giây

#### Đăng nhập (`/login`)
- Form: Email, Mật khẩu (hiện/ẩn)
- Link "Quên mật khẩu"
- Lưu token JWT vào localStorage

#### Quên mật khẩu (`/forgot-password`)
- Nhập email → Nhận OTP qua email
- Nhập OTP + mật khẩu mới → Reset

#### Tài khoản (`/account`) — Yêu cầu đăng nhập

**Tab 1: Thông tin cá nhân**
- Xem và cập nhật: Họ tên, Email, SĐT
- Validation client-side trước khi gửi

**Tab 2: Đơn hàng của tôi**
- Danh sách đơn hàng theo thời gian
- Click mở rộng: xem chi tiết sản phẩm, địa chỉ giao, tổng tiền
- Badge trạng thái màu sắc: Chờ duyệt / Đang giao / Hoàn tất

**Tab 3: Địa chỉ nhận hàng**
- Địa chỉ mặc định (viền vàng, badge ★)
- Danh sách địa chỉ phụ
- Thêm địa chỉ mới (inline form)
- Sửa / Xóa từng địa chỉ
- Đặt làm địa chỉ mặc định → tự cập nhật context

**Tab 4: Đổi mật khẩu**
- Nhập mật khẩu cũ, mật khẩu mới, xác nhận

#### Blog (`/blog`)
- Danh sách bài viết, lọc theo danh mục
- Chi tiết bài viết: nội dung, ngày đăng, danh mục

#### Trang đang phát triển
- `/booking` — Đặt lịch hẹn
- `/custom-order` — Đặt hàng theo yêu cầu
- `/contact` — Liên hệ

---

## 8. API Endpoints

> Base URL: `https://localhost:7090/api`  
> Swagger UI: `https://localhost:7090/swagger`

### Auth (JWT)

| Method | Endpoint | Mô tả | Auth |
|--------|---------|-------|------|
| POST | `/auth/register` | Đăng ký tài khoản | — |
| POST | `/auth/login` | Đăng nhập, trả về JWT | — |
| POST | `/auth/forgot-password` | Gửi OTP reset qua email | — |
| POST | `/auth/reset-password` | Reset mật khẩu bằng OTP | — |
| POST | `/auth/change-password` | Đổi mật khẩu | JWT |

**Body đăng ký:**
```json
{
  "fullName": "Nguyễn Văn A",
  "email": "example@gmail.com",
  "phone": "0909123456",
  "address": "TP. Hồ Chí Minh",
  "password": "abc123"
}
```

**Response đăng nhập:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": { "id": 1, "fullName": "Nguyễn Văn A", "email": "...", "phone": "...", "address": "..." }
}
```

---

### Products

| Method | Endpoint | Mô tả | Auth |
|--------|---------|-------|------|
| GET | `/products` | Danh sách sản phẩm (có phân trang `?page=1&pageSize=9`) | — |
| GET | `/products/:id` | Chi tiết sản phẩm (kèm Images) | — |
| GET | `/products/category/:categoryId` | Sản phẩm theo danh mục | — |
| GET | `/products/search?keyword=` | Tìm kiếm theo tên | — |
| POST | `/products` | Thêm sản phẩm mới | Admin Cookie |
| PUT | `/products/:id` | Cập nhật sản phẩm | Admin Cookie |
| DELETE | `/products/:id` | Xóa sản phẩm | Admin Cookie |
| POST | `/products/:id/images` | Upload ảnh gallery (`multipart/form-data`) | Admin Cookie |
| DELETE | `/products/:id/images/:imageId` | Xóa ảnh gallery | Admin Cookie |
| PUT | `/products/:id/images/:imageId/cover` | Đặt ảnh làm cover | Admin Cookie |

**Response GET /products/:id:**
```json
{
  "id": 1,
  "name": "Bình Gốm Hoa Sen",
  "description": "...",
  "price": 350000,
  "stockQuantity": 15,
  "imageUrl": "/uploads/binh-gom.jpg",
  "categoryProductId": 2,
  "categoryProduct": { "id": 2, "name": "Bình Hoa" },
  "images": [
    { "id": 1, "url": "/uploads/binh-gom-1.jpg", "sortOrder": 0 },
    { "id": 2, "url": "/uploads/binh-gom-2.jpg", "sortOrder": 1 }
  ]
}
```

---

### Customers & Addresses

| Method | Endpoint | Mô tả | Auth |
|--------|---------|-------|------|
| GET | `/customers` | Danh sách khách hàng | Admin Cookie |
| GET | `/customers/:id` | Thông tin khách hàng | Admin Cookie |
| POST | `/customers` | Đăng ký (dùng cho `/auth/register`) | — |
| PUT | `/customers/:id` | Cập nhật thông tin | JWT |
| DELETE | `/customers/:id` | Xóa tài khoản | Admin Cookie |
| GET | `/customers/:id/addresses` | Danh sách địa chỉ (auto-seed từ profile nếu trống) | JWT |
| POST | `/customers/:id/addresses` | Thêm địa chỉ mới | JWT |
| PUT | `/customers/:id/addresses/:addrId` | Cập nhật địa chỉ | JWT |
| DELETE | `/customers/:id/addresses/:addrId` | Xóa địa chỉ | JWT |
| PUT | `/customers/:id/addresses/:addrId/default` | Đặt làm mặc định | JWT |

---

### Orders

| Method | Endpoint | Mô tả | Auth |
|--------|---------|-------|------|
| GET | `/orders` | Tất cả đơn hàng | Admin Cookie |
| GET | `/orders/:id` | Chi tiết đơn | Admin Cookie / JWT |
| GET | `/orders/customer/:customerId` | Đơn hàng của khách | JWT |
| POST | `/orders/checkout` | Đặt hàng mới | JWT |
| PUT | `/orders/:id` | Cập nhật trạng thái | Admin Cookie |

**Body checkout:**
```json
{
  "customerId": 1,
  "recipientName": "Nguyễn Văn A",
  "recipientPhone": "0909123456",
  "shippingAddress": "123 Đường ABC, P.1, Q.1, TP.HCM",
  "notes": "Giao giờ hành chính",
  "items": [
    { "productId": 3, "quantity": 2 },
    { "productId": 7, "quantity": 1 }
  ]
}
```

**Response checkout thành công:**
```json
{
  "message": "Đặt hàng thành công!",
  "orderId": 42,
  "total": 850000,
  "email": "customer@gmail.com",
  "shippingAddress": "123 Đường ABC...",
  "updatedCustomer": { "id": 1, "address": "123 Đường ABC...", "phone": "0909123456" }
}
```

---

### Posts & Categories

| Method | Endpoint | Mô tả |
|--------|---------|-------|
| GET | `/posts` | Tất cả bài viết (kèm danh mục) |
| GET | `/posts/:id` | Chi tiết bài viết |
| GET | `/posts/category/:categoryId` | Bài viết theo danh mục |
| GET | `/categories` | Danh sách danh mục blog |
| GET | `/categoriesproduct` | Danh mục sản phẩm |

---

## 9. Xác thực & Phân quyền

### Hai cơ chế song song

```
┌──────────────────────────────────────────────────────┐
│                   ASP.NET Core Auth                   │
├────────────────────────┬─────────────────────────────┤
│   Cookie Auth          │   JWT Bearer Auth            │
│   (Admin MVC)          │   (Customer API)             │
├────────────────────────┼─────────────────────────────┤
│ Login: POST /Account/  │ Login: POST /api/auth/login  │
│ Logout                 │                              │
│ Session: 60 phút       │ Token TTL: 7 ngày            │
│ Storage: Cookie HTTP   │ Storage: localStorage        │
│ Dùng: [Authorize] MVC  │ Dùng: [Authorize(JWT)] API   │
└────────────────────────┴─────────────────────────────┘
```

### JWT Token

```json
// Payload chứa:
{
  "sub":   "1",
  "email": "customer@gmail.com",
  "name":  "Nguyễn Văn A",
  "exp":   1735689600
}
```

### Frontend gửi token

```js
// axiosClient.js — tự động đính token vào mọi request
axios.interceptors.request.use(config => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});
```

---

## 10. Ràng buộc & Validation

### 10.1 Ràng buộc Database

| Bảng | Trường | Ràng buộc |
|------|--------|----------|
| Customers | Email | UNIQUE, NOT NULL |
| Users | Username | UNIQUE, NOT NULL |
| Products | Price | > 0 (decimal 18,2) |
| Products | StockQuantity | >= 0 |
| Orders | Status | 0, 1, hoặc 2 |
| OrderDetails | Quantity | >= 1 |
| OrderDetails | UnitPrice | > 0 |
| CustomerAddresses | CustomerId | FK → Customers CASCADE DELETE |
| Orders | CustomerId | FK → Customers CASCADE DELETE |
| OrderDetails | OrderId | FK → Orders CASCADE DELETE |
| ProductImages | ProductId | FK → Products CASCADE DELETE |

### 10.2 Validation Backend (Controller)

#### Đăng ký khách hàng (`POST /api/customers`)
| Trường | Ràng buộc | Thông báo lỗi |
|--------|----------|-------------|
| FullName | 2–100 ký tự | "Họ tên phải có ít nhất 2 ký tự" |
| Email | Đúng format, không trùng | "Email đã được đăng ký" |
| Phone | 8–20 ký tự, chỉ số/ký tự hợp lệ | "Số điện thoại không hợp lệ" |
| Password | Tối thiểu 6 ký tự | "Mật khẩu phải có ít nhất 6 ký tự" |

#### Đặt hàng (`POST /api/orders/checkout`)
| Trường | Ràng buộc | Thông báo lỗi |
|--------|----------|-------------|
| Items | Không được rỗng | "Giỏ hàng trống" |
| RecipientName | 2–100 ký tự | "Tên người nhận phải có ít nhất 2 ký tự" |
| ShippingAddress | Tối thiểu 5 ký tự | "Địa chỉ giao hàng quá ngắn" |
| RecipientPhone | 8–20 ký tự (nếu nhập) | "Số điện thoại không hợp lệ" |
| Quantity | > 0 | "Số lượng sản phẩm không hợp lệ" |
| StockQuantity | Đủ tồn kho | "Sản phẩm X chỉ còn Y sản phẩm" |

#### Thêm/sửa sản phẩm
| Trường | Ràng buộc | Thông báo lỗi |
|--------|----------|-------------|
| Name | 3–200 ký tự | "Tên sản phẩm phải có ít nhất 3 ký tự" |
| Price | > 0 | "Giá sản phẩm phải lớn hơn 0" |
| StockQuantity | >= 0 | "Tồn kho không thể là số âm" |
| CategoryProductId | Phải tồn tại | "Danh mục sản phẩm không tồn tại" |

### 10.3 Validation Frontend (Client-side)

#### Form Đăng ký
| Trường | Kiểm tra |
|--------|---------|
| Họ tên | Bắt buộc, >= 2 ký tự |
| Email | Bắt buộc, format `^[^\s@]+@[^\s@]+\.[^\s@]+$` |
| SĐT | Format số (8–20 ký tự), nếu có nhập |
| Mật khẩu | Bắt buộc, >= 6 ký tự |
| Xác nhận MK | Phải khớp với mật khẩu |

#### Form Đăng nhập
| Trường | Kiểm tra |
|--------|---------|
| Email | Bắt buộc, format email |
| Mật khẩu | Bắt buộc, >= 6 ký tự |

#### Form Thanh toán
| Trường | Kiểm tra |
|--------|---------|
| Tên người nhận | Bắt buộc, >= 2 ký tự |
| SĐT | Format số (8–20 ký tự), nếu có nhập |
| Địa chỉ | Bắt buộc, >= 5 ký tự |

#### Form Địa chỉ (Profile)
| Trường | Kiểm tra |
|--------|---------|
| Tên người nhận | Bắt buộc, >= 2 ký tự |
| SĐT | Format số, nếu có nhập |
| Địa chỉ | Bắt buộc, >= 5 ký tự |

#### Form Đổi mật khẩu
| Trường | Kiểm tra |
|--------|---------|
| MK cũ | Bắt buộc |
| MK mới | Bắt buộc, >= 6 ký tự, khác MK cũ |
| Xác nhận | Phải khớp MK mới |

---

## 11. Hướng dẫn kiểm thử

### 11.1 Kiểm thử chức năng Admin

#### Đăng nhập Admin
```
URL: https://localhost:7090/Account/Login
Tài khoản: admin / Admin@123
Kết quả mong đợi: Chuyển hướng về /Home (Dashboard)
```

#### Thêm sản phẩm mới
```
1. Vào /Product → Thêm sản phẩm mới
2. Điền: Tên "Bình Gốm Test", Giá 100000, Tồn kho 5, chọn Danh mục
3. Upload ảnh đại diện
4. Nhấn Lưu
Kết quả: Sản phẩm xuất hiện trong danh sách, hiển thị trên frontend
```

#### Upload gallery ảnh sản phẩm
```
1. Vào /Product → Edit sản phẩm bất kỳ
2. Kéo xuống phần "Gallery hình ảnh"
3. Nhấn "Chọn ảnh" → chọn nhiều file JPG/PNG
4. Nhấn "Upload X ảnh"
5. Nhấn "Đặt làm cover" cho ảnh muốn làm đại diện
Kết quả: Ảnh hiện trong gallery trang chi tiết sản phẩm frontend
```

#### Cập nhật trạng thái đơn hàng
```
1. Vào /Order → click vào đơn hàng bất kỳ
2. Đổi Status từ 0 → 1 (Đang giao) → Lưu
Kết quả: Badge đơn hàng cập nhật, khách thấy thay đổi trong /account
```

---

### 11.2 Kiểm thử chức năng Khách hàng

#### Đăng ký tài khoản mới
```
URL: http://localhost:3000/register
Nhập: Họ tên "Nguyễn Test", Email mới, Mật khẩu "123456"
Kết quả: Thông báo thành công, tự chuyển sang /login sau 2 giây
```

#### Đăng nhập và giỏ hàng
```
1. Đăng nhập với email/mật khẩu đã đăng ký
2. Vào /products → click "Thêm vào giỏ" nhiều sản phẩm
3. Vào /cart → kiểm tra số lượng, tổng tiền
4. Điều chỉnh số lượng từng sản phẩm
Kết quả: Giỏ hàng cập nhật realtime, header hiển thị badge số lượng
```

#### Đặt hàng (Golden Path)
```
1. Đăng nhập → Thêm sản phẩm vào giỏ → Vào /checkout
2. Chọn địa chỉ đã lưu HOẶC nhấn "Địa chỉ mới" điền form
3. Nhập ghi chú (tuỳ chọn)
4. Nhấn "Đặt hàng"
Kết quả mong đợi:
  - Hiện màn hình "Đặt hàng thành công!" với mã đơn
  - Email xác nhận gửi đến địa chỉ đã đăng ký
  - Tồn kho sản phẩm giảm tương ứng
  - Địa chỉ vừa dùng tự lưu vào tab Địa chỉ (nếu chưa có)
  - Giỏ hàng được xóa
```

#### Quản lý địa chỉ nhận hàng
```
1. Vào /account → tab "Địa chỉ nhận hàng"
2. Lần đầu: tự động hiện địa chỉ từ profile làm mặc định
3. Nhấn "Thêm địa chỉ" → điền form → Lưu
4. Nhấn "Đặt mặc định" cho địa chỉ mới
Kết quả: Địa chỉ mặc định có viền vàng + badge ★ Mặc định
```

#### Xem lịch sử đơn hàng
```
1. Vào /account → tab "Đơn hàng của tôi"
2. Click vào đơn → mở rộng xem chi tiết
Kết quả: Hiện danh sách sản phẩm, địa chỉ giao, tổng tiền, trạng thái
```

---

### 11.3 Kiểm thử Swagger API

Truy cập `https://localhost:7090/swagger` để test trực tiếp tất cả API.

#### Test đăng ký + đăng nhập
```
1. POST /api/auth/register — body: { fullName, email, phone, password }
2. POST /api/auth/login    — body: { email, password }
3. Copy token từ response
4. Nhấn "Authorize" → nhập "Bearer {token}"
5. Test các endpoint cần JWT
```

---

### 11.4 Test Cases quan trọng

#### TC01: Email trùng khi đăng ký
```
Input: Đăng ký email đã tồn tại
Expected: HTTP 400 — "Email đã được đăng ký"
```

#### TC02: Đặt hàng vượt tồn kho
```
Input: Nhập số lượng > StockQuantity của sản phẩm
Expected: HTTP 400 — "Sản phẩm X chỉ còn Y sản phẩm"
```

#### TC03: Đặt hàng sản phẩm hết hàng
```
Input: Sản phẩm có StockQuantity = 0
Expected: HTTP 400 — "Sản phẩm X đã hết hàng"
```

#### TC04: Giá sản phẩm = 0
```
Input: Admin nhập Price = 0 khi thêm sản phẩm
Expected: HTTP 400 — "Giá sản phẩm phải lớn hơn 0"
```

#### TC05: Địa chỉ tự động seed lần đầu
```
Điều kiện: Khách có Customer.Address nhưng chưa có CustomerAddresses
Action: Mở tab "Địa chỉ nhận hàng"
Expected: Tự động tạo 1 entry mặc định từ customer.Address
```

#### TC06: Xóa địa chỉ mặc định
```
Action: Xóa địa chỉ đang là mặc định
Expected: Địa chỉ tiếp theo tự động thành mặc định
```

#### TC07: Đặt hàng với địa chỉ mới → Tự lưu
```
Action: Checkout với địa chỉ chưa có trong danh sách
Expected: Sau đặt hàng, địa chỉ xuất hiện trong tab Địa chỉ
```

#### TC08: Upload ảnh sản phẩm không hợp lệ
```
Input: Upload file .pdf hoặc file > 6MB
Expected: Báo lỗi "Chỉ chấp nhận file ảnh" / "Kích thước tối đa 6MB"
```

#### TC09: Mật khẩu mới trùng mật khẩu cũ
```
Input: Nhập mật khẩu mới = mật khẩu cũ
Expected (frontend): "Mật khẩu mới không được trùng với mật khẩu cũ"
```

#### TC10: Token hết hạn / không hợp lệ
```
Input: Gửi request API với token sai/hết hạn
Expected: HTTP 401 Unauthorized
```

---

## 12. Các trường hợp lỗi thường gặp

### Backend không khởi động được

**Lỗi: Port 7090 đã bị chiếm**
```powershell
# Kill process đang giữ port
Get-Process -Name "CMS.Backend" | Stop-Process -Force
# Hoặc tìm process theo port
netstat -ano | findstr :7090
taskkill /PID <PID> /F
```

**Lỗi: DLL bị lock khi build**
```powershell
Get-Process -Name "CMS.Backend" | Stop-Process -Force
dotnet build CMS.Backend/CMS.Backend.csproj
```

**Lỗi: Cannot connect to SQL Server**
```
- Kiểm tra SQL Server service đang chạy
- Kiểm tra tên server trong appsettings.json
- Kiểm tra tên database đã tồn tại
- Thử kết nối bằng SSMS với cùng connection string
```

### Database

**Lỗi: EF Migration snapshot không khớp**
```powershell
# Tạo migration rỗng để sync snapshot
dotnet ef migrations add SyncSnapshot `
  --project CMS.Data/CMS.Data.csproj `
  --startup-project CMS.Backend/CMS.Backend.csproj
# Xóa nội dung Up()/Down() trong file migration vừa tạo
```

**Lỗi: Duplicate key khi tạo Unique Index**
```sql
-- Tìm email trùng trước
SELECT Email, COUNT(*) cnt FROM Customers
GROUP BY Email HAVING COUNT(*) > 1;
-- Xử lý duplicate rồi mới tạo index
```

### Frontend

**Lỗi: CORS — Cannot access API từ React**
```
- Đảm bảo backend đang chạy tại https://localhost:7090
- Kiểm tra appsettings — CORS chỉ cho phép http://localhost:3000
- Không dùng port khác cho React khi dev
```

**Lỗi: 401 Unauthorized khi gọi API cần JWT**
```
- Kiểm tra localStorage có key 'token'
- Token có thể đã hết hạn (7 ngày) — đăng nhập lại
- Kiểm tra axiosClient có gắn Authorization header
```

**Lỗi: Ảnh sản phẩm không hiển thị**
```
- File ảnh phải nằm trong CMS.Backend/wwwroot/uploads/
- URL trả về từ API dạng: /uploads/ten-file.jpg
- Frontend ghép: https://localhost:7090 + /uploads/ten-file.jpg
```

---

## Thông tin liên hệ

> **Sinh viên:** Nguyễn Duy Anh Tuấn— MSSV: 2123110162  
> **Email:** tuan04112005@gmail.com  
> **Lớp:** CCQ2311E
