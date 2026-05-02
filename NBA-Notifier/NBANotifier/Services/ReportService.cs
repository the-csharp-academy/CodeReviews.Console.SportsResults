using NBANotifier.Interfaces;
using NBANotifier.Models;
using RazorLight;

namespace NBANotifier.Services;

public class ReportService : IReportService
{
    public async Task<string> BuildHtmlReportAsync(DailyReportModel model)
    {
        var engine = new RazorLightEngineBuilder()
            .UseFileSystemProject(Utils.GetTemplateDirectory())
            .UseMemoryCachingProvider()
            .Build();

        return await engine.CompileRenderAsync("DailyReport.cshtml", model);
    }
}