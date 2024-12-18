package config

import (
	"fmt"

	"github.com/ilyakaznacheev/cleanenv"
)

type (
	Config struct {
		App  `yaml:"app"`
		HTTP `yaml:"http"`
		Log  `yaml:"logger"`
		JWT  `yaml:"jwt"`
	}

	JWT struct {
		UseFakeAuth bool   `yaml:"use_fake_auth"    env:"JWT_USE_FAKE_AUTH" env-default:"false"`
		SecretKey   string `yaml:"secret_key"    env:"JWT_SECRET_KEY"`
	}

	Log struct {
		Level     string `env-required:"true" yaml:"log_level"   env:"LOG_LEVEL"`
		UseSentry bool   `yaml:"use_sentry" env:"LOG_USE_SENTRY" env-default:"false"`
		SentryDSN string `yaml:"sentry_dsn"   env:"LOG_SENTRY_DSN"`
	}

	App struct {
		Name        string `env-required:"true" yaml:"name"    env:"APP_NAME"`
		Version     string `env-required:"true" yaml:"version" env:"APP_VERSION"`
		Environment string `env-required:"true" yaml:"environment" env:"APP_ENVIRONMENT"`
	}

	HTTP struct {
		Port string `env-required:"true" yaml:"port" env:"HTTP_PORT"`
	}
)

func NewConfig(path string) (*Config, error) {
	cfg := &Config{}

	err := cleanenv.ReadConfig(path, cfg)
	if err != nil {
		return nil, fmt.Errorf("config - NewConfig - can't read config: %w", err)
	}

	err = cleanenv.ReadEnv(cfg)
	if err != nil {
		return nil, fmt.Errorf("config - NewConfig - can't read env: %w", err)
	}

	return cfg, nil
}
