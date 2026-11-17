using st_dotnet.Data;
using st_dotnet.Models;
using st_dotnet.UnitTests.Helpers;

namespace st_dotnet.UnitTests.Data;

public class SqlCategoryDataTests : IDisposable
{
    private readonly SqliteDb db = new();
    private readonly GalleryDbContext.SqlCategoryData data;

    public SqlCategoryDataTests()
    {
        data = new GalleryDbContext.SqlCategoryData(db.Context);
    }

    public void Dispose() => db.Dispose();

    private Category Seed(string name)
    {
        var category = new Category { Name = name, Image = $"http://img/{name}" };
        data.Add(category);
        data.Commit();
        return category;
    }

    [Fact]
    public void Add_then_Commit_persists_the_category()
    {
        var category = Seed("Music");

        using var fresh = db.NewContext();
        var stored = Assert.Single(fresh.categories);
        Assert.Equal(category.Id, stored.Id);
        Assert.Equal("Music", stored.Name);
    }

    [Fact]
    public void Add_without_Commit_does_not_persist()
    {
        data.Add(new Category { Name = "Pending", Image = "x" });

        using var fresh = db.NewContext();
        Assert.Empty(fresh.categories);
    }

    [Fact]
    public void GetbyId_returns_category_with_its_channels()
    {
        var category = Seed("Games");
        db.Context.channels.Add(new Channel
        {
            Name = "speedruns", Image = "i", Preview = "p", Content = "c", ChannelTypeId = 1,
            Categories = new List<Category> { category }
        });
        db.Context.SaveChanges();
        db.Context.ChangeTracker.Clear();

        var found = data.GetbyId(category.Id);

        Assert.NotNull(found);
        Assert.Equal("speedruns", Assert.Single(found.Channels).Name);
    }

    [Fact]
    public void GetbyId_returns_null_when_missing()
    {
        Assert.Null(data.GetbyId(999));
    }

    [Fact]
    public void GetbyName_returns_first_prefix_match_in_alphabetical_order()
    {
        Seed("Music");
        Seed("Movies");
        Seed("Games");

        Assert.Equal("Movies", data.GetbyName("M").Name);
        Assert.Equal("Music", data.GetbyName("Mu").Name);
    }

    [Fact]
    public void GetbyName_returns_null_when_nothing_matches()
    {
        Seed("Music");

        Assert.Null(data.GetbyName("Zzz"));
    }

    [Fact]
    public void Search_returns_categories_containing_the_text_ordered_by_name()
    {
        Seed("Retro games");
        Seed("Board games");
        Seed("Music");

        var names = data.Search("games").Select(c => c.Name).ToList();

        Assert.Equal(new[] { "Board games", "Retro games" }, names);
    }

    [Fact]
    public void Delete_removes_existing_category_after_Commit()
    {
        var category = Seed("Music");

        var deleted = data.Delete(category.Id);
        data.Commit();

        Assert.Same(category, deleted);
        using var fresh = db.NewContext();
        Assert.Empty(fresh.categories);
    }

    [Fact]
    public void Delete_returns_null_when_missing()
    {
        Assert.Null(data.Delete(42));
    }

    [Fact]
    public void Update_persists_changes_after_Commit()
    {
        var category = Seed("Music");
        db.Context.ChangeTracker.Clear();

        data.Update(new Category { Id = category.Id, Name = "Live music", Image = category.Image });
        data.Commit();

        using var fresh = db.NewContext();
        Assert.Equal("Live music", fresh.categories.Single().Name);
    }
}
