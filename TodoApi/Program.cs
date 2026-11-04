using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   // כל כתובת
              .AllowAnyMethod()   // GET, POST, PUT, DELETE...
              .AllowAnyHeader();  // כל header
    });
});
// הזרקת ה-Context ל-Services
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration["ToDoDB"],
        ServerVersion.Parse("8.0.46-mysql")));

// הוספת Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// הפעלת Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");


// ניתוב בסיסי
app.MapGet("/", () => "ToDoApi");

// שליפת כל המשימות
app.MapGet("/items", async (ToDoDbContext db) =>
    await db.Items.ToListAsync());

// הוספת משימה חדשה
app.MapPost("/items", async (ToDoDbContext db, Item item) =>
{
    db.Items.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/items/{item.Id}", item);
});

// עדכון משימה
app.MapPut("/items/{id}", async (ToDoDbContext db, int id, Item inputItem) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    item.Name = inputItem.Name;
    item.IsComplete = inputItem.IsComplete;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// מחיקת משימה
app.MapDelete("/items/{id}", async (ToDoDbContext db, int id) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    db.Items.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
