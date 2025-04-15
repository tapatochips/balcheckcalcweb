var builder = WebApplication.CreateBuilder(args);

// add services to the container.
builder.Services.AddRazorPages();

//register application services
builder.Services.AddScoped<IPolicyCalculatorService, PolicyCalculatorService>();

var app = builder.Build();

// config the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();