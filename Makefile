MAKEFLAGS += --silent


help: ## Показывает список команд
	@printf "\033[33m%s:\033[0m\n" 'Доступные команды'
	@awk 'BEGIN {FS = ":.*?## "} /^[a-zA-Z_-]+:.*?## / {printf "  \033[32m%-18s\033[0m %s\n", $$1, $$2}' $(MAKEFILE_LIST)

install: ## Установка зависимостей
	cd frontend && npm install
	cd backend && dotnet restore

# start: start-frontend start-backend info ## Запуск приложения

start-frontend: ## Запуск фронтенда
	cd frontend && npm run dev
start-backend: ## Запуск бекенда
	cd backend/Source/Presentation/ChimpSolution.API && dotnet run --launch-profile ChimpSolution.API

cs-fix: ## Форматирование кода
	cd frontend && npm run cs:fix

info:
	tail -n "+17" README.md
