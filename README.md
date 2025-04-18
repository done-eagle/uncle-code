## Инструмент сортировки импортов в проекте

### Как пользоваться (JetBrains Rider):
- Подключаем к проекту как `ProjectReference` (в будущем как `PackageReference`) с параметрами `OutputItemType="Analyzer"` и `ReferenceOutputAssembly="false"`
- Подкидываем в корень проекта файл `.editorconfig` с указанием префиксов корпоративных проектов, которые будут импортироваться в файлах с кодом.

  Пример:
  `[*.cs]
  dotnet_sorting_corporate_prefixes = MyCompany1.,MyCompany2.`
  
- Rider автоматически его подхватит, подсветит проблемные места и даст соответствующие предупреждения в панели ошибок.
- Исправления кода доступны через Alt+Enter в контекстном меню IDE.
  
- Примечание: Для корректной работы файлик `.editorconfig` должен отобразиться в *.csproj проекта в секции `ItemGroup` как `<AdditionalFiles Include=".editorconfig" />`
