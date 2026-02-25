namespace PlayGround.Shared.DTOs
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? Error { get; set; }

        public static ApiResponse<T> Success(T data) =>
            new() { IsSuccess = true, Data = data };

        public static ApiResponse<T> Fail(string error) =>
            new() { IsSuccess = false, Error = error };
    }

    public class PagedList<T>
    {
        public List<T> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)Size);

        public PagedList() { }

        public PagedList(List<T> items, int totalCount, int page, int size)
        {
            Items = items;
            TotalCount = totalCount;
            Page = page;
            Size = size;
        }
    }
}
