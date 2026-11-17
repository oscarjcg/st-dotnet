using Amazon.Runtime;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using st_dotnet.Data;
using st_dotnet.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.HttpOverrides;
using st_dotnet.Areas.Identity.Data;
using st_dotnet.Services;


var builder = WebApplication.CreateBuilder(args);
const string CORS_POLICY = "CORS_POLICY";

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IChannelRepository, ChannelRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IChannelTypeRepository, ChannelTypeRepository>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: CORS_POLICY, builder =>
        {
            //for when you're running on localhost
            builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
            .AllowAnyHeader().AllowAnyMethod();
        });
    });
}
else
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: CORS_POLICY, builder =>
        {
            builder.WithOrigins(
                "http://simple-twitch.codename-project.com",
                "https://simple-twitch.codename-project.com",
                "http://st-react.codename-project.com",
                "https://st-react.codename-project.com")
                .AllowAnyHeader().AllowAnyMethod();
        });
    });
}

builder.Services.AddDbContextPool<GalleryDbContext>(options =>
{
    //if (builder.Environment.IsDevelopment())
    //{
        options.UseMySql(
            builder.Configuration["GalleryMySql:Conn"], 
            ServerVersion.AutoDetect(builder.Configuration["GalleryMySql:Conn"])
        );
    //}
});

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<GalleryDbContext>();
builder.Services.AddScoped<ICategoryData, GalleryDbContext.SqlCategoryData>();
builder.Services.AddScoped<IChannelData, GalleryDbContext.SqlChannelData>();
builder.Services.AddScoped<ICommentData, GalleryDbContext.SqlCommentData>();
builder.Services.AddScoped<IChannelTypeData, GalleryDbContext.SqlChannelTypeData>();



var credentials = new BasicAWSCredentials(
    builder.Configuration["S3:AccessKeyId"],
    builder.Configuration["S3:SecretKey"]
);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.Configure<S3Settings>(builder.Configuration.GetSection(S3Settings.SectionName));
var s3ServiceUrl = builder.Configuration["S3:ServiceUrl"];
if (string.IsNullOrWhiteSpace(s3ServiceUrl))
{
    var awsOption = builder.Configuration.GetAWSOptions();
    awsOption.Credentials = credentials;
    awsOption.Region = Amazon.RegionEndpoint.EUCentral1;
    builder.Services.AddDefaultAWSOptions(awsOption);
    builder.Services.AddAWSService<IAmazonS3>();
}
else
{
    builder.Services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(credentials, new AmazonS3Config
    {
        ServiceURL = s3ServiceUrl,
        ForcePathStyle = true,
        AuthenticationRegion = Amazon.RegionEndpoint.EUCentral1.SystemName
    }));
}

builder.Services.AddHttpClient<INewMessageNotifier, NodeNewMessageNotifier>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(5);
});


var app = builder.Build();

if (app.Configuration.GetValue<bool>("Testing:EnsureDatabaseCreated"))
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<GalleryDbContext>().Database.EnsureCreated();
}

/* TODO: Fix migrations on fresh db
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<GalleryDbContext>();
    db.Database.Migrate();
}
*/

// Configure the HTTP request pipeline.
app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseCors(CORS_POLICY);

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();


if (app.Environment.IsDevelopment())
{
    app.MapGet("/debug/routes", (IEnumerable<EndpointDataSource> endpointSources) =>
        string.Join("\n", endpointSources.SelectMany(source => source.Endpoints)));
}

app.Run();


