using Hotels.Web.Options;
using Hotels.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var apiSection = builder.Configuration.GetSection(RapidApiOptions.SectionName);
builder.Services.Configure<RapidApiOptions>(apiSection);
var api = apiSection.Get<RapidApiOptions>()
          ?? throw new InvalidOperationException("appsettings.json içinde 'RapidApi' bölümü bulunamadý.");

if (!api.Headers.ContainsKey("x-rapidapi-key"))
    throw new InvalidOperationException(
        "API anahtarý bulunamadý.");

// BookingApiService'e verilecek HttpClient
builder.Services.AddHttpClient<IBookingService, BookingApiService>(client =>
{
    client.BaseAddress = new Uri(api.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(api.TimeoutSeconds);

    foreach (var header in api.Headers)
        client.DefaultRequestHeaders.Add(header.Key, header.Value);
});

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
