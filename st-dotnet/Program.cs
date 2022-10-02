using Amazon.Runtime;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using st_dotnet.Data;
using st_dotnet.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IChannelRepository, ChannelRepository>();

builder.Services.AddDbContextPool<GalleryDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
        options.UseSqlServer(builder.Configuration["GalleryDb:Conn"]);
    else
        options.UseSqlServer(builder.Configuration.GetConnectionString("GalleryDb"));
});
builder.Services.AddScoped<ICategoryData, GalleryDbContext.SqlCategoryData>();
builder.Services.AddScoped<IChannelData, GalleryDbContext.SqlChannelData>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

