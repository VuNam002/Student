namespace Student_management.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static WebApplication ConfigureMiddlewarePipeline(this WebApplication app, string corsPolicy)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(corsPolicy);//fe
            app.UseAuthentication();//quyen
            app.UseAuthorization();
            app.MapControllers();

            return app;
        }
    }
}