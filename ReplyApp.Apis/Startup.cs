using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NoticeApp.Models;
using UploadApp.Models;

namespace ReplyApp.Apis
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            #region CORS
            //[CORS][1] CORS 사용 등록
            //[CORS][1][1] 기본: 모두 허용
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin",
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
            //[CORS][1][2] 참고: 모두 허용
            services.AddCors(o => o.AddPolicy("AllowAllPolicy", options =>
            {
                options.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
            }));
            //[CORS][1][3] 참고: 특정 도메인만 허용
            services.AddCors(o => o.AddPolicy("AllowSpecific", options =>
                    options.WithOrigins("https://localhost:44356")
                           .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE")
                           .WithHeaders("accept", "content-type", "origin", "X-TotalRecordCount")));
            #endregion

            AddDependencyInjectionContainerForNoticeApp(services);
            AddDependencyInjectionContainerForUploadApp(services);
        }

        /// <summary>
        /// 공지사항(NoticeApp) 관련 의존성(종속성) 주입 관련 코드만 따로 모아서 관리 
        /// </summary>
        /// <param name="services"></param>
        private void AddDependencyInjectionContainerForNoticeApp(IServiceCollection services)
        {
            // NoticeAppDbContext.cs Inject: New DbContext Add
            services.AddEntityFrameworkSqlServer().AddDbContext<NoticeAppDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // INoticeRepositoryAsync.cs Inject: DI Container에 서비스(리포지토리) 등록 
            services.AddTransient<INoticeRepositoryAsync, NoticeRepositoryAsync>();
        }


        /// <summary>
        /// 자료실(UploadApp) 관련 의존성(종속성) 주입 관련 코드만 따로 모아서 관리 
        /// </summary>
        /// <param name="services"></param>
        private void AddDependencyInjectionContainerForUploadApp(IServiceCollection services)
        {
            // UploadAppDbContext.cs Inject: New DbContext Add
            services.AddEntityFrameworkSqlServer().AddDbContext<UploadAppDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // INoticeRepositoryAsync.cs Inject: DI Container에 서비스(리포지토리) 등록 
            services.AddTransient<IUploadRepository, UploadRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
