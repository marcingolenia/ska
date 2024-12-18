package middlewares

import (
	"errors"
	"net/http"
	"skavar_projectName/internal/pkg/contextPayload"
	"skavar_projectName/internal/pkg/jwtparser"
	"strings"
)

const TestTenant string = "test_tenant"
const TestUserId string = "TestUserId"
const TestUsername string = "TestUsername"

func FakeAuthentication(next http.Handler) http.Handler {
	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		ctx := contextPayload.ContextWithIgniteClaims(r.Context(), TestUserId, TestUsername, TestTenant)
		ctx = contextPayload.ContextWithAuthToken(ctx, TestTenant)
		r = r.WithContext(ctx)
		next.ServeHTTP(w, r)
	})
}

func extractToken(r *http.Request) (string, error) {
	authHeaderValue := r.Header.Get("Authorization")
	if authHeaderValue == "" {
		return "", errors.New("http - ExtractToken : Missing Authorization header")
	}
	authHeaderParts := strings.Split(authHeaderValue, " ")
	if len(authHeaderParts) != 2 || authHeaderParts[0] != "JWT" {
		return "", errors.New("http - ExtractToken : Expects Bearer token")
	}
	return authHeaderParts[1], nil
}

func JwtAuthMiddleware(signingKey string) func(http.Handler) http.Handler {
	return func(next http.Handler) http.Handler {
		tokenValidator := jwtparser.NewJWTParser(signingKey)
		return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
			token, err := extractToken(r)
			if err != nil {
				w.WriteHeader(http.StatusUnauthorized)
				w.Header().Set("Content-Type", "text/plain")
			}
			claims, err := tokenValidator.Parse(token)
			if err != nil {
				w.WriteHeader(http.StatusUnauthorized)
				w.Header().Set("Content-Type", "text/plain")
			} else {
				ctx := contextPayload.ContextWithIgniteClaims(r.Context(), claims.UserID, claims.Username, claims.Tenant)
				ctx = contextPayload.ContextWithAuthToken(ctx, token)
				r = r.WithContext(ctx)
				next.ServeHTTP(w, r)
			}
		})
	}
}
