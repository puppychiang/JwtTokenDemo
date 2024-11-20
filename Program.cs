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
        options.JsonSerializerOptions.PropertyNamingPolicy = null;                                  //Json�^�ǰѼƦW�٫���model���j�p�g
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; //Json�^�ǰѼ�null���^��
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create( //�ϥιw�]�s�X��
            UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs);
        options.JsonSerializerOptions.WriteIndented = false;                                        //�O�_�����ϥ��Y��JSON�^��
        options.JsonSerializerOptions.AllowTrailingCommas = true;                                   //���\�h��","
    });

// ��API�M�ױĥ�JWT�覡�i�樭������
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = Configuration["Jwt:Audience"],
            ValidateLifetime = true,   //�ɮĹL���ɬO�_�N�L�k�ϥΦ�Token
            ClockSkew = TimeSpan.Zero, //�ɮĹL����ߧY����
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

app.UseAuthentication();// ����

app.UseAuthorization();// ���v

app.MapControllers();

app.Run();
