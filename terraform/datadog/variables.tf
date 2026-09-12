variable "datadog_api_key" {
  description = "Datadog API Key (mesma usada pelo Agent, ver k8s/datadog-secret.yaml / secret DATADOG_API_KEY)"
  type        = string
  sensitive   = true
}

variable "datadog_app_key" {
  description = "Datadog Application Key (Organization Settings > Application Keys) — necessaria para o provider gerenciar dashboards via API"
  type        = string
  sensitive   = true
}

variable "datadog_site" {
  description = "Site do Datadog (ex.: datadoghq.com, datadoghq.eu, us5.datadoghq.com)"
  type        = string
  default     = "datadoghq.com"
}

variable "datadog_env" {
  description = "Tag env usada nas queries (bate com DD_ENV do configmap.yaml)"
  type        = string
  default     = "production"
}

variable "datadog_service" {
  description = "Tag service usada nas queries (bate com DD_SERVICE do configmap.yaml)"
  type        = string
  default     = "techchallenger-api"
}

variable "alert_notification" {
  description = "Handle de notificacao anexado as mensagens dos monitors (ex.: \"@slack-tech-challenge\", \"@time@empresa.com\"). Vazio = alerta so fica visivel na UI do Datadog, sem notificar ninguem."
  type        = string
  default     = ""
}

variable "erros_os_threshold_critical" {
  description = "Quantidade de erros em techchallenger.os.erros nos ultimos 5 minutos que dispara alerta critico"
  type        = number
  default     = 10
}

variable "erros_os_threshold_warning" {
  description = "Quantidade de erros em techchallenger.os.erros nos ultimos 5 minutos que dispara alerta de warning"
  type        = number
  default     = 5
}
