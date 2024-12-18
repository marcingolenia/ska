package logger

import (
	"context"
	"fmt"
	"github.com/getsentry/sentry-go"
)

type SentryLoggerDecorator struct {
	decorated Logger
	sentry    *sentry.Hub
}

func NewSentryLoggerDecorator(logger Logger, sentry *sentry.Hub) *Logger {
	var log Logger
	log = SentryLoggerDecorator{
		decorated: logger,
		sentry:    sentry,
	}
	return &log
}

func (decorator SentryLoggerDecorator) Debug(ctx context.Context, message string, metadata map[string]interface{}) {
	decorator.decorated.Debug(ctx, message, metadata)
}

func (decorator SentryLoggerDecorator) Info(ctx context.Context, message string, metadata map[string]interface{}) {
	decorator.decorated.Info(ctx, message, metadata)
}

func (decorator SentryLoggerDecorator) Warn(ctx context.Context, message string, metadata map[string]interface{}) {
	decorator.decorated.Debug(ctx, message, metadata)
}

func (decorator SentryLoggerDecorator) Error(ctx context.Context, err error, metadata map[string]interface{}) {
	decorator.reportError(err, metadata)
	decorator.decorated.Error(ctx, err, metadata)
}

func (decorator SentryLoggerDecorator) Fatal(ctx context.Context, err error, metadata map[string]interface{}) {
	decorator.reportError(err, metadata)
	decorator.decorated.Fatal(ctx, err, metadata)
}

func (decorator SentryLoggerDecorator) reportError(err error, metadata map[string]interface{}) {
	decorator.sentry.WithScope(func(s *sentry.Scope) {
		if metadata != nil {
			for k, v := range metadata {
				s.SetTag(k, fmt.Sprintf("%v", v))
			}
		}
		decorator.sentry.CaptureException(err)
	})
}
