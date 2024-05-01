using Microsoft.EntityFrameworkCore;
using ProjectWaifu.Infrastructure;

namespace ProjectWaifu.Services;

public class TweetService
{
    public async Task AddTweet(string message)
    {
        using (var _context = new TweetContext())
        {
            await _context.Database.EnsureCreatedAsync();
            var tweet = new WaifuTweet() { Message = message, CreationDate = DateTime.Now };


            await _context.Tweets.AddAsync(tweet);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<WaifuTweet?> GetLatestTweet()
    {
        using (var _context = new TweetContext())
        {
            await _context.Database.EnsureCreatedAsync();
            return await _context.Tweets.OrderByDescending(t => t.CreationDate).FirstOrDefaultAsync();

        }
    }
}
