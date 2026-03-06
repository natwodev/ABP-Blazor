## Hướng dẫn tích hợp API ngoài vào ABP

### Mục tiêu

- Dùng API bên ngoài (REST) để lấy/ghi dữ liệu.
- Đóng gói lại dưới dạng Application Service/DTO trong ABP (tầng Application + Application.Contracts).
- UI (Blazor/MVC/SPA khác) chỉ gọi AppService, không gọi API ngoài trực tiếp.

---

### 1. Phân tích API ngoài

- Ví dụ: `GET http://example.com/api/items`
- Cần xác định:
  - HTTP method: `GET`, `POST`, `PUT`, `DELETE`, ...
  - Base URL và path (vd: base url + `api/items`).
  - Header đặc biệt (Authorization, API key, ...).
  - JSON response mẫu để thiết kế DTO.

---

### 2. Định nghĩa DTO & AppService contract (Application.Contracts)

**Bước 1 – Tạo DTO**

- File gợi ý: `src/BookStore.Application.Contracts/<Feature>/ExternalItemDto.cs`
- Ví dụ:

```csharp
namespace BookStore.ExternalItems;

public class ExternalItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Code { get; set; }
    // Thêm các trường khác theo JSON thực tế
}
```

**Bước 2 – Tạo interface AppService**

- File gợi ý: `src/BookStore.Application.Contracts/<Feature>/IExternalItemAppService.cs`

```csharp
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BookStore.ExternalItems;

public interface IExternalItemAppService : IApplicationService
{
    Task<ListResultDto<ExternalItemDto>> GetListAsync();
}
```

Quy ước:

- Mỗi loại dữ liệu/feature → 1 DTO + 1 interface AppService riêng.
- Namespace theo dạng `BookStore.<Feature>`.

---

### 3. Implement AppService gọi API ngoài (Application)

**Bước 3 – Tạo AppService**

- File gợi ý: `src/BookStore.Application/<Feature>/ExternalItemAppService.cs`

```csharp
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BookStore.ExternalItems;

public class ExternalItemAppService : ApplicationService, IExternalItemAppService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ExternalItemAppService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ListResultDto<ExternalItemDto>> GetListAsync()
    {
        var client = _httpClientFactory.CreateClient("ExternalItemsApi");

        var items = await client.GetFromJsonAsync<List<ExternalItemDto>>(
            "api/items", // path tương ứng với API thật
            CancellationToken.None);

        return new ListResultDto<ExternalItemDto>(items ?? new List<ExternalItemDto>());
    }
}
```

Nếu JSON API khác tên trường so với DTO:

- Tạo thêm DTO raw (vd: `ExternalItemRawDto`) để deserialize.
- Map thủ công hoặc dùng ObjectMapper/AutoMapper/Mapperly sang `ExternalItemDto`.

---

### 4. Cấu hình HttpClient cho API ngoài

**Bước 4 – Cấu hình trong ApplicationModule**

- File gợi ý: `src/BookStore.Application/BookStoreApplicationModule.cs`

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace BookStore;

public class BookStoreApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Các cấu hình khác...

        context.Services.AddHttpClient("ExternalItemsApi", client =>
        {
            client.BaseAddress = new Uri("http://example.com/"); // Đọc từ config khi triển khai thật
            // Có thể set timeout, default headers...
        });
    }
}
```

Khuyến nghị: đưa BaseUrl vào `appsettings.json`:

```jsonc
"ExternalApis": {
  "ExternalItems": {
    "BaseUrl": "http://example.com/"
  }
}
```

Sau đó đọc từ `IConfiguration` để set `BaseAddress` thay vì hard‑code.

---

### 5. Sử dụng trong UI (Blazor / MVC)

**Blazor Server / WebApp**

Trong component `.razor`:

```razor
@page "/external-items"
@inject BookStore.ExternalItems.IExternalItemAppService ExternalItemAppService

<h3>Danh sách từ API ngoài</h3>

@if (_items == null)
{
    <p>Đang tải...</p>
}
else
{
    <ul>
        @foreach (var item in _items)
        {
            <li>@item.Name (@item.Code)</li>
        }
    </ul>
}

@code {
    private List<BookStore.ExternalItems.ExternalItemDto> _items;

    protected override async Task OnInitializedAsync()
    {
        var result = await ExternalItemAppService.GetListAsync();
        _items = result.Items;
    }
}
```

**MVC / Razor Pages**

- Inject `IExternalItemAppService` vào controller/page.
- Gọi `GetListAsync()` và truyền model ra view để hiển thị.

---

### 6. Expose lại qua HttpApi (nếu cần cho frontend khác)

Tùy chọn: tạo controller trong `src/BookStore.HttpApi`:

```csharp
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace BookStore.ExternalItems;

[Route("api/app/external-items")]
public class ExternalItemController : BookStoreController
{
    private readonly IExternalItemAppService _externalItemAppService;

    public ExternalItemController(IExternalItemAppService externalItemAppService)
    {
        _externalItemAppService = externalItemAppService;
    }

    [HttpGet]
    public Task<ListResultDto<ExternalItemDto>> GetListAsync()
    {
        return _externalItemAppService.GetListAsync();
    }
}
```

Nhờ vậy, React/Angular/mobile có thể gọi `/api/app/external-items` của ABP,  
ABP sẽ gọi tiếp API ngoài và trả dữ liệu về.

---

### 7. Best practices chung

- Không gọi API ngoài trực tiếp từ UI:
  - Luôn thông qua AppService → dễ test, dễ đổi nguồn dữ liệu.
- Không hard‑code URL/token:
  - Đưa vào `appsettings.json` hoặc secret store.
- Có timeout và xử lý lỗi:
  - Bọc call HTTP trong `try/catch`, log lỗi, ném `UserFriendlyException` nếu cần.
- Đặt tên HttpClient theo chức năng:
  - Ví dụ: `"TracNghiemApi"`, `"PaymentApi"`, `"SmsGatewayApi"`.

