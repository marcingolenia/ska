package example

import (
	"context"
	"skavar_projectName/internal/pkg/contextPayload"
	"skavar_projectName/internal/pkg/logger"

	"encoding/json"
	"errors"
	"fmt"
	"net/http"

	"github.com/go-chi/chi/v5"
	"github.com/go-chi/render"
)

type ExampleDto struct {
	Message string `json:"message"`
}

func (dto *ExampleDto) Bind(r *http.Request) error {
	if dto == nil {
		return errors.New("missing required fields.")
	}
	return nil
}

type ExampleRouter struct {
	logger logger.Logger
}

func RouteTraffic(logger logger.Logger) func(r chi.Router) {
	router := ExampleRouter{logger: logger}
	return func(r chi.Router) {
		r.Get("/", router.GetExample)
		r.Post("/", router.CreateExample)
		r.Delete("/{id}", router.DeleteExample)
	}
}

func (self *ExampleRouter) GetExample(w http.ResponseWriter, r *http.Request) {
	handleError := func(ctx context.Context, err error, refId string) {
		self.logger.Error(ctx, fmt.Errorf("GetExample;%w", err), map[string]interface{}{"refId": refId})
	}
	ctx := r.Context()
	claims, err := contextPayload.ReadIgniteClaims(ctx)
	self.logger.Info(ctx, claims.Tenant, nil)
	if err != nil {
		self.logger.Warn(ctx, fmt.Sprintf("GetExample;%s", err.Error()), nil)
		w.WriteHeader(http.StatusBadRequest)
		return
	}
	refId := r.URL.Query().Get("refId")
	dtos := []ExampleDto{
		{
			Message: fmt.Sprintf("Hello There! %s", claims.Tenant),
		},
	}
	if err != nil {
		handleError(ctx, err, refId)
		w.WriteHeader(http.StatusInternalServerError)
	} else {
		err := json.NewEncoder(w).Encode(dtos)
		if err != nil {
			handleError(ctx, err, refId)
			return
		}
		w.Header().Set("Content-Type", "application/json")
	}
}

func (self *ExampleRouter) DeleteExample(w http.ResponseWriter, r *http.Request) {
	ctx := r.Context()
	idAsString := chi.URLParam(r, "id")
	self.logger.Info(ctx, fmt.Sprintf("Delete: %s", idAsString), nil)
	w.Header().Set("Content-Type", "application/json")
}

func (self *ExampleRouter) CreateExample(w http.ResponseWriter, r *http.Request) {
	data := &ExampleDto{}
	if err := render.Bind(r, data); err != nil {
		w.WriteHeader(http.StatusBadRequest)
		return
	}
	example := ExampleDto{
		Message: data.Message,
	}
	w.Header().Set("Content-Type", "application/json")
	w.Header().Add("Location", fmt.Sprintf("/example/%s", "imagineItsAnId"))
	w.WriteHeader(http.StatusCreated)
	err := json.NewEncoder(w).Encode(example)
	if err != nil {
		w.WriteHeader(http.StatusInternalServerError)
		return
	}
}
