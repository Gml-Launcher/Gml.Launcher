# Steps to Set Up CI | Инструкция по применению CI

## 🇬🇧 In English

1. **Renaming files:**
   - Remove the `.dis` extension from the file `ci.yml.dis` in the `.github/workflows/` folder:

2. **Configuring variables in `ci.yml`:**

   | Variable             | Description                                                                            | Default Value |
   | -------------------- | -------------------------------------------------------------------------------------- | ------------- |
   | BUILD_WINDOWS        | Enable Windows build (true/false)                                                      | `true`        |
   | BUILD_LINUX          | Enable Linux build (true/false)                                                        | `true`        |
   | BUILD_ARM            | Enable ARM64 build (true/false)                                                        | `false`       |
   | SELF_CONTAINED       | Publish as self-contained (user won't need to install .NET, greatly increases size)    | `false`       |
   | PUBLISH_READY_TO_RUN | Publish as ReadyToRun (optimization for faster startup, increases size and build time) | `true`        |
   | HIDE_PDB             | Hide debug files (.pdb)                                                                | `true`        |

   Open the `.github/workflows/ci.yml` file and in the `setup` section find the variables block. Change the default values as needed.

3. **Commit changes:**
   - Add changed files to git: `git add .`
   - Create a commit: `git commit -m "Enable CI workflows"`
   - Push changes: `git push`

After this, CI will automatically run on pushes to master/main branches or can be triggered manually via GitHub Actions.

## 🇷🇺 На русском языке

### Шаги по настройке CI (Continuous Integration)

1. **Переименование файлов:**
   - Удалите расширение `.dis` у файла `ci.yml.dis` в папке `.github/workflows/`:

2. **Настройте переменные в файле `ci.yml`:**

   | Переменная           | Описание                                                                                                  | Значение по умолчанию |
   | -------------------- | --------------------------------------------------------------------------------------------------------- | --------------------- |
   | BUILD_WINDOWS        | Включить сборку под Windows (true/false)                                                                  | `true`                |
   | BUILD_LINUX          | Включить сборку под Linux (true/false)                                                                    | `true`                |
   | BUILD_ARM            | Включить сборку под ARM64 (true/false)                                                                    | `false`               |
   | SELF_CONTAINED       | Публиковать как self-contained (от пользователя не потребуется установка .NET, сильно увеличивает размер) | `false`               |
   | PUBLISH_READY_TO_RUN | Публиковать как ReadyToRun (оптимизация для быстрого запуска, увеличивает размер и время сборки)          | `true`                |
   | HIDE_PDB             | Скрывать файлы отладки (.pdb)                                                                             | `true`                |

   Откройте файл `.github/workflows/ci.yml` и в разделе `setup` найдите блок с переменными. Измените значения по умолчанию на нужные.

3. **Коммит изменений:**
   - Добавьте измененные файлы в git: `git add .`
   - Создайте коммит: `git commit -m "Enable CI workflows"`
   - Отправьте изменения: `git push`

После этого CI будет автоматически запускаться при пуше в ветки master/main или можно запустить вручную через GitHub Actions.
