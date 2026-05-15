module Templating =
    open System.Xml
    open System.Xml.XPath
    open System.Text.RegularExpressions

    /// Default "Static" category (for templates without an explicit category, e.g. "[Key]").
    let staticCategory = "Static"

    /// A curried lookup function: Category -> Key -> Translation.
    type Lookup = string -> string -> string

    /// Template regex matching both "[Key]" and "[Category:Key]".
    /// (match groups: 2=Category, 3=Key)
    let private templateRegex = Regex("\[((\w+):)?([^]]+)\]")

    /// Translate a string,i.e. replace all template matches in it with theirs translations.
    /// Templates without an explicit category are looked up in the `defaultCategory`.
    let searchAndTranslate (lookup: Lookup, defaultCategory: string) (text: string): string =
        // replace match with its translation
        let translateMatch (m: Match): string =
            let gs = m.Groups
            let category =
                let catGr = gs[2]
                if catGr.Success then catGr.Value else defaultCategory
            let key = gs[3].Value

            lookup category key

        // execute the replacements on the text (-1 = all matches, 0 = start at the beginning)
        templateRegex.Replace(text, translateMatch, -1, 0)

    /// Translates the XML document contents in-place, replacing all templates with their translations.
    /// Only templates in attribute values and text nodes are translated.
    /// Note: This function modifies the XML document in-place, create a copy if you need to access the original.
    let translateDocument (lookup: Lookup, defaultCategory: string) (doc: XmlDocument): Unit =
        // iterate over all attributes and text nodes, translating their values
        doc.SelectNodes("//@* | //text()")
        |> Seq.cast<XmlNode>
        |> Seq.iter (fun node ->
            node.Value <- searchAndTranslate (lookup, defaultCategory) node.Value
        )
