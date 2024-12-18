# https://registry.terraform.io/providers/hashicorp/google/latest/docs/resources/google_service_account
resource "google_service_account" "sa" {
  project      = var.project_id
  account_id   = "cr-${var.name}"
  display_name = "cr-${var.name}"
  description  = "Service account for ${var.name} cloud run"
}

# https://registry.terraform.io/providers/hashicorp/google/latest/docs/resources/secret_manager_secret_iam
resource "google_secret_manager_secret_iam_member" "secrets" {
  for_each  = var.env_vars_secrets
  project   = var.project_id
  role      = "roles/secretmanager.secretAccessor"
  secret_id = each.value
  member    = "serviceAccount:${google_service_account.sa.email}"
}

resource "google_secret_manager_secret_iam_member" "volume_secrets" {
  for_each = {
    for index, volume_secret in var.volumes_secrets :
    volume_secret.name => volume_secret
  }
  project   = var.project_id
  role      = "roles/secretmanager.secretAccessor"
  secret_id = each.value.secret_name
  member    = "serviceAccount:${google_service_account.sa.email}"
}

resource "google_cloud_run_service_iam_member" "member" {
  project  = google_cloud_run_service.default.project
  service  = google_cloud_run_service.default.name
  location = google_cloud_run_service.default.location
  role     = "roles/run.developer"
  member   = "serviceAccount:${google_service_account.sa.email}"
}

data "google_cloud_run_service" "run-service" {
  name     = var.name
  location = var.region
}

resource "google_cloud_run_service" "default" {
  provider = google-beta

  name                       = var.name
  location                   = var.region
  autogenerate_revision_name = true

  template {
    spec {
      service_account_name = google_service_account.sa.email
      containers {
        image = var.docker_image
        startup_probe {
          initial_delay_seconds = 30
          timeout_seconds       = 5
          period_seconds        = 20
          failure_threshold     = 10
          tcp_socket {
            port = 8080
          }
        }
        liveness_probe {
          http_get {
            path = "/healthz"
          }
        }
        dynamic "env" {
          for_each = var.env_vars
          content {
            name  = env.key
            value = env.value
          }
        }
        dynamic "env" {
          for_each = var.env_vars_secrets
          content {
            name = env.key
            value_from {
              secret_key_ref {
                key  = "latest"
                name = env.value
              }
            }
          }
        }
        dynamic "volume_mounts" {
          for_each = var.volumes_secrets

          content {
            name       = volume_mounts.value.name
            mount_path = volume_mounts.value.path
          }
        }
      }
      dynamic "volumes" {
        for_each = var.volumes_secrets

        content {
          name = volumes.value.name

          secret {
            secret_name = volumes.value.secret_name

            items {
              key  = "latest"
              path = volumes.value.filename
              mode = volumes.value.mode
            }
          }
        }
      }
    }
    # vpc-access-egress and vpc-access-connector is needed to let container call services inside private network
    # cpu-throttling is for always on
    metadata {
      annotations = {
        "run.googleapis.com/vpc-access-egress"    = "all-traffic"
        "run.googleapis.com/vpc-access-connector" = "cloud-functions"
        "run.googleapis.com/cpu-throttling"       = false
      }
    }
  }


  traffic {
    percent         = 100
    latest_revision = true
  }
  metadata {
    // Accept requests from an external HTTP(S) load balancer (Cloudflare) but not directly from the internet.
    // https://cloud.google.com/run/docs/securing/ingress
    annotations = {
      "run.googleapis.com/ingress" = "internal-and-cloud-load-balancing"
    }
  }
  timeouts {
    create = "5m"
    update = "5m"
    delete = "5m"
  }
  lifecycle {
    ignore_changes = [
      metadata.0.annotations,
      template.0.metadata.0.annotations
    ]
  }

  depends_on = [
    google_service_account.sa,
    google_secret_manager_secret_version.cert_secret-version
  ]
}

# https://registry.terraform.io/providers/hashicorp/google/latest/docs/resources/cloud_run_service_iam
resource "google_cloud_run_service_iam_member" "noauth" {
  project  = google_cloud_run_service.default.project
  service  = google_cloud_run_service.default.name
  location = google_cloud_run_service.default.location
  role     = "roles/run.invoker"
  member   = "allUsers"
}
