using Amazon.Runtime;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using st_dotnet.Data;
using st_dotnet.Models;
using Microsoft.AspNetCore.Identity;
using st_dotnet.Areas.Identity.Data;


var builder = WebApplication.CreateBuilder(args);
const string CORS_POLICY = "CORS_POLICY";

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IChannelRepository, ChannelRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IChannelTypeRepository, ChannelTypeRepository>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CORS_POLICY, builder =>
    {
        //for when you're running on localhost
        builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
        .AllowAnyHeader().AllowAnyMethod();


        //builder.WithOrigins("url from where you're trying to do the requests")
    });
});

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



var credentials = builder.Environment.IsDevelopment() ?
    new BasicAWSCredentials(
        builder.Configuration["S3:AccessKeyId"],
        builder.Configuration["S3:SecretKey"]
    ) :
    new BasicAWSCredentials(
        builder.Configuration.GetSection("S3_Config")["AccessKeyId"],
        builder.Configuration.GetSection("S3_Config")["SecretKey"]
    );

var awsOption = builder.Configuration.GetAWSOptions();
awsOption.Credentials = credentials;
awsOption.Region = Amazon.RegionEndpoint.EUCentral1;
builder.Services.AddDefaultAWSOptions(awsOption);
builder.Services.AddAWSService<IAmazonS3>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

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

app.UseCors(CORS_POLICY);


app.Run();


