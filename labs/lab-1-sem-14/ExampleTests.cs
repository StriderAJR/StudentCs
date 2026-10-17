using NUnit.Framework;

public class ReportBuilderTests
{
    [Test]
    public void BuildReport_ContainsServerName()
    {
        var report = ReportBuilder.Build("report-data.txt");
        Assert.That(report, Does.Contain("Northland-01"));
    }
}
