package middlewares

import (
	"net/http"
	"skavar_projectName/internal/pkg/contextPayload"
)

const correlationIDHeader = "x-correlation-id"
const requestIDHeader = "x-request-id"

func TracingMiddleware(next http.Handler) http.Handler {
	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		correlationID := r.Header.Get(correlationIDHeader)
		requestID := r.Header.Get(requestIDHeader)
		ctx := contextPayload.ContextWithTracingMetadata(r.Context(), requestID, correlationID)
		r = r.WithContext(ctx)
		if len(correlationID) != 0 {
			w.Header().Add(correlationIDHeader, correlationID)
		}
		if len(requestID) != 0 {
			w.Header().Add(requestIDHeader, requestID)
		}
		next.ServeHTTP(w, r)
	})
}
