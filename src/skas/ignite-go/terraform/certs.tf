locals {
  leaf_cert         = google_privateca_certificate.default.pem_certificate
  intermediate_cert = google_privateca_certificate.default.pem_certificate_chain[0]
  root_cert         = google_privateca_certificate.default.pem_certificate_chain[1]

  cert_secrets = [
    {
      secret_id   = "skavar_projectName_CLOUDRUN_KEY"
      secret_data = data.local_sensitive_file.key.content
    },
    {
      secret_id   = "skavar_projectName_CLOUDRUN_CERT"
      secret_data = join("", [local.leaf_cert, local.intermediate_cert])
    },
    {
      secret_id   = "skavar_projectName_CLOUDRUN_CA_CERT"
      secret_data = local.root_cert
    }
  ]
}

# https://registry.terraform.io/providers/hashicorp/local/latest/docs/data-sources/sensitive_file
data "local_sensitive_file" "key" {
  filename = "private_key.pem"
}

# https://registry.terraform.io/providers/hashicorp/random/latest/docs/resources/id
resource "random_id" "suffix" {
  keepers = {
    name = data.local_sensitive_file.key.content
  }

  byte_length = 5
}

# https://registry.terraform.io/providers/hashicorp/google/latest/docs/data-sources/privateca_certificate_authority
data "google_privateca_certificate_authority" "sub_ca" {
  project                  = var.project_id
  location                 = var.region
  pool                     = "intermediate-ca"
  certificate_authority_id = "intermediate-ca"
}

# https://registry.terraform.io/providers/hashicorp/tls/latest/docs/resources/cert_request
resource "tls_cert_request" "default" {
  private_key_pem = data.local_sensitive_file.key.content

  subject {
    common_name  = var.name
    organization = "Ignite Procurement"
  }
  dns_names = [
    "${var.name}.cloudrun",
  ]
}

# https://registry.terraform.io/providers/hashicorp/google/latest/docs/resources/privateca_certificate
resource "google_privateca_certificate" "default" {
  project               = var.project_id
  pool                  = "intermediate-ca"
  location              = var.region
  certificate_authority = data.google_privateca_certificate_authority.sub_ca.certificate_authority_id
  lifetime              = "157784760s" # 5 years
  name                  = "${var.name}_cloudrun-${random_id.suffix.hex}"
  pem_csr               = tls_cert_request.default.cert_request_pem
  certificate_template  = "projects/${var.project_id}/locations/${var.region}/certificateTemplates/serverless-template"
}

# https://registry.terraform.io/providers/hashicorp/google/latest/docs/resources/secret_manager_secret
resource "google_secret_manager_secret" "cert_secrets" {
  provider  = google-beta
  for_each  = { for index, s in local.cert_secrets : s.secret_id => s }
  secret_id = each.value.secret_id
  project   = var.project_id

  replication {
    automatic = true
  }
}

# https://registry.terraform.io/providers/hashicorp/google/latest/docs/resources/secret_manager_secret_version
resource "google_secret_manager_secret_version" "cert_secret-version" {
  provider    = google-beta
  for_each    = { for index, s in local.cert_secrets : s.secret_id => s }
  secret      = "projects/${var.project_id}/secrets/${each.value.secret_id}"
  secret_data = each.value.secret_data

  depends_on = [
    google_secret_manager_secret.cert_secrets,
  ]
}
