public interface ICategoryService
{
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto command);
    Task<IReadOnlyCollection<CategoryDto>> GetAllCategoriesAsync();
    Task<CategoryDto?> GetCategoryByIdAsync(int id);
    Task UpdateCategoryAsync(int id, UpdateCategoryDto command);
    Task DeleteCategoryAsync(int id);
}
