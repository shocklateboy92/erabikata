import com.atilika.kuromoji.ipadic.Tokenizer;
import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.reflect.TypeToken;
import com.worksap.nlp.sudachi.DictionaryFactory;

import java.io.*;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.function.Consumer;
import java.util.function.Function;
import java.util.regex.Pattern;
import java.util.stream.Stream;

import static com.worksap.nlp.sudachi.Tokenizer.*;

public class Main {
  static final Pattern SEARCH_PATTERN = Pattern.compile("/input/[^/]*\\.json$");
  private static final Tokenizer tokenizer = new Tokenizer();

  public static void main(String[] args) throws IOException {
    Sudachi.initialize();
    System.out.println("Starting...");
    Gson gson = new GsonBuilder().setPrettyPrinting().create();

    Stream<Path> files =
        Files.walk(Paths.get("."))
            .filter(p -> SEARCH_PATTERN.matcher(p.toAbsolutePath().toString()).find());

    files.forEach(
        file -> {
          try {
            analyzeFile(gson, file, Main::analyzeLines, "kuromoji");
            analyzeFile(gson, file, dialog -> Sudachi.analyzeLines(dialog, SplitMode.A), "sudachi_a");
            analyzeFile(gson, file, dialog -> Sudachi.analyzeLines(dialog, SplitMode.B), "sudachi_b");
            analyzeFile(gson, file, dialog -> Sudachi.analyzeLines(dialog, SplitMode.C), "sudachi_c");
          } catch (Exception e) {
            System.err.println("Error while processing " + file);
            e.printStackTrace();
          }
        });
  }

  private static void analyzeFile(Gson gson, Path file, Function<List<Dialog>, Stream<AnalyzedDialog>> anaylzeFunc, String name) throws Exception {
    List<Dialog> lines =
        gson.fromJson(
            new FileReader(file.toFile()), new TypeToken<ArrayList<Dialog>>() {}.getType());

    var parent = file.getParent();
    if (!parent.getFileName().toString().equals("input")) {
      throw new Exception("Encountered invalid directory: " + parent.getFileName());
    }

    var analyzed = anaylzeFunc.apply(lines).toArray(AnalyzedDialog[]::new);
    var outDir = parent.getParent().resolve(name).toFile();
    if (!outDir.exists()) {
      outDir.mkdir();
    }

    var outFileName = outDir.getAbsolutePath() + File.separator + file.getFileName();

    try (Writer writer = new FileWriter(outFileName)) {
      gson.toJson(analyzed, writer);
    }
  }

  public static Stream<AnalyzedDialog> analyzeLines(List<Dialog> dialog) {
    return dialog.stream()
        .map(
            ds -> {
              return new AnalyzedDialog(
                  ds,
                  Arrays.stream(ds.getText())
                      .map(
                          line -> {
                            var tokens = tokenizer.tokenize(line);
                            return tokens.stream()
                                .map(
                                    token -> {
                                      var word = new AnalyzedDialog.Word();
                                      word.base = token.getBaseForm();
                                      word.dictionary = token.getBaseForm();
                                      word.conjugationType = token.getConjugationType();
                                      word.original = token.getSurface();
                                      word.partOfSpeech =
                                          new String[] {
                                            token.getPartOfSpeechLevel1(),
                                            token.getPartOfSpeechLevel2(),
                                            token.getPartOfSpeechLevel3(),
                                            token.getPartOfSpeechLevel4()
                                          };
                                      word.reading = token.getReading();
                                      word.pronunciation = token.getPronunciation();

                                      return word;
                                    })
                                .toArray(AnalyzedDialog.Word[]::new);
                          })
                      .toArray(AnalyzedDialog.Word[][]::new));
            });
  }
}
