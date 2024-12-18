package composition

import (
	"context"
	"skavar_projectName/config"
	"skavar_projectName/internal/pkg/contextPayload"
	"skavar_projectName/internal/pkg/logger"

	"github.com/getsentry/sentry-go"
)

type DependencyTree struct {
	Logger logger.Logger
}

func ComposeDependencies(settings *config.Config) (DependencyTree, error) {
	loggerToUse := initLogger(&settings.Log)
	return DependencyTree{
		Logger: *loggerToUse,
	}, nil
}

func readTracingMetadataMapper() func(ctx context.Context) (map[string]string, error) {
	return func(ctx context.Context) (map[string]string, error) {
		tracingMetadata, err := contextPayload.ReadTracingMetadata(ctx)
		if err != nil {
			return map[string]string{}, err
		}
		return map[string]string{"id": tracingMetadata.Id, "correlationId": tracingMetadata.CorrelationId, "causationId": tracingMetadata.CausationId}, nil
	}
}

func initLogger(cfg *config.Log) *logger.Logger {
	var appLogger logger.Logger
	appLogger = logger.NewZerologLogger(cfg.Level)
	if cfg.UseSentry {
		sentryHub := sentry.CurrentHub()
		decoratedLogger := logger.NewSentryLoggerDecorator(appLogger, sentryHub)
		appLogger = *decoratedLogger
	}
	metadataDecoratedLogger := logger.NewMetadataLoggerDecorator(appLogger, readTracingMetadataMapper())
	appLogger = *metadataDecoratedLogger
	return &appLogger
}
