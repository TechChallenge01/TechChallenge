# Dashboard exigido pela rubrica (item "Expor dashboards com"):
#   - Volume diario de ordens de servico
#   - Tempo medio de execucao por status (Diagnostico, Execucao, Finalizacao)
#   - Erros e falhas nas integracoes
#
# Metricas custom publicadas via DogStatsD em src/Infra/Services/DatadogMetricsService.cs.
resource "datadog_dashboard" "techchallenge_os" {
  title       = "TechChallenge API - Ordens de Servico"
  description = "Volume diario, tempo medio de execucao por status e erros/falhas nas integracoes. Provisionado via Terraform (terraform/datadog)."
  layout_type = "ordered"

  widget {
    group_definition {
      title       = "Volume diario de Ordens de Servico"
      layout_type = "ordered"

      widget {
        timeseries_definition {
          title = "OS criadas por dia"

          request {
            q            = "sum:techchallenger.os.criadas{env:${var.datadog_env},service:${var.datadog_service}}.as_count().rollup(sum, 86400)"
            display_type = "bars"
          }
        }
      }
    }
  }

  widget {
    group_definition {
      title       = "Tempo medio de execucao por status"
      layout_type = "ordered"

      widget {
        timeseries_definition {
          title = "Tempo medio de execucao (segundos) por status"

          request {
            q            = "avg:techchallenger.os.tempo_execucao_segundos.avg{env:${var.datadog_env},service:${var.datadog_service}} by {status}"
            display_type = "line"
          }
        }
      }
    }
  }

  widget {
    group_definition {
      title       = "Erros e falhas nas integracoes"
      layout_type = "ordered"

      widget {
        timeseries_definition {
          title = "Erros por operacao"

          request {
            q            = "sum:techchallenger.os.erros{env:${var.datadog_env},service:${var.datadog_service}} by {operacao}.as_count()"
            display_type = "bars"
          }
        }
      }

      widget {
        toplist_definition {
          title = "Ranking de erros por operacao (24h)"

          request {
            q = "top(sum:techchallenger.os.erros{env:${var.datadog_env},service:${var.datadog_service}} by {operacao}.as_count(), 10, 'sum', 'desc')"
          }
        }
      }
    }
  }

  widget {
    group_definition {
      title       = "Latencia das APIs e recursos de Kubernetes"
      layout_type = "ordered"

      widget {
        timeseries_definition {
          title = "Latencia das APIs (p95, via APM)"

          request {
            q            = "p95:trace.aspnet_core.request{env:${var.datadog_env},service:${var.datadog_service}} by {resource_name}"
            display_type = "line"
          }
        }
      }

      widget {
        timeseries_definition {
          title = "CPU/memoria dos pods (kubelet)"

          request {
            q            = "avg:kubernetes.cpu.usage.total{env:${var.datadog_env},service:${var.datadog_service}} by {pod_name}"
            display_type = "line"
          }

          request {
            q            = "avg:kubernetes.memory.usage{env:${var.datadog_env},service:${var.datadog_service}} by {pod_name}"
            display_type = "line"
          }
        }
      }
    }
  }
}
