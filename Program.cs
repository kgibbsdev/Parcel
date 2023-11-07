using Microsoft.AspNetCore.Mvc;
using Parcel;
using System.Net;
using System.Net.Mail;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

app.UseHttpsRedirection();

string fileName = "env.json";
string jsonString = File.ReadAllText(fileName);
NetworkCredential creds = JsonSerializer.Deserialize<NetworkCredential>(jsonString);

app.MapPost("/send", ([FromBody] EmailRequest emailRequest) =>
{
    SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
    client.UseDefaultCredentials = false;
    client.Credentials = creds;
    client.EnableSsl = true;

    MailAddress from = new MailAddress(creds.UserName);
    MailAddress to = new MailAddress(emailRequest.To);

    MailMessage message = new MailMessage(from, to);

    message.Subject = emailRequest.Subject;
    message.SubjectEncoding = System.Text.Encoding.UTF8;
    message.Body = emailRequest.Body;
    message.BodyEncoding = System.Text.Encoding.UTF8;
    message.IsBodyHtml = true;

    client.Send(message);
})
.WithName("SendEmail")
.WithOpenApi();

app.Run();
