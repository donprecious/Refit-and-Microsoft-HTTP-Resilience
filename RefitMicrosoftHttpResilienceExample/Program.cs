using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Refit;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using RefitPollyExample;

// Configure service collection
var services = new ServiceCollection();

// Configure logging
services.AddLogging(config =>
{
    config.AddConsole();
});

// Add Refit client with built-in resilience features from Microsoft.Extensions.Http.Resilience
services.AddTransient<LoggingHandler>();

services.AddRefitClient<IJsonPlaceholderApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://jsonplaceholder.typicode.com"))
    .AddHttpMessageHandler<LoggingHandler>() // Custom logging handler
    .AddStandardResilienceHandler();         // Adds built-in resilience policies

// Build the service provider
var serviceProvider = services.BuildServiceProvider();

// Get the API service
var api = serviceProvider.GetRequiredService<IJsonPlaceholderApi>();

// Use logger
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

// Fetch posts
await FetchPostsAsync(api, logger);

// Fetch a single post
await FetchPostAsync(api, 1, logger);

// Method to fetch posts
async Task FetchPostsAsync(IJsonPlaceholderApi api, ILogger logger)
{
    logger.LogInformation("Fetching all posts...");

    try
    {
        var posts = await api.GetPostsAsync();
        foreach (var post in posts)
        {
            Console.WriteLine($"Title: {post.Title}");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error fetching posts.");
    }
}

// Method to fetch a single post
async Task FetchPostAsync(IJsonPlaceholderApi api, int id, ILogger logger)
{
    logger.LogInformation("Fetching post {Id}...", id);

    try
    {
        var post = await api.GetPostAsync(id);
        Console.WriteLine($"Post {id} - Title: {post.Title}");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error fetching post {Id}.", id);
    }
}