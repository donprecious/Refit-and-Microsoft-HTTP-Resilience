using Refit;

namespace RefitPollyExample;



public interface IJsonPlaceholderApi
{
    [Get("/posts")]
    Task<List<Post>> GetPostsAsync();

    [Get("/posts/{id}")]
    Task<Post> GetPostAsync(int id);
}