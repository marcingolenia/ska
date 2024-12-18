package logger

import (
	"fmt"
	"github.com/getsentry/sentry-go"
)

func CreateSentryHubFromConfig(sentryDsn, env, version string) (*sentry.Hub, error) {
	opts := sentry.ClientOptions{
		Dsn:         sentryDsn,
		Environment: env,
		Release:     version,
	}
	err := sentry.Init(opts)
	if err != nil {
		return nil, fmt.Errorf("createSentryHubFromConfig - can't init sentry client; %w", err)
	}
	sentryClient, err := sentry.NewClient(opts)
	if err != nil {
		return nil, fmt.Errorf("createSentryHubFromConfig - can't create sentry client; %w", err)
	}

	return sentry.NewHub(sentryClient, sentry.NewScope()), nil
}
