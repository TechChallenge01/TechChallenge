output "cluster_name" {
  description = "Nome do cluster EKS"
  value       = aws_eks_cluster.cluster.name
}

output "cluster_endpoint" {
  description = "Endpoint da API do EKS"
  value       = aws_eks_cluster.cluster.endpoint
}

output "database_endpoint" {
  description = "Endpoint de conexao do RDS SQL Server"
  value       = aws_db_instance.sqlserver.endpoint
}

output "database_username" {
  description = "Usuario master do banco"
  value       = aws_db_instance.sqlserver.username
  sensitive   = true
}
