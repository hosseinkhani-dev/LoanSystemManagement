using Autofac;
using Autofac.Extensions.DependencyInjection;
using LoanManagement.Infrastructures.Applications;
using LoanManagement.Persistance.EF;
using LoanManagement.Persistance.EF.Users;
using LoanManagement.Services.Users;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<EFDbContext>(options =>
options.UseSqlServer(
    builder.Configuration
    .GetConnectionString("LoanManagementConnectionString")));

//Use autofac as DI container
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterAssemblyTypes(typeof(EFUserRepository).Assembly)
    .AssignableTo<Repository>()
    .AsImplementedInterfaces()
    .InstancePerLifetimeScope();

    containerBuilder.RegisterAssemblyTypes(typeof(UserAppService).Assembly)
    .AssignableTo<Service>()
    .AsImplementedInterfaces()
    .InstancePerLifetimeScope();

    containerBuilder.RegisterType<EFUnitOfWork>()
    .As<UnitOfWork>()
    .InstancePerLifetimeScope();
});


var app = builder.Build();

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
