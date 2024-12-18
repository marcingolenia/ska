variable "docker_image" {
  type = string
}

variable "project_id" {
  type    = string
  default = "skavar_googleCloudProject"
}

variable "region" {
  type    = string
  default = "europe-north1"
}

variable "name" {
  type    = string
  default = "skavar_projectName"
}

variable "env_vars" {
  description = "The key/value environment variables for the service."
  type        = map(string)
  default = {
    "APP_ENVIRONMENT" : "production"
    "HTTP_PORT" : "8080"
    "GRPC_PORT" : "42079"
    "LOG_LEVEL" : "error"
    "LOG_USE_SENTRY" : "true"
    "JWT_USE_FAKE_AUTH" : "false"
  }
}

variable "env_vars_secrets" {
  description = "The key/value environment variables from secret manager for the service."
  type        = map(string)
  default = {
    "JWT_SECRET_KEY" : "SECRET_KEY"
    #"LOG_SENTRY_DSN" : "skavar_projectName_PRODUCTION_SENTRY_DSN"
  }
}

variable "volumes_secrets" {
  type = list(object({
    name        = string
    secret_name = string
    path        = string
    filename    = string
    mode        = optional(number)
  }))

  default = [
  ]
  description = "Bucket for storing files. For instance certificates (DB, TLS)."
}


variable "cloudflare_zone_id" {
  type        = string
  description = "The DNS zone ID to add the record to"
  default     = "3ac45fb6f121becd9180ae39efa57d53"
}
