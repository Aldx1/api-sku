var builder = WebApplication.CreateBuilder(args);
builder.ConfigureServices();

var app = builder.Build();
app.ConfigureApp();
app.Run();


public partial class SkuProgram { }