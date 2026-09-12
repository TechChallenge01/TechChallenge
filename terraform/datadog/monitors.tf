# Alerta para falhas no processamento de Ordens de Servico (item "Alertas para
# falhas no processamento de ordens de servico" da rubrica). Usa a mesma
# metrica techchallenger.os.erros{operacao} ja publicada por DatadogMetricsService.
resource "datadog_monitor" "os_erros" {
  name = "[TechChallenge] Falhas no processamento de Ordens de Servico"
  type = "metric alert"

  message = <<-EOT
    {{#is_alert}}
    Mais de {{threshold}} falhas registradas em techchallenger.os.erros nos ultimos 5 minutos.
    {{/is_alert}}
    {{#is_warning}}
    Aumento de falhas no processamento de OS nos ultimos 5 minutos (acima de {{warn_threshold}}).
    {{/is_warning}}
    {{#is_recovery}}
    Falhas no processamento de OS voltaram ao normal.
    {{/is_recovery}}

    ${var.alert_notification}
  EOT

  query = "sum(last_5m):sum:techchallenger.os.erros{env:${var.datadog_env},service:${var.datadog_service}}.as_count() > ${var.erros_os_threshold_critical}"

  monitor_thresholds {
    critical = var.erros_os_threshold_critical
    warning  = var.erros_os_threshold_warning
  }

  notify_no_data    = false
  renotify_interval = 60
  include_tags      = true

  tags = ["env:${var.datadog_env}", "service:${var.datadog_service}"]
}

# Healthcheck/uptime do endpoint GET /health, via integracao http_check do
# proprio Datadog Agent (Autodiscovery annotation em k8s/deployment.yaml) —
# item "Healthchecks e uptime" da rubrica. Nao depende de Synthetics (fora do
# free tier), reaproveita o Agent que ja roda no cluster.
resource "datadog_monitor" "api_uptime" {
  name = "[TechChallenge] techchallenger-api fora do ar (GET /health)"
  type = "service check"

  message = <<-EOT
    {{#is_alert}}
    GET /health parou de responder em um ou mais pods do techchallenger-api.
    {{/is_alert}}
    {{#is_recovery}}
    GET /health voltou a responder normalmente.
    {{/is_recovery}}

    ${var.alert_notification}
  EOT

  query = "\"http.can_connect\".over(\"instance:techchallenger-api-health\").by(\"host\").last(4).count_by_status()"

  monitor_thresholds {
    ok       = 1
    warning  = 1
    critical = 1
  }

  notify_no_data = false
  tags           = ["env:${var.datadog_env}", "service:${var.datadog_service}"]
}
