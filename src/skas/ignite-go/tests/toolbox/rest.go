package tests

import (
	"encoding/json"
	"io"
	"log"
	"net/http"
	"net/http/httptest"
	"skavar_projectName/composition"
	"skavar_projectName/config"
	rest "skavar_projectName/internal/drivers/rest"
)

type TestServer struct {
	mux *http.ServeMux
}

func NewTestServer() *TestServer {
	cfg, err := config.NewConfig("./toolbox/test-config.yml")
	if err != nil {
		log.Fatalf("tests - Request - can't make config: %s", err)
	}
	compositionRoot, err := composition.ComposeDependencies(cfg)
	if err != nil {
		log.Fatalf("tests - Request - can't composition root: %s", err)
	}
	mux := rest.BuildServer(&compositionRoot, false, true, "")
	return &TestServer{mux: mux}

}

func (ts *TestServer) RecordRequest(request *http.Request) *http.Response {
	responseRecorder := httptest.NewRecorder()
	ts.mux.ServeHTTP(responseRecorder, request)
	return responseRecorder.Result()
}

func (ts *TestServer) Request(method, path string, body io.Reader) *http.Response {
	request := httptest.NewRequest(method, path, body)
	request.Header.Add("Content-Type", "application/json")
	return ts.RecordRequest(request)
}

func Decode[T any](response *http.Response) T {
	result := new(T)
	json.NewDecoder(response.Body).Decode(result)
	return *result
}
