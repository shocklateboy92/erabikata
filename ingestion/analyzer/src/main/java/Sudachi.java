import com.worksap.nlp.sudachi.DictionaryFactory;
import com.worksap.nlp.sudachi.Tokenizer;

import java.io.IOException;
import java.util.Arrays;
import java.util.List;
import java.util.stream.Stream;

public class Sudachi {
  private static Tokenizer tokenizer;

  public static void initialize() throws IOException {
    tokenizer = new DictionaryFactory().create(System.getenv("BASE_PATH"), "{\"systemDict\": \"dict/system_full.dic\"}", true).create();
  }

  public static Stream<AnalyzedDialog> analyzeLines(List<Dialog> dialog, Tokenizer.SplitMode mode) {
    return dialog.stream()
        .map(
            ds -> new AnalyzedDialog(
                ds,
                Arrays.stream(ds.getText())
                    .map(
                        line -> {
                          var tokens = tokenizer.tokenize(mode, line);
                          return tokens.stream()
                              .map(
                                  token -> {
                                    var word = new AnalyzedDialog.Word();
                                    word.base = token.normalizedForm();
//                                      word.base = token.dictionaryForm();
//                                      word.conjugationType = token.getConjugationType();
                                    word.original = token.surface();
                                    word.partOfSpeech = token.partOfSpeech().toArray(new String[0]);
                                    word.reading = token.readingForm();
//                                      word.pronunciation = token.normalizedForm();

                                    return word;
                                  })
                              .toArray(AnalyzedDialog.Word[]::new);
                        })
                    .toArray(AnalyzedDialog.Word[][]::new)));
  }
}
