namespace CustomerImageApi.DTOs.Common;

public class PaginatedResponse<T>
{
    public List<T> Items { get; set; } = new List<T>();
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }

    public PaginatedResponse()
    {
    }

    public PaginatedResponse(List<T> items, int totalItems, int currentPage, int pageSize)
    {
        Items = items;
        TotalItems = totalItems;
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        HasPrevious = currentPage > 1;
        HasNext = currentPage < TotalPages;
    }
}