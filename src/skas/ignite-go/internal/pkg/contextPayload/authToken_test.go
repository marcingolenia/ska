package contextPayload

import (
	"context"
	"github.com/stretchr/testify/assert"
	"testing"
)

func TestCanSetAndReadTokenFromContext(t *testing.T) {
	// Arrange
	expectedToken := "MY_TOKEN"
	ctx := context.Background()
	// Act
	ctx = ContextWithAuthToken(ctx, expectedToken)
	// Assert
	result, err := ReadIgniteAuthToken(ctx)
	assert.Nil(t, err)
	assert.Equal(t, expectedToken, result)
}
