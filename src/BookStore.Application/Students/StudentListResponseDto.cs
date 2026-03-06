using System.Collections.Generic;

namespace BookStore.Students;

public class StudentListResponseDto
{
    public bool Success { get; set; }
    public List<StudentDto> Data { get; set; } = new();
}

