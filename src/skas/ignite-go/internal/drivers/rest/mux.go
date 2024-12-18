package rest

import (
	"fmt"
	"net/http"
	"skavar_projectName/composition"
	"skavar_projectName/internal/drivers/middlewares"

	//+ska_chi_router_with_example_operations
	"skavar_projectName/internal/example"

	"github.com/go-chi/chi/v5"
	//-ska_chi_router_with_example_operations
)

func healthCheckHandler(w http.ResponseWriter, _ *http.Request) {
	w.WriteHeader(http.StatusOK)
	_, _ = fmt.Fprintf(w, "healthy")
}

func BuildServer(dependencyTree *composition.DependencyTree, useSentry, useFakeAuth bool, secretKey string) *http.ServeMux {
	mux := http.NewServeMux()
	//+ska_chi_router_with_example_operations
	r := chi.NewRouter()
	r.Use(middlewares.TracingMiddleware)
	r.Use(middlewares.Cors)
	if useFakeAuth {
		r.Use(middlewares.FakeAuthentication)
	} else {
		r.Use(middlewares.JwtAuthMiddleware(secretKey))
	}
	if useSentry {
		r.Use(middlewares.Sentry)
	}
	r.Route("/example", example.RouteTraffic(dependencyTree.Logger))
	handler := r.ServeHTTP
	mux.HandleFunc("/", handler)
	//-ska_chi_router_with_example_operations
	mux.HandleFunc("/healthz", healthCheckHandler)
	return mux
}
