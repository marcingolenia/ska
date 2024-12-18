package contextPayload

import (
	"context"
	"errors"
)

var AuthTokenCtxKey = &contextKey{"authToken"}

func ReadIgniteAuthToken(ctx context.Context) (string, error) {
	igniteClaims, ok := ctx.Value(AuthTokenCtxKey).(string)
	if !ok {
		return "", errors.New("contextPayload - ReadIgniteAuthToken: token not found")
	}
	return igniteClaims, nil
}

func ContextWithAuthToken(ctx context.Context, authToken string) context.Context {
	return context.WithValue(ctx, AuthTokenCtxKey, authToken)
}
