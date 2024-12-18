package tests

import (
	"io"
	toolbox "skavar_projectName/tests/toolbox"
	"testing"

	"github.com/stretchr/testify/require"
)

func TestRestHealthCheck(t *testing.T) {
	testServer := toolbox.NewTestServer()
	resp := testServer.Request("GET", "/healthz", nil)
	responseBody, _ := io.ReadAll(resp.Body)
	require.Equal(t, "healthy", string(responseBody))
}
