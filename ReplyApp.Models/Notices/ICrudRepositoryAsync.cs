using Dul.Domain.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoticeApp.Models
{
    /// <summary>
    /// [3] Generic Repository Interface
    /// </summary>
    public interface ICrudRepositoryAsync<T>
    {
        Task<T> AddAsync(T model); // 입력
        Task<List<T>> GetAllAsync(); // 출력
        Task<T> GetByIdAsync(int id); // 상세
        Task<bool> EditAsync(T model); // 수정
        Task<bool> DeleteAsync(int id); // 삭제
        Task<PagingResult<T>> GetAllAsync(int pageIndex, int pageSize); // 페이징
        Task<PagingResult<T>> GetAllByParentIdAsync(int pageIndex, int pageSize, int parentId); // 부모
        Task<PagingResult<T>> SearchAllAsync(int pageIndex, int pageSize, string searchQuery); // 검색
        Task<PagingResult<T>> SearchAllByParentIdAsync(int pageIndex, int pageSize, string searchQuery, int parentId); // 검색+부모
    }
}
