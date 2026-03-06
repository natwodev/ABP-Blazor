using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BookStore.Students;

public interface IStudentAppService : IApplicationService
{
    Task<ListResultDto<StudentDto>> GetListAsync();
}

