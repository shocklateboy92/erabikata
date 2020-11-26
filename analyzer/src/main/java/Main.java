import com.atilika.kuromoji.ipadic.Tokenizer;
import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;

import java.io.*;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.regex.Pattern;
import java.util.stream.Stream;

public class Main {
    static final Pattern SEARCH_PATTERN = Pattern.compile("/subs/[^/]*\\.json$");
    private static final Tokenizer tokenizer = new Tokenizer();

    public static void main(String[] args) throws IOException {
        System.out.println("Starting...");
        Gson gson = new Gson();

        Stream<Path> files = Files
                .walk(Paths.get("."))
                .filter(p -> SEARCH_PATTERN.matcher(p.toAbsolutePath().toString()).find());

        files.forEach(file -> {
            try {
                List<Dialog> lines = gson.fromJson(new FileReader(file.toFile()), new TypeToken<ArrayList<Dialog>>() {
                }.getType());
                var analyzed = analyzeLines(lines).toArray(AnalyzedDialog[]::new);
                Arrays.stream(analyzed).forEach(ad -> ad.tokenized = Arrays.stream(ad.analyzed).map(d -> d.original).toArray(String[]::new));

                var parent = file.getParent();
                if (!parent.getFileName().toString().equals("subs")) {
                    throw new Exception("Encountered invalid directory: " + parent.getFileName());
                }

                var outDir = parent.resolve("analyzed_subs").toFile();
                if (!outDir.exists()) {
                    outDir.mkdir();
                }

                var outFileName = outDir.getAbsolutePath() + File.separator + file.getFileName();

                try (Writer writer = new FileWriter(outFileName)) {
                    gson.toJson(analyzed, writer);
                }
            } catch (Exception e) {
                System.err.println("Error while processing " + file);
                e.printStackTrace();
            }
        });
    }

    public static Stream<AnalyzedDialog> analyzeLines(List<Dialog> lines) {
        return lines.stream().map(line -> {
            var tokens = tokenizer.tokenize(line.getText());
            return new AnalyzedDialog(line, tokens.stream().map(token -> {
                var word = new AnalyzedDialog.Word();
                word.base = token.getBaseForm();
                word.conjugationType = token.getConjugationType();
                word.original = token.getSurface();
                word.partOfSpeech = new String[]{
                        token.getPartOfSpeechLevel1(),
                        token.getPartOfSpeechLevel2(),
                        token.getPartOfSpeechLevel3(),
                        token.getPartOfSpeechLevel4()
                };
                word.reading = token.getReading();
                word.pronunciation = token.getPronunciation();

                return word;
            }).toArray(AnalyzedDialog.Word[]::new));
        });
    }
}
