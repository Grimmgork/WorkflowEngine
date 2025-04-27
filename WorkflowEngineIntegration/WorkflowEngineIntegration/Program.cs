using WorkflowEngineIntegration;
using Workflows;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IWorkflowFunctionInstanceFactory, DefaultWorkflowFunctionInstanceFactory>();
builder.Services.AddSingleton<IWorkflowSignalHandler, ConsoleSignalHandler>();
builder.Services.AddHostedService<WorkflowEngineService>();

ModuleActivator activator = new ModuleActivator();
activator.RegisterServices(builder.Services);
var app = builder.Build();
activator.RegisterWorkflowFunctions(
    app.Services.GetRequiredService<IWorkflowFunctionInstanceFactory>(), 
    app.Services.GetRequiredService<IServiceScopeFactory>());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
