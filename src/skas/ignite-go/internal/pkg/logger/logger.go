package logger

import (
	"context"
	"os"
	"strings"

	"github.com/rs/zerolog"
)

type Logger interface {
	Debug(ctx context.Context, message string, metadata map[string]interface{})
	Info(ctx context.Context, message string, metadata map[string]interface{})
	Warn(ctx context.Context, message string, metadata map[string]interface{})
	Error(ctx context.Context, err error, metadata map[string]interface{})
	Fatal(ctx context.Context, err error, metadata map[string]interface{})
}

type ZerologLogger struct {
	logger *zerolog.Logger
}

func NewZerologLogger(level string) *ZerologLogger {
	var logLevel zerolog.Level

	switch strings.ToLower(level) {
	case "error":
		logLevel = zerolog.ErrorLevel
	case "warn":
		logLevel = zerolog.WarnLevel
	case "info":
		logLevel = zerolog.InfoLevel
	case "debug":
		logLevel = zerolog.DebugLevel
	default:
		logLevel = zerolog.InfoLevel
	}

	zerolog.SetGlobalLevel(logLevel)

	skipFrameCount := 3
	logger := zerolog.New(os.Stdout).With().Timestamp().CallerWithSkipFrameCount(zerolog.CallerSkipFrameCount + skipFrameCount).Logger()

	return &ZerologLogger{
		logger: &logger,
	}
}

func (logger *ZerologLogger) Debug(_ context.Context, message string, metadata map[string]interface{}) {
	log(logger.logger.Debug(), message, metadata)
}

func (logger *ZerologLogger) Info(_ context.Context, message string, metadata map[string]interface{}) {
	log(logger.logger.Info(), message, metadata)
}

func (logger *ZerologLogger) Warn(_ context.Context, message string, metadata map[string]interface{}) {
	log(logger.logger.Warn(), message, metadata)
}

func (logger *ZerologLogger) Error(_ context.Context, err error, metadata map[string]interface{}) {
	log(logger.logger.Error().Err(err), "", metadata)
}

func (logger *ZerologLogger) Fatal(_ context.Context, err error, metadata map[string]interface{}) {
	log(logger.logger.Fatal().Err(err), "", metadata)
}

func appendMetadata(event *zerolog.Event, metadata map[string]interface{}) {
	if metadata == nil || len(metadata) == 0 {
		return
	}
	event.Fields(metadata)
}

func log(event *zerolog.Event, message string, metadata map[string]interface{}) {
	appendMetadata(event, metadata)
	event.Msg(message)
}
