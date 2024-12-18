terraform {
  backend "gcs" {
    bucket = "ignite-cloudrun"
    prefix = "cr-skavar_projectName"
  }
  required_providers {
    cloudflare = {
      source  = "cloudflare/cloudflare"
      version = "~> 3.0"
    }
    google = {
      source  = "hashicorp/google"
      version = ">= 4.39.0, < 5.0"
    }
    google-beta = {
      source  = "hashicorp/google-beta"
      version = ">= 4.39.0, < 5.0"
    }
  }

  required_version = ">= 1.3"
}

provider "google" {
  project = var.project_id
  region  = var.region
}

provider "google-beta" {
  project = var.project_id
  region  = var.region
}
