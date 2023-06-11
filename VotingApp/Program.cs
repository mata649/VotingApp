using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using VotingApp.Context;
using VotingApp.Infrastructure;
using VotingApp.Option.Application;
using VotingApp.Option.Domain;
using VotingApp.Poll.Application;
using VotingApp.Poll.Domain;
using VotingApp.Rest;
using VotingApp.User.Application;
using VotingApp.User.Domain;
using VotingApp.Vote.Application;
using VotingApp.Vote.Data;
using VotingApp.Vote.Domain;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Users
builder.Services.AddScoped<IUserService, UserService>();


// polls
builder.Services.AddScoped<IPollService, PollService>();
// Options
builder.Services.AddScoped<IOptionService, OptionService>();
// Votes
builder.Services.AddScoped<IVoteService, VoteService>();
builder.Services.AddScoped<IVoteNotificationService, VoteNotificationService>();
builder.Services.AddSingleton<IVoteCountCache, VoteCountCache>();

// BD
builder.Services.AddNpgsql<VotingAppContext>(builder.Configuration.GetConnectionString("cnVotingApp"));

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Authentication

builder.Services.AddScoped<JWT>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(s: builder.Configuration["Jwt:Key"])),
        ValidateIssuer = false,
        ValidateAudience = false,

    };
});

builder.Services.AddAuthorization();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<DashboardHub>("/dashboard");

app.MapControllers();

app.Run();
