using Hotels.Web.Options;
using Hotels.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    var messages = options.ModelBindingMessageProvider;
    messages.SetValueMustNotBeNullAccessor(_ => "Bu alan zorunludur.");
    messages.SetValueIsInvalidAccessor(value => $"'{value}' geçerli bir deðer deðil.");
    messages.SetAttemptedValueIsInvalidAccessor((value, field) => $"'{value}' geçerli bir deðer deðil.");
    messages.SetValueMustBeANumberAccessor(field => "Bu alan bir sayý olmalýdýr.");
    messages.SetMissingBindRequiredValueAccessor(field => "Bu alan zorunludur.");
    messages.SetMissingKeyOrValueAccessor(() => "Bu alan zorunludur.");
    messages.SetUnknownValueIsInvalidAccessor(field => "Girilen deðer geçersiz.");
});

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
