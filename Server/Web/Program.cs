// using DAL;

using Common.Util.Log.Ms;
using Common.Util.Log.Ms.Appender;
using Common.IO.Transport.Server.WebSocket;
using IsoNet.Iso.Server;

var builder = WebApplication.CreateBuilder(args);

StartServer(builder);

// Add services to the container.
builder.Services.AddRazorPages();

// builder.Services.AddScoped<IGameRepository, GameRepositoryJson>();
// builder.Services.AddScoped<IPlayerTokenRepository, PlayerTokenRepositoryJson>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();

app.Run();
return;

void StartServer(WebApplicationBuilder webApplicationBuilder)
{
    var loggerFactory = LoggerFactory.Create(builder =>
    {
        FileAppender.AnnounceAppender = ConsoleAppender.Instance;
        builder
            .SetMinimumLevel(LogLevel.Debug)
            .AddProvider(AbstractLogger.LoggerProvider<DefaultLogger>(
                ConsoleAppender.Instance));
    });

    var server = new WebSocketServer("http://localhost:7000/ws/");
    server.Logger = loggerFactory.CreateLogger("server");
    server.Start();

    var isoServer = new IsoServer(server).Init();
    isoServer.OnClientConnected += client =>
    {
        client.Rmi.Logger = loggerFactory.CreateLogger("serverRmi");
    };
    
    builder.Services.AddSingleton(server);
    builder.Services.AddSingleton(isoServer);
}
