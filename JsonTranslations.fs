module JsonTranslations =

    open System.IO
    open System.Text.Json

    /// A nested map: Category -> Key -> Translation
    type Translations = Map<string, Map<string, string>>

    /// Load translations from a Json file.
    /// The file should have the following structure:
    /// {
    ///   "Category1": {
    ///     "Key1": "Translation1",
    ///     "Key2": "Translation2"
    ///   },
    ///   "Category2": {
    ///     "Key1": "Translation1",
    ///     "Key2": "Translation2"
    ///   }
    /// }
    let loadTranslations (filePath: string): Translations =
        File.ReadAllText(filePath)
        |> JsonSerializer.Deserialize<Map<string,Map<string,string>>>

    /// Turn translations into a lookup function.
    let toLookup (translations: Translations): Templating.Lookup =
        fun category key ->
            translations
            |> Map.find category
            |> Map.find key
