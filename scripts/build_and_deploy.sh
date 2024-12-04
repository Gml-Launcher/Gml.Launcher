#!/bin/bash

# Установите переменные
PROJECT_PATH="../src/Gml.Launcher/" # Не менять!
OUTPUT_DIR="/*/*/Gml.Backend/data/GmlBackend/LauncherBuilds" # Путь к директории на сервере куда загружать файлы
SERVER="" # IP-адрес сервера ssh 
USER="" # Пользователя сервера ssh
DATE=$(date +%Y-%m-%d_%H-%M)

# Функция для билда и деплоя
build_and_deploy() {
  local runtime=$1
  local output_folder=$2

  # Билд проекта
  dotnet publish $PROJECT_PATH -r $runtime -c Release -f net8.0 -p:PublishSingleFile=true --self-contained true -p:IncludeNativeLibrariesForSelfExtract=true -p:IncludeAllContentForSelfExtract=true -p:PublishReadyToRun=true

  # Создание директории на сервере
  ssh $USER@$SERVER "mkdir -p $OUTPUT_DIR/$DATE/$output_folder"

  # Загрузка файлов на сервер
  scp -r $PROJECT_PATH/bin/Release/net8.0/$runtime/publish/* $USER@$SERVER:$OUTPUT_DIR/$DATE/$output_folder/
}

# Билд и деплой для linux-x64
build_and_deploy "linux-x64" "linux-x64"

# Билд и деплой для win-x64
build_and_deploy "win-x64" "win-x64"
