using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using newsmng_bussinessobject;

namespace newsmng_dao
{
    public class NewsArticleDAO
    {

        private FunewsManagementContext _dbContext;
        private static NewsArticleDAO instance;

        public NewsArticleDAO()
        {
            _dbContext = new FunewsManagementContext();
        }

        public static NewsArticleDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NewsArticleDAO();
                }
                return instance;
            }
        }

        public NewsArticle GetOne(string id)
        {
            return _dbContext.NewsArticles
                .Include(a => a.Tags)
                .SingleOrDefault(a => a.NewsArticleId.Equals(id));
        }

        public List<NewsArticle> GetAll()
        {
            return _dbContext.NewsArticles
                .Include(m => m.Category)
                .Include(m => m.CreatedBy)
                .ToList();
        }

        public List<NewsArticle> GetAllTrue()
        {
            return _dbContext.NewsArticles
                .Where(m => m.NewsStatus.Equals(true))
                .Include(m => m.Category)
                .Include(m => m.CreatedBy)
                .ToList();
        }

        public void Add(NewsArticle article)
        {
            NewsArticle cur = GetOne(article.NewsArticleId);
            if (cur != null)
            {
                throw new Exception("News article with this ID already exists");
            }

            // Handle the tags properly
            if (article.Tags != null && article.Tags.Any())
            {
                var tagIds = article.Tags.Select(t => t.TagId).ToList();

                // Clear the original collection
                var originalTags = article.Tags.ToList();
                article.Tags.Clear();

                // Fetch and attach existing tags from the database
                foreach (var tagId in tagIds)
                {
                    var dbTag = _dbContext.Tags.Find(tagId);
                    if (dbTag != null)
                    {
                        article.Tags.Add(dbTag);
                    }
                }
            }

            _dbContext.NewsArticles.Add(article);
            _dbContext.SaveChanges();
        }

        public void Update(NewsArticle article)
        {
            NewsArticle existingArticle = _dbContext.NewsArticles
                .Include(a => a.Tags)
                .FirstOrDefault(a => a.NewsArticleId.Equals(article.NewsArticleId));
                
            if (existingArticle == null)
            {
                throw new Exception("News article not found");
            }
            
            // Update basic properties
            _dbContext.Entry(existingArticle).CurrentValues.SetValues(article);
            
            // Handle tag relationships
            if (article.Tags != null)
            {
                // Get the IDs of the new tags
                var newTagIds = article.Tags.Select(t => t.TagId).ToList();
                
                // Remove tags that are no longer selected
                var tagsToRemove = existingArticle.Tags
                    .Where(t => !newTagIds.Contains(t.TagId))
                    .ToList();
                    
                foreach (var tag in tagsToRemove)
                {
                    existingArticle.Tags.Remove(tag);
                }
                
                // Add newly selected tags
                foreach (var tagId in newTagIds)
                {
                    // Check if the tag is already associated with this article
                    if (!existingArticle.Tags.Any(t => t.TagId == tagId))
                    {
                        // Find the tag in the database
                        var dbTag = _dbContext.Tags.Find(tagId);
                        if (dbTag != null)
                        {
                            existingArticle.Tags.Add(dbTag);
                        }
                    }
                }
            }
            else
            {
                // If no tags were passed, remove all tags
                existingArticle.Tags.Clear();
            }
            
            _dbContext.SaveChanges();
        }

        public void Delete(string id)
        {
            var article = _dbContext.NewsArticles
                .Include(a => a.Tags) // Load related tags
                .FirstOrDefault(a => a.NewsArticleId.Equals(id));

            if (article != null)
            {
                // Clear the relationship between article and tags
                // This will remove entries from the join table without deleting the tags
                if (article.Tags != null)
                {
                    article.Tags.Clear();
                    _dbContext.SaveChanges(); // Save changes to remove relationships
                }

                // Now remove the article
                _dbContext.NewsArticles.Remove(article);
                _dbContext.SaveChanges(); // Delete the article
            }
        }

        public List<NewsArticle> Search(string title, string content)
        {
            if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(content))
                return GetAll();

            if (title != null)
            {
                title = title.Trim().ToLower();
            }
            if (content != null)
            {
                content = content.Trim().ToLower();
            }

            return _dbContext.NewsArticles
                .Where(m =>
                    m.NewsTitle.ToLower().Contains(title) ||
                    m.NewsContent.ToLower().Contains(content))
                .OrderBy(e => e.NewsArticleId)
                .ToList();
        }

        public List<NewsArticle> SearchTrue(string title, string content)
        {
            // If both title and content are empty or whitespace, return all articles with NewsStatus = true
            if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(content))
            {
                return _dbContext.NewsArticles
                    .Where(m => m.NewsStatus == true)
                    .OrderBy(e => e.NewsArticleId)
                    .ToList();
            }

            // Trim and convert to lowercase for case-insensitive search
            if (title != null)
            {
                title = title.Trim().ToLower();
            }
            if (content != null)
            {
                content = content.Trim().ToLower();
            }

            // Return articles with NewsStatus = true and matching title or content
            return _dbContext.NewsArticles
                .Where(m =>
                    m.NewsStatus == true && // Ensure only articles with NewsStatus = true are considered
                    (string.IsNullOrEmpty(title) || m.NewsTitle.ToLower().Contains(title)) && // Match title if provided
                    (string.IsNullOrEmpty(content) || m.NewsContent.ToLower().Contains(content))) // Match content if provided
                .OrderBy(e => e.NewsArticleId)
                .ToList();
        }

        public class PageableResult<T>
        {
            public List<T> Data { get; set; }
            public int TotalPages { get; set; }
        }

        public PageableResult<NewsArticle> Pageable(List<NewsArticle> data, int curPage, int pageSize)
        {
            if (curPage <= 0)
                throw new ArgumentException("Current page must be greater than 0.");
            if (pageSize <= 0)
                throw new ArgumentException("Number of items per page must be greater than 0.");

            // Get total count of items
            int totalItems = data.Count();

            // Calculate total pages
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Get data for current page
            var Res = data
                .OrderBy(e => e.NewsArticleId) // Make sure results are in a deterministic order
                .Skip((curPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PageableResult<NewsArticle>
            {
                Data = Res,
                TotalPages = totalPages
            };
        }

        public Dictionary<int, int> GetDashboard(int year)
        {
            // Initialize dictionary with all months set to 0
            var monthlyData = new Dictionary<int, int>();
            for (int month = 1; month <= 12; month++)
            {
                monthlyData[month] = 0;
            }

            // Get start and end dates for the year
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year, 12, 31, 23, 59, 59);

            // Query to get news articles created in the specified year
            var articlesInYear = _dbContext.NewsArticles
                .Where(a => a.CreatedDate >= startDate && a.CreatedDate <= endDate)
                .ToList();

            // Group by month and count
            // Handle potential null values or date format issues
            var monthlyGroups = articlesInYear
                .Where(a => a.CreatedDate != null)
                .GroupBy(a => ((DateTime)a.CreatedDate).Month)  // Ensure proper DateTime access
                .Select(g => new { Month = g.Key, Count = g.Count() });

            // Update the dictionary with actual counts
            foreach (var group in monthlyGroups)
            {
                monthlyData[group.Month] = group.Count;
            }

            return monthlyData;
        }
    }
}