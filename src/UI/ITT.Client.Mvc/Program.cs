using ITT.Client.Mvc.ApiClients;
using ITT.Client.Mvc.Extensions;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient("Client", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Api:BaseUrl"]!);
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", string.Empty ?? string.Empty);
    client.Timeout = TimeSpan.FromMinutes(10);
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IDocumentApiClient, DocumentApiClient>();
builder.Services.AddScoped<ITextSummaryApiClient, TextSummaryApiClient>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseExceptionHandler("/Error/HandleError");
app.UseStatusCodePagesWithReExecute("/Error/HandleError/{0}");
app.UseCustomMiddleware();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Main}/{id?}");

app.Run();


