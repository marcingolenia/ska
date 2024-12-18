package contextPayload

import (
	"context"
	"errors"
)

var ClaimsCtxKey = &contextKey{"claims"}

type IgniteClaims struct {
	UserId   string `json:"user_id"`
	Username string `json:"username"`
	Tenant   string `json:"tenant"`
}

func ReadIgniteClaims(ctx context.Context) (IgniteClaims, error) {
	igniteClaims, ok := ctx.Value(ClaimsCtxKey).(IgniteClaims)
	if !ok {
		return IgniteClaims{}, errors.New("contextPayload - ReadIgniteClaims: claims not found")
	}
	return igniteClaims, nil
}

func ContextWithIgniteClaims(ctx context.Context, userId, username, tenant string) context.Context {
	return context.WithValue(ctx, ClaimsCtxKey, IgniteClaims{
		Tenant:   tenant,
		UserId:   userId,
		Username: username,
	})
}
