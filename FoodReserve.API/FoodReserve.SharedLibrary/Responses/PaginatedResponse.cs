namespace FoodReserve.SharedLibrary.Responses
{
    public class PaginatedResponse<T>
    {
        public required T Response { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }
    }
}
