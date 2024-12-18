package main

import (
	"context"
	"fmt"
	"log"
	"net/http"
	"os"
	"os/signal"
	"skavar_projectName/composition"
	"skavar_projectName/config"
	"skavar_projectName/internal/drivers/rest"
	"syscall"
	"time"

	"github.com/getsentry/sentry-go"
)

func main() {

	interrupt := make(chan os.Signal, 1)
	serverNotify := make(chan error, 1)

	ctx := context.Background()
	ctx, cancel := context.WithCancel(ctx)
	signal.Notify(interrupt, os.Interrupt, syscall.SIGTERM)
	configPath := "./config/config.yml"
	cfg, err := config.NewConfig(configPath)
	if err != nil {
		log.Fatalf("main - Config error: %s", err)
	}
	initSentry(cfg)
	dependencies, err := composition.ComposeDependencies(cfg)
	if err != nil {
		log.Fatalf("main - can't make composition root: %s", err.Error())
	}

	if err != nil {
		dependencies.Logger.Fatal(ctx, fmt.Errorf("main - can't migrate existing tenants: %w", err), nil)
	}

	restHandlers := rest.BuildServer(&dependencies, cfg.Log.UseSentry, cfg.JWT.UseFakeAuth, cfg.JWT.SecretKey)
	httpServer := http.Server{Addr: fmt.Sprintf(":%s", cfg.HTTP.Port), Handler: restHandlers}
	go func() {
		dependencies.Logger.Info(ctx, fmt.Sprintf("main - httpserver %s: starting ...", cfg.HTTP.Port), nil)
		serverNotify <- httpServer.ListenAndServe()
		close(serverNotify)
	}()

	select {
	case s := <-interrupt:
		dependencies.Logger.Info(ctx, fmt.Sprintf("main - signal: %s", s.String()), nil)
	case err := <-serverNotify:
		dependencies.Logger.Error(ctx, fmt.Errorf("main - httpServer - notify: %w", err), nil)
	}
	cancel()
	err = shutdownHttpServerGracefully(&httpServer)
	if err != nil {
		dependencies.Logger.Error(ctx, fmt.Errorf("main - httpServer.Shutdown: %w", err), nil)
	}
}

func shutdownHttpServerGracefully(server *http.Server) error {
	ctx, cancel := context.WithTimeout(context.Background(), time.Second*10)
	defer cancel()

	return server.Shutdown(ctx)
}

func initSentry(cfg *config.Config) {
	if cfg.Log.UseSentry {
		err := sentry.Init(sentry.ClientOptions{
			Dsn:              cfg.Log.SentryDSN,
			AttachStacktrace: true,
			TracesSampleRate: 1.0,
			Environment:      cfg.App.Environment,
		})
		if err != nil {
			log.Fatalf("main - can't init sentry: %s", err)
		}
	}
}
