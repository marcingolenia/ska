package logger

import "context"

type MetadataLoggerDecorator struct {
	decorated   Logger
	getMetadata func(ctx context.Context) (map[string]string, error)
}

func NewMetadataLoggerDecorator(logger Logger, getMetadata func(ctx context.Context) (map[string]string, error)) *Logger {
	var log Logger
	log = MetadataLoggerDecorator{
		decorated:   logger,
		getMetadata: getMetadata,
	}
	return &log
}

func (decorator MetadataLoggerDecorator) combineMetadata(ctx context.Context, metadata map[string]interface{}) map[string]interface{} {
	output := map[string]interface{}{}
	if metadata != nil {
		for k, v := range metadata {
			output[k] = v
		}
	}
	decoratedMetadata, err := decorator.getMetadata(ctx)
	if err == nil {
		if decoratedMetadata != nil {
			for k, v := range decoratedMetadata {
				output[k] = v
			}
		}
	}
	return output
}

func (decorator MetadataLoggerDecorator) Debug(ctx context.Context, message string, metadata map[string]interface{}) {
	combinedMetadata := decorator.combineMetadata(ctx, metadata)
	decorator.decorated.Debug(ctx, message, combinedMetadata)
}

func (decorator MetadataLoggerDecorator) Info(ctx context.Context, message string, metadata map[string]interface{}) {
	combinedMetadata := decorator.combineMetadata(ctx, metadata)
	decorator.decorated.Info(ctx, message, combinedMetadata)
}

func (decorator MetadataLoggerDecorator) Warn(ctx context.Context, message string, metadata map[string]interface{}) {
	combinedMetadata := decorator.combineMetadata(ctx, metadata)
	decorator.decorated.Warn(ctx, message, combinedMetadata)
}

func (decorator MetadataLoggerDecorator) Error(ctx context.Context, err error, metadata map[string]interface{}) {
	combinedMetadata := decorator.combineMetadata(ctx, metadata)
	decorator.decorated.Error(ctx, err, combinedMetadata)
}

func (decorator MetadataLoggerDecorator) Fatal(ctx context.Context, err error, metadata map[string]interface{}) {
	combinedMetadata := decorator.combineMetadata(ctx, metadata)
	decorator.decorated.Fatal(ctx, err, combinedMetadata)
}
