package contextPayload

import (
	"context"
	"github.com/stretchr/testify/assert"
	"testing"
)

func TestCanSetAndReadClaimsFromContext(t *testing.T) {
	// Arrange
	expectedUserId := "12345"
	expectedTenant := "Tenant"
	expectedUsername := "MrTest"
	ctx := context.Background()
	// Act
	ctx = ContextWithIgniteClaims(ctx, expectedUserId, expectedUsername, expectedTenant)
	// Assert
	result, err := ReadIgniteClaims(ctx)
	assert.Nil(t, err)
	assert.Equal(t, expectedTenant, result.Tenant)
	assert.Equal(t, expectedUserId, result.UserId)
	assert.Equal(t, expectedUsername, result.Username)
}
