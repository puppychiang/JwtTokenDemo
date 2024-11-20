using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;                                  //Json回傳參數名稱按照model的大小寫
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; //Json回傳參數null不回傳
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create( //使用預設編碼器
            UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs);
        options.JsonSerializerOptions.WriteIndented = false;                                        //是否取消使用縮減JSON回傳
        options.JsonSerializerOptions.AllowTrailingCommas = true;                                   //允許多的","
    });

// 此API專案採用JWT方式進行身分驗證
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = Configuration["Jwt:Audience"],
            ValidateLifetime = true,   //時效過期時是否就無法使用此Token
            ClockSkew = TimeSpan.Zero, //時效過期後立即失效
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:KEY"]))
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication();// 驗證

app.UseAuthorization();// 授權

app.MapControllers();

app.Run();
