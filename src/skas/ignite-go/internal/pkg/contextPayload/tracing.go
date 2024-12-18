package contextPayload

import (
	"context"
	"errors"
	"github.com/google/uuid"
)

var TracingCtxKey = &contextKey{"tracing"}

type TracingMetadata struct {
	Id            string `json:"id"`
	CausationId   string `json:"causationId"`
	CorrelationId string `json:"correlationId"`
}

func ReadTracingMetadata(ctx context.Context) (TracingMetadata, error) {
	tracingMetadata, ok := ctx.Value(TracingCtxKey).(TracingMetadata)
	if !ok {
		return TracingMetadata{}, errors.New("contextPayload - ReadTracingMetadata: tracing metadata not found")
	}
	return tracingMetadata, nil
}

func ContextWithTracingMetadata(ctx context.Context, causationId, correlationId string) context.Context {
	messageId := uuid.NewString()
	messageCausationId := causationId
	messageCorrelationId := correlationId
	if len(messageCorrelationId) == 0 {
		messageCorrelationId = messageCausationId
	}
	if len(messageCorrelationId) == 0 {
		messageCorrelationId = messageId
	}
	return context.WithValue(ctx, TracingCtxKey, TracingMetadata{
		Id:            messageId,
		CausationId:   messageCausationId,
		CorrelationId: messageCorrelationId,
	})
}
