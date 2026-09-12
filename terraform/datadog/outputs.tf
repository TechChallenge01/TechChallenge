output "dashboard_url" {
  description = "URL do dashboard no Datadog"
  value       = "https://${var.datadog_site}${datadog_dashboard.techchallenge_os.url}"
}
