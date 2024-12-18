# https://registry.terraform.io/providers/hashicorp/google/latest/docs/resources/compute_backend_service
resource "google_compute_backend_service" "default" {
  name = "${var.name}-backend"

  protocol    = "HTTP"
  port_name   = "http"
  timeout_sec = 30

  backend {
    group = google_compute_region_network_endpoint_group.cloudrun_neg.id
  }
}

# https://registry.terraform.io/providers/hashicorp/google/latest/docs/resources/compute_region_network_endpoint_group
resource "google_compute_region_network_endpoint_group" "cloudrun_neg" {
  name                  = "${var.name}-neg"
  network_endpoint_type = "SERVERLESS"
  region                = var.region
  cloud_run {
    service = google_cloud_run_service.default.name
  }
}

# https://registry.terraform.io/providers/hashicorp/google/latest/docs/resources/compute_managed_ssl_certificate
resource "google_compute_managed_ssl_certificate" "default" {
  provider = google-beta

  name = "${var.name}-cert"
  managed {
    domains = ["${var.name}.igniteprocurement.com"]
  }
}

# https://registry.terraform.io/providers/hashicorp/google/latest/docs/resources/compute_global_forwarding_rule
resource "google_compute_global_forwarding_rule" "default" {
  name       = "${var.name}-fwdrule"
  target     = google_compute_target_https_proxy.default.id
  port_range = "443"
  ip_address = google_compute_global_address.static.address
}

# https://registry.terraform.io/providers/hashicorp/google/latest/docs/resources/compute_url_map
resource "google_compute_url_map" "default" {
  name = "${var.name}-urlmap"

  default_service = google_compute_backend_service.default.id
}

# https://registry.terraform.io/providers/hashicorp/google/latest/docs/resources/compute_target_https_proxy
resource "google_compute_target_https_proxy" "default" {
  name = "${var.name}-https-proxy"

  url_map          = google_compute_url_map.default.id
  ssl_certificates = [google_compute_managed_ssl_certificate.default.id]
}

# https://registry.terraform.io/providers/hashicorp/google/latest/docs/resources/compute_address
resource "google_compute_global_address" "static" {
  name = var.name
}

# HTTP-to-HTTPS resources

resource "google_compute_url_map" "https_redirect" {
  name = "${var.name}-https-redirect"

  default_url_redirect {
    https_redirect         = true
    redirect_response_code = "MOVED_PERMANENTLY_DEFAULT"
    strip_query            = false
  }
}

resource "google_compute_target_http_proxy" "https_redirect" {
  name    = "${var.name}-http-proxy"
  url_map = google_compute_url_map.https_redirect.id
}

resource "google_compute_global_forwarding_rule" "https_redirect" {
  name = "${var.name}-fwdrule-https"

  target     = google_compute_target_http_proxy.https_redirect.id
  port_range = "80"
  ip_address = google_compute_global_address.static.address
}

# https://registry.terraform.io/providers/cloudflare/cloudflare/latest/docs/resources/record
resource "cloudflare_record" "record" {
  zone_id = var.cloudflare_zone_id
  name    = var.name
  value   = google_compute_global_address.static.address
  type    = "A"
  ttl     = 1
  proxied = true
}
