using Microsoft.EntityFrameworkCore;

public class CategoryService : ICategoryService
{
    private readonly MovieDbContext _dbContext;

    public CategoryService(MovieDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto command)
    {
        var category = Category.Create(command.Name);

        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync();

        return new CategoryDto(category.Id, category.Name);
    }

        public async Task<IReadOnlyCollection<CategoryDto>> GetAllCategoriesAsync()
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(c.Id, c.Name))
            .ToListAsync();
    }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CategoryDto(c.Id, c.Name))
            .FirstOrDefaultAsync();
    }

    public async Task UpdateCategoryAsync(int id, UpdateCategoryDto command)
    {
        var category = await _dbContext.Categories.FindAsync(id);
        if (category is null)
            throw new ArgumentNullException($"Invalid Category Id.");

        category.Update(command.Name);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _dbContext.Categories.FindAsync(id);

        if (category is null)
            return;

        _dbContext.Categories.Remove(category);
        await _dbContext.SaveChangesAsync();
    }
}
