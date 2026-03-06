using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BookStore.Students;

public class StudentAppService : ApplicationService, IStudentAppService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public StudentAppService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ListResultDto<StudentDto>> GetListAsync()
    {
        var client = _httpClientFactory.CreateClient("TracNghiemApi");

        var response = await client.GetFromJsonAsync<StudentListResponseDto>(
            "api/admin-user/get-students",
            CancellationToken.None);

        var students = response?.Data ?? new List<StudentDto>();

        return new ListResultDto<StudentDto>(students);
    }
}

