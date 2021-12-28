namespace Conductor.Models
{
    public class PaginationParameter
    {        
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public bool IsValid() => PageNumber > 0 && PageSize > 0;
    }
}
