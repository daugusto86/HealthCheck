﻿using HealthCheck.WebApp.MVC.Helpers;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.WebApp.MVC.Extensions
{
    public class SystemMemoryHealthCheck : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var client = new MemoryMetricsClient();
            var metrics = client.GetWindowsMetrics();

            var percentUsed = 100 * metrics.Used / metrics.Total;

            var status = HealthStatus.Healthy;

            if (percentUsed > 80)
            {
                status = HealthStatus.Degraded;
            }

            if (percentUsed > 90)
            {
                status = HealthStatus.Unhealthy;
            }

            var data = new Dictionary<string, object>();
            data.Add("Total", metrics.Total);
            data.Add("Used", metrics.Used);
            data.Add("Free", metrics.Free);

            var result = new HealthCheckResult(status, null, null, data);

            return await Task.FromResult(result);
        }
    }
}
