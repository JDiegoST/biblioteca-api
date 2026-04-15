using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Aplication.DTOs.common
{
    public class PaginationParams : Params
    {
        private const int MaxPageSize = PaginationPolicy.MAX_PAGE_SIZE;

        [Range(1, int.MaxValue, ErrorMessage = $"PageNumber debe ser mayor o igualW a 1")]
        public int PageNumber { get; set; } = PaginationPolicy.DEFAULT_PAGE_NUMBER;

        private int _pageSize = PaginationPolicy.DEFAULT_PAGE_SIZE;

        [Range(1, PaginationPolicy.MAX_PAGE_SIZE, ErrorMessage = "PageSize debe de estar entre el {1} y {2}")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}
