# Установите переменные
$projectPath = "../src/Gml.Launcher/" # Не менять!
$outputDir = "/*/*/Gml.Backend/data/GmlBackend/LauncherBuilds" # Путь к директории на сервере куда загружать файлы
$server = "" # IP-адрес сервера ssh
$user = "" # Пользователь сервера ssh
$date = Get-Date -Format "yyyy-MM-dd_HH-mm" 

# Функция для билда и деплоя
function BuildAndDeploy {
    param (
        [string]$runtime,
        [string]$outputFolder
    )

    # Билд проекта
    dotnet publish $projectPath -r $runtime -c Release -f net8.0 -p:PublishSingleFile=$true --self-contained $true -p:IncludeNativeLibrariesForSelfExtract=$true -p:IncludeAllContentForSelfExtract=$true -p:PublishReadyToRun=$true

    # Создание директории на сервере
    ssh $user@$server "mkdir -p $outputDir/$date/$outputFolder"

    # Загрузка файлов на сервер
    scp -r "${projectPath}/bin/Release/net8.0/${runtime}/publish/*" "${user}@${server}:${outputDir}/${date}/${outputFolder}/"
}

# Билд и деплой для linux-x64
BuildAndDeploy -runtime "linux-x64" -outputFolder "linux-x64"

# Билд и деплой для win-x64
BuildAndDeploy -runtime "win-x64" -outputFolder "win-x64"
