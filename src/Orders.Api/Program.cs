using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Orders.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

var app = builder.Build();

// Apply any pending EF Core migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.MapPost("/api/orders", async (OrderRequest req, AppDbContext db, CancellationToken ct) =>
{
    var order = new Order
    {
        OrderId      = req.OrderId,
        CustomerName = req.CustomerName,
        Amount       = req.Amount,
        CreatedAtUtc = req.CreatedAtUtc,
    };

    db.Orders.Add(order);

    try
    {
        await db.SaveChangesAsync(ct);
        return Results.Created($"/api/orders/{order.Id}", order);
    }
    catch (DbUpdateException ex) when (
        ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627)
        || ex.InnerException?.Message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) == true
        || ex.InnerException?.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) == true)
    {
        // Duplicate OrderId – idempotent: return 200 instead of failing
        return Results.Ok("duplicate – already processed");
    }
});

app.Run();

// Minimal request record (mirrors OrderEventDto)
record OrderRequest(string OrderId, string CustomerName, decimal Amount, DateTime CreatedAtUtc);
