open System
open System.Xml

let dumpXml (doc: XmlDocument): Unit =
    use writer = XmlWriter.Create(Console.Out, XmlWriterSettings(Indent = true))
    doc.Save(writer)
    Console.WriteLine()
    Console.WriteLine()

// Testing:
let doc = XmlDocument()
doc.Load("data/Document.xml")
dumpXml doc

let translations =
    JsonTranslations.loadTranslations "data/Translations.json"
    |> JsonTranslations.toLookup

Templating.translateDocument (translations, Templating.staticCategory) doc
dumpXml doc
