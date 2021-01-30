from sudachipy import tokenizer, dictionary

analyzer = dictionary.Dictionary().create()

res = analyzer.tokenize("国家公務員", tokenizer.Tokenizer.SplitMode.A)
results = [{"surface": a.surface(), "dictionary": a.dictionary_form()} for a in res]

print(results)