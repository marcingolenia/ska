package jwtparser

import (
	"errors"
	"fmt"
	"github.com/golang-jwt/jwt/v4"
)

type JWTParser struct {
	signingKey string
}

type Claims struct {
	UserID   string
	Username string
	Email    string
	Tenant   string
}

func NewJWTParser(signingKey string) *JWTParser {
	return &JWTParser{signingKey: signingKey}
}

func (validator *JWTParser) Parse(tokenString string) (*Claims, error) {
	token, err := jwt.Parse(tokenString, func(token *jwt.Token) (interface{}, error) {
		return []byte(validator.signingKey), nil
	})
	if errors.Is(err, jwt.ErrTokenMalformed) {
		return nil, fmt.Errorf("jwtparser - Parse - token is malformed; %w", err)
	} else if errors.Is(err, jwt.ErrTokenExpired) || errors.Is(err, jwt.ErrTokenNotValidYet) {
		return nil, fmt.Errorf("jwtparser - Parse - token expired; %w", err)
	} else if err != nil {
		return nil, fmt.Errorf("jwtparser - Parse - unknown error; %w", err)
	}
	claimsMap, ok := token.Claims.(jwt.MapClaims)
	if !ok {
		return nil, fmt.Errorf("jwtparser - Parse - can't read claims; %w", err)
	}
	claims, err := mapClaims(claimsMap)
	if err != nil {
		return nil, fmt.Errorf("jwtparser - Parse - can't map claims; %w", err)
	}
	return claims, nil
}

func mapClaims(claims jwt.MapClaims) (*Claims, error) {
	userId := claims["user_id"]
	if userId == nil {
		return nil, errors.New("jwtparser - mapClaims - user_id doesn't exist")
	}
	username := claims["username"]
	if username == nil {
		return nil, errors.New("jwtparser - mapClaims - username doesn't exist")
	}
	email := claims["email"]
	if email == nil {
		return nil, errors.New("jwtparser - mapClaims - email doesn't exist")
	}
	tenant := claims["tenant"]
	if tenant == nil {
		return nil, errors.New("jwtparser - mapClaims - tenant doesn't exist")
	}
	return &Claims{
		UserID:   fmt.Sprint(userId),
		Username: fmt.Sprint(username),
		Email:    fmt.Sprint(email),
		Tenant:   fmt.Sprint(tenant),
	}, nil
}
