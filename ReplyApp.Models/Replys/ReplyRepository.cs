using Dul.Articles;
using Dul.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReplyApp.Models
{
    /// <summary>
    /// [6] Repository Class: ADO.NET or Dapper or Entity Framework Core
    /// </summary>
    public class ReplyRepository : IReplyRepository
    {
        private readonly ReplyAppDbContext _context;
        private readonly ILogger _logger;

        public ReplyRepository(ReplyAppDbContext context, ILoggerFactory loggerFactory)
        {
            this._context = context;
            this._logger = loggerFactory.CreateLogger(nameof(ReplyRepository));
        }

        //[6][1] 입력
        public async Task<Reply> AddAsync(Reply model)
        {
            try
            {
                _context.Replys.Add(model);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError($"ERROR({nameof(AddAsync)}): {e.Message}");
            }

            return model; 
        }

        //[6][2] 출력
        public async Task<List<Reply>> GetAllAsync()
        {
            return await _context.Replys.OrderByDescending(m => m.Id)
                //.Include(m => m.ReplysComments)
                .ToListAsync(); 
        }

        //[6][3] 상세
        public async Task<Reply> GetByIdAsync(int id)
        {
            return await _context.Replys
                //.Include(m => m.ReplysComments)
                .SingleOrDefaultAsync(m => m.Id == id);
        }

        //[6][4] 수정
        public async Task<bool> EditAsync(Reply model)
        {
            try
            {
                _context.Replys.Attach(model);
                _context.Entry(model).State = EntityState.Modified;
                return (await _context.SaveChangesAsync() > 0 ? true : false);
            }
            catch (Exception e)
            {
                _logger.LogError($"ERROR({nameof(EditAsync)}): {e.Message}");
            }

            return false; 
        }

        //[6][5] 삭제
        public async Task<bool> DeleteAsync(int id)
        {
            //var model = await _context.Replys.SingleOrDefaultAsync(m => m.Id == id);
            try
            {
                var model = await _context.Replys.FindAsync(id);
                _context.Remove(model);
                return (await _context.SaveChangesAsync() > 0 ? true : false);
            }
            catch (Exception e)
            {
                _logger.LogError($"ERROR({nameof(DeleteAsync)}): {e.Message}");
            }

            return false; 
        }

        //[6][6] 페이징
        public async Task<PagingResult<Reply>> GetAllAsync(int pageIndex, int pageSize)
        {
            var totalRecords = await _context.Replys.CountAsync();
            var models = await _context.Replys
                .OrderByDescending(m => m.Id)
                //.Include(m => m.ReplysComments)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagingResult<Reply>(models, totalRecords); 
        }

        // 부모
        public async Task<PagingResult<Reply>> GetAllByParentIdAsync(int pageIndex, int pageSize, int parentId)
        {
            var totalRecords = await _context.Replys.Where(m => m.ParentId == parentId).CountAsync();
            var models = await _context.Replys
                .Where(m => m.ParentId == parentId)
                .OrderByDescending(m => m.Id)
                //.Include(m => m.ReplysComments)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagingResult<Reply>(models, totalRecords);
        }

        // 상태
        public async Task<Tuple<int, int>> GetStatus(int parentId)
        {
            var totalRecords = await _context.Replys.Where(m => m.ParentId == parentId).CountAsync();
            var pinnedRecords = await _context.Replys.Where(m => m.ParentId == parentId && m.IsPinned == true).CountAsync();

            return new Tuple<int, int>(pinnedRecords, totalRecords); // (2, 10)
        }

        // DeleteAllByParentId
        public async Task<bool> DeleteAllByParentId(int parentId)
        {
            try
            {
                var models = await _context.Replys.Where(m => m.ParentId == parentId).ToListAsync();

                foreach (var model in models)
                {
                    _context.Replys.Remove(model);
                }

                return (await _context.SaveChangesAsync() > 0 ? true : false);

            }
            catch (Exception e)
            {
                _logger.LogError($"ERROR({nameof(DeleteAllByParentId)}): {e.Message}");
            }

            return false; 
        }

        // 검색
        public async Task<PagingResult<Reply>> SearchAllAsync(int pageIndex, int pageSize, string searchQuery)
        {
            var totalRecords = await _context.Replys
                .Where(m => m.Name.Contains(searchQuery) || m.Title.Contains(searchQuery) || m.Title.Contains(searchQuery))
                .CountAsync();
            var models = await _context.Replys
                .Where(m => m.Name.Contains(searchQuery) || m.Title.Contains(searchQuery) || m.Title.Contains(searchQuery))
                .OrderByDescending(m => m.Id)
                //.Include(m => m.ReplysComments)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagingResult<Reply>(models, totalRecords);
        }

        public async Task<PagingResult<Reply>> SearchAllByParentIdAsync(int pageIndex, int pageSize, string searchQuery, int parentId)
        {
            var totalRecords = await _context.Replys.Where(m => m.ParentId == parentId)
                .Where(m => EF.Functions.Like(m.Name, $"%{searchQuery}%") || m.Title.Contains(searchQuery) || m.Title.Contains(searchQuery))
                .CountAsync();
            var models = await _context.Replys.Where(m => m.ParentId == parentId)
                .Where(m => m.Name.Contains(searchQuery) || m.Title.Contains(searchQuery) || m.Title.Contains(searchQuery))
                .OrderByDescending(m => m.Id)
                //.Include(m => m.ReplysComments)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagingResult<Reply>(models, totalRecords);
        }

        public async Task<SortedList<int, double>> GetMonthlyCreateCountAsync()
        {
            SortedList<int, double> createCounts = new SortedList<int, double>();

            // 1월부터 12월까지 0.0으로 초기화
            for (int i = 1; i <= 12; i++)
            {
                createCounts[i] = 0.0; 
            }

            for (int i = 0; i < 12; i++)
            {
                // 현재 달부터 12개월 전까지 반복
                var current = DateTime.Now.AddMonths(-i);
                var cnt = _context.Replys.AsEnumerable().Where(
                    m => m.Created != null
                    &&
                    Convert.ToDateTime(m.Created).Month == current.Month
                    &&
                    Convert.ToDateTime(m.Created).Year == current.Year
                ).ToList().Count();
                createCounts[current.Month] = cnt;  
            }

            return await Task.FromResult(createCounts);
        }

        public async Task<PagingResult<Reply>> GetAllByParentKeyAsync(int pageIndex, int pageSize, string parentKey)
        {
            var totalRecords = await _context.Replys.Where(m => m.ParentKey == parentKey).CountAsync();
            var models = await _context.Replys
                .Where(m => m.ParentKey == parentKey)
                .OrderByDescending(m => m.Id)
                //.Include(m => m.ReplysComments)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagingResult<Reply>(models, totalRecords);
        }

        public async Task<PagingResult<Reply>> SearchAllByParentKeyAsync(int pageIndex, int pageSize, string searchQuery, string parentKey)
        {
            var totalRecords = await _context.Replys.Where(m => m.ParentKey == parentKey)
                .Where(m => EF.Functions.Like(m.Name, $"%{searchQuery}%") || m.Title.Contains(searchQuery) || m.Title.Contains(searchQuery))
                .CountAsync();
            var models = await _context.Replys.Where(m => m.ParentKey == parentKey)
                .Where(m => m.Name.Contains(searchQuery) || m.Title.Contains(searchQuery) || m.Title.Contains(searchQuery))
                .OrderByDescending(m => m.Id)
                //.Include(m => m.ReplysComments)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagingResult<Reply>(models, totalRecords);
        }

        public async Task<ArticleSet<Reply, int>> GetArticles<TParentIdentifier>(int pageIndex, int pageSize, string searchField, string searchQuery, string sortOrder, TParentIdentifier parentIdentifier) 
        {
            //var items = from m in _context.Replys select m; // 쿼리 구문(Query Syntax)
            var items = _context.Replys.Select(m => m); // 메서드 구문(Method Syntax)

            // ParentBy 
            if (parentIdentifier is int parentId && parentId != 0)
            {
                items = items.Where(m => m.ParentId == parentId);
            }
            else if (parentIdentifier is string parentKey && !string.IsNullOrEmpty(parentKey))
            {
                items = items.Where(m => m.ParentKey == parentKey); 
            }

            // Search Mode
            if (!string.IsNullOrEmpty(searchQuery))
            {
                if (searchField == "Name")
                {
                    // Name
                    items = items.Where(m => m.Name.Contains(searchQuery));
                }
                else if (searchField == "Title")
                {
                    // Title
                    items = items.Where(m => m.Title.Contains(searchQuery));
                }
                else
                {
                    // All
                    items = items.Where(m => m.Name.Contains(searchQuery) || m.Title.Contains(searchQuery));
                }
            }

            var totalCount = await items.CountAsync();

            // Sorting
            switch (sortOrder)
            {
                case "Name":
                    items = items.OrderBy(m => m.Name);
                    break;
                case "NameDesc":
                    items = items.OrderByDescending(m => m.Name);
                    break;
                case "Title":
                    items = items.OrderBy(m => m.Title);
                    break;
                case "TitleDesc":
                    items = items.OrderByDescending(m => m.Title);
                    break;
                default:
                    items = items.OrderByDescending(m => m.Id); 
                    break;
            }

            // Paging
            items = items.Skip(pageIndex * pageSize).Take(pageSize);

            return new ArticleSet<Reply, int>(await items.ToListAsync(), totalCount);
        }
    }
}
