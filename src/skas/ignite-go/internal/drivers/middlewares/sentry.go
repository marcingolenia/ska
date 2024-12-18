package middlewares

import (
	"net/http"

	sentryhttp "github.com/getsentry/sentry-go/http"
)

func Sentry(h http.Handler) http.Handler {
	sentryHandler := sentryhttp.New(sentryhttp.Options{Repanic: true})
	return sentryHandler.HandleFunc(func(rw http.ResponseWriter, r *http.Request) {
		h.ServeHTTP(rw, r)
	})
}
