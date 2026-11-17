using NSubstitute;
using st_dotnet.Data;
using st_dotnet.Models;

namespace st_dotnet.UnitTests.Models;

public class CategoryRepositoryTests
{
    private readonly ICategoryData data = Substitute.For<ICategoryData>();
    private readonly CategoryRepository repository;

    public CategoryRepositoryTests()
    {
        repository = new CategoryRepository(data);
    }

    [Fact]
    public void Add_adds_and_commits()
    {
        var category = new Category { Name = "Music" };

        var result = repository.Add(category);

        Assert.Same(category, result);
        Received.InOrder(() =>
        {
            data.Add(category);
            data.Commit();
        });
    }

    [Fact]
    public void Update_updates_and_commits()
    {
        var category = new Category { Id = 3, Name = "Music" };

        repository.Update(category);

        Received.InOrder(() =>
        {
            data.Update(category);
            data.Commit();
        });
    }

    [Fact]
    public void Delete_deletes_commits_and_returns_deleted()
    {
        var category = new Category { Id = 3 };
        data.Delete(3).Returns(category);

        Assert.Same(category, repository.Delete(3));
        data.Received(1).Commit();
    }

    [Fact]
    public void GetAll_and_AllCategories_return_data_GetAll()
    {
        var all = new List<Category> { new() { Name = "a" } };
        data.GetAll().Returns(all);

        Assert.Same(all, repository.GetAll());
        Assert.Same(all, repository.AllCategories);
    }

    [Fact]
    public void Lookups_delegate_to_data()
    {
        var category = new Category { Id = 1, Name = "Music" };
        var found = new List<Category> { category };
        data.GetbyId(1).Returns(category);
        data.GetbyName("Mu").Returns(category);
        data.Search("us").Returns(found);

        Assert.Same(category, repository.GetbyId(1));
        Assert.Same(category, repository.GetbyName("Mu"));
        Assert.Same(found, repository.Search("us"));
    }

    [Fact]
    public void GetChannels_returns_category_channels_or_empty_when_missing()
    {
        var channels = new List<Channel> { new() { Name = "c" } };
        data.GetbyId(1).Returns(new Category { Id = 1, Channels = channels });
        data.GetbyId(2).Returns((Category)null);

        Assert.Same(channels, repository.GetChannels(1));
        Assert.Empty(repository.GetChannels(2));
        Assert.Same(channels, ((ICategoryRepository)repository).GetChannels(1).Channels);
    }
}

public class ChannelRepositoryTests
{
    private readonly IChannelData data = Substitute.For<IChannelData>();
    private readonly ChannelRepository repository;

    public ChannelRepositoryTests()
    {
        repository = new ChannelRepository(data);
    }

    [Fact]
    public void Every_operation_delegates_to_data()
    {
        var channel = new Channel { Id = 1, Name = "speedruns" };
        var list = new List<Channel> { channel };
        data.Add(channel).Returns(channel);
        data.Update(channel).Returns(channel);
        data.Delete(1).Returns(channel);
        data.GetbyId(1).Returns(channel);
        data.GetbyName("speedruns").Returns(channel);
        data.GetAll().Returns(list);
        data.Search("speed").Returns(list);

        Assert.Same(channel, repository.Add(channel));
        Assert.Same(channel, repository.Update(channel));
        Assert.Same(channel, repository.Delete(1));
        Assert.Same(channel, repository.GetbyId(1));
        Assert.Same(channel, repository.GetbyName("speedruns"));
        Assert.Same(list, repository.All());
        Assert.Same(list, repository.Search("speed"));
    }
}

public class ChannelTypeRepositoryTests
{
    private readonly IChannelTypeData data = Substitute.For<IChannelTypeData>();
    private readonly ChannelTypeRepository repository;

    public ChannelTypeRepositoryTests()
    {
        repository = new ChannelTypeRepository(data);
    }

    [Fact]
    public void Add_and_Update_return_the_given_entity()
    {
        var type = new ChannelType { Id = 1, Name = "Video" };

        Assert.Same(type, repository.Add(type));
        Assert.Same(type, repository.Update(type));
        data.Received(1).Add(type);
        data.Received(1).Update(type);
    }

    [Fact]
    public void Lookups_and_delete_delegate_to_data()
    {
        var type = new ChannelType { Id = 1, Name = "Video" };
        var list = new List<ChannelType> { type };
        data.GetbyId(1).Returns(type);
        data.GetbyName("Vi").Returns(type);
        data.GetAll().Returns(list);
        data.Delete(1).Returns(type);

        Assert.Same(type, repository.GetbyId(1));
        Assert.Same(type, repository.GetbyName("Vi"));
        Assert.Same(list, repository.GetAll());
        Assert.Same(type, repository.Delete(1));
    }
}

public class CommentRepositoryTests
{
    private readonly ICommentData data = Substitute.For<ICommentData>();
    private readonly CommentRepository repository;

    public CommentRepositoryTests()
    {
        repository = new CommentRepository(data);
    }

    [Fact]
    public void Every_operation_delegates_to_data()
    {
        var comment = new Comment { Id = 1, ChannelId = 7, Text = "hi" };
        var list = new List<Comment> { comment };
        data.Add(comment).Returns(comment);
        data.Delete(1).Returns(comment);
        data.GetAll().Returns(list);
        data.GetChannelComments(7).Returns(list);

        Assert.Same(comment, repository.Add(comment));
        Assert.Same(comment, repository.Delete(1));
        Assert.Same(list, repository.All());
        Assert.Same(list, repository.GetChannelComments(7));

        repository.DeleteChannelComments(7);
        data.Received(1).DeleteChannelComments(7);
    }
}
