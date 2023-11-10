using ChatApp.API.Services;
using ChatApp.API.Dao;

namespace ChatApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder.Services);
            // Add services to the container.

            //builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            //builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();

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
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // 添加你的新服务
            services.AddScoped<LoginService>();
        }

        private static void ClearOldData(object state)
        {
            DataBaseHelper dataBaseHelper = new DataBaseHelper();
            // 在这里编写每秒调用的逻辑
            dataBaseHelper.DeleteVlidateCode();
        }
    }

}