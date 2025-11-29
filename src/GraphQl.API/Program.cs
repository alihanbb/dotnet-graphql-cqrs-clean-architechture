using GraphQl.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddSearchServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddMessagingServices(builder.Configuration);
builder.Services.AddGraphQLServices();
builder.Services.AddHealthCheckServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseExceptionHandler("/error");
app.UseRouting();
app.MapHealthChecks("/health");
app.MapGraphQL();

app.Run();
